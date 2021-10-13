using System;
using UnityEngine;

// This script is dynamically added to each selected object (just once).

public class SelectedObjectDynamic : MonoBehaviour
{
    private GameController _gameControllerInstance;
    public float boundingSphereRadius;
    public float verticalExtents;
    public float horizontalToVerticalSizeRatio;
    public float cameraDistance;

    private void Awake()
    {
        CalculateBoundingSphereRadius();  // This is needed to be in Awake()
    }

    private void OnDestroy()
    {
        _gameControllerInstance.SelectNone();
    }

    private void CalculateBoundingSphereRadius()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;

        var thisTransform = transform;
        var lossyScale = thisTransform.lossyScale;
        verticalExtents = mesh.bounds.extents.y * lossyScale.y;

        // The same bounding sphere as SphereCollider has (I believe)
        boundingSphereRadius = Mathf.Max(
            mesh.bounds.extents.x * lossyScale.x,
            verticalExtents,
            mesh.bounds.extents.z * lossyScale.z);

        horizontalToVerticalSizeRatio = Mathf.Max(
            mesh.bounds.extents.x * lossyScale.x,
            mesh.bounds.extents.z * lossyScale.z) / verticalExtents;

        /*  Real bounding sphere, but it's too big :(  */
        // Mathf.Sqrt(Mathf.Pow(mesh.bounds.extents.x, 2) +
        // Mathf.Pow(mesh.bounds.extents.y, 2) +
        // Mathf.Pow(mesh.bounds.extents.z, 2)) * transform.lossyScale.z;

        // https://stackoverflow.com/questions/14614252/how-to-fit-camera-to-object
        var tg = Mathf.Tan(GameController.SelectedObjectCameraFOV.y * .0087266f);  // .0087266 = 1/(2*57.3) (2 is from formula, 57.3 is deg->rad conversion)
        var cameraDistanceFromHeight = verticalExtents * 2 / tg;
        var cameraDistanceFromWidth = Mathf.Max(
            mesh.bounds.extents.x * lossyScale.x,
            mesh.bounds.extents.z * lossyScale.z) * 2 / tg;

        cameraDistance = Mathf.Max(cameraDistanceFromHeight, cameraDistanceFromWidth) * .7f;
    }
}
