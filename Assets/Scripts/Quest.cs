using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public struct Quest
{
    // TODO: Implement sub-quests. Main quest will have its _mainQuests of own sub-quests. GetNext() will get next sub-quest; if there is none, get next main quest.
    //       add _subQuests 
    
    private readonly string _name;    
    private readonly string _description;
    private readonly string _accomplishedText;
    private readonly GameObject _questTarget;
    private bool _done;
    private static int _activeQuestIndex;  // index of active quest
    // add type?
    // add parent GameObject?

    private static Quest _active;
    // private static readonly Quest[] MainQuests;
    // List<int> termsList = new List<int>();

    private static readonly List<Quest> MainQuests = new List<Quest>();

    private static string _questText;  // Text to be shown in UI
    private static readonly Text QuestsText;
    private static readonly GameObject TargetLocationMarker;

    public static void Init()
    {
        /* Must be called, so the static constructor is executed. Just to fuck with me. */
        //https://stackoverflow.com/questions/59938630/why-the-static-constructor-in-a-struct-gets-called-when-calling-a-non-static-met
        
        ShowActiveQuest();
    }

    // dynamic constructor (has a static context)
    private Quest(string name, string description, string accomplishedText/*, Vector3 location*/)
    {
        _name = name;
        _description = description;
        _accomplishedText = accomplishedText;
        _questTarget = GameObject.Find("questTargets").transform.GetChild(MainQuests.Count).gameObject;
        _done = false;
    }

    // static constructor
    static Quest()
    {
        //https://stackoverflow.com/questions/59938630/why-the-static-constructor-in-a-struct-gets-called-when-calling-a-non-static-met

        /* Initialize quests */
        MainQuests.Add(new Quest(
            "Fly there! (#1)",
            "Locate and reach target destination.",
            "First destination reached!"/*,
            new Vector3( 200, 50, -100)*/));

        MainQuests.Add(new Quest(
            "Now fly there! (#2)",
            "Locate and reach this target destination.",
            "Send destination reached!"/*,
            new Vector3( 300, 40, -120)*/));

        MainQuests.Add(new Quest(
            "And now fly there! (#3)",
            "Reach and investigate target destination.",
            "Third destination reached!"/*,
            new Vector3( 250, 60, -80)*/));
        
        MainQuests.Add(new Quest(
            "No quest",
            "You have finished all quests. Now you can fuck off.",
            "Fuck off."/*,
            new Vector3( 0, 0, 0)*/));
        
        // MainQuests = new[] {quest0, quest1, quest2, quest3};

        _active = MainQuests[0];

        QuestsText = GameObject.Find("questsText").GetComponent<Text>();
        TargetLocationMarker = GameObject.Find("questTarget");
    }

    // private static Quest GetFirst()
    // {
    //     var quest = MainQuests[0];
    //     ShowActiveQuest();
    //     return quest;
    // }

    private static void Next()
    {
        if (_activeQuestIndex == MainQuests.Count - 1)
            return;
        // var quest = MainQuests[++_activeQuestIndex];
        // ShowActiveQuest();
        // return quest;
        _active = MainQuests[++_activeQuestIndex];
        ShowActiveQuest();
    }

    public static void Complete()
    {
        _active._done = true;
        // TODO: Show _accomplishedText
        Next();
    }

    // private static Quest GetActive()
    // {
    //     var quest = MainQuests[_activeQuestIndex];
    //     ShowActiveQuest();
    //     return quest;
    // }

    private static void ShowActiveQuest()  // https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html
    {
        var quest = MainQuests[_activeQuestIndex];
        
        // ▶▷▸▹▻◆◈◇
        QuestsText.text = "◈ <b>" + quest._name + "</b>" +
                     "\n   " + new string('-', quest._name.Length) + "\n" +
                     quest._description;

        // ShowTextDynamically();  // TODO: Buď postupně zobrazit po znacích, nebo spíš zobrazit hned všechny znaky, ale probordelené a vybordelit z nich správný text.

        // TargetLocationMarker.transform.position = quest._questTarget;
    }

    private void ShowTextDynamically()
    {
        //QuestsText.text += "";
    }
}