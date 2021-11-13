using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    // ►► TODO: Vyjebávku ↓ by se dalo aplikovat na GameController, pak by všechny metody mohly být statické
    public static UI Instance;  // Vyjebávka, jak volat instanci ze static metody: https://answers.unity.com/questions/813021/call-invokerepeating-from-a-static-method-c.html
    private static Text _questText;
    public static string _questTextValue;
    public static int _questTextNameLength;
    private static string _additiveQuestText;
    private static GameObject _selectedObjectCameraTexture;
    private static Text _selectedObjectCameraText;
    private static GameObject _rocketCameraTexture;

    void Start()
    {
        Instance = this;
        // print(transform.Find("questText"));
        _questText = transform.Find("questText").GetComponent<Text>();
        _rocketCameraTexture = transform.Find("rocketCameraTexture").gameObject;
        _selectedObjectCameraTexture = transform.Find("selectedObjectCameraTexture").gameObject;
        _selectedObjectCameraText = _selectedObjectCameraTexture.transform.Find("selectedObjectCameraText").GetComponent<Text>();
    }

    public static void ToggleCameraTexture(string cameraName, bool enable)
    {
        if (cameraName == "selectedObjectCamera")
            _selectedObjectCameraTexture.SetActive(enable);
        else if (cameraName == "rocketCamera")
            _rocketCameraTexture.SetActive(enable);
    }

    public static void SetSelectedObjectTextureText(string text = "")
    {
        _selectedObjectCameraText.text = text;
    }
    
    public static void ShowQuestText(string name, string description)
    {
        // TODO: Buď postupně zobrazit po znacích, nebo spíš zobrazit hned všechny znaky, ale probordelené a vybordelit z nich správný text.
        _questTextValue = description;  // My way to pass parameter to invoked method
        _questText.text = $"◈ <b> {name} </b>\n    {new string('-', name.Length)}\n";
        _questTextNameLength = _questText.text.Length;
        Instance.InvokeRepeating(nameof(ShowTextDynamically), 0, 3);
        
        // ► Use of delegate: button.GetComponent<Button>().onClick.AddListener(delegate { Test2(i);});
    }
    
    private void ShowTextDynamically()
    {
        // TODO: Skip whitespaces + add _ or █ symbol at the end
        // var stringToAdd = _questTextValue.Substring(_questText.text.Length - _questTextNameLength, 2);
  //var stringToAdd = _questTextValue[_questText.text.Length - _questTextNameLength];
        // if (stringToAdd == " " || stringToAdd == "\n" || stringToAdd == "\t") // String.IsNullOrWhiteSpace()
        //     stringToAdd += _questTextValue.Substring(_questText.text.Length - _questTextNameLength + 1, 1);
        _questText.text += _questTextValue[_questText.text.Length - _questTextNameLength];
        if (_questText.text.Length == _questTextNameLength + _questTextValue.Length)
            Instance.CancelInvoke(nameof(ShowTextDynamically));
    }
}
