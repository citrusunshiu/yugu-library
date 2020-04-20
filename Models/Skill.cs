using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using YuguLibrary.Enumerations;

namespace YuguLibrary
{
    namespace Models
    {
        /// <summary>
        /// Unique actions that allow units to interact with each other in combat.
        /// </summary>
        public class Skill
        {
            /// <summary>
            /// The skill's displayed name.
            /// </summary>
            private string name;

            /// <summary>
            /// The skill's short description.
            /// </summary>
            private string description;

            /// <summary>
            /// The skill's detailed description.
            /// </summary>
            private string longDescription;

            /// <summary>
            /// The file path to the skill's display icon.
            /// </summary>
            private string iconFilePath;

            /// <summary>
            /// The skill's type category.
            /// </summary>
            private EncounterSkillTypes encounterSkillType;

            /// <summary>
            /// The type of unit that the skill is able to target.
            /// </summary>
            /// <remarks>
            /// Value of <see cref="TargetTypes.Unique"/> indicates that <see cref="SelectSkillTargets"/> will be used
            /// for filtering targets.
            /// </remarks>
            private TargetTypes targetType;

            /// <summary>
            /// The skill's category for AI usage.
            /// </summary>
            /// <remarks>
            /// Uses <see cref="AISkillCategories"/>.
            /// </remarks>
            private AISkillCategories aiSkillCategory;

            /// <summary>
            /// The skill's cost to be used.
            /// </summary>
            /// <remarks>
            /// Key uses <see cref="UnitStats.HP"/>, <see cref="UnitStats.MP"/>, or a value from
            /// <see cref="Enumerations.SpecialResources"/>. All costs must be met by the unit for the skill to be used.
            /// </remarks>
            private Dictionary<SkillResources, int> cost;

            /// <summary>
            /// The skill's cooldown when used.
            /// </summary>
            private int cooldown;

            /// <summary>
            /// The skill's current cooldown.
            /// </summary>
            /// <remarks>
            /// Value is set to the value of <see cref="cooldown"/> when the skill is used by the owner unit. All skill 
            /// cooldowns are decreased by 1 at the beginning of the owner unit’s turn. Skills can only be used when 
            /// currentCooldown is 0.
            /// </remarks>
            private int currentCooldown;

            /// <summary>
            /// Whether the skill can be used or not.
            /// </summary>
            private bool isEnabled = true;

            /// <summary>
            /// The unit that the skill is attached to.
            /// </summary>
            private Unit unit;


            public Skill(string skillJSONFilePath, int levelObtained, int progressionPointObtained)
            {

            }

            /// <summary>
            /// Sets the owner of the skill object to a given unit.
            /// </summary>
            /// <param name="unit">The unit that the skill should be set to.</param>
            public void AttachSkillToUnit(Unit unit)
            {
                this.unit = unit;
            }

        }
    }

}