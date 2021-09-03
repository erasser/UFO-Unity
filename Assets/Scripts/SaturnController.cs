using System;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: Zcela odstranit vzorový asteroid ze scény - dá se vytvořit prefab (drag & drop z hierarchy view do assets) a použít ten? 
// TODO: Apply physics? :-O
public class SaturnController : MonoBehaviour
{
    private GameObject _asteroid;
    private GameObject _saturnRing;
    private const float TwoPI = 2 * Mathf.PI;

    void Start()
    {
        _asteroid = GameObject.Find("asteroid");
        _saturnRing = GameObject.Find("SaturnRing");

        for (var i = 0; i < 200; i++)
        {
            var innerRadius = 100;
            var outerRadius = 10000;

            float x = Mathf.Cos(TwoPI) * (outerRadius - innerRadius) + innerRadius;
            float z = Mathf.Sin(TwoPI) * (outerRadius - innerRadius) + innerRadius;

            float distance = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(z, 2));

            //           max  * /                   range <0;1>                      \  min size
            var scale = 2 * (outerRadius - innerRadius) / (distance - innerRadius) + .1f;  // more distant => bigger asteroid

            var asteroid = Instantiate(_asteroid, _saturnRing.transform, true);
            asteroid.transform.position = new Vector3(x, Random.value * 10 - 5, z);
            asteroid.transform.rotation = Random.rotation;
            asteroid.transform.localScale = new Vector3(scale, scale, scale);
        }

        /*MeshFilter meshFilter = _saturnRing.GetComponent<MeshFilter>();
        _saturnRing.GetComponent<MeshRenderer>().enabled = false;

        for (var i = 0; i < meshFilter.mesh.vertexCount; i++)
        {
            var asteroid = Instantiate(_asteroid, _saturnRing.transform, true);
            asteroid.transform.position = meshFilter.mesh.vertices[i];
            asteroid.transform.rotation = Random.rotation;
            var scale = Random.value + .2f;
            asteroid.transform.localScale = new Vector3(scale, scale, scale);
        }*/
    }

    // Zdá se mi, že při FixedUpdate() je pohyb asteroidů méně sekaný než při Update()
    void FixedUpdate()
    {
        transform.Rotate(Vector3.up, .2f * Time.fixedDeltaTime);
    }
}
