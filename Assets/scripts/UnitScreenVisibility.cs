using UnityEngine;
using System.Collections;

public class UnitScreenVisibility : MonoBehaviour {

    public Vector2 ScreenSpacePos;
    public bool IsVisibleOnScreen;
    public bool IsSelected;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (!IsSelected)
        { 
            if (UnitSelection.IsUnitVisibleOnScreen(this.gameObject))
            {
                if(!IsVisibleOnScreen)
                {
                    UnitSelection.UnitsVisibleOnScreen.Add(this.gameObject);
                    IsVisibleOnScreen = true;
                }
            }
            else
            {
                if(IsVisibleOnScreen)
                {
                    UnitSelection.UnitsVisibleOnScreen.Remove(this.gameObject);
                    IsVisibleOnScreen = false;
                }

            }
        }
	}
}
