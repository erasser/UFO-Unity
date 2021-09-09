using UnityEngine;
using UnityEngine.UI;

public static class Performance
{
    private static readonly Text _performanceText;  // UI element
    private static int _frames;
    private static float _timeSum;

    static Performance()  // static constructor
    {
        _performanceText = GameObject.Find("PerformanceText").GetComponent<Text>();
    }

    // called from Update()
    public static void ShowFPS()
    {
        if (_timeSum > 1)
        {
            _performanceText.text = Mathf.Floor(_frames / _timeSum).ToString();
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