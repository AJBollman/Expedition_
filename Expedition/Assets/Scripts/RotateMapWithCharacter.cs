using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMapWithCharacter : MonoBehaviour
{
    public int rotIncrement = 15;
    private GameObject player;
    private float cameraRot;

    private float snapGoal;
    private float diff;
    private float increment;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        cameraRot = Camera.main.transform.rotation.eulerAngles.y;

        snapGoal = (Mathf.Round(cameraRot / rotIncrement) * rotIncrement);
        //diff = Mathf.Abs(snapGoal - increment);
        //increment = Mathf.Lerp(playerRot, snapGoal, diff);
        transform.localRotation = Quaternion.AngleAxis(snapGoal, Vector3.forward);
    }
}
