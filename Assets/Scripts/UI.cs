using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    // ►► TODO: Vyjebávku ↓ by se dalo aplikovat na GameController, pak by všechny metody mohly být statické
    private static UI _instance;  // Vyjebávka, jak volat instanci ze static metody: https://answers.unity.com/questions/813021/call-invokerepeating-from-a-static-method-c.html
    private static Text _questText;
    public static string _questTextFunctionParameter;
    private static string _additiveQuestText;

    void Start()
    {
        _instance = this;
        // _questText = transform.Find("questText").GetComponent<Text>();
    }

    public static void ShowQuestText(string text)
    {
        return;
        // TODO: Buď postupně zobrazit po znacích, nebo spíš zobrazit hned všechny znaky, ale probordelené a vybordelit z nich správný text.
        _questTextFunctionParameter = text;  // My way to pass parameter to invoked method
        _questText.text = "";
        _instance.InvokeRepeating(nameof(ShowTextDynamically), 0, .1f);
    }
    
    private void ShowTextDynamically()
    {
        return;
        // TODO: Skip whitespaces + add _ or █ symbol at the end
        _questText.text += _questTextFunctionParameter[_questText.text.Length];
        if (_questText.text.Length == _questTextFunctionParameter.Length)
            _instance.CancelInvoke(nameof(ShowTextDynamically));

    }
}
