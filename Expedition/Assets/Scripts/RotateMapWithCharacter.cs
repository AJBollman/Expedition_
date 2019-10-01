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
        //if (!Player.mapSpins) return;
        cameraRot = Camera.main.transform.rotation.eulerAngles.y;
        increment = (Player.mapIsFull || !Player.mapSpins) ? 90 : rotIncrement;

        snapGoal = (Mathf.Round(cameraRot / increment) * increment);

        //increment = Mathf.Lerp(cameraRot, snapGoal, Time.deltaTime * 0.1f); // todo
        transform.localRotation = Quaternion.AngleAxis(snapGoal, Vector3.forward);
    }
}
