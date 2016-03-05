using UnityEngine;
using System.Collections;
using Pathfinding;

public class UnitPathFinding : MonoBehaviour {

    public Path Path;
    public float Speed;

    private Seeker seeker;
    private CharacterController characterController;
    private int currentWaypoint = 0;
    private Unit unit;
    private float NextWaypointDistance = 10f;


    // Use this for initialization
    void Start () {
        seeker = GetComponent<Seeker>();
        characterController = GetComponent<CharacterController>();
        unit = GetComponent<Unit>();
	}
	
	void LateUpdate()
    {
        if(unit.IsSelected && unit.IsMoveable)
        {
            if(Input.GetMouseButton(1)) //controlKey
            {
                seeker.StartPath(transform.position, UnitSelectionHandler.RightMouseClickToMove, OnPathCommplete);
            }
        }

    }

    private void OnPathCommplete(Path path)
    {
        if(!path.error)
        {
            Path = path;
            currentWaypoint = 0;
        }

    }

    public void FixedUpdate()
    {
        if (Path != null &&currentWaypoint < Path.vectorPath.Count && unit.IsMoveable)
        {
            Vector3 direction = Path.vectorPath[currentWaypoint] - transform.FindChild("Model").transform.position;
            direction = direction.normalized * Speed * Time.fixedDeltaTime;

            characterController.SimpleMove(direction);

            if(Vector3.Distance(transform.position, Path.vectorPath[currentWaypoint]) < NextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
    }
}
