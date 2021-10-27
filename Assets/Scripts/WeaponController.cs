using SparseDesign.ControlledFlight;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject rocketPrefab;
    public static WeaponController Script;


    void Start()
    {
        Script = GetComponent<WeaponController>();
    }


    void Update()
    {
        
    }
    
    /// <param name="shooter">Shooter object</param>
    public void FireRocket(GameObject shooter)
    {
        if (!Projectile.CanBeShot())
            return;

        var newRocket = Instantiate(rocketPrefab);
        var missileSupervisor = newRocket.GetComponent<MissileSupervisor>();

        newRocket.transform.localPosition = transform.localPosition;
        newRocket.transform.Translate(Vector3.down);
        missileSupervisor.m_guidanceSettings.m_target = SetWeaponTarget(shooter, missileSupervisor);
        missileSupervisor.m_launchCustomDir = transform.forward;
        missileSupervisor.StartLaunchSequence();

        // _gameControllerInstance.SelectObject(newRocket);
    }

    /// <param name="shooter">Shooter object</param>
    /// <param name="missileSupervisor">Used to pair missileSupervisor and its target both ways</param> 
    private GameObject SetWeaponTarget(GameObject shooter, MissileSupervisor missileSupervisor)
    {
        var target = Instantiate(GameController.Script.missileSupervisorTargetPrefab);
        target.GetComponent<MissileSupervisorTarget>().missileSupervisor = missileSupervisor;
        var shooterTransform = shooter.transform;

        if (GameController.SelectedObject)
        {
            target.transform.SetParent(GameController.SelectedObject.transform);
            target.transform.localPosition = Vector3.zero;
        }
        else
            target.transform.position = shooterTransform.position + 10000 * shooterTransform.forward;

        return target;
    }

}
