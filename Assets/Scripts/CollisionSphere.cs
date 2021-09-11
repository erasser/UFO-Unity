using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // print(" • Collided with " + other.gameObject.name);
        // print(" • Collided with " + other.gameObject.tag);
        // if (other.CompareTag("asteroid"))
        //     return true;

        AsteroidController.CollidedAsteroid();
    }
    
    
}
