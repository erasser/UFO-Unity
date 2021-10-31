using UnityEngine;

/// This script is dynamically added to each selected object (just once).

public class SelectedObjectDynamic : MonoBehaviour
{
    // private GameController _gameControllerScript;
    public float boundingSphereRadius;
    public float verticalExtents;
    // public float horizontalToVerticalSizeRatio;
    public float cameraDistance;

    private void Awake()
    {
        CalculateBoundingSphereRadius();  // This is needed to be in Awake()
        // _gameControllerScript = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // TODOO: Ne všichni mají mesh filter (např. UFO). Asi to bude chtít traversovat children.
    private void CalculateBoundingSphereRadius()  // .extents is half of bounds
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

        // horizontalToVerticalSizeRatio = Mathf.Max(  // Use horizontalDiameter if I use this
        //     mesh.bounds.extents.x * lossyScale.x,
        //     mesh.bounds.extents.z * lossyScale.z) / verticalExtents;

        var horizontalDiameter = Mathf.Sqrt(mesh.bounds.extents.x * lossyScale.x * mesh.bounds.extents.z * lossyScale.z);

        /*  Real bounding sphere, but it's too big :(  */
        // Mathf.Sqrt(Mathf.Pow(mesh.bounds.extents.x, 2) +
        // Mathf.Pow(mesh.bounds.extents.y, 2) +
        // Mathf.Pow(mesh.bounds.extents.z, 2)) * transform.lossyScale.z;

        // https://stackoverflow.com/questions/14614252/how-to-fit-camera-to-object
        var tg = Mathf.Tan(GameController.SelectedObjectCameraFOV.y * .0087266f);  // .0087266 = 1/(2*57.3) (2 is from formula, 57.3 is deg->rad conversion)

        // var ratio = horizontalDiameter / (verticalExtents * 2);

        cameraDistance = Mathf.Max(verticalExtents / tg, horizontalDiameter / tg) * 1.4f;
    }
}
