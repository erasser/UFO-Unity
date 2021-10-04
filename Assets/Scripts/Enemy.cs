using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private float _hull = 100;
    
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // transform.Translate(Vector3.forward / 16);
        _rigidBody.transform.Translate(Vector3.forward / 10);
    }

    public void GetDamage()
    {
        _hull -= 50;

        // if (_hull <= 0)
        //     DestroyMe();
    }

    private void DestroyMe()
    {
        // UfoController.SelectNone();
        // Destroy(this);
    }

    public void OnCollisionEnter(Collision other)  // TODO: Rather do this by rocket MissileSupervisor.interceptEvent
    {
        print(other.collider.tag);
    }
}
