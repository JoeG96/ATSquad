using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommands : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] Camera camera;
    [SerializeField] float maxCommandDistance = 100f;
    [SerializeField] GameObject targetIndicator;
    private MeshRenderer indicatorRenderer;
    [SerializeField] Material squad1Mat, squad2Mat, squad3Mat;


    [SerializeField] float distance;

    [SerializeField] GameObject squadMember1;
    [SerializeField] GameObject squadMember2;
    [SerializeField] GameObject squadMember3;

    [SerializeField] GameObject squadFollowPos1;
    [SerializeField] GameObject squadFollowPos2;
    [SerializeField] GameObject squadFollowPos3;

    private followPlayer squad1Follow;
    private followPlayer squad2Follow;
    private followPlayer squad3Follow;

    Transform squad1TargetPos;
    Transform squad2TargetPos;
    Transform squad3TargetPos;
    Transform targetPos;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        indicatorRenderer = targetIndicator.GetComponent<MeshRenderer>();

        distance = 0f;

        squadFollowPos1 = GameObject.Find("FollowPos 1");
        squadFollowPos2 = GameObject.Find("FollowPos 2");
        squadFollowPos3 = GameObject.Find("FollowPos 3");

        squad1Follow = squadMember1.GetComponent<followPlayer>();
        squad1TargetPos = squad1Follow.GetTarget(targetPos);
        squad2Follow = squadMember2.GetComponent<followPlayer>();
        squad2TargetPos = squad2Follow.GetTarget(targetPos);
        squad3Follow = squadMember3.GetComponent<followPlayer>();
        squad3TargetPos = squad3Follow.GetTarget(targetPos);

        //Debug.Log(targetPos);

    }

    private void Update()
    {
        //ReceiveInput();
        /*
         * If recall = true, go to SetToFollow, after set recall = false
         * if ()
        {}*/


    }

    public void ReceiveInput()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit) && CheckDistance(hit.point) == true)
        {
            SetTarget(hit.point);
        }
    }

    public bool CheckDistance(Vector3 hitPos)
    {
        distance = Vector3.Distance(camera.transform.position, hitPos);
        if (distance > maxCommandDistance)
        {
            Debug.Log("Distance too far:" + distance);
            return false;
        }
        else return true;
    }

    private void SetTarget(Vector3 hitPos)
    {
        //Debug.Log("Waypoint set: " + distance);
        targetIndicator.transform.position = hitPos;
        if (inputManager.SquadMember1Selected())
        {
            indicatorRenderer.material = squad1Mat;
            squad1Follow.SetTarget(hitPos);
        }
        else if (inputManager.SquadMember2Selected())
        {
            indicatorRenderer.material = squad2Mat;
            squad2Follow.SetTarget(hitPos);
        }
        else
        {
            indicatorRenderer.material = squad3Mat;
            squad3Follow.SetTarget(hitPos);
        }

    }

    public void SetToRecall()
    {

        squad1Follow.SetToRecall(squadFollowPos1.transform.position);
        squad2Follow.SetToRecall(squadFollowPos2.transform.position);
        squad3Follow.SetToRecall(squadFollowPos3.transform.position);

        Debug.Log("Set To Follow Player");


    }

    public void SetToFollow()
    {
        squad1Follow.SetToFollow();
        squad2Follow.SetToFollow();
        squad3Follow.SetToFollow();
    }

    public void SetToRoam()
    {
        squad1Follow.SetToRoam();
        squad2Follow.SetToRoam();
        squad3Follow.SetToRoam();
    }

    public void SetToFind()
    { 
        if (inputManager.SquadMember1Selected())
        {
            squad1Follow.FindCollectables();
        }
        else if (inputManager.SquadMember2Selected())
        {
            squad2Follow.FindCollectables();
        }
        else
        {
            squad3Follow.FindCollectables();
        }
    }

    public void SetToEnemyFind()
    {
        if (inputManager.SquadMember1Selected())
        {
            squad1Follow.HuntEnemies();
        }
        else if (inputManager.SquadMember2Selected())
        {
            squad2Follow.HuntEnemies();
        }
        else
        {
            squad3Follow.HuntEnemies();
        }
        //squad1Follow.HuntEnemies();
        //squad2Follow.HuntEnemies();
        //squad3Follow.HuntEnemies();
    }

}
