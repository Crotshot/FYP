using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    Transform player, cameraOrbitPoint, playerCamera;
    [SerializeField] float mouseModifier, cameraMoveLimit = 12f, cameraMoveMinimum = 8f, cameraSpeedModifier = 4f;

    Inputs inputs;
    private void Start()
    {
        player = transform.parent;
        transform.parent = null;
        cameraOrbitPoint = transform.GetChild(0);
        playerCamera = cameraOrbitPoint.GetChild(0);
        cameraOrbitPoint.SetParent(null);

        inputs = FindObjectOfType<Inputs>();
        //->Cursor.lockState = CursorLockMode.Confined; //Maybe move this to somewhere better
    }

    private void Update()
    {
        transform.position = player.position; //Set position to player
        CameraRotation();
        LerpCameraPosition();
    }

    //Rotates camera whe
    private void CameraRotation()
    {
        if (inputs.GetMouseMovement().x != 0 && inputs.GetShiftAlt() > 0)
        {
            transform.RotateAround(transform.position, transform.up, mouseModifier * inputs.GetMouseMovement().x * Time.deltaTime);
            cameraOrbitPoint.rotation = transform.rotation;
        }
    }

    //Smoothly moves the point that the camera looks at
    private void LerpCameraPosition()
    {
        Vector3 lerpPoint = YInterceptOfCameraRay();
        lerpPoint = RestrictPosition(lerpPoint, cameraMoveLimit, cameraMoveMinimum);
        lerpPoint = new Vector3(
            (lerpPoint.x + transform.position.x) / 2,
            (lerpPoint.y + transform.position.y) / 2,       //Halfway point between the restricted camera point and player position
            (lerpPoint.z + transform.position.z) / 2);

        cameraOrbitPoint.position = Vector3.Lerp(cameraOrbitPoint.position, lerpPoint, cameraSpeedModifier * Time.deltaTime);
        playerCamera.LookAt(cameraOrbitPoint.position);
    }
    
    //Returns the position of the mouse in world space at y = 0
    private Vector3 YInterceptOfCameraRay() //Gets a the mouse position in worldspace at y = 0
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(inputs.GetMousePosition());
        Plane yInterceptPlane = new Plane(Vector3.up, Vector3.zero); //Plane to represent the y intercept

        if (yInterceptPlane.Raycast(cameraRay, out float distance))
        {
            return cameraRay.GetPoint(distance);
        }
        return Vector3.zero;
    }

    //Stops camera from moving when mouse is close to screen centre
    private Vector3 RestrictPosition(Vector3 pos, float maxDistance, float minDistance)
    { //If distance is not high enough dont move the camera
        if(StaticHelpers.Vector3Distance(transform.position, pos) < minDistance) 
        {
            return transform.position;
        }
        
        //X
        if (pos.x > transform.position.x + maxDistance)
            pos.x = transform.position.x + maxDistance;
        else if(pos.x < transform.position.x - maxDistance)
            pos.x = transform.position.x - maxDistance;

        //No need to do for y as the points are on y = 0

        //Z
        if (pos.z > transform.position.z + maxDistance)
            pos.z = transform.position.z + maxDistance;
        else if (pos.z < transform.position.z - maxDistance)
            pos.z = transform.position.z - maxDistance;

        return pos;
    }
}