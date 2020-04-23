using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuguLibrary
{
    namespace Models
    {
        public class SkillChoreography
        {
            #region Variables
            /// <summary>
            /// 
            /// </summary>
            private string nameID;

            /// <summary>
            /// 
            /// </summary>
            private int animationPatternIndex;

            /// <summary>
            /// 
            /// </summary>
            private int totalFrames;

            /// <summary>
            /// 
            /// </summary>
            private bool isAttackSpeedDependent;

            /// <summary>
            /// 
            /// </summary>
            private List<List<Hitbox>> hitboxGroups;
            #endregion

            #region Constructors
            public SkillChoreography(string nameID, int animationPatternIndex, int totalFrames, bool isAttackSpeedDependent, List<List<Hitbox>> hitboxGroups)
            {
                this.nameID = nameID;
                this.animationPatternIndex = animationPatternIndex;
                this.totalFrames = totalFrames;
                this.isAttackSpeedDependent = isAttackSpeedDependent;
                this.hitboxGroups = hitboxGroups;
            }

            #endregion

            #region Functions
            #endregion
        }
    }
}
