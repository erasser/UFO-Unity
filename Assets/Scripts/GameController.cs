using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// ►  All fields and methods should be static

public class GameController : MonoBehaviour
{
    [SerializeField] private FixedJoystick joystickHorizontalPlane;
    [SerializeField] private FixedJoystick joystickVerticalPlane;
    private static GameObject _arrow;
    private static Text _infoText; // UI element
    private Button _laserButton; // UI element
    private static GameObject _selectionSprite;
    public static GameObject SelectedObject;
    private static float _selectedObjectRadius;
    private RaycastHit _selectionHit;
    private static GameObject _selectedObjectCamera;
    private static GameObject _selectedObjectCameraTexture; // UI element
    private static Text _selectedObjectCameraText; // UI element

    private static bool
        _selectedObjectMustCenterPivot; // Buildings have pivot at bottom => pivot must be centered for camera rotation.  

    private GameObject _3dGrid;
    
    private Ufo _ufoInstance;


    void Start()
    {
        _infoText = GameObject.Find("InfoText").GetComponent<Text>();
        _arrow = GameObject.Find("arrow");
        _laserButton = GameObject.Find("laserButton").GetComponent<Button>();
        _selectionSprite = GameObject.Find("selectionSprite");
        _selectedObjectCamera = GameObject.Find("CameraSelectedObject");
        _selectedObjectCameraTexture = GameObject.Find("SelectedObjectCameraTexture");
        _selectedObjectCameraText = GameObject.Find("selectedObjectCameraText").GetComponent<Text>();
        _3dGrid = GameObject.Find("3d_grid_planes");
        _selectedObjectCamera.SetActive(false);
        _selectedObjectCameraTexture.SetActive(false);
        _selectionSprite.SetActive(false);
        _laserButton.onClick.AddListener(Ufo.ToggleLaser);
        _ufoInstance = GameObject.Find("UFO").transform.GetComponent<Ufo>();  // Ufo.cs - allows me to call non-static method

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

    public static void SelectObject(GameObject obj)  // Set clicked object as selected
    {
        SelectedObject = obj;

        _selectedObjectCamera.SetActive(true);
        _selectedObjectCameraTexture.SetActive(true);
        _selectedObjectCameraText.text = SelectedObject.name;
        _selectedObjectMustCenterPivot = SelectedObject.transform.parent && SelectedObject.transform.parent.gameObject == GameObject.Find("buildings");

        var addedCollider = false;
        if (!SelectedObject.GetComponent<SphereCollider>())
        {
            SelectedObject.AddComponent<SphereCollider>();
            addedCollider = true;
        }
        _selectedObjectRadius = SelectedObject.GetComponent<SphereCollider>().radius;

        if (addedCollider)
            Destroy(SelectedObject.GetComponent<SphereCollider>());

        _selectionSprite.SetActive(true);
        _selectionSprite.transform.SetParent(SelectedObject.transform);
        _selectionSprite.transform.localPosition = _selectedObjectMustCenterPivot ? new Vector3(0, _selectedObjectRadius, 0) : Vector3.zero;
        var scale = _selectedObjectRadius * 3;
        _selectionSprite.transform.localScale = new Vector3(scale, scale, scale);  // uff

        _selectedObjectRadius *= SelectedObject.transform.lossyScale.x;
    }

    public void SelectNone()
    {
        SelectedObject = null;
        _selectionSprite.SetActive(false);
        _selectedObjectCamera.SetActive(false);
        _selectedObjectCameraTexture.SetActive(false);
        _selectedObjectCameraText.text = "";
    }

    private void UpdateSelection()  // Updates selection camera & selection sprite in realtime
    {
        if (!SelectedObject) return;

        var relativePivotPosition = _selectedObjectMustCenterPivot ? new Vector3(0, _selectedObjectRadius, 0) : Vector3.zero;
        
        _selectionSprite.transform.LookAt(Ufo.UfoCamera.transform.position);

        _selectedObjectCamera.transform.position = SelectedObject.transform.position + new Vector3(
            Mathf.Cos(Time.time / 4) * _selectedObjectRadius * 2,
            _selectedObjectRadius * 1.1f,
            Mathf.Sin(Time.time / 4) * _selectedObjectRadius * 2) + relativePivotPosition;

        _selectedObjectCamera.transform.LookAt(SelectedObject.transform.position + relativePivotPosition, Vector3.up);
    }

    private void Update3dGrid()
    {
        // _3dGrid.transform.rotation = Quaternion.LookRotation(_rigidBody.transform.eulerAngles);
        // _3dGrid.transform.rotation = transform.rotation;
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
