using System;
using DigitalRuby.Tween;
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

    /// <summary>
    ///     <para>UFO shoots at selected target (or just straight if none), enemies shoot at UFO</para>
    /// </summary>
    /// <param name="shooter">Shooter object</param>
    public void FireRocket(GameObject shooter)
    {
        if (!Projectile.CanBeShot())
            return;

        var rocket = Instantiate(rocketPrefab);
        var missileSupervisor = rocket.GetComponent<MissileSupervisor>();
        rocket.transform.Find("rocketCamera").gameObject.SetActive(true);

        // Pull the rocket down using tween
        var startPos = shooter.transform.position;
        rocket.gameObject.Tween("MoveRocket", startPos, new Vector3(startPos.x, startPos.y - 1, startPos.z), 2, TweenScaleFunctions.Smoothstep, UpdatePos, MoveCompleted);

        void UpdatePos(ITween<Vector3> t)
        {
            rocket.transform.position = t.CurrentValue;
        }

        void MoveCompleted(ITween<Vector3> t)
        {
            missileSupervisor.m_guidanceSettings.m_target = SetWeaponTarget(shooter, missileSupervisor);
            missileSupervisor.m_launchCustomDir = transform.forward;
            missileSupervisor.StartLaunchSequence();
        }

        // _gameControllerInstance.SelectObject(newRocket);
    }

    /// <param name="shooter">Shooter object</param>
    /// <param name="missileSupervisor">Used to pair missileSupervisor and its target both ways</param> 
    private GameObject SetWeaponTarget(GameObject shooter, MissileSupervisor missileSupervisor)
    {
        var target = Instantiate(GameController.Script.missileSupervisorTargetPrefab);
        target.GetComponent<MissileSupervisorTarget>().missileSupervisor = missileSupervisor;
        var shooterTransform = shooter.transform;

        if (shooter == GameController.ufo)
        {
            if (GameController.SelectedObject)
            {
                target.transform.SetParent(GameController.SelectedObject.transform);
                target.transform.localPosition = Vector3.zero;
            }
            else
                target.transform.position = shooterTransform.position + 10000 * shooterTransform.forward;
        }
        else
        {
            target.transform.SetParent(GameController.ufo.transform);
            target.transform.localPosition = Vector3.zero;
        }

        return target;
    }

}
