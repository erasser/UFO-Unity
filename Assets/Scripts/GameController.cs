using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// ►  All fields and methods should be static

public class GameController : MonoBehaviour
{
    [SerializeField] private FixedJoystick joystickHorizontalPlane;
    [SerializeField] private FixedJoystick joystickVerticalPlane;
    [SerializeField] private GameObject selectionSpritePrefab;
    private static GameObject _arrow;
    private static Text _infoText; // UI element
    private Button _laserButton; // UI element
    public static GameObject SelectedObject;
    public static float SelectedObjectWorldRadius;
    public static Vector3 SelectedObjectRelativeUpPosition;
    private RaycastHit _selectionHit;
    private static GameObject _selectedObjectCamera;
    private static GameObject _selectedObjectCameraTexture; // UI element
    private static Text _selectedObjectCameraText; // UI element
    public static bool SelectedObjectMustCenterPivot; // Buildings have pivot at bottom => pivot must be centered for camera rotation.  
    private GameObject _3dGrid;
    private Ufo _ufoInstance;
    private GameObject _selectionSpriteInstance;

    void Start()
    {
        _infoText = GameObject.Find("InfoText").GetComponent<Text>();
        _arrow = GameObject.Find("arrow");
        _laserButton = GameObject.Find("laserButton").GetComponent<Button>();
        _selectedObjectCamera = GameObject.Find("CameraSelectedObject");
        _selectedObjectCameraTexture = GameObject.Find("SelectedObjectCameraTexture");
        _selectedObjectCameraText = GameObject.Find("selectedObjectCameraText").GetComponent<Text>();
        _3dGrid = GameObject.Find("3d_grid_planes");
        _selectedObjectCamera.SetActive(false);
        _selectedObjectCameraTexture.SetActive(false);
        _laserButton.onClick.AddListener(Ufo.ToggleLaser);
        _ufoInstance = GameObject.Find("UFO").transform.GetComponent<Ufo>();            // Ufo.cs - allows me to call non-static method
        _selectionSpriteInstance = Instantiate(selectionSpritePrefab);
        
        Quest.Init();
    }

    void Update()
    {
        ProcessTouchEvent(); // Test click & touch event, (un)select object
    }

    private void FixedUpdate()
    {
        ProcessControls();  // cause UFO movement
        UpdateSelection();   // Updates selection sprite & camera
        UpdateArrow();
        Update3dGrid();
    }

    private void ProcessControls()
    {
        if (!(
            joystickHorizontalPlane.Direction.magnitude > 0 ||
            joystickVerticalPlane.Direction.magnitude > 0 ||
            Input.GetKey(KeyCode.Space) ||
            Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.Q) ||
            Input.GetKey(KeyCode.E) ||
            Input.GetKey(KeyCode.LeftShift)
        )) return;

        _ufoInstance.MoveUfo(joystickHorizontalPlane, joystickVerticalPlane);
    }

    private void ProcessTouchEvent()
    {
        // TODO: Add selected object info
        // TODO: Use layers to avoid touching UI https://answers.unity.com/questions/132586/game-object-as-touch-trigger.html
        if (!EventSystem.current.IsPointerOverGameObject() && (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (Physics.Raycast(Ufo.UfoCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out _selectionHit))
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
        SelectedObject = obj;

        if (!SelectedObject.transform.GetComponent<Ufo>())
            SelectedObject.AddComponent(Type.GetType("SelectedObjectDynamic"));

        _selectedObjectCamera.SetActive(true);
        _selectedObjectCameraTexture.SetActive(true);
        _selectedObjectCameraText.text = SelectedObject.name;
        SelectedObjectMustCenterPivot = SelectedObject.transform.parent && SelectedObject.transform.parent.gameObject == GameObject.Find("buildings");

        var sphereCollider = SelectedObject.AddComponent<SphereCollider>();
        SelectedObjectWorldRadius = sphereCollider.radius * SelectedObject.transform.lossyScale.x;
        Destroy(sphereCollider);
        SelectedObjectRelativeUpPosition = SelectedObjectMustCenterPivot ? new Vector3(0, SelectedObjectWorldRadius, 0) : Vector3.zero;
        var scale = SelectedObjectWorldRadius * 4;
        _selectionSpriteInstance.transform.localScale = new Vector3(scale, scale, scale);  // uff
        _selectionSpriteInstance.SetActive(true);
    }

    public void SelectNone()
    {
        SelectedObject = null;
        _selectedObjectCamera.SetActive(false);
        _selectedObjectCameraTexture.SetActive(false);
        _selectedObjectCameraText.text = "";
        _selectionSpriteInstance.SetActive(false);
    }

    private void UpdateSelection()  // Updates selection camera & selection sprite in realtime
    {
        if (!SelectedObject) return;

        var relativePivotPosition = SelectedObjectMustCenterPivot ? new Vector3(0, SelectedObjectWorldRadius, 0) : Vector3.zero;

        _selectedObjectCamera.transform.position = SelectedObject.transform.position + new Vector3(
            Mathf.Cos(Time.time / 4) * SelectedObjectWorldRadius * 2,
            SelectedObjectWorldRadius * 1.1f,
            Mathf.Sin(Time.time / 4) * SelectedObjectWorldRadius * 2) + relativePivotPosition;

        _selectedObjectCamera.transform.LookAt(SelectedObject.transform.position + relativePivotPosition, Vector3.up);
    }

    private void Update3dGrid()
    {
        _3dGrid.transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            -transform.localEulerAngles.y,
            transform.localEulerAngles.z);
    }

    private void UpdateArrow()
    {
        if (!Quest.Current.QuestTarget)
            return;
        _arrow.transform.LookAt(Quest.Current.QuestTarget.transform);
    }
    
    private static float SignedSqrt(float number)
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
