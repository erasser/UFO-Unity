using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: Zcela odstranit vzorový asteroid ze scény - dá se vytvořit prefab (drag & drop z hierarchy view do assets) a použít ten? 

public class SaturnController : MonoBehaviour
{
    public GameObject asteroidPrefab;
    //public GameObject collisionSphere;

    void Start()
    {
        GenerateAsteroids(800);
        
        /*
        _asteroid = GameObject.Find("asteroid");
        _asteroidsParent = GameObject.Find("AsteroidsParent");
        // _saturnRing = GameObject.Find("SaturnRing");

        for (var i = 0; i < 1000; i++)
        {
            var innerRadius = 2000;
            var outerRadius = 4000;

            var distance = Random.Range(innerRadius, outerRadius);
            var angle = Random.value * TwoPI;
            var rotation = Random.rotation;  // TODO: Isn't this in a range <0;PI>? (due to a character of quaternions)

            float x = Mathf.Cos(angle) * distance;
            float z = Mathf.Sin(angle) * distance;

            // float distance = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(z, 2));

            //           max  * /                   range <0;1>                      \  min size
            // var scale = 2 * (outerRadius - innerRadius) / (distance - innerRadius) + .1f;  // more distant => bigger asteroid
            var scale = Random.Range(.1f, 4) / transform.localScale.x;  // Make asteroid scale independent on Saturn's scale
            
            var asteroid = Instantiate(_asteroid, _asteroidsParent.transform, true);
            asteroid.transform.position = new Vector3(x, Random.value * 200 - 100, z);  // TODO: Make them more dense to y=0
            asteroid.transform.rotation = rotation;
            asteroid.transform.localScale = new Vector3(scale, scale, scale);

            asteroid.GetComponent<Rigidbody>().mass = Mathf.Pow(scale + .8f, 3) * 1200;
            // Can't use this if isKinematic & non-convex properties are set to true
            // asteroid.GetComponent<Rigidbody>().AddRelativeTorque(rotation.eulerAngles * 100, ForceMode.Force);  // use mass
            
            // asteroid.GetComponent<Rigidbody>().AddComponent<ConstantForce>();
            // var constantTorque = asteroid.GetComponent<ConstantForce>();
            // constantTorque.relativeTorque = new Vector3(0,10,0);
        }

        
        // MeshFilter meshFilter = _saturnRing.GetComponent<MeshFilter>();
        // _saturnRing.GetComponent<MeshRenderer>().enabled = false;
        //
        // for (var i = 0; i < meshFilter.mesh.vertexCount; i++)
        // {
        //     var asteroid = Instantiate(_asteroid, _saturnRing.transform, true);
        //     asteroid.transform.position = meshFilter.mesh.vertices[i];
        //     asteroid.transform.rotation = Random.rotation;
        //     var scale = Random.value + .2f;
        //     asteroid.transform.localScale = new Vector3(scale, scale, scale);
        // }
        */
    }

    private void GenerateAsteroids(int count = 1000)
    {
        for (var i = 0; i < count; i++)
            Instantiate(asteroidPrefab)
                .GetComponent<AsteroidController>()
                .Create(i/*, collisionSphere*/);
    }

    // Zdá se mi, že při FixedUpdate() je pohyb asteroidů méně sekaný než při Update()
    void FixedUpdate()
    {
        // transform.Rotate(Vector3.up, .1f * Time.fixedDeltaTime);
        
        // napiču rotace
        // foreach (Transform asteroid in _asteroidsParent.transform)
        // {
            // var amount = 1 / asteroid.transform.localScale.x;
            // asteroid.transform.Rotate(asteroid.transform.eulerAngles, .9f);
            // asteroid.transform.Rotate(new Vector3(0,1,0), .9f);
            
        // }
    }
}
