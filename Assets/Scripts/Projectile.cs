using System.Collections.Generic;
using SparseDesign.ControlledFlight;
using UnityEngine;

/// Opodmínkovat TrailRenderer, pokud se bude používat s něčím bez trailu!

public class Projectile : MonoBehaviour
{
    private GameController _gameControllerInstance;
    private static float _lifespan = 30;        // lifespan (s)
    private static float _rocketsDelay = .5f;   // Delay until next projectile can be fired (s)
    private static float _lastShotTime;         // Time of last shot projectile
    private float _shootTime;                   // Time, when this projectile was shot
    public float halfHeight;
    public Rigidbody hitObjectRigidbody;
    public GameObject hitObject;
    public Vector3 collisionCoordinates;
    public static List<GameObject> ProjectilesRockets = new List<GameObject>();  // Used for easily get count, first, next, last...
    public GameObject rocketCamera;
    // public static int ProjectileCounter = 0;  // Now the Pool manages this

    void Awake()
    {
        _gameControllerInstance = GameObject.Find("GameController").GetComponent<GameController>();
        halfHeight = GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * transform.lossyScale.y;
    }
    
    void Start()
    {
        if (CompareTag("rocket"))
        {
            _lastShotTime = _shootTime = Time.time;
            InvokeRepeating(nameof(CheckLifespan), 0, 1);  // seconds
            SetRocketCamera();
        }
    }

    public void Reset()
    {
        // TODO: Cancel invoke (see Ui.cs)
        // CancelInvoke(nameof(CheckLifespan));
        if (CompareTag("rocket"))
        {
            _lastShotTime = _shootTime = Time.time;
            transform.position.Set(0, 0, 0);
            transform.eulerAngles.Set(0, 0, 0);
            GetComponent<TrailRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            DestroyImmediate(GetComponent<MissileSupervisor>());  // Must be destroyed immediately, else MissileSupervisor reset doesn't work
            var go = gameObject;
            go.AddComponent<MissileSupervisor>();
            SetRocketCamera();
            go.SetActive(true);
            // var newMissile = rocket.GetComponent<MissileSupervisor>();
            // var missilePrefab = WeaponController.Instance.rocketPrefab.GetComponent<MissileSupervisor>();
            // newMissile.m_guidanceSettings.m_target = WeaponController.Instance.SetWeaponTarget(Ufo.Instance.gameObject, newMissile); // Must be set before end of the tween
            // newMissile.m_guidanceSettings.m_N = missilePrefab.m_guidanceSettings.m_N;
            // newMissile.m_guidanceSettings.m_limitAcceleration = missilePrefab.m_guidanceSettings.m_limitAcceleration;
            // newMissile.m_guidanceSettings.m_maxAcceleration = missilePrefab.m_guidanceSettings.m_maxAcceleration;
            // newMissile.m_launchType = missilePrefab.m_launchType;
            // newMissile.m_autoLaunch = missilePrefab.m_autoLaunch;
            // newMissile.m_launchSpeed = missilePrefab.m_launchSpeed;
            // newMissile.m_motorStages[0].m_limitMotorAcceleration = missilePrefab.m_motorStages[0].m_limitMotorAcceleration;
            // newMissile.m_motorStages[0].m_speed = missilePrefab.m_motorStages[0].m_speed;
        }
    }

    private void SetRocketCamera()
    {
        ProjectilesRockets.Add(gameObject);
        if (ProjectilesRockets.Count == 1) // Assign follow camera, if it's the first shot
        {
            GameController.RocketCamera.transform.SetParent(transform, false);
            GameController.ToggleRenderTextureCamera(GameController.RocketCamera, true);
        }
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (CompareTag("rocket"))
        {
            // hitObjectRigidbody = other.gameObject.GetComponent<Rigidbody>(); // For rocket blast to exclude hit object from blast effect
            hitObject = other.gameObject; // For rocket blast to exclude hit object from blast effect
            collisionCoordinates = other.GetContact(0).point;
        }

        other.gameObject.GetComponent<Enemy>()?.GetDamage(50);
        _gameControllerInstance.DestroyGameObject(gameObject, true);
    }

    // Ensures the projectile doesn't collide with the shooter itself
    // public void OnTriggerExit(Collider other)
    // {
        // print("rocket trigger exit");
        // GetComponent<BoxCollider>().isTrigger = false;
    // }

    private void CheckLifespan()  // There is also 'limit flight time' in missileSupervisor - just stops tracking, flight continues
    {
        print("checking lifespan");
        if (Time.time - _shootTime > 10)
            gameObject.GetComponent<TrailRenderer>().enabled = false;
        
        if (Time.time - _shootTime > _lifespan)
            _gameControllerInstance.DestroyGameObject(gameObject, true);
    }

    public static bool CanBeShot()
    {
        return Time.time > _lastShotTime + _rocketsDelay;
    }
}
