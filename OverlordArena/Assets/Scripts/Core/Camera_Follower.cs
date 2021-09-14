using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follower : MonoBehaviour
{

    [SerializeField] GameObject playerCamera, navTarget, cameraOrbitPoint;
    GameObject player;
    [SerializeField] float mouseModifier, cameraMoveLimit = 12f, cameraMoveMinimum = 8f, cameraSpeedModifier = 4f;

    Inputs inputs;
    private void Awake()
    {
        player = FindObjectOfType<Player_Controller>().gameObject;
        cameraOrbitPoint.transform.SetParent(null);
        inputs = FindObjectOfType<Inputs>();
        //Cursor.lockState = CursorLockMode.Confined; //Maybe move this to somewhere better
    }

    private void Update()
    {
        transform.position = player.transform.position;

        float mouseMovement = inputs.MouseLooking();
        if (mouseMovement != 0)
        {
            transform.RotateAround(transform.position,transform.up, mouseModifier * mouseMovement * Time.deltaTime);
            cameraOrbitPoint.transform.rotation = transform.rotation;
        }
        LerpCameraPosition();

    }

    private void LerpCameraPosition()
    {
        Vector3 lerpPoint = YInterceptOfCameraRay();
        lerpPoint = RestrictPosition(lerpPoint, cameraMoveLimit, cameraMoveMinimum); //------------------Magic number!!!
        lerpPoint = new Vector3(
            (lerpPoint.x + transform.position.x) / 2,
            (lerpPoint.y + transform.position.y) / 2,       //Halfway point between the restricted camera point and player position
            (lerpPoint.z + transform.position.z) / 2);

        cameraOrbitPoint.transform.position = Vector3.Lerp(cameraOrbitPoint.transform.position, lerpPoint, cameraSpeedModifier * Time.deltaTime);
        playerCamera.transform.LookAt(cameraOrbitPoint.transform.position);
    }

    private Vector3 YInterceptOfCameraRay() //Gets a the mouse position in worldspace at y = 0
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(inputs.GetMousePosition());
        Plane yInterceptPlane = new Plane(Vector3.up, Vector3.zero); //Plane to represent the y intercept

        float distance;
        if (yInterceptPlane.Raycast(cameraRay, out distance))
        {
            return cameraRay.GetPoint(distance);
        }
        return Vector3.zero;
    }

    private Vector3 RestrictPosition(Vector3 pos, float maxDistance, float minDistance)
    { //If didtance is not high enough dont move the camera
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