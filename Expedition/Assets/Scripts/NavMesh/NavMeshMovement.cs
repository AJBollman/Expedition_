using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public List<Vector3> pointList;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                //get a list of vector 3 points
                pointList.Add(hit.point);
                agent.SetDestination(hit.point);
            }
        }
    }
    void navMove()
    {
        
        
    }
}
