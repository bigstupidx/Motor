using UnityEngine;
using System.Collections;

public class AlwaysForward : MonoBehaviour
{
    public float Speed;
    public float yRotation;

    void FixedUpdate()
    {
        transform.position = transform.position + transform.forward * Speed;
        transform.Rotate(Vector3.up, yRotation);
    }
}
