using System;
using UnityEngine;

namespace ActionHelper.scripts.Editor
{
    [Serializable]
    public class Entity
    {
        /// <summary>
        /// 이름
        /// </summary>
        public string name;
        
        /// <summary>
        /// 애니메이션 클립
        /// </summary>
        public AnimationClip animationClip;
    }
}