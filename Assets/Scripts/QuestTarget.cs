using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTarget : MonoBehaviour
{
    // public int questIndex;  // index from Quest.MainQuests array
    
    void FixedUpdate()
    {
        transform.Rotate (- Time.deltaTime * 20 * Vector3.up, Space.World);
    }
}
