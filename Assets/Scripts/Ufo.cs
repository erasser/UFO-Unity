using SparseDesign.ControlledFlight;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine.Rendering;
using UnityEngine;

// TODO:  Limit speed by applying magnitude of velocity vector, so speed of particular axes are not independent and so UFO has its 'total' max speed limit
// TODO:  Try to apply <TrailRenderer>
// TODO:  Make static, what can be made static

public class Ufo : MonoBehaviour
{
    public static Ufo Instance;
    private static GameObject _forceBeam;
    private static bool _forceBeamEnabled;
    private static Rigidbody _rigidBody;
    private static Vector3 _velocityChange = Vector3.zero;  // Add an instant velocity change to the rigidbody (ignoring its mass)
    private static Vector3 _rotationChange = Vector3.zero;
    private Vector3 _velocityCoefficient;
    private int _rotationCoefficient;
    public static GameObject UfoCamera;
    public static Camera UfoCameraComponentCamera;
    private Vector3 _initialCameraUfoLocalPosition;
    private GameObject _topCamera;
    private float _timeInterval;
    [SerializeField] private bool _isAutoLeveling;  // TODO: Just _autoLevelingTime or _fromTransform could be used
    private float _autoLevelingTime;
    private Quaternion _fromUfoQuaternion;
    private static GameObject _laser;
    private GameObject _laserLight;
    public GameObject rocketPrefab;
    private GameController _gameControllerScript;
    // public static GameObject ufo;
    public GameObject arrow;
    private GameObject _alienHead;
    private GameObject _motherShip;
    private Vector3 _lastVelocity = Vector3.zero;
    private float _alienHeadAcceleration;

    void Awake()
    {
        Application.targetFrameRate = 666;
        // ufo = Instantiate(_gameControllerScript.ufoPrefab);
        arrow = transform.Find("arrow").gameObject;
    }
    
    void Start()
    {
        Instance = this;
        _rigidBody = GetComponent<Rigidbody>();
        _forceBeam = GameObject.Find("ForceBeam");
        UfoCamera = GameObject.Find("CameraUfo");
        UfoCameraComponentCamera = UfoCamera.GetComponent<Camera>();
        _topCamera = GameObject.Find("CameraTop");
        _laser = GameObject.Find("laser");
        _laserLight = GameObject.Find("laserLight");
        _laser.SetActive(false);
        _laserLight.SetActive(false);
        _gameControllerScript = GameObject.Find("GameController").transform.GetComponent<GameController>();
        _alienHead = transform.Find("alienHead").gameObject;
        _motherShip = GameObject.Find("ufoMotherShip");

        if (UfoCamera != null)
            _initialCameraUfoLocalPosition = UfoCamera.transform.localPosition;  // It's set in editor

        // foreach (Transform child in GameObject.Find("buildings").transform)  // I've decided to do it in editor to prefabs (it doesn't recommend to set isStatic & colliders dynamically)
        // {
        //     child.gameObject.AddComponent<Rigidbody>();
        //     var rb = child.gameObject.GetComponent<Rigidbody>();
        //     rb.isKinematic = true;
        //     child.gameObject.AddComponent<BoxCollider>();
        //     child.gameObject.isStatic = true;  // isStatic nikdy nepou????vat na nic!! Smysl to m?? jen v editoru, ale i tam mi to kurv?? ??ivot!
        // }

        // _forceBeam.GetComponent<CapsuleCollider>().enabled = false;
        // _forceBeam.SetActive(false);

        // _arrowHelper.GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        Performance.ShowFPS();
        
        if (Input.GetKeyDown(KeyCode.G))  // switch gravity
        {
            var drag = _rigidBody.drag;
            _rigidBody.useGravity = !_rigidBody.useGravity;

            _rigidBody.drag = _rigidBody.useGravity ? .5f : drag;
        }

        if (Input.GetKey(KeyCode.T)) // TODO: Remove
        {
            _rigidBody.rotation = Quaternion.Euler(45, 0, 80);
        }

        if (Input.GetKey(KeyCode.R))  // auto level
        {
            InitiateAutoLeveling();
            
            // _rigidBody.transform.rotation = Quaternion.Euler(0, _rigidBody.transform.rotation.y, 0);
            // TODO: Do auto leveling by adding torque

            // _rigidBody.AddRelativeTorque(new Vector3(0,0,.1f), ForceMode.VelocityChange);
            /*_rigidBody.AddRelativeTorque(new Vector3(
                _rigidBody.transform.eulerAngles.x / -5000,
                0,
                _rigidBody.transform.eulerAngles.z / -5000
            ), ForceMode.VelocityChange);*/ // TODO: Try replacing with RotateLocal to remove inertia
        }

        // if (Input.GetKeyDown(KeyCode.F))  // switch force field
        // {
        //     _forceBeamEnabled = !_forceBeamEnabled;
        //     _forceBeam.SetActive(_forceBeamEnabled);
        //
        //     // _ufoCamera.transform.Rotate(_forceBeamEnabled ? new Vector3(10, 0, 0) : new Vector3(-10, 0, 0));
        // }
    }
    
    void FixedUpdate()
    {
        // ApplyForceBeam();

        if (_isAutoLeveling)
            UpdateAutoLeveling();
   
        UpdateCameraUfoDistance();  // Do it although nothing is pressed, UFO can be moving due to inertia. Could be conditioned by velocity.magnitude.
        // AlignTopCamera();
        // AlignUfoCamera();
     
        if (_motherShip)
            _motherShip.transform.Rotate(0, .2f, 0);
        
        // Debug.DrawRay(_jet.transform.position, _jet.transform.TransformDirection (Vector3.forward) * 20, Color.magenta);
    }

    public void MoveUfo(Joystick joystickHorizontalPlane = null, Joystick joystickVerticalPlane = null)
    {
        /*******  JOYSTICK MOVEMENT & ROTATION  *******/
        if (joystickHorizontalPlane)
        {
            _rotationChange.y = 30 * Time.fixedDeltaTime * joystickHorizontalPlane.Direction.x;
            _velocityChange.z = 120 * Time.fixedDeltaTime * joystickHorizontalPlane.Direction.y;
        }

        if (joystickVerticalPlane)
        {
            _velocityChange.x = 40 * Time.fixedDeltaTime * joystickVerticalPlane.Direction.x;
            _velocityChange.y = 40 * Time.fixedDeltaTime * joystickVerticalPlane.Direction.y;
        }

        if (joystickHorizontalPlane || joystickVerticalPlane)
            goto DoTransform;

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

        /*******  KEY MOVEMENT  *******/
        if (Input.GetKey(KeyCode.Space))  // Accelerate depending of key pressed
            _velocityChange.y = Time.fixedDeltaTime * _velocityCoefficient.y;
        else if (Input.GetKey(KeyCode.LeftControl))
            _velocityChange.y = - Time.fixedDeltaTime * _velocityCoefficient.y;
        if (Input.GetKey(KeyCode.Space) == Input.GetKey(KeyCode.LeftControl))  // No up/down key pressed or both are pressed, apply no force
            _velocityChange.y = 0;
        
        if (Input.GetKey(KeyCode.W))
            _velocityChange.z = Time.fixedDeltaTime * _velocityCoefficient.z;
        else if (Input.GetKey(KeyCode.S))
            _velocityChange.z = - Time.fixedDeltaTime * _velocityCoefficient.z;
        if (Input.GetKey(KeyCode.W) == Input.GetKey(KeyCode.S))
            _velocityChange.z = 0;

        if (Input.GetKey(KeyCode.Q))
            _velocityChange.x = - Time.fixedDeltaTime * _velocityCoefficient.x;
        else if (Input.GetKey(KeyCode.E))
            _velocityChange.x = Time.fixedDeltaTime * _velocityCoefficient.x;
        if (Input.GetKey(KeyCode.Q) == Input.GetKey(KeyCode.E))
            _velocityChange.x = 0;

        /*******  KEY ROTATION  *******/
        if (Input.GetKey(KeyCode.A))
            _rotationChange.y = - Time.fixedDeltaTime * _rotationCoefficient;
        else if (Input.GetKey(KeyCode.D))
            _rotationChange.y = Time.fixedDeltaTime * _rotationCoefficient;
        if (Input.GetKey(KeyCode.A) == Input.GetKey(KeyCode.D))
            _rotationChange.y = 0;

        DoTransform:

        // TODOO ----------- Ud??lat po????dn?? + vy??e??it i nad jin??mi objekt ne?? nad zem?? (Raycast, SphereCast), obej??t se bez odmocniny
        // TODOO ----------- Mo??n?? by sta??ila v??t???? koule jako trigger, kterej by hodn?? zv??t??il drag, bylo by jednoduch?? a univerz??ln?? (musely by se z toho excludnout projektily apod.)
        // if (transform.localPosition.y < 10 && _velocityChange.y < 0)
            // _velocityChange.y /= (11 - transform.position.y) * 10;
            // _velocityChange.y = - Mathf.Sqrt(transform.localPosition.y / 3.75f);

        if (_velocityChange.x != 0 || _velocityChange.y != 0 || _velocityChange.z != 0)
            _rigidBody.AddRelativeForce(_velocityChange, ForceMode.VelocityChange);
            
        if (_rotationChange.x != 0 || _rotationChange.y != 0 || _rotationChange.z != 0)
            _rigidBody.AddRelativeTorque(_rotationChange, ForceMode.VelocityChange);

        var velocity = _rigidBody.velocity;
        var acceleration = (velocity - _lastVelocity) / Time.fixedDeltaTime;
        _lastVelocity = velocity;

        // _alienHead.transform.localEulerAngles = new Vector3(_alienHead.transform.localEulerAngles.x, 0, transform.InverseTransformDirection(acceleration).x);
        // _alienHead.transform.localEulerAngles = new Vector3(transform.InverseTransformDirection(acceleration).z, 0, transform.InverseTransformDirection(acceleration).x);
        _alienHead.transform.localEulerAngles = new Vector3(transform.InverseTransformDirection(acceleration).z, 0,  transform.InverseTransformDirection(acceleration).x);
        // _alienHead.transform.localEulerAngles = new Vector3(transform.InverseTransformDirection(acceleration).z, 0,  GameController.SignedSqrt(transform.InverseTransformDirection(acceleration).x));

        // _arrowHelper.transform.rotation = Quaternion.LookRotation(_rigidBody.velocity.normalized);

        // _timeInterval += Time.fixedDeltaTime;
        // if (_timeInterval > .04f)
        // {
        //     _timeInterval = 0;
        //     var cubeHelper = Instantiate(_cubeHelper);
        //     cubeHelper.transform.position = _rigidBody.transform.position;  // Its not a reference, because its a struct?
        // }
    }
    
    private void UpdateCameraUfoDistance()
    {
        if (!UfoCamera) return;

        var localVelocity = transform.InverseTransformDirection(_rigidBody.velocity);
        
        // TODOO:  ??? Apply camera target.  ??? Invert forward/backward camera position
        
        UfoCamera.transform.localEulerAngles = new Vector3(
            - localVelocity.y / .4f,    // pitch
            - localVelocity.x / .4f,      // yaw  // TODO: Rozv??tvit. (-) pro zat????en?? (setrva??nost), (+) rpo strafov??n??
            - _rigidBody.angularVelocity.y * 6  // roll
            // - Mathf.Asin(_rigidBody.angularVelocity.y) * 3  // can cause NaN
        );

        var zCoefficient = localVelocity.z < 0 ? 6 : 18;
        
        UfoCamera.transform.localPosition = new Vector3(
            0,
            _initialCameraUfoLocalPosition.y,
            _initialCameraUfoLocalPosition.z + localVelocity.z / zCoefficient
        );

        /* Replaced by camera rotation
        // var localVelocity = transform.rotation * _rigidBody.velocity;  // has no effect :-/
        var localVelocity = transform.InverseTransformDirection(_rigidBody.velocity);

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
        if (!UfoCamera) return;

        // TODO: Mo??n?? by to cht??lo tu kameru odebrat od UFO?
        
        // auto level ufo camera
        // ??? v = get local forward vector of UFO
        // ??? set global camera position to (global) UFO position  -v * some distance  (i.e. 4)
        // ??? ensure that camera local position y is always 0 (or a bit higher)
        // ??? ensure that camera local rotation z is always 0
        // ??? set camera look at UFO

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

        // Sta??ilo by d??t j?? jako child k UFO, ne?
        var cameraTransform = transform;
        var localPosition = cameraTransform.localPosition;
        _topCamera.transform.localPosition = new Vector3(localPosition.x, localPosition.y + 16, localPosition.z);
        _topCamera.transform.eulerAngles = new Vector3(90, cameraTransform.eulerAngles.y, 0);
        
        // UpdateVectorComponent(ref _topCamera.transform.position, "y", 16);  // Hm, tak??e nic :D
    }

    private static void ApplyForceBeam()
    {
        if (!_forceBeamEnabled) return;
        
        _forceBeam.transform.Rotate(Vector3.up, 1);
        
        // Physics
    }

    private void InitiateAutoLeveling()
    {
        if (_isAutoLeveling)
            return;
            
        _isAutoLeveling = true;
    }

    private void UpdateAutoLeveling()  // TODO  (btw. existuje RigidBody.MoveRotation)
    {
        var torqueForce = transform.eulerAngles;
        // torqueForce.y = 0;

        // if (torqueForce.magnitude < .01f)
        //     _isAutoLeveling = false;

        // _rigidBody.AddTorque(-1 * transform.TransformDirection(torqueForce), ForceMode.Impulse);

        var lerpAngle = new Vector3(
            Mathf.LerpAngle(torqueForce.x, 0, .5f),
            0,
            Mathf.LerpAngle(torqueForce.z, 0, .5f));
        
        _rigidBody.AddTorque(transform.TransformDirection(lerpAngle), ForceMode.Impulse);
        
        if (lerpAngle.magnitude < .1f)
            _isAutoLeveling = false;
        
        // Mathf.LerpAngle, Mathf.DeltaAngle
    }

    private void InitiateAutoLeveling_old()
    {
        if (_isAutoLeveling)
            return;
            
        _isAutoLeveling = true;
        _fromUfoQuaternion = _rigidBody.transform.rotation;  // .transform is a reference, while Quaternion is a struct
        _autoLevelingTime = 0;
    }
    
    private void UpdateAutoLeveling_old()
    {
        // TODO: What if it will collide while auto leveling?

        // autoLevelingDuration = _rigidBody.transform.eulerAngles.magnitude;

        var autoLevelingDuration = 5;
        
        _autoLevelingTime += Time.fixedDeltaTime;

        float phase;
        if (_autoLevelingTime < autoLevelingDuration)
            phase = _autoLevelingTime / autoLevelingDuration;            
        else
            phase = 1;
        
        _rigidBody.rotation = Quaternion.Lerp(
            _fromUfoQuaternion,
            Quaternion.Euler(
                new Vector3(
                    0,
                    _fromUfoQuaternion.eulerAngles.y,
                    0)
            ), phase
        );

        if (phase == 1)  // I've set 1 before, so fuck Rider here
        {
            _isAutoLeveling = false;
            _autoLevelingTime = 0;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // InitiateAutoLeveling();
    }

    public static void ToggleLaser()
    {
        _laser.SetActive(!_laser.activeSelf);
        // _laserLight.SetActive(_laser.activeSelf);  // Significant performance drop!
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == Quest.Current.QuestTarget.name)
            Quest.Complete();
    }



}
