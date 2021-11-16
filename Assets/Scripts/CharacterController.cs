using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotateSpeed = 300;
    new private Rigidbody rigidbody;

    public float TurnInput { get; set; }
    public float ForwardInput { get; set; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if(TurnInput != 0f)
        {
            float angle = Mathf.Clamp(TurnInput, -1f, 1f) * rotateSpeed;
            transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle);
        }

        Vector3 move = transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed * Time.fixedDeltaTime;
        rigidbody.MovePosition(transform.position + move);
    }
}
