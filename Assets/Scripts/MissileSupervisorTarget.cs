using UnityEngine;

public class MissileSupervisorTarget : MonoBehaviour
{
    void Start()
    {
        
    }

    public void OnDestroy()
    {
        // GameObject.Find("UFO").GetComponent<MissileSupervisor>()..m_guidanceSettings.m_target = null;
    }
}
