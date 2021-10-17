using UnityEngine;

/// Opodmínkovat TrailRenderer, pokud se bude používat s něčím bez trailu!

public class Projectile : MonoBehaviour
{
    private GameController _gameControllerInstance;
    private static float _lifespan = 30;        // lifespan (s)
    private static float _rocketsDelay = .5f;   // Delay until next projectile can be fired (s)
    private static float _lastShotTime;         // Time of last shot projectile
    private float _shootTime;                   // Time, when this projectile was shot

    void Start()
    {
        _gameControllerInstance = GameObject.Find("GameController").GetComponent<GameController>();
        _lastShotTime = _shootTime = Time.time;
        InvokeRepeating(nameof(CheckLifespan), 0, 1);  // seconds
    }
    
    public void OnCollisionEnter(Collision other)
    {
        other.collider.gameObject.GetComponent<Enemy>()?.GetDamage(50);  // OnCollisionEnter
        _gameControllerInstance.DestroyGameObject(gameObject);  // must be at the end of this method
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
