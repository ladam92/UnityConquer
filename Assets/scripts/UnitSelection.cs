using UnityEngine;
using System.Collections;

public class UnitSelection : MonoBehaviour {

    private RaycastHit hit;
    public static GameObject CurrentlySelectedUnit;
    private Vector3 mouseDownPoint;

	// Use this for initialization
	void Start ()
    {
        mouseDownPoint = Vector3.zero;
        Debug.Log("Mouse selection script started");
	}
	
	// Update is called once per frame
	void Update ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.Log(hit.collider.name);
            //left mouse button
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPoint = hit.point;
            }

            if (hit.collider.name == "Terrain")
            {
                if (Input.GetMouseButtonUp(0) && DidUserClickleftMouse(hit.point))
                {
                    DeselectGameobjectIfSelected();
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0) && DidUserClickleftMouse(hit.point))
                {
                    if(hit.collider.transform.parent != null && hit.collider.transform.parent.FindChild("SelectionVisualizer"))
                    {
                        Debug.Log("User selected a Unit!");

                        if (CurrentlySelectedUnit != hit.collider.transform.parent.gameObject)
                        {
                            DeselectGameobjectIfSelected();
                            CurrentlySelectedUnit = hit.collider.transform.parent.gameObject;

                            GameObject selectedObject = hit.collider.transform.parent.FindChild("SelectionVisualizer").gameObject;
                            selectedObject.SetActive(true);
                        }
                    }
                }
            }
        }
        else
        {
            if(Input.GetMouseButtonUp(0) && DidUserClickleftMouse(hit.point))
            {
                DeselectGameobjectIfSelected();
            }
        }
	}

    private bool DidUserClickleftMouse(Vector3 leftMousePoint)
    {
        bool ret = false;

        float clickoffset = 10f;

        if((mouseDownPoint.x < leftMousePoint.x + clickoffset && mouseDownPoint.x > leftMousePoint.x - clickoffset) &&
            (mouseDownPoint.y < leftMousePoint.y + clickoffset && mouseDownPoint.y > leftMousePoint.y - clickoffset) &&
            (mouseDownPoint.z < leftMousePoint.z + clickoffset && mouseDownPoint.z > leftMousePoint.z - clickoffset))
        {
            ret = true;
        }

        return ret;
    }

    private void DeselectGameobjectIfSelected()
    {
        if (CurrentlySelectedUnit != null)
        {
            CurrentlySelectedUnit.transform.FindChild("SelectionVisualizer").gameObject.SetActive(false);
            CurrentlySelectedUnit = null;
        }
    }
}
