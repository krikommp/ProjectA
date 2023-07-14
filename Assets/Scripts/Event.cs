using System;
using UnityEngine;

namespace Client
{
    public static class Event
    {
        public static Action OnAttackAnimationEnded;
        public static Action<GameObject, GameObject> OnReceiverDamage;
        public static Action<GameObject, GameObject> OnReceiverHarm;
    }
}