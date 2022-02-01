using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactor : NetworkBehaviour
{
    Camera cam;
    Inputs inputs;
    [SyncVar] [SerializeField] ulong objectID = 0;
    ulong lastObjectID = 0;
    GameObject arrow;
    Interactable focus;
    [SyncVar] Vector3 mouseWorldPos;

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

        if(objectID != lastObjectID) {
            if (objectID != 0) {
                foreach (Interactable inter in FindObjectsOfType<Interactable>()) {
                    if (inter.GetID() == objectID) {
                        if (inter.TryGetComponent(out Team t)) {
                            if(t.GetTeam() != GetComponent<Team>().GetTeam()) {
                                arrow.transform.position = Vector3.down * 50f; //Dont show arrow above enemy minion Summoners
                                focus = null;
                            }
                            else {
                                arrow.transform.position = inter.transform.position;
                                focus = inter;
                            }
                        }
                        else {
                            arrow.transform.position = inter.transform.position;
                            focus = inter;
                        }
                    }
                }
            }
            else {
                arrow.transform.position = Vector3.down * 50f;
                focus = null;
            }
            lastObjectID = objectID;
        }
    }

    private void CameraRay(Vector3 origin, Vector3 direction) {
        if (isServer) {
            Ray cameraRay = new Ray(origin, direction);
            Debug.DrawRay(origin, direction * 99f, Color.cyan);
            int layer = 1 << LayerMask.NameToLayer("Default");
            if (Physics.Raycast(cameraRay, out RaycastHit hit, 999f, layer, QueryTriggerInteraction.Ignore)) {
                if (hit.collider.TryGetComponent(out Interactable inter)) {
                    objectID = inter.GetID();
                } else {
                    objectID = 0;
                    focus = null;
                }
                mouseWorldPos = hit.point;
            }
            else {
                objectID = 0;
                focus = null;
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

    public Interactable GetFocus() {
        return focus;
    }

    public Vector3 GetMouseWorldPos() {
        return mouseWorldPos;
    }
}