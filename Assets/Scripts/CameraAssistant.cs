using UnityEngine;

/** Used for selected object camera **/

public class CameraAssistant : MonoBehaviour
{
    public static CameraAssistant Instance;
    public Vector2 selectedObjectCameraFOV;  // .x = horizontal FOV, .y = vertical FOV

    void Awake()
    {
        Instance = this;
        var thisCamera = transform.GetComponent<Camera>();
        var vFOV = thisCamera.fieldOfView;
        selectedObjectCameraFOV = new Vector2(Camera.VerticalToHorizontalFieldOfView(vFOV, thisCamera.aspect), vFOV);
    }
}
