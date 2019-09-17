using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorReporter : MonoBehaviour
{
    public string output = "";
    public string stack = "";
    private GameObject p;
    private GameObject err;

    private void Awake()
    {
        p = GameObject.Find("The Explorer");
        err = GameObject.Find("ErrorMsg");
        err.SetActive(false);
    }

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        if (type == LogType.Error || type == LogType.Exception && p != null)
        {
            err.SetActive(true);
            Debug.Log("------ BIG OOF ------\n"+
                "Region: "+StateController.activeRegion.name+"\n"+
                "Player pos: "+p.transform.position+"\n" +
                "GameState: "+StateController.getState()+"\n" +
                "Was drawing line:"+p.GetComponent<Player>().isCameraDrawing+"\n"+
                "Last camera raycast hit: "+ p.GetComponent<Player>().lastRaycastHit);
        }
    }
}
