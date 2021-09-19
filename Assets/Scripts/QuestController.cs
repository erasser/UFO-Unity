using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    private static Text _questsText;
    
    void Start()
    {
        _questsText = GameObject.Find("quests").GetComponent<Text>();
    }

}
