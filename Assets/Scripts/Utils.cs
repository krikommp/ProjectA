using UnityEngine;

namespace Client
{
    public static class Utils
    {
        public static GameObject GetTopmostParent(this GameObject obj)
        {
            Transform parent = obj.transform.parent;
            if(parent == null)
            {
                return obj;
            }
            else
            {
                return GetTopmostParent(parent.gameObject);
            }
        }
    }
}