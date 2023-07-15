using System;
using UnityEngine;

namespace Client
{
    public class DamageReceiver : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var selfTopObj = gameObject.GetTopmostParent();
            var otherTopObj = other.gameObject.GetTopmostParent();
            if (gameObject.CheckReceiveDamage(other.gameObject))
            {
                Client.Event.OnReceiverDamage.Invoke(selfTopObj, otherTopObj);
            }

            if (gameObject.CheckReceiveHarm(other.gameObject))
            {
                Client.Event.OnReceiverHarm.Invoke(selfTopObj, otherTopObj);
            }
        }
    }
}