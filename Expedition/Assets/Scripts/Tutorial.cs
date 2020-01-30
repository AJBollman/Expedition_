using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public string onCompleteText;
    public string completeKey;
    public float smoothTime = 1f;
    public bool isButton;

    private Vector3 scaleGoal;
    private Vector3 initScale;
    private bool isActive;
    private TextMesh tm;
    private bool complete;
    private GameObject darkBall;

    // Start is called before the first frame update
    void Start()
    {
        tm = GetComponentInChildren<TextMesh>();
        darkBall = GetComponentInChildren<MeshRenderer>().gameObject;
        initScale = tm.transform.localScale;
        isActive = false;
        scaleGoal = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (complete) tm.color = new Color(33, 255, 174, 255);
        var check = (isButton) ? (Input.GetButtonDown(completeKey)) : (Input.GetKeyDown(completeKey));
        if (check && isActive && !complete)
        {
            complete = true;
            darkBall.SetActive(false);
            tm.text = onCompleteText;
            smoothTime = 1f;
            scaleGoal = Vector3.zero;
            StartCoroutine(onComplete(2f));
        }
        tm.transform.localScale = Vector3.Lerp(tm.transform.localScale, scaleGoal, Time.deltaTime * smoothTime);
        darkBall.transform.localScale = new Vector3(
            darkBall.transform.localScale.x,
            Mathf.Lerp(darkBall.transform.localScale.y, (isActive) ? 17 : 0, Time.deltaTime * smoothTime),
            darkBall.transform.localScale.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (complete) return;
        if (other.tag == "Player")
        {
            isActive = true;
            scaleGoal = initScale;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (complete) return;
        if (other.tag == "Player")
        {
            isActive = false;
            scaleGoal = Vector3.zero;
        }
    }

    private IEnumerator onComplete(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
