using UnityEngine;
using System.Collections.Generic;

public class UnitSelection : MonoBehaviour {

    private RaycastHit hit;
    private Vector3 mouseDownPointWorldSpace;
    private Vector3 mouseUpPointWorldSpace;
    private Vector3 mouseCurentPointWorldSpace;
    private bool userIsDraggingMouse;
    private float timeLeftBeforeDeclareMouseDrag;
    private Vector2 mouseDragStartInScreenSpace;

    public static List<GameObject> CurrentlySelectedUnits = new List<GameObject>();
    
    public float ClickOffset = 5.0f;
    public float DragOffset = 5.0f;
    public float TimeLimitBeforeDeclareMouseDrag = 1.0f;
    public GUIStyle MouseDragSkin;

	// Use this for initialization
	void Start ()
    {
        mouseDownPointWorldSpace = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
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
            }
            else if (Input.GetMouseButton(0)) //controlKey
            {
                if(!userIsDraggingMouse)
                {
                    timeLeftBeforeDeclareMouseDrag -= Time.deltaTime;

                    if(timeLeftBeforeDeclareMouseDrag <= 0 || IsUserDraggingMouse(mouseDragStartInScreenSpace, Input.mousePosition))
                    {
                        userIsDraggingMouse = true;
                    }
                }

                if(userIsDraggingMouse)
                {
                    Debug.Log("User is dragging");
                }
            }
            else if (Input.GetMouseButtonUp(0)) //controlKey
            {
                timeLeftBeforeDeclareMouseDrag = 0;
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
                        if (hit.collider.transform.parent != null && hit.collider.transform.parent.FindChild("SelectionVisualizer"))
                        {
                            GameObject parentObject = hit.collider.transform.parent.gameObject;

                            if (!CurrentlySelectedUnits.Contains(parentObject))
                            {
                                bool isShiftKeysDown = IsShiftKeysDown();

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
                                bool isShiftKeysDown = IsShiftKeysDown();

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
	}

    void OnGUI()
    {

        if (userIsDraggingMouse)
        {
            var boxWidth = Camera.main.WorldToScreenPoint(mouseDownPointWorldSpace).x - Camera.main.WorldToScreenPoint(mouseCurentPointWorldSpace).x;
            var boxHeight = Camera.main.WorldToScreenPoint(mouseDownPointWorldSpace).y - Camera.main.WorldToScreenPoint(mouseCurentPointWorldSpace).y;
            var boxLeft = Input.mousePosition.x;
            var boxTop = (Screen.height - Input.mousePosition.y) - boxHeight;

            GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), "", MouseDragSkin);
        }
    }

    private void DeselectUnitsIfNoShiftKeysPressed()
    {
        bool isShiftKeysDown = IsShiftKeysDown();

        if (!isShiftKeysDown)
        {
            DeselectUnitsIfSelected();
        }
    }

    private bool DidUserClickleftMouse(Vector3 leftMousePoint)
    {
        bool ret = false;

        if((mouseDownPointWorldSpace.x < leftMousePoint.x + ClickOffset && mouseDownPointWorldSpace.x > leftMousePoint.x - ClickOffset) &&
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
        if(CurrentlySelectedUnits.Remove(unit))
        {
            SetSelectionVisualizer(unit, false);
        }
    }

    private void SetSelectionVisualizer(GameObject unit, bool active)
    {
        unit.transform.FindChild("SelectionVisualizer").gameObject.SetActive(active);
    }

    private bool IsShiftKeysDown()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); //controlKey
    }

    private void AddNewUnitToCurrentlySelectedUnits(GameObject unit)
    {
        CurrentlySelectedUnits.Add(unit);
        SetSelectionVisualizer(unit, true);
    }

    private bool IsUserDraggingMouse(Vector2 DragStart, Vector2 NewPoint)
    {
        bool ret = false;

        if((NewPoint.x > DragStart.x + DragOffset || NewPoint.x < DragStart.x - DragOffset) &&
            (NewPoint.y > DragStart.y + DragOffset || NewPoint.y < DragStart.y - DragOffset))
        {
            ret = true;
        }

        return ret;
    }
}
