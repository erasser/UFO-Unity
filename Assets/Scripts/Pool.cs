using System.Collections.Generic;
using SparseDesign.ControlledFlight;
using UnityEngine;
/** Active objects are in use, inactive ones are waiting in the pool
 *  To add object to pool: GetNewInstance()
 *  To remove from pool: GameObject.SetActive(false)
 */

// TODO: Instancovat i missile targety :D
// TODO: Udělat dva pooly: pro active a pro inactive? Aby se nemuselo iterovat. Vyplatilo by se to výkonově?
// TODO: Dalo by se optimalizovat nějakym activeIndexem? Musel by se nastavovat při aktivaci i inaktivaci

public class Pool : MonoBehaviour
{
    public static List<GameObject> ProjectilesRocketsPool = new List<GameObject>();
    
    // for debug:
    // public int inPool = 0;
    // public int activeInPool = 0;
    
    void Update()
    {
        // inPool = ProjectilesRocketsPool.Count;
        // activeInPool = GetActiveCount(ProjectilesRocketsPool);
    }

    // TODO: Vyřešit jinak než větvením uvnitř fce?
    /** Pick existing instance from pool or create new instance, if no object is available */
    /// <summary>
    ///     Returns object from the pool if available, else returns new object instance. Returns active object.
    /// </summary>
    /// <param name="type">Type of projectile ("rocket")</param>
    /// <param name="shooterId">Unique instance ID</param>
    public static GameObject GetNewInstance(string type, int shooterId)
    {
        GameObject rocket = null;

        if (type == "rocket")
        {
            rocket = GetFirstInactive(ProjectilesRocketsPool);

            if (rocket)
            {
                rocket.GetComponent<Projectile>().ResetPooledObject(shooterId);  // Also sets object active
            }
            else
            {
                rocket = Instantiate(WeaponController.Instance.rocketPrefab);
                ProjectilesRocketsPool.Add(rocket);
            }
        }

        return rocket;
    }

    private static GameObject GetFirstInactive(List<GameObject> pool)
    {
        GameObject poolObject = null;
        foreach (GameObject obj in pool)
        {
            if (!obj.activeSelf)
            {
                poolObject = obj;
                break;
            }
        }

        return poolObject;
    }
    
    /*public static int GetActiveCount(List<GameObject> pool)
    {
        var count = 0;
        foreach (GameObject obj in pool)
        {
            if (obj.activeSelf)
                ++count;
        }

        return count;
    }*/
}
