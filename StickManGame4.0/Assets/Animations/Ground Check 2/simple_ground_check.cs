using UnityEngine;
using System;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Detection")]
    public LayerMask groundLayerMask = 1;
    public float checkDistance = 0.1f;
    
    [Header("Debug")]
    public bool showDebug = true;
    
    private bool isGrounded = false;
    private bool wasGrounded = false;
    
    // Events that FirstPersonAudio expects
    public event Action Grounded;
    public event Action Jumped;
    
    // Public property to check if grounded
    public bool IsGrounded => isGrounded;
    
    void Update()
    {
        CheckGroundStatus();
        HandleGroundEvents();
    }
    
    void CheckGroundStatus()
    {
        // Cast a ray downward to check for ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, checkDistance, groundLayerMask);
    }
    
    void HandleGroundEvents()
    {
        // If we just landed (wasn't grounded before, but grounded now)
        if (isGrounded && !wasGrounded)
        {
            Grounded?.Invoke(); // This is what FirstPersonAudio is listening for
        }
        
        // If we just left the ground (was grounded before, but not now)
        if (!isGrounded && wasGrounded)
        {
            Jumped?.Invoke(); // This is what FirstPersonAudio is listening for
        }
        
        wasGrounded = isGrounded;
    }
    
    void OnDrawGizmos()
    {
        if (!showDebug) return;
        
        // Draw the ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * checkDistance;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.05f);
    }
}