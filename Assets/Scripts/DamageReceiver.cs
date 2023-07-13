using System;
using UnityEngine;

namespace Client
{
    public class DamageReceiver : MonoBehaviour
    {
        public LayerMask enemyLayer;

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log("SS Collision");
            // 检查碰撞是否来自特定的层
            if (enemyLayer == (enemyLayer | (1 << other.gameObject.layer)))
            {
                // 处理伤害逻辑
                Debug.Log("受到伤害！");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("SS Trigger");
            // 检查碰撞是否来自特定的层
            if (enemyLayer == (enemyLayer | (1 << other.gameObject.layer)))
            {
                // 处理伤害逻辑
                Debug.Log("受到伤害！");
            }
        }
    }
}