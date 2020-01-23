using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorReporter : MonoBehaviour
{
    public string output = "";
    public string stack = "";
    private GameObject p;
    private string lastLog;

    private void Awake()
    {
        p = GameObject.Find("The Explorer");
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
            var regName = (StateController.activeRegion) ? StateController.activeRegion.name : "empty";
            lastLog = logString;

            string text = "------ BIG OOF ------\n" +
                logString + "\n\n" +
                "Region: " + regName + "\n" +
                "Player pos: " + p.transform.position + "\n" +
                "GameState: " + StateController.getState() + "\n" +
                "Was drawing line:" + p.GetComponent<Player>().isCameraDrawing + "\n" +
                "Last camera raycast hit: " + p.GetComponent<Player>().lastRaycastHit + "\n\n" +
                stackTrace + "\n";
            GetComponent<Text>().text = text;
            StartCoroutine(hide(10f));
        }
    }
    private IEnumerator hide(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Text>().text = "";
    }

}
