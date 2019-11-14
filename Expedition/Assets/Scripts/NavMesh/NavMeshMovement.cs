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

    public List<Vector3> pointList;

    //detrian's code crossover
    private void Awake()
    {
        //despawn();
    }


    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
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
        }*/

        agent.speed = (canMove) ? 3.5f : 0f;

        if(pointList.Count > 0)
        {
            agent.SetDestination(pointList[0]);
            if (Vector3.Distance(agent.transform.position, pointList[0]) < 2f)
            {
                pointList.RemoveAt(0);
            }
        }

        /*if(failedPath == true)
        {
            pointList.Clear();
            pointHolder = 0;
            canMove = false;
            agent.SetDestination(home.transform.position);
        }*/
    }
    public void navMove()
    {
        canMove = true;
    }

    public void spawn(Vector3 pos)
    {
        pointList.Clear();
        transform.position = new Vector3(pos.x, pos.y + 2f, pos.z);
        GetComponent<Rigidbody>().useGravity = true;
    }

    public void despawn()
    {
        pointList.Clear();
        transform.position = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = false;
    }

    public void givePath(List<Vector3> points)
    {
        if(points.Count < 1)
        {
            Debug.LogWarning("Tried to give explorer an empty list!");
            return;
        }
        points.Reverse();
        pointList = points;
        agent.SetDestination(pointList[0]);
    }
}
