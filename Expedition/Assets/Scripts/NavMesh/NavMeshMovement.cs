using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    private Vector3 Pos;
    public GameObject home;
    public GameObject destination;

    public bool failedPath;
    public bool canMove;

    public int pointHolder = 0;
    public List<Vector3> pointList;

    // Update is called once per frame
    void Update()
    {
        Pos = agent.transform.position;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                //get a list of vector 3 points
                pointList.Add(hit.point);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            canMove = true;
        }

        if(canMove == true)
        {
            if (pointHolder < pointList.Count)
            {
                agent.SetDestination(pointList[pointHolder]);
                if (Vector3.Distance(Pos, pointList[pointHolder]) < 2f)
                {
                    pointHolder++;
                }
            }
            else canMove = false;
        }

        if(failedPath == true)
        {
            pointList.Clear();
            pointHolder = 0;
            canMove = false;
            agent.SetDestination(home.transform.position);
        }
    }
    void navMove()
    {
        
        
    }
}
