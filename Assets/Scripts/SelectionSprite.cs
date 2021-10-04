using UnityEngine;
using static GameController;

// This script is not running when selection sprite is inactive

public class SelectionSprite : MonoBehaviour
{
    // private GameController _gameControllerInstance;

    void Start()
    {
        // _gameControllerInstance = GameObject.Find("GameController").transform.GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        UpdateMe();
    }

    private void UpdateMe()
    {
        transform.position = SelectedObjectMustCenterPivot  // I've chosen this approach, so I need not to manage instantiating & destroying the selection sprite
            ? SelectedObject.transform.position + SelectedObjectRelativeUpPosition
            : SelectedObject.transform.position;

        transform.LookAt(Ufo.UfoCamera.transform.position);
    }
}
