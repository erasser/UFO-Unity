using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSprite : MonoBehaviour
{
    void Start()
    {
    }

    void FixedUpdate()
    {
        UpdateMe();
    }

    private void UpdateMe()
    {
        transform.LookAt(Ufo.UfoCamera.transform.position);
    }

    private void OnDestroy()
    {
        print("Selection sprite destroyed");
    }
}
