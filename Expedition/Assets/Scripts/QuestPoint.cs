
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum QuestState { hidden, undiscovered, discovered, completeable, complete, next };

[RequireComponent(typeof(SoundPlayer))]
public sealed class QuestPoint : MonoBehaviour
{
    [SerializeField] public Texture2D questIcon;

    [SerializeField] public QuestPoint goalQuest;
    [HideInInspector] public QuestPoint parentQuest;

    [SerializeField] private QuestState _state;
    [SerializeField] private string questName;

    private GameObject _mapIcon;
    private GameObject _mapIconBg;
    private GameObject _ballEffect;
    private GameObject _lookTarget;
    private bool _triggerActive;
    private bool _isInsideTrigger;
    private Color _color;
    private SoundPlayer _sound;

    private void Awake() {
        _sound = GetComponent<SoundPlayer>();
        _mapIcon = transform.Find("MapIcon").gameObject;
        _mapIconBg = _mapIcon.transform.Find("MapIconBackground").gameObject;
        _lookTarget = transform.Find("LookTarget").gameObject;
        _ballEffect = transform.Find("BallEffect").gameObject;

        // https://forum.unity.com/threads/how-to-save-manually-save-a-png-of-a-camera-view.506269
        // rendering icon textures dynamically, using a camera.
        /*Camera Cam = GetComponentInChildren<Camera>();
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
        _mapIcon.GetComponentInChildren<Renderer>().SetPropertyBlock(block);*/
    }

    void Start()
    {
        setState(_state);
        if(goalQuest != null) goalQuest.parentQuest = this;
    }

    void Update()
    {
        if (!EditorApplication.isPlaying && Selection.Contains(gameObject) && goalQuest != null) Debug.DrawLine(transform.position, goalQuest.transform.position, Color.cyan, 0.1f, false);
        if(Player.isInQuestZone) {
            
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(_triggerActive && other.gameObject == Player.Explorer) {
            _isInsideTrigger = true;
            setColorFade();
            if(_state == QuestState.next) { // meaning this questpoint was paired with another one, now they are both ready to be connected by a redline.
                parentQuest.setState(QuestState.completeable);
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
        if(_triggerActive && other.gameObject == Player.Explorer) {
            _isInsideTrigger = false;
            setColorFade();
            //CameraOperator.lookAtObject = null;
            //CameraOperator.doLookAtObject = false;
        }
    }

    private IEnumerator QuestSequence() {
        _sound.Play("Get");
        yield return new WaitForSeconds(0.688f);

    }

    private void setColorFade() {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", _color);
        block.SetFloat("_SoftParticlesFarFadeDistance", 2);
        _ballEffect.GetComponent<Renderer>().SetPropertyBlock(block);
        _mapIconBg.GetComponent<Renderer>().SetPropertyBlock(block);
    }

    public void setState(QuestState state) {
        _state = state;
        switch(_state) {
            case QuestState.hidden: { // Quest is invisible.
                _mapIcon.SetActive(false);
                _color = Expedition.questColorUniscovered;
                Debug.LogWarning("Questpoint '"+questName+"' set to 'hidden'");
                break;
            }
            case QuestState.undiscovered: { // Quest can be found by exploring on foot.
                _mapIcon.SetActive(false);
                _color = Expedition.questColorUniscovered;
                Debug.LogWarning("Questpoint '"+questName+"' set to 'undiscovered'");
                break;
            }
            case QuestState.discovered: { // Quest goal is visible on the minimap.
                _mapIcon.SetActive(true);
                _color = Expedition.questColorUncompleted;
                Debug.LogWarning("Questpoint '"+questName+"' set to 'discovered'");
                if(goalQuest == null) { // Meaning this is the end of a chain of questpoints.
                    setState(QuestState.complete);
                }
                else if(goalQuest._state == QuestState.hidden) goalQuest.setState(QuestState.next);
                break;
            }
            case QuestState.completeable: { // Quest is ready to be red-lined.
                _mapIcon.SetActive(true);
                _color = Expedition.questColorCompleteable;
                Debug.LogWarning("Questpoint '"+questName+"' set to 'completeable'");
                break;
            }
            case QuestState.complete: { // Quest is done.
                _mapIcon.SetActive(true);
                _color = Expedition.questColorCompleted;
                Debug.LogWarning("Questpoint '"+questName+"' set to 'complete'");
                /*if(goalQuest != null) {
                    goalQuest.setState(QuestState.complete);
                }*/
                break;
            }
            case QuestState.next: { // Like discoverable, but it solves its parent quest.
                _mapIcon.SetActive(false);
                _color = Expedition.questColorUncompleted;
                Debug.LogWarning("Questpoint '"+questName+"' set to 'next'");
                break;
            }
        }
        if(_state == QuestState.hidden) {
            _triggerActive = false;
            _ballEffect.SetActive(false);
        }
        else {
            _triggerActive = true;
            _ballEffect.SetActive(true);
            setColorFade();
        }
    }
}
