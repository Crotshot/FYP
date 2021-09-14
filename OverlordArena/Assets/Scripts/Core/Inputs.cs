using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    /*
     * 
     * A class dedicated to getting player inputs
     * 
     */

    public float MouseLooking()
    {
        if (Input.GetAxisRaw("ShiftAlt") > 0)
        {
            return Input.GetAxis("Mouse X");
        }
        else
            return 0;
    }

    public Vector3 MovementInput() { return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); }
    public Vector3 GetMousePosition(){ return Input.mousePosition; }
    public float GetAttackInput() { return Input.GetAxisRaw("Attack"); }
    public float GetAbility1Input(){ return Input.GetAxisRaw("Ability1"); }
    public float GetAbility2Input(){ return Input.GetAxisRaw("Ability2"); }
    public float GetAbility3Input(){ return Input.GetAxisRaw("Ability3"); }
}