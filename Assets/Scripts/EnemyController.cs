using System;
using UnityEngine;

namespace Client
{
    public class EnemyController : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private bool isDamaged;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
            Client.Event.OnReceiverDamage += OnReceiveDamage;
        }

        private void OnReceiveDamage(GameObject receiver, GameObject sender)
        {
            if (receiver != gameObject)
            {
                return;
            }

            if (!isDamaged)
            {
                spriteRenderer.color = Color.red;
                isDamaged = true;
                Invoke("ResetColor", 0.2f); // 0.2 秒后恢复原色
            }
        }

        private void ResetColor()
        {
            spriteRenderer.color = originalColor;
            isDamaged = false;
        }
    }
}