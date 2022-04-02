using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float degreesPerSec;

    private void Update() {
        transform.RotateAround(transform.position, transform.up,degreesPerSec * Time.deltaTime);
    }
}
