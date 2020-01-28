#if (UNITY_EDITOR)

// This keeps us from doing weird stuff with Region prefabs in the editor.
// There's an option for allowing rotation, if we want to get really freaky.

using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RegionEditModeTweaks : MonoBehaviour
{
    public bool canRotate = false;

    void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            if(Selection.Contains(gameObject))
            {
                transform.localScale = new Vector3(transform.localScale.x, 1000, transform.localScale.x);
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.position.z);
                if (!canRotate) transform.rotation = new Quaternion(0, 0, 0, 0);
                GetComponentInChildren<Camera>().orthographicSize = transform.localScale.x / 2;
            }
        }
        else this.enabled = false;
    }

}

#endif