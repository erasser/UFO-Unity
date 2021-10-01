using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTarget : MonoBehaviour
{
    // public int questIndex;  // index from Quest.MainQuests array
    
    void Update()
    {
        transform.Rotate (- Time.deltaTime * 20 * Vector3.up, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UFO"))
        {
            print("complete");
            Quest.Complete();
        }
    }

    // public int Get()
    // {
    //     return questIndex;
    // }
}
