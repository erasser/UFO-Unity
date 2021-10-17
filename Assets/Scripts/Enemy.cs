using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameController _gameControllerScript;
    private Rigidbody _rigidBody;
    public int hull;
    
    void Start()
    {
        // _gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
        _rigidBody = GetComponent<Rigidbody>();
        _gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();

        if (hull == 0)
            throw new Exception("This enemy has not assigned hull value!");
    }

    void FixedUpdate()
    {
        _rigidBody.transform.Translate(Vector3.forward / 10);
    }

    public void GetDamage(int damage)
    {
        // print(name + " takes " + damage + " damage!");
        hull -= damage;
        
        if (hull <= 0)
            _gameControllerScript.DestroyGameObject(gameObject);
    }
}