using System;
using UnityEngine;

// This script is dynamically added to each selected object (just once).

public class SelectedObjectDynamic : MonoBehaviour
{
    private GameController _gameControllerInstance;
    public float boundingSphereRadius;
    public float verticalRadius;

    private void Awake()
    {
        CalculateBoundingSphereRadius();
    }

    void Start()
    {
        _gameControllerInstance = GameObject.Find("GameController").transform.GetComponent<GameController>();
    }

    private void OnDestroy()
    {
        _gameControllerInstance.SelectNone();
    }

    private void CalculateBoundingSphereRadius()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;

        verticalRadius = mesh.bounds.extents.y * transform.lossyScale.y;

        // The same bounding sphere as SphereCollider has (I believe)
        boundingSphereRadius = Mathf.Max(
            mesh.bounds.extents.x * transform.lossyScale.x,
            verticalRadius,
            mesh.bounds.extents.z * transform.lossyScale.z);

        // Real bounding sphere, but it's too big :(
        // Mathf.Sqrt(Mathf.Pow(mesh.bounds.extents.x, 2) +
        // Mathf.Pow(mesh.bounds.extents.y, 2) +
        // Mathf.Pow(mesh.bounds.extents.z, 2)) * transform.lossyScale.z;



    }
}
