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
    private Vector3 _ufoVelocityChange;  // Add an instant velocity change to the rigidbody (ignoring its mass)
    private static Vector3 _ufoRotationChange;
    private static Text _infoText;  // UI element
    private static GameObject _arrowHelper;
    private static GameObject _cubeHelper;
    private const int MAXSpeed = 5;     // Now the drag property takes care of this
    private float _timeInterval;
    
    void Start()
    {
        _ufo = GameObject.Find("UFO");      // TODO: This is this! :D
        _ufoRigidBody = _ufo.GetComponent<Rigidbody>();
        _ufoLights = GameObject.Find("UfoLights");
        _infoText = GameObject.Find("InfoText").GetComponent<Text>();
        _arrowHelper = GameObject.Find("arrow");
        _cubeHelper = GameObject.Find("CubeHelper");

        // _arrowHelper.GetComponent<MeshRenderer>().enabled = false;
    }

    void FixedUpdate()
    {
        _ufoLights.transform.Rotate(0, 0, Time.fixedDeltaTime * -100);

        MoveUfo();
        
        if (Input.GetKeyDown(KeyCode.F))  // switch gravity
        {
            var drag = _ufoRigidBody.drag;
            _ufoRigidBody.useGravity = !_ufoRigidBody.useGravity;

            if (_ufoRigidBody.useGravity)
                _ufoRigidBody.drag = .5f;
            else
                _ufoRigidBody.drag = drag;
        }

        if (Input.GetKey(KeyCode.R))  // auto level
        {
            // _ufoRigidBody.transform.rotation = Quaternion.Euler(0, _ufoRigidBody.transform.rotation.y, 0);
            // TODO: Do auto leveling by adding torque
        }
    }

    private void MoveUfo()
    {
        /******* MOVEMENT *******/
        // TODO: Either one of these should be valid. If both are pressed, behave as none of them is pressed.
        if (Input.GetKey(KeyCode.Space))  // Accelerate depending of key pressed
            _ufoVelocityChange.y = 20 * Time.fixedDeltaTime;
        else if (Input.GetKey(KeyCode.LeftControl))
            _ufoVelocityChange.y = -20 * Time.fixedDeltaTime;
        else                              // No up/down key pressed, slow down vertically
        // {
        //     if (_ufoRigidBody.velocity.y > 0)
        //         _ufoVelocityChange.y = -3 * Time.fixedDeltaTime;  // TODO: When making these constants, this value should be lower than those in the upper block
        //     else if (_ufoRigidBody.velocity.y < 0)
        //         _ufoVelocityChange.y = 3 * Time.fixedDeltaTime;
        
            _ufoVelocityChange.y = 0;
        // }

        // _ufoRigidBody.drag = 4;
        // TODO: Either one of these should be valid. If both are pressed, behave as none of them is pressed.
        if (Input.GetKey(KeyCode.W))      // Accelerate depending of key pressed
            _ufoVelocityChange.z = 30 * Time.fixedDeltaTime;
        else if (Input.GetKey(KeyCode.S))
            _ufoVelocityChange.z = -30 * Time.fixedDeltaTime;
        else                              // No forward/backward key pressed, slow down horizontally z
        {
            // if (_ufoRigidBody.velocity.z > 0)
            //     ufoVelocityChange.z = -5 * Time.fixedDeltaTime;
            // else if (_ufoRigidBody.velocity.z < 0)
            //     ufoVelocityChange.z = 5 * Time.fixedDeltaTime;

            _ufoVelocityChange.z = 0;
            // _ufoRigidBody.velocity = Vector3.zero;
            // _ufoRigidBody.drag = 8;
        }

        if (Input.GetKey(KeyCode.Q))
            _ufoVelocityChange.x = 20 * Time.fixedDeltaTime;
        else if (Input.GetKey(KeyCode.E))
            _ufoVelocityChange.x = -20 * Time.fixedDeltaTime;
        else
            _ufoVelocityChange.x = 0;


        // Compute average Δ-time to find optimal value?
        // if (_ufoRigidBody.velocity.magnitude > 0 && _ufoRigidBody.velocity.magnitude < .0001)
        // {
        //     ufoVelocityChange = Vector3.zero;
        //     _ufoRigidBody.velocity = Vector3.zero;
        // }
        //
        // if (_ufoRigidBody.velocity.magnitude > MAXSpeed)
        // {
        //     _ufoVelocityChange = Vector3.zero;
        //     _ufoRigidBody.velocity = _ufoRigidBody.velocity.normalized * MAXSpeed;
        // }

        _infoText.text = _ufoRigidBody.velocity.magnitude.ToString();
        _ufoRigidBody.AddRelativeForce(_ufoVelocityChange, ForceMode.VelocityChange);

        // _ufoVelocityChangeSwappedXY.x = _ufoVelocityChange.x;
        // _ufoVelocityChangeSwappedXY.y = _ufoVelocityChange.z;
        // _ufoVelocityChangeSwappedXY.z = _ufoVelocityChange.y;
        // _ufoRigidBody.AddRelativeForce(_ufoVelocityChangeSwappedXY, ForceMode.VelocityChange);  // AddLocalForce has swapped y and z
        
        // Force	        Add a continuous force to the rigidbody, using its mass.
        // Acceleration	    Add a continuous acceleration to the rigidbody, ignoring its mass.
        // Impulse	        Add an instant force impulse to the rigidbody, using its mass.
        // VelocityChange	Add an instant velocity change to the rigidbody, ignoring its mass.
        
        /******* ROTATION *******/
        // TODO: Either one of these should be valid. If both are pressed, behave as none of them is pressed.
        if (Input.GetKey(KeyCode.A))
        {
            _ufoRotationChange.y = -8 * Time.fixedDeltaTime;
            // _ufoRotationChange.y = -80 / (10f + _ufoRigidBody.velocity.magnitude * 3);  // there is also sqrMagnitude
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _ufoRotationChange.y = 8 * Time.fixedDeltaTime;
        }
        else
        {
            _ufoRotationChange.y = 0;
            // _ufoRigidBody.angularVelocity = Vector3.zero;
        }

        _ufoRigidBody.AddRelativeTorque(_ufoRotationChange, ForceMode.VelocityChange);
        // _ufoRigidBody.transform.Rotate(_ufoRotationChange);  // with _ufoRotationChange.y = 80 * Time.fixedDeltaTime;

        // _arrowHelper.GetComponent<MeshRenderer>().enabled = _ufoRigidBody.velocity.magnitude != 0;
        // _arrowHelper.transform.rotation = Quaternion.LookRotation(_ufoRigidBody.velocity.normalized);
        // _arrowHelper.transform.rotation = Quaternion.LookRotation(_ufoVelocityChange.normalized);


        // _timeInterval += Time.fixedDeltaTime;
        // if (_timeInterval > .04f)
        // {
        //     _timeInterval = 0;
        //     var cubeHelper = Instantiate(_cubeHelper);
        //     cubeHelper.transform.position = _ufoRigidBody.transform.position;  // Its not a reference, because its a struct?
        // }
    }
}
