using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public sealed class AutoInitializer : MonoBehaviour
{
    private static readonly string n = "Overlay";
    private static bool initInEditor;
    private bool b;
    private void Update() {
        if(b) return;
        b=true;
        if(Application.isEditor && !Application.isPlaying) {
            if(EditorSceneManager.GetSceneByName(n).isLoaded) return;
            EditorSceneManager.OpenScene("Assets/Scenes/"+n+".unity", OpenSceneMode.Additive);
        }
        else {
            if(SceneManager.GetSceneByName(n).isLoaded) return;
            SceneManager.LoadScene(n, LoadSceneMode.Additive);
        }
    }
}
