using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public List<Vector3> pointList;
    private GameObject home;
    private GameObject destination;
    public bool failedPath;
    private bool canMove;
    private Vector3 navPos;
    private int x = 0;
    // Update is called once per frame

    void Update()
    {
        navPos = agent.transform.position;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                //get a list of vector 3 points
                pointList.Add(hit.point);
                //agent.SetDestination(hit.point);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            canMove = true;
        }

        if (canMove == true)
        {
            if (x < pointList.Count)
            {
                agent.SetDestination(pointList[x]);
                if (Vector3.Distance(pointList[x], navPos) < 2f)
                {
                    x++;
                }
            }
            else
            {
                canMove = false;
            }
        }

        if (failedPath == true)
        {
            agent.SetDestination(home.transform.position);
            pointList.Clear();
        }

        /*if(GetComponent<Destination>().ReachedDestination == true)
        {
            Debug.Log("YOU MADE IT");
        }*/
    }
}
