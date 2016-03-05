using UnityEngine;
using System.Collections.Generic;

public class UnitSelection : MonoBehaviour {

    private RaycastHit hit;
    private Vector3 mouseDownPointWorldSpace;
    private Vector3 mouseCurentPointWorldSpace;
    private bool userIsDraggingMouse;
    private float timeLeftBeforeDeclareMouseDrag;
    private Vector2 mouseDragStartInScreenSpace;
    private bool userIsFinishedDragging;
    private bool userIsStartedDragging;

    //GUI
    private float dragBoxWidth;
    private float dragBoxHeight;
    private float dragBoxLeft;
    private float dragBoxTop;
    private Vector2 dragBoxStartPoint;
    private Vector2 dragBoxFinishPoint;

    public static List<GameObject> CurrentlySelectedUnits = new List<GameObject>();
    public static List<GameObject> UnitsVisibleOnScreen = new List<GameObject>();
    public static List<GameObject> UnitsInDragBox = new List<GameObject>();

    public float ClickOffset = 5.0f;
    public float DragOffset = 5.0f;
    public float TimeLimitBeforeDeclareMouseDrag = 1.0f;
    public GUIStyle MouseDragSkin;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(ray.origin, ray.direction * 1000.0f, Color.yellow);
            mouseCurentPointWorldSpace = hit.point;

            if (Input.GetMouseButtonDown(0)) //controlKey
            {
                mouseDownPointWorldSpace = hit.point;
                timeLeftBeforeDeclareMouseDrag = TimeLimitBeforeDeclareMouseDrag;
                mouseDragStartInScreenSpace = Input.mousePosition;
                userIsStartedDragging = true;
            }
            else if (Input.GetMouseButton(0)) //controlKey
            {
                if (!userIsDraggingMouse)
                {
                    timeLeftBeforeDeclareMouseDrag -= Time.deltaTime;

                    if (timeLeftBeforeDeclareMouseDrag <= 0 || IsUserDraggingMouse(mouseDragStartInScreenSpace, Input.mousePosition))
                    {
                        userIsDraggingMouse = true;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0)) //controlKey
            {
                if(userIsDraggingMouse)
                {
                    userIsFinishedDragging = true;
                }

                userIsDraggingMouse = false;
            }

            if (!userIsDraggingMouse)
            {
                if (hit.collider.name == "Terrain")
                {
                    if (Input.GetMouseButtonUp(0) && DidUserClickleftMouse(hit.point)) //controlKey
                    {
                        DeselectUnitsIfNoShiftKeysPressed();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonUp(0) && DidUserClickleftMouse(hit.point)) //controlKey
                    {
                        //TODO instead of checking the existance of SelectionVisualizer use other means (does it have a component named Unit?)
                        if (hit.collider.transform.parent != null && hit.collider.transform.parent.FindChild("SelectionVisualizer"))
                        {
                            GameObject parentObject = hit.collider.transform.parent.gameObject;

                            if (!CurrentlySelectedUnits.Contains(parentObject))
                            {
                                bool isShiftKeysDown = HelperMethods.IsShiftKeysDown();

                                if (isShiftKeysDown)
                                {
                                    AddNewUnitToCurrentlySelectedUnits(parentObject);
                                }
                                else
                                {
                                    DeselectUnitsIfSelected();
                                    AddNewUnitToCurrentlySelectedUnits(parentObject);
                                }
                            }
                            else
                            {
                                bool isShiftKeysDown = HelperMethods.IsShiftKeysDown();

                                if (isShiftKeysDown)
                                {
                                    RemoveUnitFromCurentlySelectedUnits(parentObject);
                                }
                                else
                                {
                                    DeselectUnitsIfSelected();
                                    AddNewUnitToCurrentlySelectedUnits(parentObject);
                                }
                            }
                        }
                        else
                        {
                            DeselectUnitsIfNoShiftKeysPressed();
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0) && DidUserClickleftMouse(hit.point)) //controlKey
                {
                    DeselectUnitsIfNoShiftKeysPressed();
                }
            }
        }

        CalculateDragBox();

        if (!HelperMethods.IsShiftKeysDown() && userIsStartedDragging && userIsDraggingMouse)
        {
            //At selection start we delete the currently selected units array if shift key is not pressed
            DeselectUnitsIfSelected();
            userIsStartedDragging = false;
        }
    }

    void LateUpdate()
    {
        UnitsInDragBox.Clear();

        if((userIsDraggingMouse || userIsFinishedDragging) && UnitsVisibleOnScreen.Count > 0)
        {
            UnitsVisibleOnScreen.ForEach(i => {

                if (!UnitsInDragBox.Contains(i))
                {
                    if(IsUnitInsideDragBox(i))
                    {
                        UnitsInDragBox.Add(i);
                        SetSelectionVisualizer(i, true);
                    }
                    else
                    {
                        if (!CurrentlySelectedUnits.Contains(i))
                        {
                            SetSelectionVisualizer(i, false);
                        }
                    }
                }
            });
        }  

        if(userIsFinishedDragging)
        {
            userIsFinishedDragging = false;
            PutDraggedUnitsIntoCurrentlySelectedUnits();
        }
    }

    void OnGUI()
    {

        if (userIsDraggingMouse)
        {
            GUI.Box(new Rect(dragBoxLeft, dragBoxTop, dragBoxWidth, dragBoxHeight), "", MouseDragSkin);
        }
    }

    private void CalculateDragBox()
    {
        if (userIsDraggingMouse)
        {
            dragBoxWidth = Camera.main.WorldToScreenPoint(mouseDownPointWorldSpace).x - Camera.main.WorldToScreenPoint(mouseCurentPointWorldSpace).x;
            dragBoxHeight = Camera.main.WorldToScreenPoint(mouseDownPointWorldSpace).y - Camera.main.WorldToScreenPoint(mouseCurentPointWorldSpace).y;
            dragBoxLeft = Input.mousePosition.x;
            dragBoxTop = (Screen.height - Input.mousePosition.y) - dragBoxHeight;

            if (dragBoxWidth > 0)
            {
                if (dragBoxHeight > 0)
                {
                    //Box mouse point is bottom left
                    dragBoxStartPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y + dragBoxHeight);
                }
                else
                {
                    //Box mouse point is top left
                    dragBoxStartPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
            }
            else
            {
                if (dragBoxHeight > 0)
                {
                    ///Box mouse point is bottom right
                    dragBoxStartPoint = new Vector2(Input.mousePosition.x + dragBoxWidth, Input.mousePosition.y + dragBoxHeight);
                }
                else
                {
                    ///Box mouse point is top right
                    dragBoxStartPoint = new Vector2(Input.mousePosition.x + dragBoxWidth, Input.mousePosition.y);
                }
            }

            dragBoxFinishPoint = new Vector2(dragBoxStartPoint.x + Mathf.Abs(dragBoxWidth), dragBoxStartPoint.y - Mathf.Abs(dragBoxHeight));
        }
    }

    private void DeselectUnitsIfNoShiftKeysPressed()
    {
        bool isShiftKeysDown = HelperMethods.IsShiftKeysDown();

        if (!isShiftKeysDown)
        {
            DeselectUnitsIfSelected();
        }
    }

    private bool DidUserClickleftMouse(Vector3 leftMousePoint)
    {
        bool ret = false;

        if ((mouseDownPointWorldSpace.x < leftMousePoint.x + ClickOffset && mouseDownPointWorldSpace.x > leftMousePoint.x - ClickOffset) &&
            (mouseDownPointWorldSpace.y < leftMousePoint.y + ClickOffset && mouseDownPointWorldSpace.y > leftMousePoint.y - ClickOffset) &&
            (mouseDownPointWorldSpace.z < leftMousePoint.z + ClickOffset && mouseDownPointWorldSpace.z > leftMousePoint.z - ClickOffset))
        {
            ret = true;
        }

        return ret;
    }

    private void DeselectUnitsIfSelected()
    {
        if (CurrentlySelectedUnits.Count != 0)
        {
            CurrentlySelectedUnits.ForEach(i => SetSelectionVisualizer(i, false));
            CurrentlySelectedUnits.Clear();
        }
    }

    private void RemoveUnitFromCurentlySelectedUnits(GameObject unit)
    {
        if (CurrentlySelectedUnits.Remove(unit))
        {
            SetSelectionVisualizer(unit, false);
        }
    }

    private void SetSelectionVisualizer(GameObject unit, bool active)
    {
        unit.transform.FindChild("SelectionVisualizer").gameObject.SetActive(active);
        unit.GetComponent<UnitScreenVisibility>().IsVisibleOnScreen = active;
    }

    private void AddNewUnitToCurrentlySelectedUnits(GameObject unit)
    {
        CurrentlySelectedUnits.Add(unit);
        SetSelectionVisualizer(unit, true);
    }

    private bool IsUserDraggingMouse(Vector2 DragStart, Vector2 NewPoint)
    {
        bool ret = false;

        if ((NewPoint.x > DragStart.x + DragOffset || NewPoint.x < DragStart.x - DragOffset) &&
            (NewPoint.y > DragStart.y + DragOffset || NewPoint.y < DragStart.y - DragOffset))
        {
            ret = true;
        }

        return ret;
    }

    public static bool IsUnitVisibleOnScreen(GameObject unit)
    {
        bool ret = false;
        Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);

        if (unitScreenPosition.x < Screen.width && unitScreenPosition.y < Screen.height &&
            unitScreenPosition.x > 0 && unitScreenPosition.y > 0)
        {
            ret = true;
        }

        return ret;
    }

    private bool IsUnitInsideDragBox(GameObject unit)
    {
        bool ret = false;
        Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);

        if (unitScreenPosition.x > dragBoxStartPoint.x && unitScreenPosition.y < dragBoxStartPoint.y &&
            unitScreenPosition.x < dragBoxFinishPoint.x && unitScreenPosition.y > dragBoxFinishPoint.y)
        {
            ret = true;
        }

        return ret;
    }

    private void PutDraggedUnitsIntoCurrentlySelectedUnits()
    {
        UnitsInDragBox.ForEach(i =>
        {
            if (!CurrentlySelectedUnits.Contains(i))
            {
                AddNewUnitToCurrentlySelectedUnits(i);
            }
        });

        UnitsInDragBox.Clear();
    }

}
