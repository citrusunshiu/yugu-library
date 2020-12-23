using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using YuguLibrary.Controllers;
using YuguLibrary.Enumerations;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        /// <summary>
        /// Unique actions that allow units to interact with each other in combat.
        /// </summary>
        public abstract class Skill
        {
            #region Variables
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
            protected List<SkillChoreography> skillChoreographies;

            /// <summary>
            /// The name of the function contained in <see cref="SkillHub"/> associated with the skill object's logic.
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

            /// <summary>
            /// The level that the skill is obtained at.
            /// </summary>
            private int levelObtained;

            /// <summary>
            /// The unit progression point that the skill becomes available at.
            /// </summary>
            /// <remarks>
            /// Both the unit's progression point and level must be met for a skill to become available.
            /// </remarks>
            private int progressionPointObtained;
            #endregion

            #region Constructors
            public Skill(string skillJSONFileName, int levelObtained, int progressionPointObtained)
            {
                this.levelObtained = levelObtained;
                this.progressionPointObtained = progressionPointObtained;

                SkillJSONParser skillJSONParser = new SkillJSONParser(skillJSONFileName);

                InitializeSkillValues(skillJSONParser);
            }
            #endregion

            #region Functions
            #region Public Functions
            /// <summary>
            /// Sets the owner of the skill object to a given unit.
            /// </summary>
            /// <param name="unit">The unit that the skill should be set to.</param>
            public void AttachSkillToUnit(Unit unit)
            {
                this.unit = unit;
            }

            /// <summary>
            /// Runs the function in <see cref="SkillHub"/> with the name specified by <see cref="skillFunctionName"/>
            /// to execute the skill's logic.
            /// </summary>
            public void ExecuteSkill()
            {
                Debug.Log("execskill");
                Debug.Log(skillFunctionName);
                if (skillFunctionName.Equals(""))
                {
                    RunSkillDefault();
                }
                else
                {
                    MethodInfo method = GetType().GetMethod(skillFunctionName, BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(this, null);
                }
            }

            /// <summary>
            /// Sets the cooldown of the skill to 0.
            /// </summary>
            public void ResetCooldown()
            {
                currentCooldown = 0;
            }

            /// <summary>
            /// Allows the skill to be executed.
            /// </summary>
            public void EnableSkill()
            {
                isEnabled = true;
            }

            /// <summary>
            /// Prevents the skill from being executed.
            /// </summary>
            public void DisableSkill()
            {
                isEnabled = false;
            }
            #endregion

            #region Private Functions
            /// <summary>
            /// Runs all of the skill's skill choreographies in order.
            /// </summary>
            protected void RunSkillDefault()
            {
                Debug.Log("skilldefault: count = " + skillChoreographies.Count);
                foreach (SkillChoreography skillChoreography in skillChoreographies)
                {
                    RunSkillChoreography(skillChoreography);
                }
            }

            /// <summary>
            /// Sets the unit's animation to the one specified by the skill choreography, and places the hitboxes 
            /// specified.
            /// </summary>
            /// <param name="skillChoreography">The skill choreography to run.</param>
            private void RunSkillChoreography(SkillChoreography skillChoreography)
            {
                Debug.Log("skillcho");
                int framesGiven = skillChoreography.GetTotalFrames();

                if (skillChoreography.GetIsAttackSpeedDependent())
                {
                    Debug.Log(framesGiven);
                    Debug.Log(unit.attackSpeed);
                    framesGiven = (int)Math.Round(framesGiven/unit.attackSpeed);
                }

                unit.SetAnimation(skillChoreography.GetAnimationPatternIndex(), framesGiven, false);

                UnitDetector unitDetector = UtilityFunctions.GetActiveUnitDetector();

                foreach (List<Hitbox> hitboxGroup in skillChoreography.GetHitboxGroups())
                {
                    string hitboxGroupID = "id:" + 
                        unit.GetHashCode() + 
                        "_" + GetHashCode() + 
                        "_" + skillChoreographies.GetHashCode() + 
                        "_" + hitboxGroup.GetHashCode() + 
                        "_" + UtilityFunctions.UnixTimeNow();

                    foreach(Hitbox hitbox in hitboxGroup)
                    {
                        hitbox.SetHitboxGroupID(hitboxGroupID);
                        hitbox.SetUnit(unit);
                        hitbox.SetSkill(this);
                        hitbox.AdjustPositionToUnit();

                        if (skillChoreography.GetIsAttackSpeedDependent())
                        {
                            hitbox.AdjustToAttackSpeed(unit.attackSpeed);
                        }
                        
                        unitDetector.PlaceHitbox(hitbox);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="skillJSONParser">The JSON parser containing the skill's data.</param>
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
            #endregion

            #region Getters & Setters
            public List<Hit> GetHits()
            {
                return hits;
            }

            public bool IsEnabled()
            {
                return isEnabled;
            }
            #endregion

            #region Protected Skill Functionality Parts
            /// <summary>
            /// Moves a specified projectile hitbox a specified number of tiles at a specified speed.
            /// </summary>
            /// <param name="projectileChoreography">The <see cref="SkillChoreography"/> to be ran during the projectile's travel time.</param>
            /// <param name="onConnect">The <see cref="SkillChoreography"/> to be ran when the projectile connects with a target.</param>
            /// <param name="range">The amount of tiles the projectile should travel.</param>
            /// <param name="movementFrames">The amount of frames taken for the projectile to advance one tile.</param>
            protected void LaunchProjectile(SkillChoreography projectileChoreography, SkillChoreography onConnect, int range, 
                float movementFrames)
            {
                //may need more params/extra info from json

                //TODO: projectile fire/movement
                RunSkillChoreography(onConnect);
            }

            protected void Type2Target()
            {

            }

            protected void Type3Target()
            {

            }

            protected void MultiPromptInput()
            {

            }
            #endregion
            #endregion
        }

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
            public int GetAnimationPatternIndex()
            {
                return animationPatternIndex;
            }

            public int GetTotalFrames()
            {
                return totalFrames;
            }

            public bool GetIsAttackSpeedDependent()
            {
                return isAttackSpeedDependent;
            }

            public List<List<Hitbox>> GetHitboxGroups()
            {
                return hitboxGroups;
            }
            #endregion
        }
    }

}