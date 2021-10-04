using UnityEngine;

// This script is dynamically added to each selected object (just once).

public class SelectedObjectDynamic : MonoBehaviour
{
    private GameController _gameControllerInstance;
    public float boundingSphereRadius;

    void Start()
    {
        _gameControllerInstance = GameObject.Find("GameController").transform.GetComponent<GameController>();
        CalculateBoundingSphereRadius();
    }

    private void OnDestroy()
    {
        _gameControllerInstance.SelectNone();
    }

    private void CalculateBoundingSphereRadius()
    {
        /*  TODO: Remove this  -------------------------------------------------------------------------------
        if (boundingSphereRadius != 0) return;
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        if (!mesh.isReadable) return;
        print("zzz");
        float maxRadius = 0;
        var length = mesh.vertices.Length;
        for (var i = 0; i < length; i++)  // https://codingsight.com/foreach-or-for-that-is-the-question
        {
            maxRadius = Mathf.Max(maxRadius, Mathf.Abs(Mathf.Sqrt(
                mesh.vertices[i].x * mesh.vertices[i].x +
                mesh.vertices[i].y * mesh.vertices[i].y +
                mesh.vertices[i].z * mesh.vertices[i].z)));
        }*/

        var mesh = GetComponent<MeshFilter>().sharedMesh;
        
        boundingSphereRadius = Mathf.Sqrt(
            mesh.bounds.extents.x * mesh.bounds.extents.x + transform.localScale.x * transform.localScale.x +
            mesh.bounds.extents.y * mesh.bounds.extents.y + transform.localScale.y * transform.localScale.y +
            mesh.bounds.extents.z * mesh.bounds.extents.z + transform.localScale.z * transform.localScale.z);
    }
}
