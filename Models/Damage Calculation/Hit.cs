using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;

namespace YuguLibrary
{
    namespace Models
    {
        /// <summary>
        /// An object utilized by <see cref="Skill"/> to manipulate the HP stat of <see cref="OverworldObject"/> objects.
        /// </summary>
        public class Hit
        {
            #region Variables

            #region Damage Values
            public static readonly float DAMAGE_LOW = 1000F;
            public static readonly float DAMAGE_MODERATE = 2000F;
            public static readonly float DAMAGE_HIGH = 4000F;
            public static readonly float DAMAGE_HEAVY = 8000F;
            public static readonly float DAMAGE_MASSIVE = 16000F;
            #endregion

            #region Aggro Values
            public static readonly float AGGRO_NONE = 0;
            public static readonly float AGGRO_LOW = 0.25F;
            public static readonly float AGGRO_AVERAGE = 0.5F;
            public static readonly float AGGRO_HIGH = 1F;
            public static readonly float AGGRO_VERYHIGH = 2F;
            #endregion

            #region Proc Values
            public static readonly int EFFECT_LOW = 10;
            public static readonly int EFFECT_MODERATE = 30;
            public static readonly int EFFECT_HIGH = 70;
            public static readonly int EFFECT_GUARANTEED = 100;
            #endregion

            /// <summary>
            /// The localized database ID containing the hit's name.
            /// </summary>
            private string nameID;

            /// <summary>
            /// The hit's damage/healing modifier.
            /// </summary>
            /// <remarks>
            /// Used to calculate the hit's damage or healing.
            /// </remarks>
            private float hitModifier;

            /// <summary>
            /// The hit's aggro modifier.
            /// </summary>
            /// <remarks>
            /// Used to calculate the hit's aggro generation.
            /// </remarks>
            public float aggroModifier;
            
            /// <summary>
            /// The hit's attributes.
            /// </summary>
            /// <remarks>
            /// Size is equivalent to the length of <see cref="HitAttributes"/>. Toggle an attribute's index 
            /// value to true to apply the respective attribute.
            /// </remarks>
            public bool[] attributes = new bool[Enum.GetNames(typeof(HitAttributes)).Length];

            /// <summary>
            /// Statuses that the hit may inflict on its targets.
            /// </summary>
            /// 
            /// <remarks>
            /// Key is the name of the JSON file containing the data for the <see cref="Status"/> object; value is the 
            /// chance for the status to activate (value between 0-100).
            /// </remarks>
            public Dictionary<string, int> statuses;
            #endregion

            #region Constructors
            public Hit(string nameID, float hitModifier, float aggroMultiplier, List<HitAttributes> attributes,
                Dictionary<string, int> statuses)
            {
                this.nameID = nameID;
                this.hitModifier = hitModifier;
                this.aggroModifier = aggroMultiplier;
                this.statuses = statuses;

                foreach (HitAttributes attribute in attributes)
                {
                    this.attributes[(int)attribute] = true;
                }
            }
            #endregion

            #region Functions
            /// <summary>
            /// Checks if the hit possesses a given <see cref="HitAttributes"/> value.
            /// </summary>
            /// <param name="attribute">HitAttributes value to check for.</param>
            /// <returns>True if the hit possesses the value, and false otherwise.</returns>
            public bool CheckAttribute(HitAttributes attribute)
            {
                return attributes[(int)attribute];
            }

            #region Getters & Setters
            public float GetHitModifier()
            {
                return hitModifier;
            }

            public float GetAggroModifier()
            {
                return aggroModifier;
            }
            #endregion
            #endregion
        }
    }
}
