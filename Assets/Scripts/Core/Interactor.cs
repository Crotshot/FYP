using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactor : NetworkBehaviour
{
    Camera cam;
    Inputs inputs;
    [SerializeField] ulong objectID;
    GameObject arrow;

    bool setUp;

    public void Setup() {
        if (!hasAuthority)
            return;
        Invoke(nameof(InvokedSetUp), 3f);
    }


    public void InvokedSetUp() {
        inputs = FindObjectOfType<Inputs>();
        cam = FindObjectOfType<Camera>();
        setUp = true;

        arrow = GameObject.FindGameObjectWithTag("arrow");
        if (GetComponent<Team>().GetTeam() == 2) {
            arrow.GetComponent<Renderer>().material = FindObjectOfType<Colors>().GetTeamMaterial(2);
            var pS = arrow.GetComponentInChildren<ParticleSystem>().main;
            pS.startColor = FindObjectOfType<Colors>().GetTeamColor(2);
        }
    }

    private void Update() {
        if (!setUp)
            return;
        inputs.GetMousePosition();
        Ray cameraRay = cam.ScreenPointToRay(inputs.GetMousePosition());
        CameraRay(cameraRay.origin, cameraRay.direction);
    }

    private void CameraRay(Vector3 origin, Vector3 direction) {
        if (isServer) {
            Ray cameraRay = new Ray(origin, direction);
            Debug.DrawRay(origin, direction * 99f, Color.cyan);
            int layer = 1 << LayerMask.NameToLayer("Default");
            if (Physics.Raycast(cameraRay, out RaycastHit hit, 999f, layer, QueryTriggerInteraction.Ignore)) {
                if(hit.collider.TryGetComponent(out Interactable inter)) {
                    InformID(inter.GetID());
                }
            }
            else {
                InformID(0);
            }
        }
        else {
            CmdServerRay(origin, direction);
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdServerRay(Vector3 origin, Vector3 direction) {
        CameraRay(origin, direction);
    }

    [ClientRpc]
    public void InformID(ulong ID) {
        objectID = ID;
    }
}