using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    [Tooltip("Zýplanmýþ sayýlmak için gereken maksimum eðim")]
    [Range(5f, 60f)]
    public float slopeLimit = 45f;
    [Tooltip("Hareket hýzý")]
    public float moveSpeed = 15f;
    [Tooltip("Dönme hýzý")]
    public float turnSpeed = 300;
    [Tooltip("Karakterin zýplama izni var mý")]
    public bool allowJump = true;
    [Tooltip("Zýplama hýzý")]
    public float jumpSpeed = 10f;

    public bool IsGrounded { get; private set; }
    public float ForwardInput { get; set; }
    public float TurnInput { get; set; }
    public bool JumpInput { get; set; }

    new private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        ProcessCheck();
    }

    //Karakter yerde mi kontrol ediliyor.
    private void GroundCheck()
    {
        IsGrounded = false;
        
        float capsuleHeight = Mathf.Max(capsuleCollider.radius * 2f, capsuleCollider.height);
        Vector3 capsuleBase = transform.TransformPoint(capsuleCollider.center - Vector3.up * capsuleHeight / 2f);
        
        float radius = transform.TransformVector(capsuleCollider.radius, 0f, 0f).magnitude;
        Ray ray = new Ray(capsuleBase + transform.up * .01f, -transform.up);
        RaycastHit rcHit;
        if (Physics.Raycast(ray, out rcHit, radius * 5f))
        {
            float normalAngle = Vector3.Angle(rcHit.normal, transform.up);
            if (normalAngle < slopeLimit)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                if (rcHit.distance < maxDist)
                    IsGrounded = true;
            }
        }
    }

    private void ProcessCheck()
    {
        if (TurnInput != 0f)
        {
            float angle = Mathf.Clamp(TurnInput, -1f, 1f) * turnSpeed;
            transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle);
        }
        
        if (IsGrounded)
        {
            rigidbody.velocity = Vector3.zero;
            if (JumpInput && allowJump)
            {
                rigidbody.velocity += Vector3.up * jumpSpeed;
            }

            rigidbody.velocity += transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed;
        }
        else
        {
            if (!Mathf.Approximately(ForwardInput, 0f))
            {
                Vector3 verticalVelocity = Vector3.Project(rigidbody.velocity, Vector3.up);
                rigidbody.velocity = verticalVelocity + transform.forward * Mathf.Clamp(ForwardInput, -1f, 1f) * moveSpeed / 2f;
            }
        }
    }
}
