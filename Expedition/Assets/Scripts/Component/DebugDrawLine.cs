using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugDrawLine : MonoBehaviour
{
    [SerializeField] private Transform _Target;
    [SerializeField] private Color _Color = Color.cyan;

    void Update()
    {
        if(_Target != null) {
            Debug.DrawLine(transform.position, _Target.position, _Color, 0, false);
        }
    }
}
