using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;
using Newtonsoft.Json;
using System.IO;
using AnimationScripts;

namespace YuguLibrary
{
    namespace Utilities
    {
        public abstract class JSONParser
        {
            protected object ParseStringToEnumeration(Type enumType, string enumName)
            {
                return Enum.Parse(enumType, enumName);
            }
        }

        public class UnitJSONParser : JSONParser
        {
            private string name;
            private AnimationScript animationScript;
            private UnitRoles role;
            private UnitClassifications classification;
            private SpeedTiers speedTier;

            private List<SkillResource> skillResources;

            private int hpScaling;
            private int mpScaling;
            private int mpRegenScaling;
            private int physicalAttackScaling;
            private int magicalAttackScaling;
            private int physicalDefenseScaling;
            private int magicalDefenseScaling;
            private int staggerThresholdScaling;
            private int speedScaling;

            private List<Skill> skills;
            private List<OverworldObjectAction> actions;
            private List<OverworldAI> overworldAIs;
            private List<EncounterAI> encounterAIs;

            public UnitJSONParser(string unitJSONFileName)
            {
                JsonTextReader reader = new JsonTextReader(new StreamReader(UtilityFunctions.JSON_ASSETS_UNIT_FOLDER_PATH + unitJSONFileName));
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                        if(reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("name"))
                        {
                            reader.Read();
                            name = (string)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("animationScriptJSONFilePath"))
                        {
                            reader.Read();
                            animationScript = new TestUnitAnimationScript();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("role"))
                        {
                            reader.Read();
                            role = (UnitRoles)ParseStringToEnumeration(typeof(UnitRoles), (string)reader.Value);
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("classification"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("speedTier"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("skillResources"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("hpScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("mpScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("mpRegenScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("physicalAttackScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("magicalAttackScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("physicalDefenseScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("magicalDefenseScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("speedScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("staggerThresholdScaling"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("skills"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("actions"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("overworldAIs"))
                        {
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("encounterAIs"))
                        {
                            reader.Read();
                        }
                    }
                    else
                    {
                        Debug.Log("Token: " + reader.TokenType);
                    }
                }
            }

            public string GetName()
            {
                return name;
            }

            public AnimationScript GetAnimationScript()
            {
                return animationScript;
            }

            public UnitRoles GetRole()
            {
                return role;
            }

            public UnitClassifications GetClassification()
            {
                return classification;
            }

            public SpeedTiers GetSpeedTier()
            {
                return speedTier;
            }

            public float GetHPScaling()
            {
                return hpScaling;
            }

            public float GetMPScaling()
            {
                return mpScaling;
            }

            public float GetMPRegenScaling()
            {
                return mpRegenScaling;
            }

            public float GetPhysicalAttackScaling()
            {
                return physicalAttackScaling;
            }

            public float GetMagicalAttackScaling()
            {
                return magicalAttackScaling;
            }

            public float GetPhysicalDefenseScaling()
            {
                return physicalDefenseScaling;
            }

            public float GetMagicalDefenseScaling()
            {
                return magicalDefenseScaling;
            }

            public float GetStaggerThresholdScaling()
            {
                return staggerThresholdScaling;
            }

            public float GetSpeedScaling()
            {
                return speedScaling;
            }

            public List<SkillResource> GetSkillResources()
            {
                return skillResources;
            }

            public List<Skill> GetSkills()
            {
                return skills;
            }

            public List<OverworldObjectAction> GetActions()
            {
                return actions;
            }

            public List<OverworldAI> GetOverworldAIs()
            {
                return overworldAIs;
            }

            public List<EncounterAI> GetEncounterAIs()
            {
                return encounterAIs;
            }
        }
    }
}
