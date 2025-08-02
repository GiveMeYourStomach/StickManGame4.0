using UnityEngine;
using System;

public class CustomGroundSensor : MonoBehaviour
{
    [Header("Ground Detection Settings")]
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayerMask = 1; // Default layer
    [SerializeField] private bool showDebugGizmos = true;

    [Header("Ground Check Results")]
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private float distanceToGround = 0f;

    // Public properties
    public bool IsGrounded => isGrounded;
    public float DistanceToGround => distanceToGround;

    // Events
    public Action OnGrounded;
    public Action OnLeftGround;

    private bool wasGrounded = false;
    private RaycastHit groundHit;

    void Update()
    {
        CheckGround();
        HandleGroundEvents();
    }

    private void CheckGround()
    {
        // Perform sphere cast downward
        isGrounded = Physics.CheckSphere(transform.position, checkRadius, groundLayerMask);

        // Get distance to ground for more detailed info
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, checkRadius * 2f, groundLayerMask))
        {
            distanceToGround = groundHit.distance;
        }
        else
        {
            distanceToGround = float.MaxValue;
        }
    }

    private void HandleGroundEvents()
    {
        // Just landed
        if (isGrounded && !wasGrounded)
        {
            OnGrounded?.Invoke();
        }
        // Just left ground
        else if (!isGrounded && wasGrounded)
        {
            OnLeftGround?.Invoke();
        }

        wasGrounded = isGrounded;
    }

    // Alternative method using OverlapSphere for more control
    public bool CheckGroundOverlap()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, groundLayerMask);
        return colliders.Length > 0;
    }

    // Method to check if standing on specific tag
    public bool IsStandingOnTag(string tag)
    {
        if (isGrounded && groundHit.collider != null)
        {
            return groundHit.collider.CompareTag(tag);
        }
        return false;
    }

    // Method to get the ground normal (useful for slopes)
    public Vector3 GetGroundNormal()
    {
        if (isGrounded && groundHit.collider != null)
        {
            return groundHit.normal;
        }
        return Vector3.up;
    }

    // Method to get what we're standing on
    public GameObject GetGroundObject()
    {
        if (isGrounded && groundHit.collider != null)
        {
            return groundHit.collider.gameObject;
        }
        return null;
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Draw the check sphere
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        // Draw a line to show the raycast
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (checkRadius * 2f));

        // Draw ground hit point if we have one
        if (groundHit.collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundHit.point, 0.1f);

            // Draw ground normal
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(groundHit.point, groundHit.point + groundHit.normal * 0.5f);
        }
    }
}