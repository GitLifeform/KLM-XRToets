using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public GameObject[] Hangars;
    public GameObject[] Planes;
    public Button LightsOnButton, ParkButton, CameraButton;
    public GameObject SuccessText;

    private bool ParkingPlanes = false;
    private List<NavMeshAgent> NavAgents = new List<NavMeshAgent>();
    private List<Camera> PlaneCameras = new List<Camera>();
    private Camera MainCamera;


    int RandomSort(GameObject a, GameObject b)
    {
        return Random.Range(0, Hangars.Length);

    }
    // Start is called before the first frame update
    void Start()
    {
        LightsOnButton.onClick.AddListener(ToggleLights);
        ParkButton.onClick.AddListener(ParkToggle);
        CameraButton.onClick.AddListener(ToggleCamera);
        
        //Shuffle arrays for some randomness
        System.Array.Sort(Hangars, RandomSort);
        System.Array.Sort(Planes, RandomSort);
        //Loop through Hangars and planes to set their in-world UI to their index.
        //We can use this single loop because the assignment requires the amount of Hangars and Planes to be the same.
        for(int i = 0; i < Hangars.Length; ++i)
        {
            Hangars[i].GetComponent<TextMeshPro>().SetText(i.ToString(), true);
            Planes[i].GetComponent<TextMeshPro>().SetText(i.ToString(), true);
            Planes[i].GetComponent<Plane>().GoalTransform = Hangars[i].transform;
            Planes[i].GetComponent<NavMeshAgent>();
            NavAgents.Add(Planes[i].GetComponent<NavMeshAgent>());
            PlaneCameras.Add(Planes[i].GetComponentInChildren<Camera>(true));
        }
        MainCamera = Camera.main;
    }

    void ParkToggle()
    {
        //Reset success text
        SuccessText.SetActive(false);

        //Loop through planes to set new destinations
        for (int i = 0; i < Planes.Length; ++i)
        {
            Plane curPlane = Planes[i].GetComponent<Plane>();
            if (curPlane.Patrolling)
            {
                Vector3 DestinationLocation = curPlane.GoalTransform.position;
                Planes[i].GetComponent<NavMeshAgent>().SetDestination(DestinationLocation);
                curPlane.Patrolling = false;
                ParkingPlanes = true;
            }
            else
            {
                curPlane.Patrolling = true;
                curPlane.ChooseNewRandomGoal();
            }
            ParkButton.GetComponentInChildren<Text>().text = curPlane.Patrolling ? "Park planes" : "Patrol planes";
        }
    }

    void ToggleCamera()
    {
        if(MainCamera.isActiveAndEnabled)
        {
            PlaneCameras[Random.Range(0, PlaneCameras.Count - 1)].enabled = true;
            MainCamera.enabled = false;
        }
        else
        {
            for (int i = 0; i < PlaneCameras.Count; ++i)
            {
                PlaneCameras[i].enabled = false;
            }
            MainCamera.enabled = true;
        }
        CameraButton.GetComponentInChildren<Text>().text = MainCamera.enabled ? "Pilot view" : "Overview";

    }
    void ToggleLights()
    {
        for (int i = 0; i < Planes.Length; ++i)
        {
            Light curLight = Planes[i].GetComponent<Light>();
            curLight.enabled = !curLight.enabled;
            LightsOnButton.GetComponentInChildren<Text>().text = curLight.enabled ? "Turn lights off" : "Turn lights on";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ParkingPlanes)
        {
            bool allPlanesParked = true;
            for (int i = 0; i < NavAgents.Count; ++i)
            {
                if (NavAgents[i].hasPath)
                {
                    allPlanesParked = false;
                    break;
                }
            }
            if (allPlanesParked)
            {
                ParkingPlanes = false;
                SuccessText.SetActive(true);
            }
        }
    }
}
