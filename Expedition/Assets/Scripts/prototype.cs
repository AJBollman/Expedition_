using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prototype : MonoBehaviour
{

    private GameObject minimapCam;
    private GameObject freeMapCam;
    private GameObject fullMap;
    private GameObject handMap;
    private bool fullScreenMap;
    private GameObject indicator;

    // Start is called before the first frame update
    void Start()
    {
        minimapCam = GameObject.Find("Minimap Cam");
        freeMapCam = GameObject.Find("Free Map Cam");
        fullMap = GameObject.Find("FullMap");
        handMap = GameObject.Find("HandMap");
    }

    // Update is called once per frame
    void Update()
    {
        minimapCam.transform.position = new Vector3(
            Player.Explorer.transform.position.x,
            minimapCam.transform.position.y,
            Player.Explorer.transform.position.z
        );

        if(Input.GetKeyDown(KeyCode.F)) {
            fullScreenMap = !fullScreenMap;
            fullMap.SetActive(fullScreenMap);
            handMap.SetActive(!fullScreenMap);
            CameraOperator.enableControls = !fullScreenMap;
        }
    }


}
