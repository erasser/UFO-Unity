// using System;
// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class UfoForceBeamCollisionController : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        print("Collided!");
        if (other.gameObject.name == "Cow")
        {
            print("Beaming a cow!");
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
