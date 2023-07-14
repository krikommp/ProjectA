using UnityEngine;

namespace Client
{
    public static class ActionMaskLayer
    {
        public static LayerMask Attack = new LayerMask() { value = 6 };
        public static LayerMask Harm = new LayerMask() { value = 7 };
        public static LayerMask Defense = new LayerMask() { value = 8 };

        public static bool CheckReceiveDamage(this GameObject self, GameObject other)
        {
            if (Harm == self.layer)
            {
                if (Attack == other.layer)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static bool CheckReceiveHarm(this GameObject self, GameObject other)
        {
            if (Harm == self.layer)
            {
                if (Harm == other.layer)
                {
                    return true;
                }
            }

            return false;
        }
    }
}