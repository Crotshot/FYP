using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarProj : MonoBehaviour
{
    private Vector3 targetPos;
    [SerializeField] GameObject spawnOnCollision;

    private void FixedUpdate() {
        if (targetPos == null)
            return;

        //Follow bezier curve to target
    }


    public void SetTargetPos(Vector3 pos) {
        targetPos = pos;
    }
}
