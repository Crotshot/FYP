using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationMovement
{
    public string animName;
    public bool lockMoveRot; //Lock player so they cannot turn
    public float[] agentSpeed, //Alter move speed at times by index eg -> 3.5,   3,   2.2,  4.1, 
        percentChange; //Values of percentChange would be for example  -> 0%, 0.1%, 0.37%, 0.78%
    public Vector3[] direction; //Alter the direction at different times
    //If speed and Vector3.y = -1 skip for this time
}