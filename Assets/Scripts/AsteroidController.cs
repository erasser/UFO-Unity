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
    private const int RingsMaxRadius = 700;
    private const int RingsThickness = 30;
    private Rigidbody _rigidBody;
    private GameObject _cubeHelper;
    private static bool _testSphereHasCollided;
    private Vector3 _tmpV3;
    private int _frames;
    /* Individual attributes */
    private float _angle, _distance, _x, _y, _z, _scale;
    private Quaternion _rotation;
    private Vector3 _torque;
    private bool _hasCollided;
    private GameObject _particlesSaturnRing;
    private GameObject _particlesSaturnRingIce;
    private GameObject _selectionSprite;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _particlesSaturnRing = GameObject.Find("ParticleSaturnRing");
        _particlesSaturnRingIce = GameObject.Find("ParticleSaturnRingIce");
        _selectionSprite = GameObject.Find("selectionSprite");
    }

    private void Update()
    {
        _particlesSaturnRing.transform.Rotate(new Vector3(0, 0, .0003f * Time.deltaTime));
        _particlesSaturnRingIce.transform.Rotate(new Vector3(0, 0, .0002f * Time.deltaTime));
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
        name = $"asteroid_{i}";

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
        // _scale = Random.Range(.8f, 20);  // for low poly asteroid
        _scale = Random.Range(.05f, .3f);


        transform.SetPositionAndRotation(new Vector3(_x, _y, _z), _rotation);
        // transform.position = new Vector3(_x, _y, _z);  // TODO: Make them more dense to y=0
        // transform.rotation = _rotation;  // TODO: Can be done in Instantiate()
        transform.localScale = new Vector3(_scale, _scale, _scale);

        // GetComponent<Rigidbody>().mass = Mathf.Pow(_scale, 3) * 100;
        GetComponent<Rigidbody>().mass = _scale * 300;

        _torque = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
        if (_torque.magnitude > 2)  // Optimization: Do not apply, if torque would be too small
            GetComponent<Rigidbody>().AddRelativeTorque(_torque, ForceMode.Acceleration);

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

        // if (_frames++ % 2 != 0) return;  // Or Project settings -> Time -> Fixed timestep (default = .02 ms)

        // TODO: Asteroids stops moving when collided by UFO. Solve it.
        // TODO: Optimization: Distant asteroids doesn't need to be rotated 

        if (_hasCollided || name == "asteroid") return;  // TODO: Get rid of 'asteroid' (I already have a prefab, just delete the main asteroid from scene, or set it as inactive)
if (_distance == 0) return;
        // closer to the planet => faster motion
        // _angle -= RingsMaxRadius / _distance * Time.fixedDeltaTime / 100;
        _angle -= Mathf.Pow(RingsMaxRadius / _distance, 3) * Time.fixedDeltaTime / 200;

        _x = Mathf.Cos(_angle) * _distance;
        _z = Mathf.Sin(_angle) * _distance;

        // _rigidBody.MovePosition(new Vector3(_x, _y, _z));  // It takes interpolation into account.
        _rigidBody.position = new Vector3(_x, _y, _z);

        // _rigidBody.AddRelativeTorque(_torque, ForceMode.VelocityChange);

        /* Další možnost: Každej by mohl mít svýho parenta, kterej by s ním točil = obdobnej přístup jako ten úplně původní (vyzkoušeno, výkonově nepomohlo) */
    }

    public static void CollidedAsteroid()
    {
        // _testSphereHasCollided = true;
    }

    private void OnMouseUpAsButton()
    {
        // Sometimes refuse to select. Has to be assigned to all selectable objects.  TODO: Choose another approach.
        // _selectionSprite.transform.SetParent(transform);
        // _selectionSprite.transform.localPosition = Vector3.zero;
    }
}