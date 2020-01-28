#if false
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour
{
    private Light dir;

    private void OnEnable() {
        dir = GetComponent<Light>();
    }

    private void Update() {
        // Switch to soft shadows if not moving and FPS > 30
        //dir.shadows = (Input.GetAxis("Horizontal") < 0.2f && Input.GetAxis("Vertical") < 0.2f && Time.deltaTime < 0.02f) ? LightShadows.Soft : LightShadows.Hard;
    }
}
#endif