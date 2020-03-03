
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class Quest : MonoBehaviour
{
    public static List<Quest> AllQuests = new List<Quest>();

    #region [Public]

    [HideInInspector] public Transform proxyScroll { get; private set; }
    [HideInInspector] public static Quest Active { get; private set; }
    [HideInInspector] public QuestBall StartPoint { get; private set; }
    [HideInInspector] public QuestBall EndPoint { get; private set; }
    [HideInInspector] public QuestState state { get; private set; } = QuestState.hidden;

    [Tooltip("All quests to show after this one is completed")]
    [SerializeField] private List<Quest> _QuestsToUnlockOnCompletion;

    [Tooltip("Each object in this list has it's active state toggled on/off on completion of this quest")]
    [SerializeField] private List<GameObject> _ObjectsToToggleOnCompletion;

    [Tooltip("The least number of redline points that can be used to beat this quest")]
    [SerializeField] private int _bestPossibleScore;

    //[Tooltip("Icon to show on the map")]
    [SerializeField] public Texture2D QuestIcon { get; private set; }

    [Tooltip("Generate icon procedurally using the quest icon camera")]
    [SerializeField] private bool _generateIconFromCamera;

    [Tooltip("Description to show in some places")]
    [SerializeField] private string _flavorText;

    //[Tooltip("The Traveller associated with this quest")]
    [SerializeField] public TravellerType travellerType { get; private set; }

    [Tooltip("If this quest is initially visible on game start")]
    [SerializeField] private bool _isVisibleOnStart;

    private List<LineVertex> RedLine;

    #endregion

    #region [Private]
    private Quest _ParentQuest;
    private Camera _QuestIconCamera;
    private Color _ballColor;

    #endregion




    #region [Events]
    private void Awake() {
        StartPoint = transform.Find("StartPoint").GetComponent<QuestBall>();
        EndPoint = transform.Find("EndPoint").GetComponent<QuestBall>();
        proxyScroll = transform.Find("Quest Scroll Proxy");
        proxyScroll.gameObject.SetActive(false);
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
            QuestIcon = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
            QuestIcon.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
            QuestIcon.Apply();
            RenderTexture.active = currentRT;

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetTexture("_BaseMap", QuestIcon);
            EndPoint._MinimapIconObj.GetComponentInChildren<Renderer>().SetPropertyBlock(block);
            StartPoint._MinimapIconObj.GetComponentInChildren<Renderer>().SetPropertyBlock(block);
        }
        setState((_isVisibleOnStart) ? QuestState.undiscovered : QuestState.hidden);
    }
    #endregion;


    #region [Methods]
    private IEnumerator QuestSequence() {
        //_SoundPlayer.Play("Get");
        yield return new WaitForSeconds(0.688f);

    }

    private void setVisibility(bool showStart, bool showEnd, bool showStartIcon, bool showEndIcon) {
        EndPoint._MinimapIconObj.SetActive(showEndIcon);
        StartPoint._MinimapIconObj.SetActive(showStartIcon);
        EndPoint.gameObject.SetActive(showEnd);
        StartPoint.gameObject.SetActive(showStart);
    }

    public void setState(QuestState state) {
        this.state = state;
        switch(this.state) {
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
                    foreach(GameObject g in _ObjectsToToggleOnCompletion) {
                        g.SetActive(!g.activeSelf);
                    }
                    foreach(Quest q in _QuestsToUnlockOnCompletion) {
                        q.setState(QuestState.undiscovered);
                    }
                    Expedition.CheckGameCompletion();
                break;
            }
        }
        EndPoint.SetBallColor(_ballColor);
        StartPoint.SetBallColor(_ballColor);
        //Debug.Log("Questpoint set to "+_state);
    }

    public void onStartpointEnter() {
        Active = this;
        //Debug.Log("Entered Startpoint");
        switch(state) {
            case QuestState.undiscovered: { // Quest can be found by exploring on foot.
                setState(QuestState.discovered);
                Expedition.CinematicGetQuest(this);
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
        Active = null;
        //Debug.Log("Exited Startpoint");
        switch(state) {
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

    public void onEndpointEnter() {
        Active = this;
        //Debug.Log("Entered Endpoint");
        switch(state) {
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
        Active = null;
        //Debug.Log("Exited Endpoint");
        switch(state) {
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

    public void SetRedLine(List<LineVertex> redLine) {
        // validation
        RedLine = redLine;
        // setstate complete
    }

    public void onTravellerEnter() {
        setState(QuestState.complete);
        Traveller.DestroyActive();
        Expedition.Map.ConfirmRedline();
    }

    #endregion
    
}
