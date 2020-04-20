using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;
using Newtonsoft.Json;
using System.IO;
using AnimationScripts;
using YuguLibrary.OverworldObjectActions;

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

            protected object GetValueFromJSON(JsonTextReader reader)
            {
                reader.Read();
                return reader.Value;
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

            private float hpScaling;
            private float mpScaling;
            private float mpRegenScaling;
            private float physicalAttackScaling;
            private float magicalAttackScaling;
            private float physicalDefenseScaling;
            private float magicalDefenseScaling;
            private float staggerThresholdScaling;
            private float speedScaling;

            private List<Skill> skills;
            private List<OverworldObjectAction> actions;
            private List<OverworldAI> overworldAIs;
            private List<EncounterAI> encounterAIs;

            public UnitJSONParser(string unitJSONFileName)
            {
                skillResources = new List<SkillResource>();
                skills = new List<Skill>();
                actions = new List<OverworldObjectAction>();
                overworldAIs = new List<OverworldAI>();
                encounterAIs = new List<EncounterAI>();

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
                            role = (UnitRoles)ParseStringToEnumeration(typeof(UnitRoles), (string)GetValueFromJSON(reader));
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("classification"))
                        {
                            reader.Read();
                            classification = (UnitClassifications)ParseStringToEnumeration(typeof(UnitClassifications), (string)reader.Value);
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("speedTier"))
                        {
                            reader.Read();
                            speedTier = (SpeedTiers)ParseStringToEnumeration(typeof(SpeedTiers), (string)reader.Value);
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("skillResources"))
                        {
                            SkillResources resource = SkillResources.HP;
                            long maxValue = 0;

                            reader.Read(); //StartArray
                            while(reader.Read() && reader.TokenType != JsonToken.EndArray) //StartObject OR EndArray
                            {
                                if(reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("resource"))
                                {
                                    reader.Read();
                                    resource = (SkillResources)ParseStringToEnumeration(typeof(SkillResources), (string)reader.Value);
                                }

                                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("maxValue"))
                                {
                                    reader.Read();
                                    maxValue = (long)reader.Value;
                                }

                                if(reader.TokenType == JsonToken.EndObject)
                                {
                                    skillResources.Add(new SkillResource(resource, (int)maxValue));
                                    Debug.Log("created: " + resource + " (max value: " + maxValue + ")");
                                    resource = SkillResources.HP;
                                    maxValue = 0;
                                }
                            }
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("hpScaling"))
                        {
                            reader.Read();
                            hpScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("mpScaling"))
                        {
                            reader.Read();
                            mpScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("mpRegenScaling"))
                        {
                            reader.Read();
                            mpRegenScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("physicalAttackScaling"))
                        {
                            reader.Read();
                            physicalAttackScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("magicalAttackScaling"))
                        {
                            reader.Read();
                            magicalAttackScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("physicalDefenseScaling"))
                        {
                            reader.Read();
                            physicalDefenseScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("magicalDefenseScaling"))
                        {
                            reader.Read();
                            magicalDefenseScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("speedScaling"))
                        {
                            reader.Read();
                            speedScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("staggerThresholdScaling"))
                        {
                            reader.Read();
                            staggerThresholdScaling = (long)reader.Value;
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("skills"))
                        {
                            string skillJSONFilePath = "";
                            long levelObtained = 0;
                            long progressionPointObtained = 0;

                            reader.Read(); //StartArray
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray) //StartObject OR EndArray
                            {

                                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("skillJSONFilePath"))
                                {
                                    reader.Read();
                                    skillJSONFilePath = (string)reader.Value;
                                }

                                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("levelObtained"))
                                {
                                    reader.Read();
                                    levelObtained = (long)reader.Value;
                                }

                                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("progressionPointObtained"))
                                {
                                    reader.Read();
                                    progressionPointObtained = (long)reader.Value;
                                }

                                if (reader.TokenType == JsonToken.EndObject)
                                {
                                    skills.Add(new Skill(skillJSONFilePath, (int)levelObtained, (int)progressionPointObtained));
                                    Debug.Log("created: " + skillJSONFilePath + " (levelObtained: " + levelObtained + ", progressionPointObtained: " + progressionPointObtained + ")");
                                    skillJSONFilePath = "";
                                    levelObtained = 0;
                                    progressionPointObtained = 0;
                                }

                            }
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("actions"))
                        {
                            string actionClassName = "";

                            reader.Read(); //StartArray
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("className"))
                                {
                                    reader.Read();
                                    actionClassName = (string)reader.Value;
                                    Debug.Log(actionClassName);
                                }

                                if (reader.TokenType == JsonToken.EndObject)
                                {
                                    Type type = Type.GetType(actionClassName);
                                    actions.Add((OverworldObjectAction)Activator.CreateInstance(type));
                                    Debug.Log("created: " + actionClassName);
                                    actionClassName = "";
                                }
                            }
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("overworldAIs"))
                        {
                            string overworldAIClassName = "";

                            reader.Read(); //StartArray
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("className"))
                                {
                                    reader.Read();
                                    overworldAIClassName = (string)reader.Value;
                                }

                                if (reader.TokenType == JsonToken.EndObject)
                                {
                                    Type type = Type.GetType(overworldAIClassName);
                                    overworldAIs.Add((OverworldAI)Activator.CreateInstance(type));
                                    Debug.Log("created: " + overworldAIClassName);
                                    overworldAIClassName = "";
                                }
                            }
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("encounterAIs"))
                        {
                            string encounterAIClassName = "";

                            reader.Read(); //StartArray
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("className"))
                                {
                                    reader.Read();
                                    encounterAIClassName = (string)reader.Value;
                                }

                                if (reader.TokenType == JsonToken.EndObject)
                                {
                                    Type type = Type.GetType(encounterAIClassName);
                                    encounterAIs.Add((EncounterAI)Activator.CreateInstance(type));
                                    Debug.Log("created: " + encounterAIClassName);
                                    encounterAIClassName = "";
                                }


                            }
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

        public class InstanceJSONParser : JSONParser
        {

        }

        public class SkillJSONParser : JSONParser
        {

        }

        public class AnimationScriptJSONParser : JSONParser
        {

        }
    }
}
