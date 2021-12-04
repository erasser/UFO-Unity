using System.Collections.Generic;
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
    public static int ProjectileCounter = 0;
    public Rigidbody hitObjectRigidbody;
    public GameObject hitObject;
    public Vector3 collisionCoordinates;
    public static List<GameObject> ProjectilesRockets = new List<GameObject>();
    public GameObject rocketCamera;

    void Awake()
    {
        _gameControllerInstance = GameObject.Find("GameController").GetComponent<GameController>();
        halfHeight = GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * transform.lossyScale.y;
    }
    
    void Start()
    {
        _lastShotTime = _shootTime = Time.time;
        InvokeRepeating(nameof(CheckLifespan), 0, 1);  // seconds
        if (CompareTag("rocket"))
        {
            ProjectilesRockets.Add(gameObject);
            if (ProjectilesRockets.Count == 1)  // Assign follow camera, if it's the first shot
            {
                GameController.RocketCamera.transform.SetParent(transform, false);
                GameController.ToggleRenderTextureCamera(GameController.RocketCamera, true);
            }
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
        _gameControllerInstance.DestroyGameObject(gameObject);
    }

    // Ensures the projectile doesn't collide with the shooter itself
    // public void OnTriggerExit(Collider other)
    // {
        // print("rocket trigger exit");
        // GetComponent<BoxCollider>().isTrigger = false;
    // }

    private void CheckLifespan()  // There is also 'limit flight time' in missileSupervisor - just stops tracking, flight continues
    {
        if (Time.time - _shootTime > 10)
            gameObject.GetComponent<TrailRenderer>().enabled = false;
        
        if (Time.time - _shootTime > _lifespan)
            _gameControllerInstance.DestroyGameObject(gameObject);
    }

    public static bool CanBeShot()
    {
        return Time.time > _lastShotTime + _rocketsDelay;
    }
    
    
}
