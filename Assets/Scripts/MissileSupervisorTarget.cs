using SparseDesign.ControlledFlight;
using UnityEngine;

public class MissileSupervisorTarget : MonoBehaviour
{
    public MissileSupervisor missileSupervisor;
    
    void Start()
    {
        
    }

    // public void OnDestroy()
    // {
    //     print("Target is to be destroyed");
    //     print(GetComponent<MissileSupervisor>());
    //     print(GetComponent<MissileSupervisor>().m_guidanceSettings.m_target);
    //     // GetComponent<MissileSupervisor>().m_guidanceSettings.m_target = null;
    // }
}
