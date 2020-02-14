
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class Quest : MonoBehaviour
{
    public static List<Quest> AllQuests;

    #region [Private]
    [Tooltip("All quests to show after this one is completed")]
    [SerializeField] private List<Quest> _QuestsToUnlockOnCompletion;

    [Tooltip("Each object in this list has it's active state toggled on/off on completion of this quest")]
    [SerializeField] private List<GameObject> _ObjectsToToggleOnCompletion;

    [Tooltip("The least number of redline points that can be used to beat this quest")]
    [SerializeField] private int _bestPossibleScore;

    [Tooltip("Icon to show on the map")]
    [SerializeField] private Texture2D _QuestIcon;

    [Tooltip("Generate icon procedurally using the quest icon camera")]
    [SerializeField] private bool _generateIconFromCamera;

    [Tooltip("Description to show in some places")]
    [SerializeField] private string _flavorText;

    [Tooltip("The Traveller associated with this quest")]
    [SerializeField] private TravellerType _associatedTraveller;

    private QuestState _state = QuestState.hidden;
    private Quest _ParentQuest;
    private QuestBall _StartPoint;
    private QuestBall _EndPoint;
    private Camera _QuestIconCamera;
    private Color _ballColor;

    #endregion




    #region [Events]
    private void Awake() {
        /*
        // https://forum.unity.com/threads/how-to-save-manually-save-a-png-of-a-camera-view.506269
        // rendering icon textures dynamically, using a camera.
        Camera Cam = GetComponentInChildren<Camera>();
        if(Cam == null) throw new System.Exception("ahkdfahfd");
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;
        Cam.Render();
        questIcon = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        questIcon.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        questIcon.Apply();
        RenderTexture.active = currentRT;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetTexture("_BaseMap", questIcon);
        _mapIcon.GetComponentInChildren<Renderer>().SetPropertyBlock(block);
        */
    }

    void Start()
    {
        setState(_state);
    }

    /*(private void OnTriggerEnter(Collider other) {
        if(_triggerActive && other.gameObject == S_Player.Explorer) {
            _isInsideTrigger = true;
            setColorFade();
            if(_state == QuestState.next) { // meaning this questpoint was paired with another one, now they are both ready to be connected by a redline.
                _ParentQuest.setState(QuestState.completeable);
                setState(QuestState.completeable);
            }
            else if(_state == QuestState.undiscovered) {
                setState(QuestState.discovered);
            }
            //CameraOperator.lookAtObject = _lookTarget;
            //CameraOperator.doLookAtObject = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(_triggerActive && other.gameObject == S_Player.Explorer) {
            _isInsideTrigger = false;
            setColorFade();
            //CameraOperator.lookAtObject = null;
            //CameraOperator.doLookAtObject = false;
        }
    }*/
    #endregion;


    #region [Methods]
    private IEnumerator QuestSequence() {
        //_SoundPlayer.Play("Get");
        yield return new WaitForSeconds(0.688f);

    }

    private void setColorFade() {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", _ballColor);
        block.SetFloat("_SoftParticlesFarFadeDistance", 2);
        //_StartPointBallEffectObj.GetComponent<Renderer>().SetPropertyBlock(block);
        //_mapIconBorderObj.GetComponent<Renderer>().SetPropertyBlock(block);
    }

    public void setState(QuestState state) {
        /*_state = state;
        switch(_state) {
            case QuestState.hidden: { // Quest is invisible.
                _MinimapIconObj.SetActive(false);
                _ballColor = Expedition.questColorUniscovered;
                Debug.LogWarning("Questpoint '"+_questName+"' set to 'hidden'");
                break;
            }
            case QuestState.undiscovered: { // Quest can be found by exploring on foot.
                _MinimapIconObj.SetActive(false);
                _ballColor = Expedition.questColorUniscovered;
                Debug.LogWarning("Questpoint '"+_questName+"' set to 'undiscovered'");
                break;
            }
            case QuestState.discovered: { // Quest goal is visible on the minimap.
                _MinimapIconObj.SetActive(true);
                _ballColor = Expedition.questColorUncompleted;
                Debug.LogWarning("Questpoint '"+_questName+"' set to 'discovered'");
                if(_GoalQuest == null) { // Meaning this is the end of a chain of questpoints.
                    setState(QuestState.complete);
                }
                else if(_GoalQuest._state == QuestState.hidden) _GoalQuest.setState(QuestState.next);
                break;
            }
            case QuestState.completeable: { // Quest is ready to be red-lined.
                _MinimapIconObj.SetActive(true);
                _ballColor = Expedition.questColorCompleteable;
                Debug.LogWarning("Questpoint '"+_questName+"' set to 'completeable'");
                break;
            }
            case QuestState.complete: { // Quest is done.
                _MinimapIconObj.SetActive(true);
                _ballColor = Expedition.questColorCompleted;
                Debug.LogWarning("Questpoint '"+_questName+"' set to 'complete'");
                break;
            }
            case QuestState.next: { // Like discoverable, but it solves its parent quest.
                _MinimapIconObj.SetActive(false);
                _ballColor = Expedition.questColorUncompleted;
                Debug.LogWarning("Questpoint '"+_questName+"' set to 'next'");
                break;
            }
        }
        if(_state == QuestState.hidden) {
            _triggerActive = false;
            _StartPointBallEffectObj.SetActive(false);
        }
        else {
            _triggerActive = true;
            _StartPointBallEffectObj.SetActive(true);
            setColorFade();
        }*/
    }

    public void onEndpointEnter() {
        Debug.Log("Entered Endpoint");
    }

    public void onEndpointExit() {
        Debug.Log("Exited Endpoint");
    }

    public void onStartpointEnter() {
        Debug.Log("Entered Startpoint");
    }

    public void onStartpointExit() {
        Debug.Log("Exited Startpoint");
    }
    #endregion
    
}
