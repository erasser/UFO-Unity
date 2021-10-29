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

    void Awake()
    {
        _gameControllerInstance = GameObject.Find("GameController").GetComponent<GameController>();
    }
    
    void Start()
    {
        _lastShotTime = _shootTime = Time.time;
        InvokeRepeating(nameof(CheckLifespan), 0, 1);  // seconds
    }
    
    // TODO: Triggeruje se 2×, když střílí enemy!
    public void OnCollisionEnter(Collision other)
    {
        other.collider.gameObject.GetComponent<Enemy>()?.GetDamage(50);
        _gameControllerInstance.DestroyGameObject(gameObject);
    }

    // Ensures the projectile doesn't collide with the shooter itself
    public void OnTriggerExit(Collider other)
    {
        GetComponent<BoxCollider>().isTrigger = false;
    }

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
