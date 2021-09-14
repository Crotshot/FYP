using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationMovement
{
    public string animationName;
    public bool lockMoveRot; //Lock player so they cannot turn
    public float[] agentSpeed, percentChange;
    public Vector3[] direction; //Alter move speed and the direction at different times
    //If speed and Vector3.y = -1 skip for this time
}