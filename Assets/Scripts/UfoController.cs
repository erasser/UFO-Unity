using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO:  Limit speed by applying magnitude of velocity vector, so speed of particular axes are not independent and so UFO has its 'total' max speed limit

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
    private const int MAXSpeed = 5;
    
    void Start()
    {
        _ufo = GameObject.Find("UFO");      // TODO: This is this! :D
        _ufoRigidBody = _ufo.GetComponent<Rigidbody>();
        _ufoLights = GameObject.Find("UfoLights");
        _infoText = GameObject.Find("InfoText").GetComponent<Text>();
        
        var tmp = new Vector3(0,3,4);
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
            // if (_ufoRigidBody.velocity.y < 5)  // max speed limit not reached yet
            _ufoVelocityChange.y = 4 * Time.fixedDeltaTime;
            // else
            //     _ufoVelocityChange.y = 0;      // max speed limit reached, don't accelerate
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            // if (_ufoRigidBody.velocity.y > -5)
            //     _ufoVelocityChange.y = -4 * Time.fixedDeltaTime;
            // else
            //     _ufoVelocityChange.y = 0;
        }
        else  // No up/down key pressed, slow down
        {
/*            if (_ufoRigidBody.velocity.y > 0)
            {
                _ufoVelocityChange.y = -5 * Time.fixedDeltaTime;
            }
            else if (_ufoRigidBody.velocity.y < 0)
            {
                _ufoVelocityChange.y = 5 * Time.fixedDeltaTime;
            }

            // Compute average Δ-time to find optimal value?
            // if (Mathf.Abs(_ufoRigidBody.velocity.y) > 0 && _ufoRigidBody.velocity.y < .0001)  // this value is present elsewhere too
            if (Mathf.Abs(_ufoRigidBody.velocity.magnitude) > 0 && _ufoRigidBody.velocity.magnitude < .0001)  // this value is present elsewhere too
            {
                _ufoVelocityChange.y = 0;
                _ufoRigidBody.velocity = new Vector3(0, 0, 0);
            }
*/
        }
        
        if (_ufoRigidBody.velocity.magnitude > MAXSpeed)
            _ufoRigidBody.velocity = _ufoRigidBody.velocity.normalized * MAXSpeed;  // i.e. set length
        
/*
        if (Input.GetKey(KeyCode.W))
        {
            _ufoVelocityChange.z = 6 * Time.fixedDeltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _ufoVelocityChange.z = -6 * Time.fixedDeltaTime;
        }
        else  // No forward/backward key pressed, slow down
        {
            if (_ufoRigidBody.velocity.z > 0)
            {
                _ufoVelocityChange.z = -8 * Time.fixedDeltaTime;
            }
            else if (_ufoRigidBody.velocity.z < 0)
            {
                _ufoVelocityChange.z = 8 * Time.fixedDeltaTime;
            }

            if (Mathf.Abs(_ufoRigidBody.velocity.z) > 0 && _ufoRigidBody.velocity.y < .0001)
            {
                _ufoVelocityChange.z = 0;
                _ufoRigidBody.velocity = new Vector3(0, 0, 0);
            }
        }
*/
        _ufoRigidBody.AddForce(_ufoVelocityChange, ForceMode.VelocityChange);  // AddLocalForce has swapped y and z
        _infoText.text = "velocity = " + _ufoRigidBody.velocity.magnitude.ToString();
        
        // Force	        Add a continuous force to the rigidbody, using its mass.
        // Acceleration	    Add a continuous acceleration to the rigidbody, ignoring its mass.
        // Impulse	        Add an instant force impulse to the rigidbody, using its mass.
        // VelocityChange	Add an instant velocity change to the rigidbody, ignoring its mass.
        
    }
}
