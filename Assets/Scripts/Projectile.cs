using UnityEngine;

public class Projectile : MonoBehaviour
{
    // private GameController _gameControllerScript;
    private static float _rocketsDelay = .5f;   // Delay until next projectile can be fired (s)
    private static float _lastShotTime;         // Time of last shot projectile
    private float _shootTime;                   // Time, when this projectile was shot

    void Start()
    {
        // _gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
        _lastShotTime = _shootTime = Time.time;
        InvokeRepeating ("CheckLifespan", 0, 1);  // seconds
    }
    
    public void OnCollisionEnter(Collision other)
    {
        other.collider.gameObject.GetComponent<Enemy>()?.GetDamage(50);

        Destroy(gameObject);  // must be at the end
    }

    private void CheckLifespan()
    {
        print (Time.time);
        if (Time.time - _shootTime > 10) // lifespan is hardcoded now and the same for all types of projectiles
            Destroy(gameObject);
    }

    public static bool CanBeShot()
    {
        return Time.time > _lastShotTime + _rocketsDelay;
    }
    
    
}
