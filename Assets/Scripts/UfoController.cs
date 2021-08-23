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
        // TODO: Either one of these should be valid. If both are pressed, behave as none of them is pressed.
        if (Input.GetKey(KeyCode.Space))  // Accelerate depending of key pressed
            _ufoVelocityChange.y = 4 * Time.fixedDeltaTime;
        else if (Input.GetKey(KeyCode.LeftControl))
            _ufoVelocityChange.y = -4 * Time.fixedDeltaTime;
        else                              // No up/down key pressed, slow down vertically
        {
            if (_ufoRigidBody.velocity.y > 0)
                _ufoVelocityChange.y = -3 * Time.fixedDeltaTime;  // TODO: When making these constants, this value should be lower than those in the upper block
            else if (_ufoRigidBody.velocity.y < 0)
                _ufoVelocityChange.y = 3 * Time.fixedDeltaTime;
        }

        // TODO: Either one of these should be valid. If both are pressed, behave as none of them is pressed.
        if (Input.GetKey(KeyCode.W))      // Accelerate depending of key pressed
            _ufoVelocityChange.z = 6 * Time.fixedDeltaTime;
        else if (Input.GetKey(KeyCode.S))
            _ufoVelocityChange.z = -6 * Time.fixedDeltaTime;
        else                              // No forward/backward key pressed, slow down horizontally z
        {
            if (_ufoRigidBody.velocity.z > 0)
                _ufoVelocityChange.z = -5 * Time.fixedDeltaTime;
            else if (_ufoRigidBody.velocity.z < 0)
                _ufoVelocityChange.z = 5 * Time.fixedDeltaTime;
        }

        
        // Compute average Δ-time to find optimal value?
        if (_ufoRigidBody.velocity.magnitude > 0 && _ufoRigidBody.velocity.magnitude < .0001)
        {
            _ufoVelocityChange = Vector3.zero;
            _ufoRigidBody.velocity = Vector3.zero;
        }

        if (_ufoRigidBody.velocity.magnitude > MAXSpeed)
        {
            _ufoVelocityChange = Vector3.zero;
            _ufoRigidBody.velocity = _ufoRigidBody.velocity.normalized * MAXSpeed;
        }

        _infoText.text = "velocity = " + _ufoRigidBody.velocity.magnitude.ToString();
        _ufoRigidBody.AddForce(_ufoVelocityChange, ForceMode.VelocityChange);  // AddLocalForce has swapped y and z
        
        // Force	        Add a continuous force to the rigidbody, using its mass.
        // Acceleration	    Add a continuous acceleration to the rigidbody, ignoring its mass.
        // Impulse	        Add an instant force impulse to the rigidbody, using its mass.
        // VelocityChange	Add an instant velocity change to the rigidbody, ignoring its mass.
        
    }
}
