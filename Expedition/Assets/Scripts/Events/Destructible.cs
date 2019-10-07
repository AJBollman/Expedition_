using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public Transform Original;
    public List<GameObject> Prefabs;
    private GameObject CurrentRock;

    public float prefabCount = 0;
    public float CountDownValue = 10;
    public bool StartTimer;

    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    private void Awake()
    {
        Instantiate(Prefabs[0], Original.position, Original.rotation, Original);
    }

    private void Update()
    {
        if (StartTimer == false)
        {
            StopAllCoroutines();
        }
        if (CountDownValue == 0)
        {
            prefabCount++;
            SpawnObjects();
            CountDownValue = 10;
        }
    }

    private void SpawnObjects()
    {
        switch (prefabCount)
        {
            case (0):
                Instantiate(Prefabs[0], Original.position, Original.rotation, Original);
                //Destroy(gameObject);
                break;
            case (1):

                Instantiate(Prefabs[1], Original.position, Original.rotation, Original);
                break;
            case (2):
                Instantiate(Prefabs[2], Original.position, Original.rotation, Original);
                break;
            case (3):
                Instantiate(Prefabs[3], Original.position, Original.rotation, Original);
                break;
            case (4):
                Instantiate(Prefabs[4], Original.position, Original.rotation, Original);
                break;
            default:
                break;
        }
    }

    private void OnMouseDown()
    {
        StartCoroutine(StartCountdown());
        StartTimer = true;
    }
    private void OnMouseOver()
    {
        StartTimer = true;
    }
    private void OnMouseExit()
    {
        StartTimer = false;
    }
    public IEnumerator StartCountdown()
    {
        while (CountDownValue >= 0)
        {
            Debug.Log("Countdown: " + CountDownValue);
            yield return new WaitForSeconds(1.0f);
            CountDownValue--;
        }
    }
}
