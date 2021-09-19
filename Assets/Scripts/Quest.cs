using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

//https://stackoverflow.com/questions/29833312/static-constructor-not-working-for-structs
//https://stackoverflow.com/questions/59938630/why-the-static-constructor-in-a-struct-gets-called-when-calling-a-non-static-met

public struct Quest
{
    // TODO: Implement sub-quests. Main quest will have its _mainQuests of own sub-quests. GetNext() will get next sub-quest; if there is none, get next main quest.
    //       add _subQuests 
    
    private string _name;    
    private string _description;
    private string _accomplishedText;
    private Vector3 _location;
    private bool _done;
    private static int _active;  // index of active quest
    // add type?
    // add parent GameObject?

    private static readonly Quest[] MainQuests;

    private static Text _questsText;
    private static GameObject _targetLocationMarker;



    // dynamic constructor
    public Quest(string name, string description, string accomplishedText, Vector3 location)
    {
        _name = name;
        _description = description;
        _accomplishedText = accomplishedText;
        _location = location;
        // _active = false;
        _done = false;
    }

    // static constructor
    static Quest()
    {
        /* Initialize quest _mainQuests */
        var quest0 = new Quest(
            "Fly there!",
            "Locate and reach target destination.",
            "First destination reached!",
            new Vector3( 500, 50, -100));

        var quest1 = new Quest(
            "Now fly there!",
            "Locate and reach this target destination.",
            "Send destination reached!",
            new Vector3( 300, 10, -120));

        var quest2 = new Quest(
            "And now fly there!",
            "Reach and investigate target destination.",
            "Third destination reached!",
            new Vector3( 300, 10, -120));
        
        MainQuests = new[] {quest0, quest1, quest2};
        
        _questsText = GameObject.Find("questsText").GetComponent<Text>();
        _targetLocationMarker = GameObject.Find("questTarget");
    }

    private static Quest GetFirst()
    {
        var quest = MainQuests[0];
        ShowActiveQuest();
        return quest;
    }

    private static Quest GetNext()
    {
        var quest = MainQuests[++_active];
        ShowActiveQuest();
        return quest;
    }

    private static Quest GetActive()
    {
        var quest = MainQuests[_active];
        ShowActiveQuest();
        return quest;
    }

    private static void ShowActiveQuest()
    {
        var quest = MainQuests[_active];
        // ▶▷▸▹▻◆◈◇
        _questsText.text = "◈ " + quest._name +
                           "\n   " + new string('-', quest._name.Length) + "\n" +
                           quest._description;

        _targetLocationMarker.transform.position = quest._location;
    }

    public static void Init()
    {
        /* Must be called, so the static constructor is executed. Just to fuck with me. */
        
        GetFirst();
    }
        
}