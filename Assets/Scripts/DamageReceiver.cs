using System;
using UnityEngine;

namespace Client
{
    public class DamageReceiver : MonoBehaviour
    {
        public LayerMask enemyLayer;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private bool isDamaged;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 检查碰撞是否来自特定的层
            if (enemyLayer == (enemyLayer | (1 << other.gameObject.layer)))
            {
                // 处理伤害逻辑
                Debug.Log("受到伤害！");

                // 变红
                if (!isDamaged)
                {
                    spriteRenderer.color = Color.red;
                    isDamaged = true;
                    Invoke("ResetColor", 0.2f); // 0.2 秒后恢复原色
                }
            }
        }

        private void ResetColor()
        {
            spriteRenderer.color = originalColor;
            isDamaged = false;
        }
    }
}