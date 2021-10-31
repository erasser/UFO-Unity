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
    ///     UFO shoots at selected target (or just straight if none), enemies shoot at UFO.
    /// </summary>
    /// <param name="shooter">Shooter object</param>
    public void FireRocket(GameObject shooter)
    {
        if (!Projectile.CanBeShot())
            return;

        var rocket = Instantiate(rocketPrefab);
        rocket.name = $"rocket_{Projectile.ProjectileCounter++}";
        rocket.transform.rotation = shooter.transform.rotation;
        rocket.transform.Find("rocketCamera").gameObject.SetActive(true);
        var rocketScript = rocket.GetComponent<Projectile>();
        var missileSupervisor = rocket.GetComponent<MissileSupervisor>();

        // TODO: Disable collider instead of using trigger? - Test with enemy
        // TODO: Test with rotated shooter
        // TODO: Fix rocket deploy when UFO is moving
        // TODO: Make rocket camera texture not visible by default
        // TODO: Consider making rockets destroyable by rockets?
        // TODO: Ensure rocket will not hit the shooter, if the shooter is in the rocket's way (e.g. above the shooter)
        // TODO: (done) Use fixed DeltaTime
        // TODO: (done) Other rocket fucks previous rocket motion - Maybe it's because the same Key
        // TODO: (done) Use Float Tween - I won't implement this
        
        // Pull the rocket down using tween
        rocket.transform.position = shooter.transform.position;

        float shooterHalfHeight;
        if (shooter == Ufo.Script.gameObject)  // TODO: Temporary solution, fix it in SelectedObjectDynamic
            shooterHalfHeight = shooter.transform.Find("UFO_body").GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * shooter.transform.lossyScale.y;
        else
            shooterHalfHeight = shooter.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * shooter.transform.lossyScale.y;

        rocket.transform.Translate(Vector3.down * shooterHalfHeight);

        var endPos = shooter.transform.position + (rocket.transform.position - shooter.transform.position).normalized * .7f;  // Is not affected by rocketScript.halfHeight

        var startPos = rocket.transform.position;  
        rocket.Tween($"tween_{rocket.name}", startPos, endPos, 1.2f, TweenScaleFunctions.Smootherstep, UpdatePos, MoveCompleted);

        void UpdatePos(ITween<Vector3> t)
        {
            rocket.transform.position = t.CurrentValue;
            
            // TODO: Fucking optimize! ------------------------------------------------ ! (.05 can be added once, can I get rid of .magnitude?)
            // Prevents rocket from colliding with the shooter
            // if ((rocket.transform.position - shooter.transform.position).magnitude > rocketScript.halfHeight + shooterHalfHeight + .2f)
            //     rocket.GetComponent<BoxCollider>().enabled = true;
        }

        void MoveCompleted(ITween<Vector3> t)
        {
            // if (rocket.GetComponent<BoxCollider>().enabled == false)  // For sure
                rocket.GetComponent<BoxCollider>().enabled = true;
            
            rocket.GetComponent<TrailRenderer>().enabled = true;

            missileSupervisor.m_guidanceSettings.m_target = SetWeaponTarget(shooter, missileSupervisor);
            missileSupervisor.m_launchCustomDir = rocket.transform.forward;
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
