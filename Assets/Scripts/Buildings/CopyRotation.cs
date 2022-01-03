using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Script used to copy the rotation of an axis of one object and then apply it with a ratio to another object*/
public class CopyRotation : MonoBehaviour
{
    [SerializeField] bool copyX, copyY, copyZ, applyX, applyY,applyZ;
    [SerializeField] float ratio;
    [SerializeField] Transform objectCopied;
    Vector3 rot = Vector3.zero;

    private void FixedUpdate() {
        Vector3 eulers = objectCopied.rotation.eulerAngles;//, angs = transform.eulerAngles;
        if (eulers != rot) {
            float dif = 0;
            if (copyX) {
                if (applyX) { //Rotate around right
                    dif = eulers.x - rot.x;
                    rot.x = eulers.x;
                    //transform.eulerAngles = new Vector3(angs.x + dif * ratio, angs.y, angs.);
                    transform.RotateAround(transform.position, transform.right, dif * ratio);
                }
                else if (applyY) { //Rotate around up
                    dif = eulers.x - rot.y;
                    rot.y = eulers.x;
                    transform.RotateAround(transform.position, transform.up, dif * ratio);
                }
                else if (applyZ) { //Rotate around forward
                    dif = eulers.x - rot.z;
                    rot.z = eulers.x;
                    transform.RotateAround(transform.position, transform.forward, dif * ratio);
                }
            }
            else if(copyY) {
                if (applyX) {
                    dif = eulers.y - rot.x;
                    rot.x = eulers.y;
                    transform.RotateAround(transform.position, transform.right, dif * ratio);
                }
                else if (applyY) {
                    dif = eulers.y - rot.y;
                    rot.y = eulers.y;
                    transform.RotateAround(transform.position, transform.up, dif * ratio);
                }
                else if (applyZ) {
                    dif = eulers.y - rot.z;
                    rot.z = eulers.y;
                    transform.RotateAround(transform.position, transform.forward, dif * ratio);
                }
            }
            else if (copyZ) {
                if (applyX) {
                    dif = eulers.z - rot.x;
                    rot.x = eulers.z;
                    transform.RotateAround(transform.position, transform.right, dif * ratio);
                }
                else if (applyY) {
                    dif = eulers.z - rot.y;
                    rot.y = eulers.z;
                    transform.RotateAround(transform.position, transform.up, dif * ratio);
                }
                else if (applyZ) {
                    dif = eulers.z - rot.z;
                    rot.z = eulers.z;
                    transform.RotateAround(transform.position, transform.forward, dif * ratio);
                }
            }
        }
    }
}
