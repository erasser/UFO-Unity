using System;
using DigitalRuby.Tween;
using SparseDesign.ControlledFlight;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Could use singleton for this

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [SerializeField] private FixedJoystick joystickHorizontalPlane;
    [SerializeField] private FixedJoystick joystickVerticalPlane;
    [SerializeField] private GameObject selectionSpritePrefab;
    private static GameObject _arrow;
    private static Text _infoText; // UI element
    private Button _laserButton; // UI element
    public static GameObject SelectedObject;
    public static RigidbodyAssistant RigidbodyAssistantScript;
    // public static float SelectedObjectWorldRadius;
    public static Vector3 SelectedObjectRelativeUpPosition;
    private RaycastHit _selectionHit;
    private static GameObject _selectedObjectCamera;
    private static GameObject _rocketCamera;
    public static bool SelectedObjectMustCenterPivot; // Buildings have pivot at bottom => pivot must be centered for camera rotation.  
    private GameObject _3dGrid;
    public GameObject ufoPrefab;
    // public Ufo ufoScript;
    public GameObject selectionSpriteInstance;
    public GameObject missileSupervisorTargetPrefab;  // It's just invisible dummy
    public static GameObject Enemy;
    public static GameController Script;
    public static GameObject ufo;
    private static GameObject _buildingsParent;

    void Start()
    {
        Instance = this;
        _infoText = GameObject.Find("InfoText").GetComponent<Text>();
        _arrow = GameObject.Find("arrow");
        Enemy = GameObject.Find("jet");
        _laserButton = GameObject.Find("laserButton").GetComponent<Button>();
        _laserButton.onClick.AddListener(Ufo.ToggleLaser);
        _3dGrid = GameObject.Find("3d_grid_planes");
        // _ufoInstance = GameObject.Find("UFO").transform.GetComponent<Ufo>();            // Ufo.cs - allows me to call non-static method
        selectionSpriteInstance = Instantiate(selectionSpritePrefab);
        ufo = Instantiate(ufoPrefab);
        _buildingsParent = GameObject.Find("city").transform.Find("buildings").gameObject;
        _rocketCamera = transform.Find("rocketCamera").gameObject;
        _selectedObjectCamera = transform.Find("selectedObjectCamera").gameObject;
        ToggleRenderTextureCamera(_rocketCamera, false);
        ToggleRenderTextureCamera(_selectedObjectCamera, false);

        Quest.Init();
    }

    void Update()
    {
        ProcessTouchEvent(); // Test click & touch event, (un)select object
    }

    private void FixedUpdate()
    {
        ProcessControls();   // Causes UFO movement
        UpdateSelection();   // Updates selection sprite & camera
        UpdateArrow();
        Update3dGrid();
    }

    private void ProcessControls()
    {
        // _infoText.text = EventSystem.current.IsPointerOverGameObject() + ", " + EventSystem.current.IsPointerOverGameObject(0);

        if (joystickHorizontalPlane.Direction.x != 0 || joystickHorizontalPlane.Direction.y != 0 ||
            joystickVerticalPlane  .Direction.x != 0 || joystickVerticalPlane  .Direction.y != 0)

            Ufo.Instance.MoveUfo(joystickHorizontalPlane, joystickVerticalPlane);

        if (Input.GetKey(KeyCode.Space)         ||
            Input.GetKey(KeyCode.LeftControl)   ||
            Input.GetKey(KeyCode.W)             ||
            Input.GetKey(KeyCode.A)             ||
            Input.GetKey(KeyCode.S)             ||
            Input.GetKey(KeyCode.D)             ||
            Input.GetKey(KeyCode.Q)             ||
            Input.GetKey(KeyCode.E)             ||
            Input.GetKey(KeyCode.LeftShift))

            Ufo.Instance.MoveUfo();

        if (Input.GetKey(KeyCode.X)/* ||
            Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && !EventSystem.current.IsPointerOverGameObject(0) && !Input.GetMouseButtonDown(0)*/)
            // WeaponController.Instance.FireRocket(global::Ufo.Instance.gameObject);  //ha!
            WeaponController.Instance.FireRocket(ufo);

        if (Input.GetKey(KeyCode.C))
            WeaponController.Instance.FireRocket(Enemy);

        if (Input.GetKey(KeyCode.U))
            Resources.UnloadUnusedAssets();

        // if (Input.GetKey(KeyCode.Y))
        //     ChangeTargetsDebug();
    }

    private void ProcessTouchEvent()
    {
        // TODO: Add selected object info
        // TODO: Use layers to avoid touching UI https://answers.unity.com/questions/132586/game-object-as-touch-trigger.html

        // IsPointerOverGameObject (i.e. actually EventSystem object):
        //      I observed, that without an argument it's true when hovering UI on PC. With argument (0) it's true when touching UI on mobile.

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(0))
#endif
        {
            if (Physics.Raycast(Ufo.UfoCameraComponentCamera.ScreenPointToRay(Input.mousePosition), out _selectionHit))
            {
                // TODO: Use layerMask parameter, when number of ignored objects grows. https://docs.unity3d.com/Manual/Layers.html
                if (SelectedObject != _selectionHit.collider.gameObject && !_selectionHit.collider.CompareTag("UFO"))
                {
                    SelectObject(_selectionHit.collider.gameObject);
                }
                else
                    SelectNone();
            }
            else
                SelectNone();
        }
    }

    public void SelectObject(GameObject obj)  // Set clicked object as selected
    {
        if (obj.CompareTag("questTarget")) return;

        SelectedObject = obj;

        RigidbodyAssistantScript = SelectedObject.transform.GetComponent<RigidbodyAssistant>() ?? (RigidbodyAssistant) SelectedObject.AddComponent(Type.GetType("RigidbodyAssistant"));

        ToggleRenderTextureCamera(_selectedObjectCamera, true);
        UI.SetSelectedObjectTextureText(SelectedObject.name);
        SelectedObjectMustCenterPivot = IsBuilding(SelectedObject);

        SelectedObjectRelativeUpPosition = SelectedObjectMustCenterPivot ? new Vector3(0, RigidbodyAssistantScript.verticalExtents, 0) : Vector3.zero;
        var scale = RigidbodyAssistantScript.boundingSphereRadius * 4;
        selectionSpriteInstance.transform.localScale = new Vector3(scale, scale, scale);  // uff
        selectionSpriteInstance.SetActive(true);
    }

    public void SelectNone()
    {
        try
        {
            SelectedObject = null;
            ToggleRenderTextureCamera(_selectedObjectCamera, false);
            UI.SetSelectedObjectTextureText();
            selectionSpriteInstance.SetActive(false);
        }
        catch (Exception e)
        {
            print("» This message should be printed when the game is stopped only. If you see this during the game, check the error here ↓↓ .");
            throw new Exception(e.Message);
        }
    }

    private void UpdateSelection()  // Updates selection camera & selection sprite in realtime
    {
        if (!SelectedObject) return;

        var relativePivotPosition = SelectedObjectMustCenterPivot ? new Vector3(0, RigidbodyAssistantScript.verticalExtents, 0) : Vector3.zero;
        var cameraVerticalOffset = new Vector3(0, RigidbodyAssistantScript.verticalExtents / 8, 0);  // The higher is the camera, the lower is the camera target

        // camera position
        var selectedObjectPosition = SelectedObject.transform.position;
        _selectedObjectCamera.transform.position = selectedObjectPosition + new Vector3(
            Mathf.Cos(Time.time / 4) * RigidbodyAssistantScript.cameraDistance,  // there was .boundingSphereRadius * 2
            RigidbodyAssistantScript.verticalExtents,
            Mathf.Sin(Time.time / 4) * RigidbodyAssistantScript.cameraDistance) + relativePivotPosition + cameraVerticalOffset;

        // camera target
        var cameraTarget = selectedObjectPosition + relativePivotPosition;
        _selectedObjectCamera.transform.LookAt(cameraTarget - cameraVerticalOffset, Vector3.up);

        // camera-view obstacle detection
        var raycastDirectionVector = _selectedObjectCamera.transform.position - cameraTarget;
        // Debug.DrawRay(cameraTarget, raycastDirectionVector * 10, Color.yellow);
        if (Physics.Raycast(cameraTarget, raycastDirectionVector, out var hit, raycastDirectionVector.magnitude))
        {
            // TODO: Check, if this comparison works
            if (hit.collider.gameObject != SelectedObject)  // Exclude selected object itself
               _selectedObjectCamera.transform.localPosition = hit.point;
            // TODO: Eventually the obstacle could not be rendered (e.g. if it's too close)
        }
    }

    private void Update3dGrid()
    {
    var localEulerAngles = ufo.transform.localEulerAngles;
    // _3dGrid.transform.localEulerAngles = new Vector3(
    //     localEulerAngles.x,
    //     -localEulerAngles.y,
    //     localEulerAngles.z);
    }

    private void UpdateArrow()
    {
        if (!Quest.Current.QuestTarget)
            return;
//         Ufo.Instance.gameObject.transform.LookAt(Quest.Current.QuestTarget.transform);
    }

    /// <summary>
    /// Manages destroying objects at one place, so OnDestroy() on every fucking object is not necessary.
    /// </summary>
    public void DestroyGameObject(GameObject obj)
    {
        /***  If the object is selected, unselect it (does not solve selected children, but it's not needed now). */
        if (obj == SelectedObject)
            SelectNone();

        /***  If the object is a target (i.e. has attached a missileSupervisorTarget), let projectiles continue in their actual direction. */
        var objTransform = obj.transform;
        foreach (Transform t in objTransform)
        {
            if (t.CompareTag("missileSupervisorTarget"))
            {
                t.gameObject.GetComponent<MissileSupervisorTarget>().missileSupervisor.SetDone();
            }
        }
        /*foreach (GameObject tar in targets)
        {
            // target ↓               script ↓      relevant missileSupervisor ↓    rocket ↓
            var projectile = tar.gameObject.GetComponent<MissileSupervisorTarget>().missileSupervisor.gameObject;
            tar.gameObject.transform.parent = null;
            var projectileTransform = projectile.GetComponent<Rigidbody>().transform;  // TODO: Remove rigid body when it works
            var vec = objTransform.position - projectileTransform.position;
            tar.gameObject.transform.position = projectileTransform.position + 400 * projectileTransform.forward;
        }*/

        /***  If the object is a rocket, destroy also its missileSupervisorTarget */
        if (obj.CompareTag("rocket"))
        {
            Destroy(obj.GetComponent<MissileSupervisor>().m_guidanceSettings.m_target);
            Projectile projectileScript = obj.GetComponent<Projectile>();
            WeaponController.ProcessBlast(projectileScript.collisionCoordinates, 30, 1000, projectileScript.hitObject, obj);
            // WeaponController.ProcessBlast_old(projectileScript.collisionCoordinates, 20, 100, projectileScript.hitObjectRigidbody, obj.GetComponent<Rigidbody>());
        }

        /***  If the object has a tween assigned, destroy the tween */ 
        for (int i = TweenFactory.Tweens.Count - 1; i >= 0; --i)
        {
            var tween = TweenFactory.Tweens[i];
            if (obj == tween.GetGameObject())
              TweenFactory.RemoveTween(tween, TweenStopBehavior.DoNotModify);
        }

        Destroy(obj);
    }

    public static bool IsBuilding(GameObject obj)
    {
        return obj.transform.parent?.gameObject == _buildingsParent;
    }

    public static void ToggleRenderTextureCamera(GameObject camera, bool enable)
    {
        camera.SetActive(enable);
        UI.ToggleCameraTexture(camera.name, enable);
    }
    
    public void ChangeTargetsDebug()
    {
        var objTransform = GameObject.Find("jet").transform;

        // var targets = new List<GameObject>();
        foreach (Transform t in objTransform)
        {
            if (t.CompareTag("missileSupervisorTarget"))
            {
                // target ↓               script ↓      relevant missileSupervisor ↓    rocket ↓
                var projectile = t.gameObject.GetComponent<MissileSupervisorTarget>().missileSupervisor.gameObject;
                t.gameObject.transform.parent = null;
                t.gameObject.transform.position = Vector3.zero;
            }
        }
    }
    
    public static float SignedSqrt(float number)
    {
        // square root ↓ of ↓ positive number respecting ↓ lost sign
        return Mathf.Sqrt(Math.Abs(number)) * Mathf.Sign(number);
    }

    // TODO: Co zneužít přetěžování operátorů + extendnout třídu Vector3?
    // This is because they won't allow me to change individual Vector component
    // private static void UpdateVectorComponent(ref Vector3 vectorToUpdate, string component, float value)
    // {
    //     string[] components = {"x", "y", "z"};
    //     var index = Array.IndexOf(components, component);
    //
    //     Vector3 newVector = vectorToUpdate;
    //     newVector[index] = value;
    //     vectorToUpdate = newVector;
    // }
}
