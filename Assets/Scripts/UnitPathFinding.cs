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
            var modelTransform = transform.FindChild("Model").transform;
            Vector3 direction = Path.vectorPath[currentWaypoint] - modelTransform.position;
            direction = direction.normalized * Speed * Time.fixedDeltaTime;

            Vector3 lookAtPos = new Vector3(Path.vectorPath[currentWaypoint].x, modelTransform.position.y, Path.vectorPath[currentWaypoint].z);
            modelTransform.LookAt(lookAtPos);
            
            characterController.SimpleMove(direction);

            var nextWaypointDistance = NextWaypointDistance;
            if (currentWaypoint == Path.vectorPath.Count - 1)
            {
                nextWaypointDistance = 0;
            }

            if(Vector3.Distance(modelTransform.position, Path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
    }
}
