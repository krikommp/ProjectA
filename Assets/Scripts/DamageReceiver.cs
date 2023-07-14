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
            Debug.Log($"Self: [{selfTopObj.name}, {selfTopObj.layer.ToString()}], Other: [{other.gameObject.name}, {other.gameObject.layer.ToString()}]");
            if (selfTopObj.CheckReceiveDamage(other.gameObject))
            {
                Client.Event.OnReceiverDamage.Invoke(selfTopObj, otherTopObj);
            }

            if (selfTopObj.CheckReceiveHarm(other.gameObject))
            {
                Client.Event.OnReceiverHarm.Invoke(selfTopObj, otherTopObj);
            }
        }
    }
}