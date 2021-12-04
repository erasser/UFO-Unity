using System;
using System.Collections.Generic;
using DigitalRuby.Tween;
using SparseDesign.ControlledFlight;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance;
    public GameObject rocketPrefab;
    public static WeaponController Script;
    private float _projectileRotationZ; // Used just to pass variable to passed tween function
    private static Collider[] _blastSphereColliders = new Collider[100];

    void Start()
    {
        Instance = this;
    }

    /// <summary>
    ///     UFO shoots at selected target (or just straight if none), enemies shoot at UFO.
    /// </summary>
    /// <param name="shooter">Shooter object</param>
    public void FireRocket(GameObject shooter)
    {
        if (!Projectile.CanBeShot()) return;

        var rocket = Instantiate(rocketPrefab);
        rocket.name = $"rocket_{Projectile.ProjectileCounter++}";
        rocket.transform.rotation = shooter.transform.rotation;
        // rocket.transform.Find("rocketCamera").gameObject.SetActive(true);
        _projectileRotationZ = rocket.transform.eulerAngles.z;
        var missileSupervisor = rocket.GetComponent<MissileSupervisor>();
        missileSupervisor.m_guidanceSettings.m_target = SetWeaponTarget(shooter, missileSupervisor); // Must be set before end of the tween
        // var rocketScript = rocket.GetComponent<Projectile>();

        // TODO: Add some rocket camera management (try to use just 1 cam)
        // TODO: There remains some shit badly affecting performance when firing more missiles, it remains also when re-played
        //       (It's cleared on Unity restart). TrailRenderer obviously is not the cause.
        // TODO: Fix rocket deploy when UFO is moving
        // TODO: Make rocket camera texture not visible by default
        // TODO: Consider making rockets destroyable by rockets?
        // TODO: Ensure rocket will not hit the shooter, if the shooter is in the rocket's way (e.g. above the shooter) - It will. Maybe first push a bit the rocket forward and set a delay to launch method.
        // TODO: (done) Test with rotated shooter
        // TODO: (done) Disable collider instead of using trigger? - Test with enemy
        // TODO: (done) Implement rocket blast (blast objects in proximity)
        // TODO: (done) Buildings are targeted to their bottoms
        // TODO: (done) If two rockets are fired quickly after the other one with change of target between, these rockets share the same target (thi first one is overriden)
        // TODO: (done) Implement rocket banking
        // TODO: (done) Use fixed DeltaTime
        // TODO: (done) Other rocket fucks previous rocket motion - Maybe it's because the same Key
        // TODO: (done) Use Float Tween - I won't implement this

        // Pull the rocket down using tween
        rocket.transform.position = shooter.transform.position;

        float shooterHalfHeight;
        if (shooter == Ufo.Instance.gameObject) // TODO: Temporary solution, fix it in RigidbodyAssistant
            shooterHalfHeight = shooter.transform.Find("UFO_body").GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * shooter.transform.lossyScale.y;
        else
            shooterHalfHeight = shooter.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * shooter.transform.lossyScale.y;

        rocket.transform.Translate(Vector3.down * shooterHalfHeight);

        var shooterPosition = shooter.transform.position;
        var rocketPosition = rocket.transform.position;
        var endPos = shooterPosition + (rocketPosition - shooterPosition).normalized * .8f; // Is not affected by rocketScript.halfHeight

        var startPos = rocketPosition;
        rocket.Tween($"tween_{rocket.name}", startPos, endPos, 1.2f, TweenScaleFunctions.QuadraticEaseOut, UpdatePos, MoveCompleted);

        void UpdatePos(ITween<Vector3> t)
        {
            rocket.transform.position = t.CurrentValue;

            // Tween rotation also
            var rotationZUpdate = Mathf.LerpAngle(_projectileRotationZ, 0, t.CurrentProgress);
            var rocketEulerAngles = rocket.transform.eulerAngles;
            rocketEulerAngles = new Vector3(rocketEulerAngles.x, rocketEulerAngles.y, rotationZUpdate);
            rocket.transform.eulerAngles = rocketEulerAngles;

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

            missileSupervisor.m_launchCustomDir = rocket.transform.forward;
            missileSupervisor.StartLaunchSequence();
        }
    }

    // TODO: Could also cause damage
    public static void ProcessBlast(Vector3 origin, int radius, int energy, GameObject excludeTarget, GameObject excludeInitiator = null)
    {
        var collidersCount = Physics.OverlapSphereNonAlloc(origin, radius, _blastSphereColliders);
        for (int i = 0; i < collidersCount; ++i)
        {
            var obj = _blastSphereColliders[i].gameObject;
            if (obj == excludeTarget || obj == excludeInitiator)
                continue;
            
            Rigidbody rb = obj.GetComponent<Rigidbody>();

            if (rb != null && !rb.isKinematic)
            {
                rb.AddExplosionForce(energy, origin, radius);
                // Debug.DrawRay(origin, obj.transform.position-origin, Color.magenta, 60);
            }
        }
    }
    
    // TODO: RigidBodies could be cached somehow (every object could have it as its variable)
    // TODO: Could also cause damage
    public static void ProcessBlast_old(Vector3 origin, int radius, int energy, Rigidbody excludeTargetRb, Rigidbody excludeInitiatorRb = null)
    {
        var rigidbodies = (Rigidbody[]) FindObjectsOfType(typeof(Rigidbody));

        var count = rigidbodies.Length;
        for (int i = 0; i < count; ++i)
        {
            var objRigidbody = rigidbodies[i];
            if (objRigidbody == excludeTargetRb || objRigidbody == excludeInitiatorRb || objRigidbody.isKinematic)
                continue;
            var obj = objRigidbody.gameObject;

            var forceDir = (obj.transform.position - origin).normalized;

            if (Physics.Raycast(origin, forceDir, out var hit, radius))
            {
                // if (!objRigidbody.useGravity) // Something disables it for fucking testing cubes
                //     objRigidbody.useGravity = true;
                objRigidbody.AddForce(energy * forceDir / hit.distance * hit.distance, ForceMode.Impulse);
                Debug.DrawRay(origin, forceDir * hit.distance, Color.magenta, 60);
                print(objRigidbody.gameObject.name);
            }
        }
    }

    /// <param name="shooter">Shooter object</param>
    /// <param name="missileSupervisor">Used to pair missileSupervisor and its target both ways</param> 
    private GameObject SetWeaponTarget(GameObject shooter, MissileSupervisor missileSupervisor)
    {
        var target = Instantiate(GameController.Instance.missileSupervisorTargetPrefab);
        target.GetComponent<MissileSupervisorTarget>().missileSupervisor = missileSupervisor;
        var shooterTransform = shooter.transform;

        if (shooter == GameController.ufo)
        {
            if (GameController.SelectedObject)
            {
                target.transform.SetParent(GameController.SelectedObject.transform);
                target.transform.localPosition = Vector3.zero;

                if (GameController.SelectedObjectMustCenterPivot)
                    target.transform.Translate(0, GameController.RigidbodyAssistantScript.verticalExtents, 0);
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
