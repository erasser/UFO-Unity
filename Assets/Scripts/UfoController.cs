using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO:  Z is up axis

public class UfoController : MonoBehaviour
{
    // I don't know why I did it static
    private static GameObject _ufoLights;
    private static GameObject _ufo;
    private static Rigidbody _ufoRigidBody;
    // private static Vector3 _ufoPhysicalForce;
    private static Vector3 _ufoVelocityChange;  // Add an instant velocity change to the rigidbody (ignoring its mass)
    private static Vector3 _ufoNewVelocity;  // Add an instant velocity change to the rigidbody (ignoring its mass)
    private static Text _infoText;  // UI element
    
    void Start()
    {
        _ufo = GameObject.Find("UFO");      // TODO: This is this! :D
        _ufoRigidBody = _ufo.GetComponent<Rigidbody>();
        _ufoLights = GameObject.Find("UfoLights");
        _infoText = GameObject.Find("InfoText").GetComponent<Text>();
    }

    void FixedUpdate()
    {
        _ufoLights.transform.Rotate(0, 0, Time.fixedDeltaTime * -100);

        MoveUfo();
        
        if (Input.GetKey(KeyCode.F))  // switch gravity
        {
            _ufoRigidBody.useGravity = !_ufoRigidBody.useGravity;
        }
    }
    
    void MoveUfo()  // Classic movement model.
    {
        // _infoText.text = $"velocity: {_ufoRigidBody.velocity.magnitude.ToString()}";
        // _infoText.text = _ufoRigidBody.velocity.z.ToString();

        if (Input.GetKey(KeyCode.Space))  // Accelerate depending of key pressed
        {
            if (_ufoRigidBody.velocity.y < 5)  // max speed limit not reached yet
                _ufoVelocityChange.y = 4f * Time.fixedDeltaTime;
            else
                _ufoVelocityChange.y = 0;      // max speed limit reached, don't accelerate
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            if (_ufoRigidBody.velocity.y > -5)
                _ufoVelocityChange.y = -4f * Time.fixedDeltaTime;
            else
                _ufoVelocityChange.y = 0;
        }
        else  // No key pressed, slow down
        {
            
            if (_ufoRigidBody.velocity.y > 0)
            {
                _ufoVelocityChange.y = -8f * Time.fixedDeltaTime;
            }
            else if (_ufoRigidBody.velocity.y < 0)
            {
                _ufoVelocityChange.y = 0;
                _ufoRigidBody.velocity = new Vector3(0, 0, 0);
            }
            

        }

        _infoText.text = _ufoRigidBody.velocity.y.ToString();            
            
        _ufoRigidBody.AddForce(_ufoVelocityChange, ForceMode.VelocityChange);  // AddLocalForce has swapped y and z

        
        // Force	        Add a continuous force to the rigidbody, using its mass.
        // Acceleration	    Add a continuous acceleration to the rigidbody, ignoring its mass.
        // Impulse	        Add an instant force impulse to the rigidbody, using its mass.
        // VelocityChange	Add an instant velocity change to the rigidbody, ignoring its mass.
        
    }

}
