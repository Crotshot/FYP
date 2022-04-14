using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Gate : NetworkBehaviour
{
    [SerializeField] ControlPoint[] points;
    [SerializeField] float openDelay, openTime, openDegrees, inverseFPS;
    [SerializeField] string gateName;

    public void Setup() {
        if (!isServer)
            return;

        if(points.Length > 0)
            foreach (ControlPoint point in points) {
                point.captured.AddListener(OpenCheck);
            }
        else
            RpcOpenGate();
    }

    [Server]
    private void OpenCheck() {
        if (points.Length > 0) {
            int team = points[0].GetOwningTeam();
            if (team == 0)
                return;
            foreach (ControlPoint point in points) {
                if (point.GetOwningTeam() != team)
                    return;
            }

            foreach (ControlPoint point in points) {
                point.captured.RemoveListener(OpenCheck);
            }
        }
        RpcOpenGate();//Called on both host and client
    }

    [ClientRpc]
    private void RpcOpenGate() {
        StartCoroutine("OpenGate");
    }

    IEnumerator OpenGate() {
        while (openDelay > 0) {
            openDelay -= inverseFPS;
            yield return new WaitForSeconds(inverseFPS);
        }
        while (openTime > openDelay) {
            openDelay += inverseFPS;
            transform.GetChild(0).localEulerAngles = new Vector3(0, openDegrees * (openDelay / openTime),0);
            transform.GetChild(1).localEulerAngles = new Vector3(0, 180.0f - openDegrees * (openDelay / openTime), 0);
            yield return new WaitForSeconds(inverseFPS);
        }
        if(gateName.Length > 0)
            FindObjectOfType<MinionManager>().OpenGate(gateName);

        //transform.GetChild(0).parent = null;
        //transform.GetChild(1).parent = null;
        //Destroy(GetComponent<NetworkIdentity>(), 1f);
        //Destroy(this);
    }
}