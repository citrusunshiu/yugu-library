using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;
using Newtonsoft.Json;
using System.IO;
using YuguLibrary.OverworldObjectActions;

namespace YuguLibrary
{
    namespace Utilities
    {
        public abstract class JSONParser
        {
            protected abstract void ParseJSON(JsonTextReader reader);

            protected bool CheckForProperty(string propertyName, JsonTextReader reader)
            {
                return reader.TokenType == JsonToken.PropertyName && reader.Value.Equals(propertyName);
            }

            protected bool IsArrayOngoing(JsonTextReader reader)
            {
                return reader.Read() && reader.TokenType != JsonToken.EndArray;
            }

            protected bool IsEndOfObject(JsonTextReader reader)
            {
                return reader.TokenType == JsonToken.EndObject;
            }

            protected object GetEnumerationFromJSONString(Type enumType, string enumName)
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
            private string nameID;
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
                ParseJSON(reader);
            }

            protected override void ParseJSON(JsonTextReader reader)
            {
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        //Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                        if (CheckForProperty("nameID", reader))
                        {
                            nameID = (string)GetValueFromJSON(reader);
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("animationScriptJSONFileName"))
                        {
                            reader.Read();
                            animationScript = new AnimationScript((string)reader.Value);
                        }

                        if (CheckForProperty("role", reader))
                        {
                            role = (UnitRoles)GetEnumerationFromJSONString(typeof(UnitRoles), (string)GetValueFromJSON(reader));
                        }

                        if (CheckForProperty("classification", reader))
                        {
                            classification = (UnitClassifications)GetEnumerationFromJSONString(typeof(UnitClassifications), (string)GetValueFromJSON(reader));
                        }

                        if (CheckForProperty("speedTier", reader))
                        {
                            speedTier = (SpeedTiers)GetEnumerationFromJSONString(typeof(SpeedTiers), (string)GetValueFromJSON(reader));
                        }

                        if (CheckForProperty("skillResources", reader))
                        {
                            SkillResources resource = SkillResources.HP;
                            long maxValue = 0;

                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader)) //StartObject OR EndArray
                            {
                                if (CheckForProperty("resource", reader))
                                {
                                    resource = (SkillResources)GetEnumerationFromJSONString(typeof(SkillResources), (string)GetValueFromJSON(reader));
                                }

                                if (CheckForProperty("maxValue", reader))
                                {
                                    maxValue = (long)GetValueFromJSON(reader);
                                }

                                if (reader.TokenType == JsonToken.EndObject)
                                {
                                    skillResources.Add(new SkillResource(resource, (int)maxValue));
                                    Debug.Log("created: " + resource + " (max value: " + maxValue + ")");
                                    resource = SkillResources.HP;
                                    maxValue = 0;
                                }
                            }
                        }

                        if (CheckForProperty("hpScaling", reader))
                        {
                            hpScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("mpScaling", reader))
                        {
                            mpScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("mpRegenScaling", reader))
                        {
                            mpRegenScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("physicalAttackScaling", reader))
                        {
                            physicalAttackScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("magicalAttackScaling", reader))
                        {
                            magicalAttackScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("physicalDefenseScaling", reader))
                        {
                            physicalDefenseScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("magicalDefenseScaling", reader))
                        {
                            magicalDefenseScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("speedScaling", reader))
                        {
                            speedScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("staggerThresholdScaling", reader))
                        {
                            staggerThresholdScaling = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("skills", reader))
                        {
                            string skillJSONFileName = "";
                            long levelObtained = 0;
                            long progressionPointObtained = 0;

                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader)) //StartObject OR EndArray
                            {

                                if (CheckForProperty("skillJSONFileName", reader))
                                {
                                    skillJSONFileName = (string)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("levelObtained", reader))
                                {
                                    levelObtained = (long)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("progressionPointObtained", reader))
                                {
                                    progressionPointObtained = (long)GetValueFromJSON(reader);
                                }

                                if (IsEndOfObject(reader))
                                {
                                    skills.Add(new Skill(skillJSONFileName, (int)levelObtained, (int)progressionPointObtained));
                                    Debug.Log("created: " + skillJSONFileName + " (levelObtained: " + levelObtained + ", progressionPointObtained: " + progressionPointObtained + ")");
                                    skillJSONFileName = "";
                                    levelObtained = 0;
                                    progressionPointObtained = 0;
                                }

                            }
                        }

                        if (CheckForProperty("actions", reader))
                        {
                            string actionClassName = "";

                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {
                                if (CheckForProperty("className", reader))
                                {
                                    actionClassName = (string)GetValueFromJSON(reader);
                                    Debug.Log(actionClassName);
                                }

                                if (IsEndOfObject(reader))
                                {
                                    Type type = Type.GetType(actionClassName);
                                    actions.Add((OverworldObjectAction)Activator.CreateInstance(type));
                                    Debug.Log("created: " + actionClassName);
                                    actionClassName = "";
                                }
                            }
                        }

                        if (CheckForProperty("overworldAIs", reader))
                        {
                            string overworldAIClassName = "";

                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {
                                if (CheckForProperty("className", reader))
                                {
                                    overworldAIClassName = (string)GetValueFromJSON(reader);
                                }

                                if (IsEndOfObject(reader))
                                {
                                    Type type = Type.GetType(overworldAIClassName);
                                    overworldAIs.Add((OverworldAI)Activator.CreateInstance(type));
                                    Debug.Log("created: " + overworldAIClassName);
                                    overworldAIClassName = "";
                                }
                            }
                        }

                        if (CheckForProperty("encounterAIs", reader))
                        {
                            string encounterAIClassName = "";

                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {
                                if (CheckForProperty("className", reader))
                                {
                                    encounterAIClassName = (string)GetValueFromJSON(reader);
                                }

                                if (IsEndOfObject(reader))
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
                        //Debug.Log("Token: " + reader.TokenType);
                    }
                }
            }

            #region Getters
            public string GetNameID()
            {
                return nameID;
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
            #endregion

        }

        public class InstanceJSONParser : JSONParser
        {
            private string nameID;
            private string geographyName;
            private Provinces province;
            private Districts district;
            private Areas area;
            private Vector3Int instanceCoordinates;
            private bool isHostile;
            private List<UnitSpawner> unitSpawners;
            private List<LoadingZone> loadingZones;
            private List<EventMarker> eventMarkers;
            private List<WeatherGenerator> weather;

            public InstanceJSONParser(string instanceJSONFileName)
            {
                unitSpawners = new List<UnitSpawner>();
                loadingZones = new List<LoadingZone>();
                eventMarkers = new List<EventMarker>();
                weather = new List<WeatherGenerator>();

                JsonTextReader reader = new JsonTextReader(new StreamReader(UtilityFunctions.JSON_ASSETS_INSTANCE_FOLDER_PATH + instanceJSONFileName));
                ParseJSON(reader);
            }

            protected override void ParseJSON(JsonTextReader reader)
            {
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        //Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                        if(CheckForProperty("nameID", reader))
                        {
                            nameID = (string)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("geographyName", reader))
                        {
                            geographyName = (string)GetValueFromJSON(reader);
                        }

                        if(CheckForProperty("province", reader))
                        {
                            province = (Provinces)GetEnumerationFromJSONString(typeof(Provinces), (string)GetValueFromJSON(reader));
                        }

                        if (CheckForProperty("district", reader))
                        {
                            district = (Districts)GetEnumerationFromJSONString(typeof(Districts), (string)GetValueFromJSON(reader));
                        }

                        if (CheckForProperty("area", reader))
                        {
                            area = (Areas)GetEnumerationFromJSONString(typeof(Areas), (string)GetValueFromJSON(reader));
                        }

                        if(CheckForProperty("instanceCoordinates", reader))
                        {
                            long xPos = 0;
                            long yPos = 0;
                            long zPos = 0;

                            reader.Read(); //StartObject
                            if(CheckForProperty("xPos", reader))
                            {
                                xPos = (long)GetValueFromJSON(reader);
                            }

                            if (CheckForProperty("yPos", reader))
                            {
                                yPos = (long)GetValueFromJSON(reader);
                            }

                            if (CheckForProperty("zPos", reader))
                            {
                                zPos = (long)GetValueFromJSON(reader);
                            }

                            if (IsEndOfObject(reader))
                            {
                                instanceCoordinates = new Vector3Int((int)xPos, (int)yPos, (int)zPos);
                            }
                        }

                        if(CheckForProperty("isHostile", reader))
                        {
                            isHostile = (bool)GetValueFromJSON(reader);
                        }

                        if(CheckForProperty("unitSpawners", reader))
                        {
                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {

                            }
                        }

                        if (CheckForProperty("loadingZones", reader))
                        {
                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {

                            }
                        }

                        if (CheckForProperty("eventMarkers", reader))
                        {
                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {

                            }
                        }

                        if (CheckForProperty("weather", reader))
                        {
                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {

                            }
                        }
                    }
                    else
                    {
                        //Debug.Log("Token: " + reader.TokenType);
                    }
                }
            }

            #region Getters
            public string GetNameID()
            {
                return nameID;
            }

            public string GetGeographyName()
            {
                return geographyName;
            }

            public Provinces GetProvince()
            {
                return province;
            }

            public Districts GetDistrict()
            {
                return district;
            }

            public Areas GetArea()
            {
                return area;
            }

            public Vector3Int GetInstanceCoordinates()
            {
                return instanceCoordinates;
            }

            public bool GetIsHostile()
            {
                return isHostile;
            }

            public List<UnitSpawner> GetUnitSpawners()
            {
                return unitSpawners;
            }

            public List<LoadingZone> GetLoadingZones()
            {
                return loadingZones;
            }

            public List<EventMarker> GetEventMarkers()
            {
                return eventMarkers;
            }

            public List<WeatherGenerator> GetWeather()
            {
                return weather;
            }
            #endregion
        }

        public class SkillJSONParser : JSONParser
        {
            private string nameID;
            private string descriptionID;
            private string longDescriptionID;
            private string iconFilePath;
            private TargetTypes targetType;
            private SkillTypes encounterSkillType;
            private AISkillCategories aiSkillCategory;
            private List<SkillResource> costs;
            private int cooldown;
            private List<Hit> hits;
            private List<SkillChoreography> skillChoreographies;
            private string skillFunctionName;
            

            public SkillJSONParser(string skillJSONFileName)
            {
                costs = new List<SkillResource>();
                hits = new List<Hit>();
                skillChoreographies = new List<SkillChoreography>();

                JsonTextReader reader = new JsonTextReader(new StreamReader(UtilityFunctions.JSON_ASSETS_UNIT_FOLDER_PATH + skillJSONFileName));
                ParseJSON(reader);
            }

            protected override void ParseJSON(JsonTextReader reader)
            {
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                        if(CheckForProperty("nameID", reader))
                        {
                            nameID = (string)GetValueFromJSON(reader);
                        }
                        
                        if(CheckForProperty("descriptionID", reader))
                        {
                            descriptionID = (string)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("longDescriptionID", reader))
                        {
                            longDescriptionID = (string)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("iconFilePath", reader))
                        {
                            iconFilePath = (string)GetValueFromJSON(reader);
                        }

                        if(CheckForProperty("targetType", reader))
                        {
                            targetType = (TargetTypes)GetEnumerationFromJSONString(typeof(TargetTypes), (string)GetValueFromJSON(reader));
                        }

                        if (CheckForProperty("encounterSkillType", reader))
                        {
                            encounterSkillType = (SkillTypes)GetEnumerationFromJSONString(typeof(SkillTypes), (string)GetValueFromJSON(reader));
                        }

                        if (CheckForProperty("aiSkillCategory", reader))
                        {
                            aiSkillCategory = (AISkillCategories)GetEnumerationFromJSONString(typeof(AISkillCategories), (string)GetValueFromJSON(reader));
                        }

                        if(CheckForProperty("costs", reader))
                        {
                            SkillResources resource = SkillResources.HP;
                            long value = -1;

                            reader.Read();
                            while (IsArrayOngoing(reader))
                            {

                                if(CheckForProperty("resource", reader))
                                {
                                    resource = (SkillResources)GetEnumerationFromJSONString(typeof(SkillResources), (string)GetValueFromJSON(reader));
                                }

                                if(CheckForProperty("value", reader))
                                {
                                    value = (long)GetValueFromJSON(reader);
                                }

                                if (IsEndOfObject(reader))
                                {
                                    SkillResource cost = new SkillResource(resource, (int)value);
                                    costs.Add(cost);

                                    resource = SkillResources.HP;
                                    value = -1;
                                }
                            }
                        }

                        if(CheckForProperty("cooldown", reader))
                        {
                            cooldown = (int)(long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("hits", reader))
                        {
                            string hitNameID;
                            float damageModifier;
                            float aggroModifier;
                            List<HitAttributes> attributes = new List<HitAttributes>();
                            Dictionary<string, int> statuses = new Dictionary<string, int>();

                            reader.Read();
                            while (IsArrayOngoing(reader))
                            {
                                if(CheckForProperty("nameID", reader))
                                {
                                    hitNameID = (string)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("damageModifier", reader))
                                {
                                    damageModifier = (long)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("aggroModifier", reader))
                                {
                                    aggroModifier= (long)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("attributes", reader))
                                {
                                    reader.Read();
                                    while(reader.TokenType != JsonToken.EndArray)
                                    {
                                        reader.Read();
                                        if(reader.TokenType != JsonToken.EndArray)
                                        {
                                            attributes.Add((HitAttributes)GetEnumerationFromJSONString(typeof(HitAttributes), (string)reader.Value));
                                        }
                                    }
                                }

                                if (CheckForProperty("statuses", reader))
                                {
                                    string statusJSONFileName = "";
                                    int procChance = -1;

                                    reader.Read();
                                    while (IsArrayOngoing(reader))
                                    {
                                        if(CheckForProperty("statusJSONFileName", reader))
                                        {
                                            statusJSONFileName = (string)GetValueFromJSON(reader);
                                        }   

                                        if(CheckForProperty("procChance", reader))
                                        {
                                            procChance = (int)(long)GetValueFromJSON(reader);
                                        }

                                        if (IsEndOfObject(reader))
                                        {
                                            statuses.Add(statusJSONFileName, procChance);

                                            statusJSONFileName = "";
                                            procChance = -1;
                                        }
                                        
                                    }
                                }
                            }
                        }

                        if (CheckForProperty("skillChoreographies", reader))
                        {
                            string choreographyNameID = "";
                            int animationPatternIndex = -1;
                            int totalFrames = -1;
                            bool isAttackSpeedDependent = false;
                            List<List<Hitbox>> hitboxGroups = new List<List<Hitbox>>();

                            reader.Read();
                            while (IsArrayOngoing(reader))
                            {
                                if(CheckForProperty("nameID", reader))
                                {
                                    choreographyNameID = (string)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("animationPatternIndex", reader))
                                {
                                    animationPatternIndex = (int)(long)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("totalFrames", reader))
                                {
                                    totalFrames = (int)(long)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("isAttackSpeedDependent", reader))
                                {
                                    isAttackSpeedDependent = (bool)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("hitboxGroups", reader))
                                {
                                    int startFrame = -1;
                                    int delayFrames = -1;
                                    int lingerFrames = -1;
                                    List<Hitbox> hitboxes = new List<Hitbox>();
                                    string hitFunctionName = "";
                                    int hitIndex = -1;

                                    reader.Read();
                                    while (IsArrayOngoing(reader))
                                    {
                                        if(CheckForProperty("startFrame", reader))
                                        {
                                            startFrame = (int)(long)GetValueFromJSON(reader);
                                        }

                                        if (CheckForProperty("delayFrames", reader))
                                        {
                                            delayFrames = (int)(long)GetValueFromJSON(reader);
                                        }

                                        if (CheckForProperty("lingerFrames", reader))
                                        {
                                            lingerFrames = (int)(long)GetValueFromJSON(reader);
                                        }

                                        if (CheckForProperty("hitboxes", reader))
                                        {
                                            int xPos = 0;
                                            int yPos = 0;
                                            int zPos = 0;

                                            reader.Read();
                                            while (IsArrayOngoing(reader))
                                            {
                                                if(CheckForProperty("xPos", reader))
                                                {
                                                    xPos = (int)(long)GetValueFromJSON(reader);
                                                }

                                                if (CheckForProperty("yPos", reader))
                                                {
                                                    yPos = (int)(long)GetValueFromJSON(reader);
                                                }

                                                if (CheckForProperty("zPos", reader))
                                                {
                                                    zPos = (int)(long)GetValueFromJSON(reader);
                                                }

                                                if (IsEndOfObject(reader))
                                                {
                                                    hitboxes.Add(new Hitbox(startFrame, delayFrames, lingerFrames, 
                                                        new Vector3Int(xPos, yPos, zPos), hitFunctionName, hitIndex));

                                                    xPos = 0;
                                                    yPos = 0;
                                                    zPos = 0;
                                                }
                                            }
                                        }

                                        if (IsEndOfObject(reader))
                                        {
                                            hitboxGroups.Add(hitboxes);
                                        }
                                    }
                                }

                                if (IsEndOfObject(reader))
                                {
                                    SkillChoreography skillChoreography = new SkillChoreography(choreographyNameID, animationPatternIndex, totalFrames, isAttackSpeedDependent, hitboxGroups);
                                    
                                    choreographyNameID = "";
                                    animationPatternIndex = -1;
                                    totalFrames = -1;
                                    isAttackSpeedDependent = false;
                                    hitboxGroups = new List<List<Hitbox>>();
                                }
                            }
                        }

                        if (CheckForProperty("skillFunctionName", reader))
                        {
                            skillFunctionName = (string)GetValueFromJSON(reader);
                        }

                    }
                    else
                    {
                        Debug.Log("Token: " + reader.TokenType);
                    }
                }
            }

            #region Getters
            public string GetNameID()
            {
                return nameID;
            }

            public string GetDescriptionID()
            {
                return descriptionID;
            }

            public string GetLongDescriptionID()
            {
                return longDescriptionID;
            }

            public string GetIconFilePath()
            {
                return iconFilePath;
            }

            public SkillTypes GetEncounterSkillType()
            {
                return encounterSkillType;
            }

            public TargetTypes GetTargetType()
            {
                return targetType;
            }

            public AISkillCategories GetAISkillCategory()
            {
                return aiSkillCategory;
            }

            public List<SkillResource> GetCosts()
            {
                return costs;
            }

            public int GetCooldown()
            {
                return cooldown;
            }

            public List<Hit> GetHits()
            {
                return hits;
            }

            public List<SkillChoreography> GetSkillChoreographies()
            {
                return skillChoreographies;
            }

            public string GetSkillFunctionName()
            {
                return skillFunctionName;
            }
            #endregion
        }

        public class StatusJSONParser : JSONParser
        {
            public StatusJSONParser(string statusJSONFileName)
            {
                JsonTextReader reader = new JsonTextReader(new StreamReader(UtilityFunctions.JSON_ASSETS_STATUS_FOLDER_PATH + statusJSONFileName));
                ParseJSON(reader);
            }

            protected override void ParseJSON(JsonTextReader reader)
            {
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                    }
                    else
                    {
                        Debug.Log("Token: " + reader.TokenType);
                    }
                }
            }

            #region Getters
            #endregion
        }

        public class AnimationScriptJSONParser : JSONParser
        {
            private string nameID;
            private string spritesheetFileName;
            List<AnimationPattern> animationPatterns;

            public AnimationScriptJSONParser(string animationScriptJSONFileName)
            {
                animationPatterns = new List<AnimationPattern>();

                JsonTextReader reader = new JsonTextReader(new StreamReader(UtilityFunctions.JSON_ASSETS_UNIT_FOLDER_PATH + animationScriptJSONFileName));
                ParseJSON(reader);
            }
            
            protected override void ParseJSON(JsonTextReader reader)
            {
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                        if(CheckForProperty("nameID", reader))
                        {
                            nameID = (string)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("spritesheetFileName", reader))
                        {
                            spritesheetFileName = (string)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("animationPatterns", reader))
                        {
                            string animationPatternNameID = "";
                            int totalFrames = -1;
                            List<AnimationPatternSignal> animationPatternSignals = new List<AnimationPatternSignal>();

                            reader.Read();
                            while (IsArrayOngoing(reader))
                            {
                                if(CheckForProperty("nameID", reader))
                                {
                                    animationPatternNameID = (string)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("totalFrames", reader))
                                {
                                    totalFrames = (int)(long)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("animationPatternSignals", reader))
                                {
                                    int startFrame = -1;
                                    int spriteIndex = -1;

                                    reader.Read();
                                    while (IsArrayOngoing(reader))
                                    {
                                        if(CheckForProperty("startFrame", reader))
                                        {
                                            startFrame = (int)(long)GetValueFromJSON(reader);
                                        }

                                        if (CheckForProperty("spriteIndex", reader))
                                        {
                                            spriteIndex = (int)(long)GetValueFromJSON(reader);
                                        }

                                        if (IsEndOfObject(reader))
                                        {
                                            animationPatternSignals.Add(new AnimationPatternSignal(startFrame, spriteIndex));

                                            startFrame = -1;
                                            spriteIndex = -1;
                                        }
                                    }
                                }

                                if (IsEndOfObject(reader))
                                {
                                    animationPatterns.Add(new AnimationPattern(animationPatternNameID, totalFrames, animationPatternSignals));

                                    animationPatternNameID = "";
                                    totalFrames = -1;
                                    animationPatternSignals = new List<AnimationPatternSignal>();
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

            #region Getters
            public string GetNameID()
            {
                return nameID;
            }
            
            public string GetSpritesheetFileName()
            {
                return spritesheetFileName;
            }

            public List<AnimationPattern> GetAnimationPatterns()
            {
                return animationPatterns;
            }
            #endregion
        }

        public class CutsceneJSONParser : JSONParser
        {
            public CutsceneJSONParser()
            {

            }

            protected override void ParseJSON(JsonTextReader reader)
            {
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                    }
                    else
                    {
                        Debug.Log("Token: " + reader.TokenType);
                    }
                }
            }

            #region Getters
            #endregion
        }

        public class RequestJSONParser : JSONParser
        {
            public RequestJSONParser()
            {

            }

            protected override void ParseJSON(JsonTextReader reader)
            {
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                    }
                    else
                    {
                        Debug.Log("Token: " + reader.TokenType);
                    }
                }
            }

            #region Getters
            #endregion
        }
    }
}
