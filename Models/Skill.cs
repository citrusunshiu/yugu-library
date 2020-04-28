using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Utilities;

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
            /// The localized database ID containing the skill's displayed name.
            /// </summary>
            private string nameID;

            /// <summary>
            /// The localized database ID containing the skill's short description.
            /// </summary>
            private string descriptionID;

            /// <summary>
            /// The localized database ID containing the skill's detailed description.
            /// </summary>
            private string longDescriptionID;

            /// <summary>
            /// The file path to the skill's display icon.
            /// </summary>
            private string iconFilePath;

            /// <summary>
            /// The skill's type category.
            /// </summary>
            private SkillTypes encounterSkillType;

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
            /// The skill's costs to be used.
            /// </summary>
            private List<SkillResource> costs;

            /// <summary>
            /// The skill's cooldown when used.
            /// </summary>
            private int cooldown;

            /// <summary>
            /// The list of hits that the skill has access to during usage.
            /// </summary>
            private List<Hit> hits;

            /// <summary>
            /// The list of animations and hitbox timings that the skill has access to during usage.
            /// </summary>
            private List<SkillChoreography> skillChoreographies;

            /// <summary>
            /// The name of the function associated with the skill object's logic.
            /// </summary>
            private string skillFunctionName;
            
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

            private int levelObtained;

            private int progressionPointObtained;
            
            public Skill(string skillJSONFileName, int levelObtained, int progressionPointObtained)
            {
                this.levelObtained = levelObtained;
                this.progressionPointObtained = progressionPointObtained;

                SkillJSONParser skillJSONParser = new SkillJSONParser(skillJSONFileName);

                InitializeSkillValues(skillJSONParser);
            }

            private void InitializeSkillValues(SkillJSONParser skillJSONParser)
            {
                nameID = skillJSONParser.GetNameID();
                descriptionID = skillJSONParser.GetDescriptionID();
                longDescriptionID = skillJSONParser.GetLongDescriptionID();
                iconFilePath = skillJSONParser.GetIconFilePath();
                encounterSkillType = skillJSONParser.GetEncounterSkillType();
                targetType = skillJSONParser.GetTargetType();
                aiSkillCategory = skillJSONParser.GetAISkillCategory();
                costs = skillJSONParser.GetCosts();
                cooldown = skillJSONParser.GetCooldown();
                hits = skillJSONParser.GetHits();
                skillChoreographies = skillJSONParser.GetSkillChoreographies();
                skillFunctionName = skillJSONParser.GetSkillFunctionName();
            }

            /// <summary>
            /// Sets the owner of the skill object to a given unit.
            /// </summary>
            /// <param name="unit">The unit that the skill should be set to.</param>
            public void AttachSkillToUnit(Unit unit)
            {
                this.unit = unit;
            }

            public void ExecuteSkill()
            {
                MethodInfo method = GetType().GetMethod(skillFunctionName, BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(this, null);
            }

            private void TestSkill()
            {
                Debug.Log("Skill:TestSkill running");
            }
        }
    }

}