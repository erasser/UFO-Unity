using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.UI;

// TODO:  Limit speed by applying magnitude of velocity vector, so speed of particular axes are not independent and so UFO has its 'total' max speed limit

public class UfoController : MonoBehaviour
{
    // I don't know why I did it static
    private static GameObject _ufo;
    private static GameObject _ufoLights;
    private static GameObject _ufoForceBeam;
    private static Rigidbody _ufoRigidBody;
    private Vector3 _ufoVelocityChange;  // Add an instant velocity change to the rigidbody (ignoring its mass)
    private static Vector3 _ufoRotationChange;
    private static GameObject _arrowHelper;
    private static GameObject _cubeHelper;
    private GameObject _ufoCamera;
    private float _initialCameraUfoDistance;
    private GameObject _topCamera;
    private float _timeInterval;
    private static bool _forceBeamEnabled;
    private static Text _infoText;         // UI element
    private const int MAXSpeed = 5;     // Now the drag property takes care of this
    
    void Start()
    {
        _ufo = GameObject.Find("UFO");      // TODO: This is this! :D  // Odstranit _ufo z této třídy, zbytečné
        _ufoRigidBody = _ufo.GetComponent<Rigidbody>();
        _ufoLights = GameObject.Find("UfoLights");
        _ufoForceBeam = GameObject.Find("ForceBeam");
        _infoText = GameObject.Find("InfoText").GetComponent<Text>();
        _arrowHelper = GameObject.Find("arrow");
        _cubeHelper = GameObject.Find("CubeHelper");
        _ufoCamera = GameObject.Find("CameraUfo");
        _topCamera = GameObject.Find("CameraTop");

        _initialCameraUfoDistance = _ufoCamera.transform.localPosition.z;  // It's set in editor and it's the minimum distance

        // _ufoForceBeam.GetComponent<CapsuleCollider>().enabled = false;
        _ufoForceBeam.SetActive(false);
        
        // _arrowHelper.GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))  // switch gravity
        {
            var drag = _ufoRigidBody.drag;
            _ufoRigidBody.useGravity = !_ufoRigidBody.useGravity;

            if (_ufoRigidBody.useGravity)
                _ufoRigidBody.drag = .5f;
            else
                _ufoRigidBody.drag = drag;
        }

        if (Input.GetKeyDown(KeyCode.F))  // switch force field
        {
            _forceBeamEnabled = !_forceBeamEnabled;
            _ufoForceBeam.SetActive(_forceBeamEnabled);
        
            _ufoCamera.transform.Rotate(_forceBeamEnabled ? new Vector3(10, 0, 0) : new Vector3(-10, 0, 0));
        }

        Performance.ShowFPS();
    }
    
    void FixedUpdate()
    {
        _ufoLights.transform.Rotate(0, 0, Time.fixedDeltaTime * -100);

        MoveUfo();
        
        if (Input.GetKey(KeyCode.R))  // auto level
        {
            // _ufoRigidBody.transform.rotation = Quaternion.Euler(0, _ufoRigidBody.transform.rotation.y, 0);
            // TODO: Do auto leveling by adding torque
            
            // _ufoRigidBody.AddRelativeTorque(new Vector3(0,0,.1f), ForceMode.VelocityChange);
            _ufoRigidBody.AddRelativeTorque(new Vector3(
                _ufoRigidBody.transform.eulerAngles.x / -5000,
                0,
                _ufoRigidBody.transform.eulerAngles.z / -5000
                ), ForceMode.VelocityChange);  // TODO: Try replacing with RotateLocal to remove inertia
        }
    }

    private void MoveUfo()
    {
        /******* MOVEMENT *******/
        var velocityCoefficient = Input.GetKey(KeyCode.LeftShift) ? 10 : 1;

        // TODO: Either one of these should be valid. If both are pressed, behave as none of them is pressed.
        if (Input.GetKey(KeyCode.Space))  // Accelerate depending of key pressed
            _ufoVelocityChange.y = 20 * Time.fixedDeltaTime;
        else if (Input.GetKey(KeyCode.LeftControl))
            _ufoVelocityChange.y = -20 * Time.fixedDeltaTime;
        else                              // No up/down key pressed, apply no force
            _ufoVelocityChange.y = 0;

        // TODO: Either one of these should be valid. If both are pressed, behave as none of them is pressed.
        if (Input.GetKey(KeyCode.W))      // Accelerate depending of key pressed
            _ufoVelocityChange.z = 30 * Time.fixedDeltaTime * velocityCoefficient;
        else if (Input.GetKey(KeyCode.S))
            _ufoVelocityChange.z = -30 * Time.fixedDeltaTime * velocityCoefficient;
        else                              // No forward/backward key pressed, apply no force to horizontal z
        {
            _ufoVelocityChange.z = 0;
            // _ufoRigidBody.velocity = Vector3.zero;
            // _ufoRigidBody.drag = 8;
        }

        if (Input.GetKey(KeyCode.Q))
            _ufoVelocityChange.x = -20 * Time.fixedDeltaTime;
        else if (Input.GetKey(KeyCode.E))
            _ufoVelocityChange.x = 20 * Time.fixedDeltaTime;
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

        // AlignUfoCamera();

        SetCameraUfoDistance();
        
        AlignTopCamera();

        ApplyForceBeam();
    }

    private void SetCameraUfoDistance()
    {
        var localPosition = _ufoCamera.transform.localPosition;
        localPosition = new Vector3(
            localPosition.x,
            localPosition.y,
            // _initialCameraUfoDistance - _ufoRigidBody.velocity.z / 4
            _initialCameraUfoDistance - _ufoRigidBody.velocity.magnitude / 4
            // _initialCameraUfoDistance
        );
        _ufoCamera.transform.localPosition = localPosition;
    }
    
    private void AlignUfoCamera()
    {
        if (!_ufoCamera) return;

        // TODO: Možná by to chtělo tu kameru odebrat od UFO?
        
        // auto level ufo camera
        // • v = get local forward vector of UFO
        // • set global camera position to (global) UFO position  -v * some distance  (i.e. 4)
        // • ensure that camera local position y is always 0 (or a bit higher)
        // • ensure that camera local rotation z is always 0
        // • set camera look at UFO

        // Vector3 v = _ufo.transform.forward;  // normalized global forward vector
        // Vector3 v = Vector3.forward;  // normalized local forward vector
        // _ufoCamera.transform.localPosition = -v * 4;
        // Vector3 newPosition = _ufoCamera.transform.localPosition;
        // newPosition.y = 0;
        // _ufoCamera.transform.localPosition = newPosition;
        // _ufoCamera.transform.LookAt(_ufo.transform);

        // Vector3 newPosition = _ufoCamera.transform.position;
        // newPosition.y = _ufo.transform.position.y;
        // _ufoCamera.transform.localPosition = newPosition;
        // _ufoCamera.transform.LookAt(_ufo.transform);

        // _ufoCamera.transform.localEulerAngles = new Vector3(
        //     _ufoCamera.transform.localEulerAngles.x,
        //     _ufoCamera.transform.localEulerAngles.y,
        //     0
        // );

        // Vector3 cameraUp = _ufoCamera.transform.localEulerAngles;
        // cameraUp.z = -_ufo.transform.eulerAngles.z;
        // _ufoCamera.transform.localEulerAngles = cameraUp;
    }

    private void AlignTopCamera()
    {
        if (!_topCamera) return;

        _topCamera.transform.position = new Vector3(_ufo.transform.position.x, _ufo.transform.position.y + 16, _ufo.transform.position.z);
        _topCamera.transform.eulerAngles = new Vector3(90, _ufo.transform.eulerAngles.y, 0);
        
        // UpdateVectorComponent(ref _topCamera.transform.position, "y", 16);  // Hm, takže nic :D
    }

    private static void ApplyForceBeam()
    {
        if (!_forceBeamEnabled) return;
        
        _ufoForceBeam.transform.Rotate(Vector3.up, 1);
        
        // Physics
    }

    // TODO: Co zneužít přetěžování operátorů + extendnout třídu Vector3?
    // This is because they won't allow me to change individual Vector component
    private static void UpdateVectorComponent(ref Vector3 vectorToUpdate, string component, float value)
    {
        string[] components = {"x", "y", "z"};
        var index = Array.IndexOf(components, component);

        Vector3 newVector = vectorToUpdate;
        newVector[index] = value;
        vectorToUpdate = newVector;
    }
}
