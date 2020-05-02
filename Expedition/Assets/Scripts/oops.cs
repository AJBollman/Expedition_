using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class oops : MonoBehaviour {
    [SerializeField] private Vector3 moveTo;

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "The Explorer") {
            //SceneManager.LoadScene(1, LoadSceneMode.Additive);
            other.gameObject.transform.position = moveTo;
            Expedition.EnterTitleScreenState();
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.gameObject.name == "The Explorer") {
            //SceneManager.LoadScene(1, LoadSceneMode.Additive);
            other.gameObject.transform.position = moveTo;
            Expedition.EnterTitleScreenState();
        }
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.name == "The Explorer") {
            //SceneManager.LoadScene(1, LoadSceneMode.Additive);
            other.gameObject.transform.position = moveTo;
            Expedition.EnterTitleScreenState();
        }
    }

    void OnCollisionExit(Collision other) {
        if(other.gameObject.name == "The Explorer") {
            //SceneManager.LoadScene(1, LoadSceneMode.Additive);
            other.gameObject.transform.position = moveTo;
            Expedition.EnterTitleScreenState();
        }
    }
}
