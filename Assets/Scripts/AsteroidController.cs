using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: Zcela odstranit vzorový asteroid ze scény - dá se vytvořit prefab (drag & drop z hierarchy view do assets) a použít ten?
// TODO: Remove 'new Vector3' from code, use some predefined tmpVector

public class AsteroidController : MonoBehaviour
{
    private const float TwoPI = 2 * Mathf.PI;
    private const int MinRadius = 2000;
    private const int MaxRadius = 4000;
    private Rigidbody _rigidBody;
    private GameObject _cubeHelper;
    private float _angle, _distance, _x, _y, _z, _scale;
    private Quaternion _rotation;
    private bool _hasCollided;
    private static bool _testSphereHasCollided;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Performance.ShowFPS();
    }

    void FixedUpdate()
    {
        UpdateTransform();
    }

    private void OnCollisionEnter(Collision other)
    {
        // TODO!!!!  Remove when initial asteroid collisions are resolved!!
        if (other.gameObject.CompareTag("UFO"))
            _hasCollided = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // print("• triggered!");
        // Destroy(other.gameObject);
    }

    public void Create(int i, GameObject collisionSphere)
    {
        _distance = Random.Range(MinRadius, MaxRadius);
        _angle = Random.value * TwoPI;
        _x = Mathf.Cos(_angle) * _distance;
        _y = Random.value * 400 - 200;
        _z = Mathf.Sin(_angle) * _distance;

        // _x = 1000;
        // _y = 0;
        // _z = 1000;
        _rotation = Random.rotation;  // TODO: Isn't this in a range <0;PI>? (due to a character of quaternions)

        //           max  * /                   range <0;1>                      \  min size
        // var scale = 2 * (outerRadius - innerRadius) / (distance - innerRadius) + .1f;  // more distant => bigger asteroid
        _scale = Random.Range(.1f, 4);
        // _scale = 10;
        
        transform.SetPositionAndRotation(new Vector3(_x, _y, _z), _rotation);
        // transform.position = new Vector3(_x, _y, _z);  // TODO: Make them more dense to y=0
        // transform.rotation = _rotation;  // TODO: Can be done in Instantiate()
        transform.localScale = new Vector3(_scale, _scale, _scale);

        GetComponent<Rigidbody>().mass = Mathf.Pow(_scale + .8f, 3) * 1200;

        name = $"asteroid_{i}";

        // TODO: Udělat to na správnym místě, nedělat zbytečný výpočty
        collisionSphere.GetComponent<SphereCollider>().radius = 24.3f * _scale;
        collisionSphere.transform.SetPositionAndRotation(new Vector3(_x, _y, _z), _rotation);
        // print("has collided? :" + _testSphereHasCollided);
        
        // Can't use this if isKinematic & non-convex properties are set to false
        // asteroid.GetComponent<Rigidbody>().AddRelativeTorque(rotation.eulerAngles * 100, ForceMode.Force);  // use mass

        // asteroid.GetComponent<Rigidbody>().AddComponent<ConstantForce>();
        // var constantTorque = asteroid.GetComponent<ConstantForce>();
        // constantTorque.relativeTorque = new Vector3(0,10,0);

    }

    private void UpdateTransform()
    {
        if (!_hasCollided)
        {
            _angle -= Time.fixedDeltaTime / 20;
        
            _x = Mathf.Cos(_angle) * _distance;
            _z = Mathf.Sin(_angle) * _distance;
            
            _rigidBody.MovePosition(new Vector3(_x, _y, _z));
        }

        /* Další možnost: Každej by mohl mít svýho parenta, kterej by s ním točil = obdobnej přístup jako ten úplně původní */
    }

    public static void CollidedAsteroid()
    {
        // _testSphereHasCollided = true;
    }

}

