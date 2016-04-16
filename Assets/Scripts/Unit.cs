using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public Vector2 ScreenSpacePos;
    public bool IsVisibleOnScreen;
    public bool IsSelected;
    public bool IsMoveable = true;

    public bool IsRunning
    {
        get
        {
            return animator.GetBool("IsRunnig");
        }
        set
        {
            animator.SetBool("IsRunning", value);
        }
    }

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (!IsSelected)
        { 
            if (UnitSelectionHandler.IsUnitVisibleOnScreen(this.gameObject))
            {
                if(!IsVisibleOnScreen)
                {
                    UnitSelectionHandler.UnitsVisibleOnScreen.Add(this.gameObject);
                    IsVisibleOnScreen = true;
                }
            }
            else
            {
                if(IsVisibleOnScreen)
                {
                    UnitSelectionHandler.UnitsVisibleOnScreen.Remove(this.gameObject);
                    IsVisibleOnScreen = false;
                }

            }
        }
	}
}
