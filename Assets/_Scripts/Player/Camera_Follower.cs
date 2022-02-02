using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    Transform player, cameraOrbitPoint, playerCamera;
    [SerializeField] float mouseModifier, /*cameraMoveLimit = 12f, cameraMoveMinimum = 8f,*/ cameraSpeedModifier = 4f;

    Inputs inputs;
    public void Setup()
    {
        player = transform.parent;
        transform.parent = null;
        cameraOrbitPoint = transform.GetChild(0);
        playerCamera = cameraOrbitPoint.GetChild(0);
        cameraOrbitPoint.SetParent(null);

        inputs = FindObjectOfType<Inputs>();
        //Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (player == null)// Remove later
            return;
        transform.position = player.position; //Set position to player

        if (inputs.GetMouseMovement().x != 0 && inputs.GetShiftAlt() > 0) {
            transform.RotateAround(transform.position, transform.up, mouseModifier * inputs.GetMouseMovement().x * Time.deltaTime);
            cameraOrbitPoint.rotation = transform.rotation;
        }

        Ray cameraRay = playerCamera.GetComponent<Camera>().ScreenPointToRay(inputs.GetMousePosition());
        Plane yInterceptPlane = new Plane(Vector3.up, Vector3.zero); //Plane to represent the y intercept
        Vector3 lerpPoint = player.position;

        cameraOrbitPoint.position = Vector3.Lerp(cameraOrbitPoint.position, lerpPoint, cameraSpeedModifier * Time.deltaTime);
        playerCamera.LookAt(cameraOrbitPoint.position);
    }


    //Annoying chunk of pointlessness

    //if (yInterceptPlane.Raycast(cameraRay, out float distance)) {
    //    lerpPoint = cameraRay.GetPoint(distance);
    //}

    //lerpPoint = RestrictPosition(lerpPoint, cameraMoveLimit, cameraMoveMinimum);
    //lerpPoint = new Vector3(
    //    (lerpPoint.x + transform.position.x) / 2,
    //    (lerpPoint.y + transform.position.y) / 2,       //Halfway point between the restricted camera point and player position
    //    (lerpPoint.z + transform.position.z) / 2);

    ////Stops camera from moving when mouse is close to screen centre
    //private Vector3 RestrictPosition(Vector3 pos, float maxDistance, float minDistance)
    //{ //If distance is not high enough dont move the camera
    //    if (inputs.GetShiftAlt() > 0)
    //        return cameraOrbitPoint.position;

    //    if (StaticHelpers.Vector3Distance(transform.position, pos) < minDistance) 
    //    {
    //        return transform.position;
    //    }

    //    //X
    //    if (pos.x > transform.position.x + maxDistance)
    //        pos.x = transform.position.x + maxDistance;
    //    else if(pos.x < transform.position.x - maxDistance)
    //        pos.x = transform.position.x - maxDistance;

    //    //No need to do for y as the points are on y = 0

    //    //Z
    //    if (pos.z > transform.position.z + maxDistance)
    //        pos.z = transform.position.z + maxDistance;
    //    else if (pos.z < transform.position.z - maxDistance)
    //        pos.z = transform.position.z - maxDistance;

    //    return pos;
    //}
}