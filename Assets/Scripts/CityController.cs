using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
    void Start()
    {
        foreach (Transform childObject in transform)
        {
            MeshFilter meshFilter = childObject.gameObject.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                childObject.gameObject.AddComponent<Rigidbody>();                
                childObject.gameObject.AddComponent<BoxCollider>();                
            }
            
            // First we get the Mesh attached to the child object
            // Mesh mesh = childObject.gameObject.GetComponent<MeshFilter>().mesh;

            // If we've found a mesh we can use it to add a collider
            // if (mesh != null)
            // {                      
            // Add a new MeshCollider to the child object
            // MeshCollider meshCollider = childObject.gameObject.AddComponent<MeshCollider>();
            // BoxCollider boxCollider = childObject.gameObject.AddComponent<BoxCollider>();

            // Finally we set the Mesh in the MeshCollider
            // meshCollider.sharedMesh = mesh;
            // }
        }
    }

    void Update()
    {
        
    }
}
