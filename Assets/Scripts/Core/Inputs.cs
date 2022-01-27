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

    public Vector3 GetMovementInput() { return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); }
    public Vector3 GetMousePosition() { return Input.mousePosition; }
    public Vector2 GetMouseMovement() { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }

    public float GetShiftAlt() { return Input.GetAxisRaw("ShiftAlt"); }
    public float GetCtrlTab() { return Input.GetAxisRaw("CtrlTab"); }

    public float GetAttackInput() { return Input.GetAxisRaw("Attack"); }
    public float GetAbility1Input() { return Input.GetAxisRaw("Ability1"); }
    public float GetAbility2Input() { return Input.GetAxisRaw("Ability2"); }
    public float GetAbility3Input() { return Input.GetAxisRaw("Ability3"); }

    public float GetCommandAttackInput() { return Input.GetAxisRaw("Command_Attack"); }
    public float GetCommandDefendInput() { return Input.GetAxisRaw("Command_Defend"); }
    public float GetCommandRecallInput() { return Input.GetAxisRaw("Command_Recall"); }

    public float GetScrollWheelInput() { return Input.GetAxisRaw("Mouse ScrollWheel"); }

    public float GetAnyInput(string name) { return Input.GetAxisRaw(name); }
}