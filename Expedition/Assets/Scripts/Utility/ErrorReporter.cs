
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ErrorReporter : MonoBehaviour
{
    private string lastLog;
    private Text text;

    private void OnEnable() {Application.logMessageReceivedThreaded += HandleLog;}
    private void OnDisable() {Application.logMessageReceivedThreaded -= HandleLog;}

    private void Awake() {
        if(!Application.isEditor) {
            enabled = false; 
            return;
        }
        text = GetComponent<Text>();
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(!Application.isEditor) return;
        if (type != LogType.Log && logString != lastLog && type != LogType.Warning) {
            if(text.text.Length < 1) {
                text.text = "<color=silver><size=36>(Press tab to remove)</size></color>";
            }

            string str = logString;
            if(type == LogType.Warning) str = "<color=yellow>("+str+")</color>";
            text.text += "\n"+str;
            lastLog = logString;
        }
    }

    private void Update() {
        //https://answers.unity.com/questions/598015/removing-the-first-line-of-a-string.html
        if(Input.GetKeyUp(KeyCode.Tab)) {
            int index = text.text.IndexOf(System.Environment.NewLine);
            text.text = text.text.Substring(index + System.Environment.NewLine.Length);
            if(index < 1) text.text = "";
        }
    }

}
