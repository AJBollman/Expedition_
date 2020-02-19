
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class Quest : MonoBehaviour
{
    public static List<Quest> AllQuests;

    #region [Public]
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

    [Tooltip("If this quest is initially visible on game start")]
    [SerializeField] private bool _isVisibleOnStart;

    #endregion

    #region [Private]
    private QuestState _state = QuestState.hidden;
    private Quest _ParentQuest;
    private QuestBall _StartPoint;
    private QuestBall _EndPoint;
    private Camera _QuestIconCamera;
    private Color _ballColor;

    #endregion




    #region [Events]
    private void Awake() {
        _StartPoint = transform.Find("StartPoint").GetComponent<QuestBall>();
        _EndPoint = transform.Find("EndPoint").GetComponent<QuestBall>();
        AllQuests.Add(this);
    }

    private void OnDestroy() {
        AllQuests.Remove(this);
    }

    void Start()
    {
        if(_generateIconFromCamera) {
            // https://forum.unity.com/threads/how-to-save-manually-save-a-png-of-a-camera-view.506269
            // rendering icon textures dynamically, using a camera.
            Camera Cam = GetComponentInChildren<Camera>();
            if(Cam == null) throw new System.Exception("ahkdfahfd");
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = Cam.targetTexture;
            Cam.Render();
            _QuestIcon = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
            _QuestIcon.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
            _QuestIcon.Apply();
            RenderTexture.active = currentRT;

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetTexture("_BaseMap", _QuestIcon);
            _EndPoint._MinimapIconObj.GetComponentInChildren<Renderer>().SetPropertyBlock(block);
            _StartPoint._MinimapIconObj.GetComponentInChildren<Renderer>().SetPropertyBlock(block);
        }
        setState((_isVisibleOnStart) ? QuestState.undiscovered : QuestState.hidden);
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

    /*private void setColorFade() {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", _ballColor);
        block.SetFloat("_SoftParticlesFarFadeDistance", 2);
        //_StartPointBallEffectObj.GetComponent<Renderer>().SetPropertyBlock(block);
        //_mapIconBorderObj.GetComponent<Renderer>().SetPropertyBlock(block);
    }*/

    private void setVisibility(bool showStart, bool showEnd, bool showStartIcon, bool showEndIcon) {
        _EndPoint.gameObject.SetActive(showEnd);
        _StartPoint.gameObject.SetActive(showStart);
        _EndPoint._MinimapIconObj.SetActive(showEndIcon);
        _StartPoint._MinimapIconObj.SetActive(showStartIcon);
    }

    public void setState(QuestState state) {
        _state = state;
        switch(_state) {
            case QuestState.hidden: { // Quest is invisible.
                setVisibility(false, false, false, false);
                _ballColor = Expedition.questColorUniscovered;
                break;
            }
            case QuestState.undiscovered: { // Quest can be found by exploring on foot.
                setVisibility(true, false, false, false);
                _ballColor = Expedition.questColorUniscovered;
                break;
            }
            case QuestState.discovered: { // Quest goal is visible on the minimap.
                setVisibility(true, true, true, false);
                _ballColor = Expedition.questColorUncompleted;
                break;
            }
            case QuestState.completeable: { // Quest is ready to be red-lined.
                setVisibility(true, true, true, true);
                _ballColor = Expedition.questColorCompleteable;
                break;
            }
            case QuestState.complete: { // Quest is done.
                setVisibility(true, true, true, true);
                _ballColor = Expedition.questColorCompleted;
                break;
            }
        }
        _EndPoint.SetBallColor(_ballColor);
        _StartPoint.SetBallColor(_ballColor);
        Debug.Log("Questpoint set to "+_state);
    }

    public void onEndpointEnter() {
        Debug.Log("Entered Endpoint");
        switch(_state) {
            case QuestState.undiscovered: { // Quest can be found by exploring on foot.
                return;
                break;
            }
            case QuestState.discovered: { // Quest goal is visible on the minimap.
                setState(QuestState.completeable);
                break;
            }
            case QuestState.completeable: { // Quest is ready to be red-lined.
                break;
            }
            case QuestState.complete: { // Quest is done.
                break;
            }
        }
    }

    public void onEndpointExit() {
        Debug.Log("Exited Endpoint");
        switch(_state) {
            case QuestState.undiscovered: { // Quest can be found by exploring on foot.
                return;
                break;
            }
            case QuestState.discovered: { // Quest goal is visible on the minimap.
                break;
            }
            case QuestState.completeable: { // Quest is ready to be red-lined.
                break;
            }
            case QuestState.complete: { // Quest is done.
                break;
            }
        }
    }

    public void onStartpointEnter() {
        Debug.Log("Entered Startpoint");
        switch(_state) {
            case QuestState.undiscovered: { // Quest can be found by exploring on foot.
                setState(QuestState.discovered);
                break;
            }
            case QuestState.discovered: { // Quest goal is visible on the minimap.
                break;
            }
            case QuestState.completeable: { // Quest is ready to be red-lined.
                break;
            }
            case QuestState.complete: { // Quest is done.
                break;
            }
        }
    }

    public void onStartpointExit() {
        Debug.Log("Exited Startpoint");
        switch(_state) {
            case QuestState.undiscovered: { // Quest can be found by exploring on foot.
                break;
            }
            case QuestState.discovered: { // Quest goal is visible on the minimap.
                break;
            }
            case QuestState.completeable: { // Quest is ready to be red-lined.
                break;
            }
            case QuestState.complete: { // Quest is done.
                break;
            }
        }
    }
    #endregion
    
}
