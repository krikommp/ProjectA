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
        [FormerlySerializedAs("Rigidbody2D")] public Rigidbody2D rigidbody2D;

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

        [FormerlySerializedAs("AttackMoveSpeed")]
        public float attackMoveSpeed = 1;

        private float currentAttackMoveSpeed = 0;

        private bool bWaitAttack = false;
        private bool bAttack = false;
        private int attackCount = 0;
        private static readonly int AttackCount = Animator.StringToHash("AttackCount");

        [FormerlySerializedAs("JumpForce")] public float jumpForce;
        private float lastYPos = float.MaxValue;
        private float currentYPos;
        private float verticalSpeed;
        private bool bJumping = false;
        private bool bJumpPressed = false;
        private int jumpCount = 2;
        private static readonly int VerticalFlag = Animator.StringToHash("VerticalFlag");
        private static readonly int knockback = Animator.StringToHash("Knockback");

        private bool bKnockback = false;
        private static readonly int Jump1 = Animator.StringToHash("Jump");
        private static readonly int DoubleJump = Animator.StringToHash("DoubleJump");

        private void Start()
        {
            Event.OnReceiverDamage += OnReceiveDamage;
            Event.OnAttackAnimationEnded += OnFinish;
            Event.OnReceiverHarm += OnReceiveHarm;
        }

        private void FixedUpdate()
        {
            CheckVertical();
            if (!_isRolling && !bKnockback)
            {
                float horizontal = Input.GetAxisRaw("Horizontal");

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    currentAttackMoveSpeed = attackMoveSpeed;
                }

                if (bAttack)
                {
                    Move(horizontal, currentAttackMoveSpeed);
                }
                else
                {
                    Jump();
                    Move(horizontal, moveSpeed);
                }
            }
            else
            {
                Move(0, 0);
            }

            EndRoll();
        }

        private void Update()
        {
            _animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            CheckGround();
            PunchCombo();
            StartRoll();
            ListenJump();
        }

        private void CheckGround()
        {
            if (Physics2D.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask))
            {
                isGrounded = true;
                verticalSpeed = 0;
                bJumping = false;
                animator.SetInteger(VerticalFlag, 0);
            }
            else
            {
                isGrounded = false;
            }
            
            if (!isGrounded)
            {
                if (float.IsNegative(verticalSpeed))
                {
                    animator.SetInteger(VerticalFlag, -1);
                }
                else if (verticalSpeed > float.Epsilon)
                {
                    animator.SetInteger(VerticalFlag, 1);
                }
            }
        }

        private void ListenJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
            {
                bJumpPressed = true;
            }
        }

        private void Jump()
        {
            if (isGrounded)
            {
                jumpCount = 2;
                bJumping = false;
            }

            if (bJumpPressed && isGrounded)
            {
                animator.SetTrigger(Jump1);
                bJumping = true;
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
                bJumpPressed = false;
                --jumpCount;
            }else if (bJumpPressed && jumpCount > 0 && bJumping)
            {
                animator.SetTrigger(DoubleJump);
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
                --jumpCount;
                bJumpPressed = false;
            }
        }

        private void Move(float move, float speed)
        {
            animator.SetFloat(Horizontal, Mathf.Abs(move));
            rigidbody2D.velocity = new Vector2(move * speed, rigidbody2D.velocity.y);
            if (move != 0.0f && !bAttack)
            {
                _direction = (move > 0) ? Direction.Right : Direction.Left;
                transform.rotation = Quaternion.Euler(0, _direction == Direction.Right ? 0 : 180, 0);
            }
        }

        private void StartRoll()
        {
            if (!Input.GetKeyDown(KeyCode.LeftShift))
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

        private void CheckVertical()
        {
            // 获取当前帧的竖直方向上的位置
            currentYPos = transform.position.y;
            // 计算竖直方向上的速度
            verticalSpeed = (currentYPos - lastYPos) / Time.deltaTime;
            // 更新上一帧的位置
            lastYPos = currentYPos;
        }

        private void PunchCombo()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //若处于Idle状态，则直接打断并过渡到attack_a(攻击阶段一)
                if (_animatorStateInfo.IsName("Idle") && attackCount == 0)
                {
                    attackCount = 1;
                    animator.SetInteger(AttackCount, attackCount);
                    bAttack = true;
                }
                else if (_animatorStateInfo.IsName("Run") && attackCount == 0)
                {
                    attackCount = 1;
                    animator.SetInteger(AttackCount, attackCount);
                    bAttack = true;
                }
                //如果当前动画处于attack_a(攻击阶段一)并且该动画播放进度小于80%，此时按下攻击键可过渡到攻击阶段二
                else if (_animatorStateInfo.IsName("Punch01") && attackCount == 1 &&
                         _animatorStateInfo.normalizedTime < 0.8f)
                {
                    attackCount = 2;
                    bWaitAttack = true;
                }
                //同上
                else if (_animatorStateInfo.IsName("Punch02") && attackCount == 2 &&
                         _animatorStateInfo.normalizedTime < 0.8f)
                {
                    attackCount = 3;
                    bWaitAttack = true;
                }else if (_animatorStateInfo.IsName("Punch03") && attackCount == 3 &&
                          _animatorStateInfo.normalizedTime < 0.8f)
                {
                    attackCount = 1;
                    bWaitAttack = true;
                }
            }
        }

        void GoToNextAttackAction()
        {
            animator.SetInteger(AttackCount, attackCount);
        }

        void OnFinish()
        {
            if (bWaitAttack)
            {
                bWaitAttack = false;
            }
            else
            {
                attackCount = 0; //将hitCount重置为0，即Idle状态
                animator.SetInteger(AttackCount, attackCount);
                bAttack = false;
            }
        }

        void OnReceiveHarm(GameObject receiver, GameObject sender)
        {
            if (receiver != gameObject)
                return;
            //var position = transform.position;
            //Vector2 direction = new Vector2((position - sender.transform.position).x, 0f);
            //direction = direction.normalized;
            //Debug.Log(direction.x);
            transform.Translate((Vector3.left) * 1f);
            animator.SetTrigger(knockback);
            bKnockback = true;
            Invoke("DelayKnockBack", 0.8f);
        }

        void OnReceiveDamage(GameObject receiver, GameObject sender)
        {
            if (sender != gameObject)
                return;
            currentAttackMoveSpeed = 0.0f;
        }

        void DelayKnockBack()
        {
            bKnockback = false;
        }
    }
}