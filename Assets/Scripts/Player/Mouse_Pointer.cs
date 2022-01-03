using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticHelpers;
using Mirror;

public class Mouse_Pointer : NetworkBehaviour {
    [SerializeField] MouseFollower[] pointers;
    [SerializeField] Camera playerCam;
    Inputs inputs;
    Vector3 worldFocal = Vector3.zero;

    private void Start() {
        if (!isLocalPlayer && !GetComponent<PlayerController>().getOfflineTest())
            Destroy(this);
        inputs = FindObjectOfType<Inputs>();
    }

    private void Update() {
        Ray cameraRay = playerCam.ScreenPointToRay(inputs.GetMousePosition());
        if (Physics.Raycast(cameraRay, out RaycastHit hit)) {
            worldFocal = hit.point;                                            //Mouse position in World Space
        }

        for (int i = 0; i < pointers.Length; i++) {
            Vector3 swivel = pointers[i].swivel.localPosition,                 //Swivel position in Local Space
                    obj = pointers[i].pointingObject.localPosition,            //Pointing Object in Local Space
                    focalLocal = transform.InverseTransformPoint(worldFocal);  //Mouse position in Local Space
            float opp, adj, rot;
            Quaternion targetRot;

            //Swivel===========================================================================
            opp = FloatDistance(swivel.x, focalLocal.x);
            adj = FloatDistance(swivel.z, focalLocal.z);
            rot = Mathf.Rad2Deg * Mathf.Atan(opp / adj);// Angle between 0 and 90
            if (i == 2) {
                Debug.DrawRay(pointers[i].swivel.position, pointers[i].swivel.forward, Color.green, 0.1f);
                Debug.DrawRay(pointers[i].pointingObject.position, pointers[i].pointingObject.forward * 20f, Color.red, 0.1f);
            }
            if (focalLocal.z < swivel.z) {
                // Angle between 90 and 180
                    rot = 180 - rot;
            }

            if (focalLocal.x < swivel.x) { //Make negative if on local -x(left side)
                    rot *= -1;
            }

            rot = Mathf.Clamp(rot, pointers[i].swivelLimitMin, pointers[i].swivelLimitMax);
            targetRot = Quaternion.Euler(0f, rot, 0f);
            pointers[i].swivel.localRotation = Quaternion.Slerp(pointers[i].swivel.localRotation, targetRot, 0.1f);

            //Object===========================================================================
            opp = FloatDistance(pointers[i].pointingObject.position.y, worldFocal.y);
            adj = Vector2DistanceXZ(swivel, focalLocal);
            rot = Mathf.Rad2Deg * Mathf.Atan(opp / adj);//Angle between 0 and 90

            if (pointers[i].pointingObject.position.y < worldFocal.y) { //Make negative if on local -x(left side)
                rot *= -1;
            }
            /* Doesnt work, breaks rotation
            if (focalLocal.z <= obj.z) {// Angle between 90 and 180
                rot = 180 - rot;
            }
            */
            rot = Mathf.Clamp(rot, pointers[i].objectLimitMin, pointers[i].objectLimitMax);
            targetRot = Quaternion.Euler(rot, pointers[i].pointingObject.localRotation.eulerAngles.y, pointers[i].pointingObject.localRotation.eulerAngles.z);
            pointers[i].pointingObject.localRotation = Quaternion.Slerp(pointers[i].pointingObject.localRotation, targetRot, 0.1f);
        }
    }
}

[System.Serializable]
internal class MouseFollower {
    [SerializeField] public Transform swivel, pointingObject;
    //Swivel controls y value and obj controls z value
    [SerializeField] public float swivelLimitMin, swivelLimitMax, objectLimitMin, objectLimitMax;
}


/*
for(int i =0; i < pointers.Length; i++) {
    Vector3 swivel = pointers[i].swivel.localPosition,
        obj = pointers[i].pointingObject.position,
        focalLocal = pointers[i].swivel.transform.InverseTransformPoint(focalPoint);
    //
    float opp, adj, rot;
    Quaternion targetRot;
    //Swivel===========================================================================

    if (focalLocal.z > swivel.z) {
        opp = FloatDistance(swivel.x, focalLocal.x);
        adj = FloatDistance(swivel.z, focalLocal.z);
        rot = Mathf.Rad2Deg * Mathf.Atan(opp / adj);
    }
    else {
        opp = FloatDistance(swivel.z, focalLocal.z);
        adj = FloatDistance(swivel.x, focalLocal.x);
        rot = 90 + (Mathf.Rad2Deg * Mathf.Atan(opp / adj));
    }

    if (focalLocal.x < swivel.x) {
        rot *= -1f;
    }

    rot = Mathf.Clamp(rot, pointers[i].swivelLimitMin, pointers[i].swivelLimitMax);
    targetRot = Quaternion.Euler(0f, rot, 0f);
    pointers[i].swivel.localRotation = Quaternion.Slerp(pointers[i].swivel.localRotation, targetRot, 0.1f);

    //Object===========================================================================
    opp = FloatDistance(obj.y, focalPoint.y);
    adj = Vector2DistanceXZ(swivel, focalLocal);
    rot = Mathf.Rad2Deg * Mathf.Atan(opp / adj);
    rot = 90 - rot;

    if (obj.y < focalPoint.y)
        rot *= -1;

    rot = Mathf.Clamp(rot, pointers[i].objectLimitMin, pointers[i].objectLimitMax);
    targetRot = Quaternion.Euler(rot, pointers[i].pointingObject.localRotation.eulerAngles.y, pointers[i].pointingObject.localRotation.eulerAngles.z);
    pointers[i].pointingObject.localRotation = Quaternion.Slerp(pointers[i].pointingObject.localRotation, targetRot, 0.1f);

}
*/



//Pythagoras for calculating the angles that the followers will look at
//done twice using a right angle triangle, once for swivel around the y (azimath) and then around the z (elevation) of the object

//For calculating these directions we use a 
/*
 For elevation
 Get height difference, "dif", y of mouse is above object the end target rotation is made negative
 Get displacement "dist" x,z distance

 Tan (x)* = dif / dis
 x = tan^-1
 90 - x = Theta => Made negative if mouse y is above the obj y


Similar again to the swivel variant except sideways
*/
//If x of mouse is less than swivel is negative rot *= -1
//if z of mouse is less than swivel + 90 degrees unless x of mouse is less than swivel then -90 degrees