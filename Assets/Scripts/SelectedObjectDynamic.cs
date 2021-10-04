using UnityEngine;

// This script is dynamically added to each selected object (just once).

public class SelectedObjectDynamic : MonoBehaviour
{
    private GameController _gameControllerInstance;

    void Start()
    {
        _gameControllerInstance = GameObject.Find("GameController").transform.GetComponent<GameController>();
    }

    private void OnDestroy()
    {
        _gameControllerInstance.SelectNone();
    }
}
