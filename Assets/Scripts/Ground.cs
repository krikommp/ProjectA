using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Client
{
    public class Ground : MonoBehaviour
    {
        [FormerlySerializedAs("FallSpeed")] public float fallSpeed = 5f;
        [FormerlySerializedAs("GroundCheck")] public Transform groundCheck;
        [FormerlySerializedAs("GroundDistance")] public float groundDistance = 0.1f;
        [FormerlySerializedAs("GroundMask")] public LayerMask groundMask;
        private bool isGrounded = false;
        
        private void CheckGround()
        {
            if (Physics2D.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            if (!isGrounded)
            {
                transform.Translate(Vector3.down * (fallSpeed * Time.deltaTime));
            }
        }

        private void FixedUpdate()
        {
            CheckGround();
        }
    }
}