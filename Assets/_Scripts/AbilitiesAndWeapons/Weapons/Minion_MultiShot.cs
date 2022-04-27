using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class Minion_MultiShot : ArchingProjectile
{
    [SerializeField] GameObject actualProjectile;
    [SerializeField] int count;
    [SerializeField] float inaccuracy;
    Vector3 tPos;

<<<<<<< Updated upstream
    override public void Setup(Vector3 targetedPosition) {
        if (!isServer)
            Destroy(this);
        tPos = targetedPosition;
    }

    private void FixedUpdate() {
        if (tPos == null)
            return;

        if (count > 0) {
            count--;
        }
        else {
            NetworkServer.Destroy(gameObject);
        }
        if (NetworkServer.active) {
            GameObject obj = Instantiate(actualProjectile, transform.position, transform.rotation);
            NetworkServer.Spawn(obj);
            obj.transform.position = transform.position;
            obj.transform.position = transform.position;
            obj.transform.parent = null;
            obj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
            Color c = GetComponent<Team>().GetTeamColor();
            obj.GetComponent<Team>().SetTeamColor(c.r, c.g, c.b, c.a);
            obj.GetComponent<ArchingProjectile>().Setup(tPos + Helpers.RandomVector3XZ(inaccuracy));
        }
    }
}
=======

}

//override public void Setup(Vector3 targetedPosition) {
//    if (!isServer)
//        Destroy(this);
//    tPos = targetedPosition;
//}

//private void FixedUpdate() {
//    if (tPos == null)
//        return;

//    if (count > 0) {
//        count--;
//    }
//    else {
//        NetworkServer.Destroy(gameObject);
//    }
//    if (NetworkServer.active) {
//        GameObject obj = Instantiate(actualProjectile, transform.position, transform.rotation);
//        NetworkServer.Spawn(obj);
//        obj.transform.position = transform.position;
//        obj.transform.position = transform.position;
//        obj.transform.parent = null;
//        obj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
//        Color c = GetComponent<Team>().GetTeamColor();
//        obj.GetComponent<Team>().SetTeamColor(c.r, c.g, c.b, c.a);
//        obj.GetComponent<ArchingProjectile>().Setup(tPos + Helpers.RandomVector3XZ(inaccuracy));
//    }
//}
>>>>>>> Stashed changes
