using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private GameObject _cubeHelper;
    public float angle = 0;
    public float x, z;
    public bool hasCollided;
    
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Performance.ShowFPS();
    }

    void FixedUpdate()
    {
        if (!hasCollided)
        {
            angle -= Time.fixedDeltaTime / 10;

            x = Mathf.Cos(angle) * 4000;
            z = Mathf.Sin(angle) * 4000;
            
            _rigidBody.MovePosition(new Vector3(x, 0, z));
        }





        /* Další možnost: Každej by mohl mít svýho parenta, kterej by s ním točil = obdobnej přístup jako ten úplně původní */

    }

    private void OnCollisionEnter(Collision other)
    {
        print("collided!");
        hasCollided = true;
    }
}
