using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Client
{
    public class PlayerController : MonoBehaviour
    {
        private AnimatorStateInfo _animatorStateInfo;
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

        private int attackCount = 0;
        private static readonly int AttackCount = Animator.StringToHash("AttackCount");

        private void Start()
        {
            _animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

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
            _animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
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
            if ((_animatorStateInfo.IsName("Punch01") || _animatorStateInfo.IsName("Punch02") ||
                 _animatorStateInfo.IsName("Punch03")) && _animatorStateInfo.normalizedTime > 1.0f)
            {
                attackCount = 0; //将hitCount重置为0，即Idle状态
                animator.SetInteger(AttackCount, attackCount);
            }
            
            Debug.Log($"Is Punch01: {_animatorStateInfo.IsName("Punch01")}");
            Debug.Log($"Is Punch02: {_animatorStateInfo.IsName("Punch02")}");
            Debug.Log($"Is Punch03: {_animatorStateInfo.IsName("Punch03")}");

            if (Input.GetMouseButtonDown(0))
            {
                //若处于Idle状态，则直接打断并过渡到attack_a(攻击阶段一)
                if (_animatorStateInfo.IsName("Idle") && attackCount == 0)
                {
                    attackCount = 1;
                    animator.SetInteger(AttackCount, attackCount);
                }
                //如果当前动画处于attack_a(攻击阶段一)并且该动画播放进度小于80%，此时按下攻击键可过渡到攻击阶段二
                else if (_animatorStateInfo.IsName("Punch01") && attackCount == 1 && _animatorStateInfo.normalizedTime < 0.8f)
                {
                    attackCount = 2;
                }
                //同上
                else if (_animatorStateInfo.IsName("Punch02") && attackCount == 2 && _animatorStateInfo.normalizedTime < 0.8f)
                {
                    Debug.Log("Change");
                    attackCount = 3;
                    //animator.SetInteger(AttackCount, attackCount);
                }
            }
        }
        
        void GoToNextAttackAction()
        {
            Debug.Log($"Attack Count: {attackCount}");
            animator.SetInteger(AttackCount, attackCount);
        }
    }
}