using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class ComboSkill
    {
        public int ID;

        public List<AnimationClip> ComboList;
    }
}