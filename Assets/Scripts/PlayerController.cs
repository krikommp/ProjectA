using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Client
{
    public class PlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("Animator")] public Animator animator;

        [FormerlySerializedAs("SpriteRenderer")]
        public SpriteRenderer spriteRenderer;

        [FormerlySerializedAs("MoveSpeed")] public float moveSpeed;
        [FormerlySerializedAs("RollDistance")] public float rollDistance;
        [FormerlySerializedAs("RollDuration")] public float rollDuration = 0.5f;

        [FormerlySerializedAs("FallSpeed")] public float fallSpeed = 5f;

        private bool isGrounded = false;
        public Transform groundCheck;
        public float groundDistance = 0.1f;
        public LayerMask groundMask;

        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Roll1 = Animator.StringToHash("Roll");

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _rollStartTime;
        private bool _isRolling = false; // 是否正在Roll

        private Direction _direction = Direction.Right;
        private static readonly int Attack1 = Animator.StringToHash("Attack1");

        private void FixedUpdate()
        {
            CheckGround();
            if (!_isRolling)
            {
                float horizontal = Input.GetAxis("Horizontal");
                Move(horizontal);
                StartRoll();
            }
            else
            {
                EndRoll();
            }
        }

        private void Update()
        {
            PunchCombo();
        }

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

        private void Move(float move)
        {
            animator.SetFloat(Horizontal, Mathf.Abs(move));
            transform.position += new Vector3(move * moveSpeed * Time.deltaTime, 0, 0);
            if (move != 0.0f)
            {
                _direction = (move > 0) ? Direction.Right : Direction.Left;
                transform.rotation = Quaternion.Euler(0, _direction == Direction.Right ? 0 : 180, 0);
            }
        }

        private void StartRoll()
        {
            if (!Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }

            _isRolling = true;
            _startPosition = transform.position;
            _targetPosition = _startPosition + transform.right * rollDistance;
            _rollStartTime = Time.time;

            animator.SetTrigger(Roll1);
        }

        private void EndRoll()
        {
            if (_isRolling)
            {
                float t = (Time.time - _rollStartTime) / rollDuration;
                transform.position = Vector3.Lerp(_startPosition, _targetPosition, t);

                if (t >= 1.0f)
                {
                    _isRolling = false;
                }
            }
        }

        private void PunchCombo()
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger(Attack1);
            }
        }
    }
}