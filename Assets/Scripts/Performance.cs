using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
// On mobile shown FPS are always in range 9..29, it's strange

public static class Performance
{
    private static readonly Text PerformanceText;  // UI element
    private static int _frames;
    private static float _timeSum;

    static Performance()  // static constructor
    {
        PerformanceText = GameObject.Find("PerformanceText").GetComponent<Text>();
    }

    // called from Update()
    public static void ShowFPS()
    {
        if (_timeSum > 1)  // seconds
        {
            PerformanceText.text = Mathf.Floor(_frames / _timeSum).ToString();
            _frames = 0;
            _timeSum = 0;
        }
        else
        {
            _frames++;
            _timeSum += Time.deltaTime;
        }
    }

}