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
    // private GameObject _saturnRingParticlesParent;

    private const float TwoPI = 2 * Mathf.PI;
    private const int RingsMinRadius = 300;
    private const int RingsMaxRadius = 600;
    private const int RingsThickness = 60;
    private Rigidbody _rigidBody;
    private GameObject _cubeHelper;
    private float _angle, _distance, _x, _y, _z, _scale;
    private Quaternion _rotation;
    private bool _hasCollided;
    private static bool _testSphereHasCollided;
    private Vector3 _tmpV3;
    private int _frames;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        // _saturnRingParticlesParent = GameObject.Find("SaturnRingParticlesParent1");
    }

    private void Update()
    {
        // _saturnRingParticlesParent.transform.Rotate(new Vector3(0, 0, -.0004f * Time.deltaTime));
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

    public void Create(int i/*, GameObject collisionSphere*/)
    {
        _distance = Random.Range(RingsMinRadius, RingsMaxRadius);
        _angle = Random.value * TwoPI;
        _x = Mathf.Cos(_angle) * _distance;
        _y = Random.value * RingsThickness - RingsThickness / 2;
        _z = Mathf.Sin(_angle) * _distance;
        
// if (_distance == 0)
//     print($"_distance: {_distance}");

        _rotation = Random.rotation;  // TODO: Isn't this in a range <0;PI>? (due to a character of quaternions)

        //           max  * /                   range <0;1>                      \  min size
        // var scale = 2 * (outerRadius - innerRadius) / (distance - innerRadius) + .1f;  // more distant => bigger asteroid
        // _scale = Random.Range(.8f, 10);
        _scale = Random.Range(.8f, 10);


        transform.SetPositionAndRotation(new Vector3(_x, _y, _z), _rotation);
        // transform.position = new Vector3(_x, _y, _z);  // TODO: Make them more dense to y=0
        // transform.rotation = _rotation;  // TODO: Can be done in Instantiate()
        transform.localScale = new Vector3(_scale, _scale, _scale);

        GetComponent<Rigidbody>().mass = Mathf.Pow(_scale + .8f, 3) * 100;

        name = $"asteroid_{i}";

        // TODO: Udělat to na správnym místě, nedělat zbytečný výpočty
//        collisionSphere.GetComponent<SphereCollider>().radius = 24.3f * _scale;
//        collisionSphere.transform.SetPositionAndRotation(new Vector3(_x, _y, _z), _rotation);
        // print("has collided? :" + _testSphereHasCollided);
        
        // Can't use this if isKinematic & non-convex properties are set to false
        // asteroid.GetComponent<Rigidbody>().AddRelativeTorque(rotation.eulerAngles * 100, ForceMode.Force);  // use mass

        // asteroid.GetComponent<Rigidbody>().AddComponent<ConstantForce>();
        // var constantTorque = asteroid.GetComponent<ConstantForce>();
        // constantTorque.relativeTorque = new Vector3(0,10,0);

    }

    private void UpdateTransform()
    {
        // Zkusit pro pohyb 1 parenta (nepomohlo)
        // Zkusit transformovat jen viditelné - z toho mám strach, vznikaly by kolize buď mimo kameru nebo po updatu pozice (asi by k tomu ale docházelo zcela výjimečně) + asi určitě by byl hodně variabilní framerate
        // Zkusit přeskočit n snímků (pomohlo)
        
        if (_frames++ % 3 != 0) return;
        
        // TODO: Asteroids stops moving when collided by UFO. Solve it.

        if (_hasCollided || name == "asteroid") return;  // TODO: Get rid of 'asteroid' (I already have a prefab, just delete the main asteroid from scene, or set it as inactive)
if (_distance == 0) return;
        // closer to the planet => faster motion
        // _angle -= RingsMaxRadius / _distance * Time.fixedDeltaTime / 100;
        _angle -= Mathf.Pow(RingsMaxRadius / _distance, 4) * Time.fixedDeltaTime / 300;

        _x = Mathf.Cos(_angle) * _distance;
        _z = Mathf.Sin(_angle) * _distance;

        // _rigidBody.MovePosition(new Vector3(_x, _y, _z));  // It takes interpolation into account.
        _rigidBody.position = new Vector3(_x, _y, _z);

        /* Další možnost: Každej by mohl mít svýho parenta, kterej by s ním točil = obdobnej přístup jako ten úplně původní */
    }

    public static void CollidedAsteroid()
    {
        // _testSphereHasCollided = true;
    }

}