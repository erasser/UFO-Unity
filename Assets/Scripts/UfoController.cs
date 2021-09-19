using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

// TODO:  Limit speed by applying magnitude of velocity vector, so speed of particular axes are not independent and so UFO has its 'total' max speed limit
// TODO:  Try to apply <TrailRenderer>

public class UfoController : MonoBehaviour
{
    public FixedJoystick joystickHorizontalPlane;
    public FixedJoystick joystickVerticalPlane;
    // I don't know why I did it static
    private static GameObject _ufo;
    private static GameObject _ufoLights;
    private static GameObject _ufoForceBeam;
    private static Rigidbody _ufoRigidBody;
    private static Vector3 _ufoVelocityChange;  // Add an instant velocity change to the rigidbody (ignoring its mass)
    private static Vector3 _ufoRotationChange;
    private static GameObject _arrowHelper;
    private static GameObject _cubeHelper;
    private GameObject _ufoCamera;
    private Vector3 _initialCameraUfoLocalPosition;
    private GameObject _topCamera;
    private float _timeInterval;
    private static bool _forceBeamEnabled;
    private static Text _infoText;         // UI element
    private Vector3 _velocityCoefficient;
    private int _rotationCoefficient;
    private const int AutoLevelingDuration = 2;  // seconds
    private bool _isAutoLeveling;  // TODO: Just _autoLevelingTime or _fromTransform could be used
    private float _autoLevelingTime;
    private Quaternion _fromUfoQuaternion;
    
    void Start()
    {
        // _ufo = GameObject.Find("UFO_low_poly");      // TODO: This is this! :D  // Odstranit _ufo z této třídy, zbytečné
        _ufoRigidBody = GetComponent<Rigidbody>();
        // _ufoLights = GameObject.Find("UfoLights");
        _ufoForceBeam = GameObject.Find("ForceBeam");
        _infoText = GameObject.Find("InfoText").GetComponent<Text>();
        _arrowHelper = GameObject.Find("arrow");
        _cubeHelper = GameObject.Find("CubeHelper");
        _ufoCamera = GameObject.Find("CameraUfo");
        _topCamera = GameObject.Find("CameraTop");

        if (_ufoCamera != null)
            _initialCameraUfoLocalPosition = _ufoCamera.transform.localPosition;  // It's set in editor and it's the minimum distance

        Quest.Init();

        // _ufoForceBeam.GetComponent<CapsuleCollider>().enabled = false;
        // _ufoForceBeam.SetActive(false);

        // _arrowHelper.GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        Performance.ShowFPS();
        
        if (Input.GetKeyDown(KeyCode.G))  // switch gravity
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
            InitiateAutoLeveling();
            
            // _ufoRigidBody.transform.rotation = Quaternion.Euler(0, _ufoRigidBody.transform.rotation.y, 0);
            // TODO: Do auto leveling by adding torque

            // _ufoRigidBody.AddRelativeTorque(new Vector3(0,0,.1f), ForceMode.VelocityChange);
            /*_ufoRigidBody.AddRelativeTorque(new Vector3(
                _ufoRigidBody.transform.eulerAngles.x / -5000,
                0,
                _ufoRigidBody.transform.eulerAngles.z / -5000
            ), ForceMode.VelocityChange);*/ // TODO: Try replacing with RotateLocal to remove inertia
        }

        // if (Input.GetKeyDown(KeyCode.F))  // switch force field
        // {
        //     _forceBeamEnabled = !_forceBeamEnabled;
        //     _ufoForceBeam.SetActive(_forceBeamEnabled);
        //
        //     _ufoCamera.transform.Rotate(_forceBeamEnabled ? new Vector3(10, 0, 0) : new Vector3(-10, 0, 0));
        // }
    }
    
    void FixedUpdate()
    {
        // _ufoLights.transform.Rotate(0, 0, Time.fixedDeltaTime * -100);

        // ApplyForceBeam();

        if (_isAutoLeveling)
            UpdateAutoLeveling();
   
        MoveUfo();
        SetCameraUfoDistance();  // Do it although nothing is pressed, UFO can be moving due to inertia. Could be conditioned by velocity.magnitude.
        AlignTopCamera();
        // AlignUfoCamera();
    }

    private void MoveUfo()
    {
        // _infoText.text = _ufoRigidBody.velocity.magnitude.ToString();
        
        if (!(
            joystickHorizontalPlane.Direction.magnitude > 0 ||
            joystickVerticalPlane.Direction.magnitude > 0 ||
            Input.GetKey(KeyCode.Space) ||
            Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.Q) ||
            Input.GetKey(KeyCode.E) ||
            Input.GetKey(KeyCode.LeftShift)
        )) return;

        if (joystickHorizontalPlane.Direction.magnitude > 0 || joystickVerticalPlane.Direction.magnitude > 0)
        {
            _ufoRotationChange.y = 30 * Time.fixedDeltaTime * joystickHorizontalPlane.Direction.x;

            _ufoVelocityChange.z = 120 * Time.fixedDeltaTime * joystickHorizontalPlane.Direction.y;

            _ufoVelocityChange.x = 40 * Time.fixedDeltaTime * joystickVerticalPlane.Direction.x;

            _ufoVelocityChange.y = 40 * Time.fixedDeltaTime * joystickVerticalPlane.Direction.y;

            if (_ufoVelocityChange.magnitude > 0)
               _ufoRigidBody.AddRelativeForce(_ufoVelocityChange, ForceMode.VelocityChange);
            
            if (_ufoRotationChange.magnitude > 0)
                _ufoRigidBody.AddRelativeTorque(_ufoRotationChange, ForceMode.VelocityChange);

            return;
        }

        /******* MOVEMENT *******/
        if (Input.GetKey(KeyCode.LeftShift))  // Move Time.fixedDeltaTime here?
        {
            _velocityCoefficient.Set(60, 40, 300);
            _rotationCoefficient = 30;
        }
        else
        {
            _velocityCoefficient.Set(30, 20, 30);
            _rotationCoefficient = 15;
        }

        if (Input.GetKey(KeyCode.Space))  // Accelerate depending of key pressed
            _ufoVelocityChange.y = Time.fixedDeltaTime * _velocityCoefficient.y;
        else if (Input.GetKey(KeyCode.LeftControl))
            _ufoVelocityChange.y = - Time.fixedDeltaTime * _velocityCoefficient.y;
        if (Input.GetKey(KeyCode.Space) == Input.GetKey(KeyCode.LeftControl))  // No up/down key pressed or both are pressed, apply no force
            _ufoVelocityChange.y = 0;
        
        if (Input.GetKey(KeyCode.W))
            _ufoVelocityChange.z = Time.fixedDeltaTime * _velocityCoefficient.z;
        else if (Input.GetKey(KeyCode.S))
            _ufoVelocityChange.z = - Time.fixedDeltaTime * _velocityCoefficient.z;
        if (Input.GetKey(KeyCode.W) == Input.GetKey(KeyCode.S))
            _ufoVelocityChange.z = 0;

        if (Input.GetKey(KeyCode.Q))
            _ufoVelocityChange.x = - Time.fixedDeltaTime * _velocityCoefficient.x;
        else if (Input.GetKey(KeyCode.E))
            _ufoVelocityChange.x = Time.fixedDeltaTime * _velocityCoefficient.x;
        if (Input.GetKey(KeyCode.Q) == Input.GetKey(KeyCode.E))
            _ufoVelocityChange.x = 0;

        if (_ufoVelocityChange.magnitude > 0)
            _ufoRigidBody.AddRelativeForce(_ufoVelocityChange, ForceMode.VelocityChange);

        /******* ROTATION *******/
        if (Input.GetKey(KeyCode.A))
            _ufoRotationChange.y = - Time.fixedDeltaTime * _rotationCoefficient;
        else if (Input.GetKey(KeyCode.D))
            _ufoRotationChange.y = Time.fixedDeltaTime * _rotationCoefficient;
        if (Input.GetKey(KeyCode.A) == Input.GetKey(KeyCode.D))
            _ufoRotationChange.y = 0;

        if (_ufoRotationChange.magnitude > 0)
            _ufoRigidBody.AddRelativeTorque(_ufoRotationChange, ForceMode.VelocityChange);

        // _arrowHelper.transform.rotation = Quaternion.LookRotation(_ufoRigidBody.velocity.normalized);

        // _timeInterval += Time.fixedDeltaTime;
        // if (_timeInterval > .04f)
        // {
        //     _timeInterval = 0;
        //     var cubeHelper = Instantiate(_cubeHelper);
        //     cubeHelper.transform.position = _ufoRigidBody.transform.position;  // Its not a reference, because its a struct?
        // }
    }

    private void SetCameraUfoDistance()  // TODO: Rename
    {
        if (!_ufoCamera) return;

        var localVelocity = transform.InverseTransformDirection(_ufoRigidBody.velocity);

        
        // TODOO:  • Apply camera target.  • Invert forward/backward camera position
        
        _ufoCamera.transform.localEulerAngles = new Vector3(
            - localVelocity.y / 2,    // pitch
            localVelocity.x / 8,      // yaw
            - _ufoRigidBody.angularVelocity.y * 5  // roll
            // - Mathf.Asin(_ufoRigidBody.angularVelocity.y) * 3  // can cause NaN
        );

        var zCoefficient = localVelocity.z > 0 ? 6 : 18;
        
        _ufoCamera.transform.localPosition = new Vector3(
            0,
            _initialCameraUfoLocalPosition.y,
            _initialCameraUfoLocalPosition.z - localVelocity.z / zCoefficient
        );

        /* Replaced by camera rotation
        // var localVelocity = transform.rotation * _ufoRigidBody.velocity;  // has no effect :-/
        var localVelocity = transform.InverseTransformDirection(_ufoRigidBody.velocity);

        var xCoefficient = Math.Abs(localVelocity.x) > 10 ? 10 : 50;
        var yCoefficient = Math.Abs(localVelocity.y) > 10 ? 12 : 60;
        var zCoefficient = localVelocity.z > 0 ? 6 : 18;
        
        _ufoCamera.transform.localPosition = new Vector3(
            localVelocity.x / xCoefficient,
            _initialCameraUfoLocalPosition.y + localVelocity.y / yCoefficient,
            _initialCameraUfoLocalPosition.z - localVelocity.z / zCoefficient

            // This approach looks a bit unnatural (i.e. implies unnatural sense of motion) 
            // SignedSqrt(localVelocity.x) / 3,
            // _initialCameraUfoLocalPosition.y + SignedSqrt(localVelocity.y) / 4,
            // _initialCameraUfoLocalPosition.z - SignedSqrt(localVelocity.z) / 2
        );*/
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

    private static float SignedSqrt(float number)
    {
        // square root ↓ of ↓ positive number respecting ↓ lost sign
        return Mathf.Sqrt(Math.Abs(number)) * Mathf.Sign(number);
    }

    private void InitiateAutoLeveling()
    {
        if (_isAutoLeveling)
            return;
            
        _isAutoLeveling = true;
        _fromUfoQuaternion = _ufoRigidBody.transform.rotation;  // .transform is a reference, while Quaternion is a struct
        _autoLevelingTime = 0;
    }
    
    private void UpdateAutoLeveling()
    {
        // TODO: What if it will collide while auto leveling?
        _autoLevelingTime += Time.fixedDeltaTime;

        float phase;
        if (_autoLevelingTime < AutoLevelingDuration)
            phase = _autoLevelingTime / AutoLevelingDuration;            
        else
            phase = 1;
        
        _ufoRigidBody.rotation = Quaternion.Lerp(
            _fromUfoQuaternion,
            Quaternion.Euler(
                new Vector3(
                    0,
                    _fromUfoQuaternion.eulerAngles.y,
                    0)
            ), phase
        );

        if (phase == 1)  // I've set 1 before, so fuck fuck Rider here
        {
            _isAutoLeveling = false;
            _autoLevelingTime = 0;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        InitiateAutoLeveling();
    }
}
