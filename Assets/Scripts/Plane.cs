using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Plane : MonoBehaviour
{
    public string PlaneTypeBrand = "Unassigned";
    public Transform GoalTransform; //Hangar goes in here, to use for its location as a goal to move to.
    public GameObject FloorObject; //Floor plane goes in here, to use for finding our patrol locations.
    public bool Patrolling = true; //Set to true if this plane is driving to random locations.
    public GameObject PlaneTypeTextObject;

    private NavMeshAgent navAgent;
    private Vector3 navMin;
    private Vector3 navMax;
    // Start is called before the first frame update
    void Start()
    {
        //initialize some private variables
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null || FloorObject == null) return;
        navMin = FloorObject.GetComponent<MeshFilter>().mesh.bounds.min;
        navMax = FloorObject.GetComponent<MeshFilter>().mesh.bounds.max;
        TextMeshProUGUI PlaneTypeText = PlaneTypeTextObject.GetComponent<TextMeshProUGUI>();
        PlaneTypeText.SetText( PlaneTypeBrand, true );

        if(Patrolling)
        {
            ChooseNewRandomGoal();
        }
    }

    public void ChooseNewRandomGoal()
    {
        Vector3 randomGoalLocation = new Vector3(
            Random.Range(navMin.x, navMax.x),
            FloorObject.transform.position.y,
            Random.Range(navMin.z, navMax.z));

        navAgent.SetDestination(randomGoalLocation);
    }

    // Update is called once per frame
    void Update()
    {
        if(navAgent != null)
        {
            //If end of patrolling path has been reached, choose a new path
            if(!navAgent.hasPath && Patrolling)
            {
                ChooseNewRandomGoal();
            }
        }
    }
}
