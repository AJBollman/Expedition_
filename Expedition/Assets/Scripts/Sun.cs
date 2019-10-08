using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private static bool _created = false;
    private void Awake()
    {
        if (_created) { GameObject.Destroy(this.gameObject); }
    }
    private void Start()
    {
        _created = true;
    }
}
