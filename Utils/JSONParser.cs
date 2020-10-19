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
        /// <summary>
        /// Parses information from JSON files into C# objects.
        /// </summary>
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

        /// <summary>
        /// Parses data from a JSON file into a <see cref="Unit"/> object.
        /// </summary>
        public class UnitJSONParser : JSONParser
        {
            private string nameID;
            private AnimationScript animationScript;
            private UnitRoles role;
            private UnitClassifications classification;
            private SpeedTiers speedTier;
            private float baseMovementFrames;
            private float baseDescentFrames;
            private float baseAscentFrames;

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
            private List<UnitAI> unitAIs;
            private List<EncounterAI> encounterAIs;

            public UnitJSONParser(string unitJSONFileName)
            {
                skillResources = new List<SkillResource>();
                skills = new List<Skill>();
                actions = new List<OverworldObjectAction>();
                unitAIs = new List<UnitAI>();
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

                        if (CheckForProperty("baseMovementFrames", reader))
                        {
                            baseMovementFrames = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("baseDescentFrames", reader))
                        {
                            baseDescentFrames = (long)GetValueFromJSON(reader);
                        }

                        if (CheckForProperty("baseAscentFrames", reader))
                        {
                            baseAscentFrames = (long)GetValueFromJSON(reader);
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
                                    //Debug.Log("created: " + resource + " (max value: " + maxValue + ")");
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
                                    skills.Add(new SkillHub(skillJSONFileName, (int)levelObtained, (int)progressionPointObtained));
                                    //Debug.Log("created: " + skillJSONFileName + " (levelObtained: " + levelObtained + ", progressionPointObtained: " + progressionPointObtained + ")");
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
                                }

                                if (IsEndOfObject(reader))
                                {
                                    Type type = Type.GetType(actionClassName);
                                    actions.Add((OverworldObjectAction)Activator.CreateInstance(type));
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
                                    unitAIs.Add((UnitAI)Activator.CreateInstance(type));
                                    //Debug.Log("created: " + overworldAIClassName);
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
                                    //Debug.Log("created: " + encounterAIClassName);
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

            public float GetBaseMovementFrames()
            {
                return baseMovementFrames;
            }

            public float GetBaseDescentFrames()
            {
                return baseDescentFrames;
            }

            public float GetBaseAscentFrames()
            {
                return baseAscentFrames;
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

            public List<UnitAI> GetUnitAIs()
            {
                return unitAIs;
            }

            public List<EncounterAI> GetEncounterAIs()
            {
                return encounterAIs;
            }
            #endregion

        }

        /// <summary>
        /// Parses data from a JSON file into an <see cref="Instance"/> object.
        /// </summary>
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
                        Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
                        
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
                            reader.Read();
                            if (CheckForProperty("xPos", reader))
                            {
                                xPos = (long)GetValueFromJSON(reader);
                                Debug.Log(xPos);
                                reader.Read();
                            }

                            if (CheckForProperty("yPos", reader))
                            {
                                yPos = (long)GetValueFromJSON(reader);
                                Debug.Log(yPos);
                                reader.Read();
                            }

                            if (CheckForProperty("zPos", reader))
                            {
                                zPos = (long)GetValueFromJSON(reader);
                                Debug.Log(zPos);
                                reader.Read();
                            }

                            if (IsEndOfObject(reader))
                            {
                                instanceCoordinates = new Vector3Int((int)xPos, (int)yPos, (int)zPos);
                                Debug.Log(instanceCoordinates);
                            }
                        }

                        if(CheckForProperty("isHostile", reader))
                        {
                            isHostile = (bool)GetValueFromJSON(reader);
                        }

                        if(CheckForProperty("unitSpawners", reader))
                        {
                            /*reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {

                            } */
                        }

                        if (CheckForProperty("loadingZones", reader))
                        {
                            string instanceJSONFileName = "";
                            Vector3Int loadingZonePosition = new Vector3Int(-255, -255, -255);
                            Vector3Int playerUnitSpawnPosition = new Vector3Int(-255, -255, -255);

                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {
                                if (CheckForProperty("instanceJSONFileName", reader))
                                {
                                    instanceJSONFileName = (string)GetValueFromJSON(reader);
                                }

                                if (CheckForProperty("loadingZonePosition", reader))
                                {
                                    long xPos = -255;
                                    long yPos = -255;
                                    long zPos = -255;

                                    reader.Read(); //StartObject
                                    reader.Read();
                                    if (CheckForProperty("xPos", reader))
                                    {
                                        xPos = (long)GetValueFromJSON(reader);
                                        reader.Read();
                                    }

                                    if (CheckForProperty("yPos", reader))
                                    {
                                        yPos = (long)GetValueFromJSON(reader);
                                        reader.Read();
                                    }

                                    if (CheckForProperty("zPos", reader))
                                    {
                                        zPos = (long)GetValueFromJSON(reader);
                                        reader.Read();
                                    }

                                    if (IsEndOfObject(reader))
                                    {
                                        loadingZonePosition = new Vector3Int((int)xPos, (int)yPos, (int)zPos);
                                        Debug.Log(loadingZonePosition);
                                    }
                                }

                                if (CheckForProperty("playerUnitSpawnPosition", reader))
                                {
                                    long xPos = -255;
                                    long yPos = -255;
                                    long zPos = -255;

                                    reader.Read(); //StartObject
                                    reader.Read();
                                    if (CheckForProperty("xPos", reader))
                                    {
                                        xPos = (long)GetValueFromJSON(reader);
                                        reader.Read();
                                    }

                                    if (CheckForProperty("yPos", reader))
                                    {
                                        yPos = (long)GetValueFromJSON(reader);
                                        reader.Read();
                                    }

                                    if (CheckForProperty("zPos", reader))
                                    {
                                        zPos = (long)GetValueFromJSON(reader);
                                        reader.Read();
                                    }

                                    if (IsEndOfObject(reader))
                                    {
                                        Debug.Log("testtt");
                                        playerUnitSpawnPosition = new Vector3Int((int)xPos, (int)yPos, (int)zPos);
                                        Debug.Log(playerUnitSpawnPosition);
                                    }
                                }

                                if (IsEndOfObject(reader))
                                {
                                    if (!instanceJSONFileName.Equals("") && loadingZonePosition.x != -255 && playerUnitSpawnPosition.x != -255)
                                    {
                                        Debug.Log("test");
                                        LoadingZone loadingZone = new LoadingZone(loadingZonePosition, instanceJSONFileName, playerUnitSpawnPosition);
                                        loadingZones.Add(loadingZone);
                                        Debug.Log(loadingZone);

                                        Debug.Log(playerUnitSpawnPosition);

                                        instanceJSONFileName = "";
                                        loadingZonePosition = new Vector3Int(-255, -255, -255);
                                        playerUnitSpawnPosition = new Vector3Int(-255, -255, -255);
                                    }
                                }
                            }
                        }

                        if (CheckForProperty("eventMarkers", reader))
                        {
                            /*
                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {

                            } */
                        }

                        if (CheckForProperty("weather", reader))
                        {
                            /*
                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {

                            } */
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

        /// <summary>
        /// Parses data from a JSON file into a <see cref="Skill"/> object.
        /// </summary>
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
                        //Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
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
                                    skillChoreographies.Add(skillChoreography);

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
                        //Debug.Log("Token: " + reader.TokenType);
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

        /// <summary>
        /// Parses data from a JSON file into an <see cref="AnimationScript"/> object.
        /// </summary>
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
                        //Debug.Log("Token: " + reader.TokenType + ", Value: " + reader.Value);
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
                        //Debug.Log("Token: " + reader.TokenType);
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

        /// <summary>
        /// Parses data from a JSON file into a <see cref="Cutscene"/> object.
        /// </summary>
        public class CutsceneJSONParser : JSONParser
        {
            string nameID;
            List<Scene> scenes;

            public CutsceneJSONParser(string cutsceneJSONFileName)
            {
                scenes = new List<Scene>();

                JsonTextReader reader = new JsonTextReader(
                    new StreamReader(UtilityFunctions.JSON_ASSETS_CUTSCENE_FOLDER_PATH + cutsceneJSONFileName));
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

                        if(CheckForProperty("scenes", reader))
                        {
                            string instanceJSONFileName = "";

                            int year = 0;
                            Seasons season = Seasons.Spring;
                            int date = 0;
                            Times time = Times.EarlyMorning;

                            bool isCinematic = false;

                            List<string> unitsToPlace = new List<string>();
                            List<Vector3Int> unitPositions = new List<Vector3Int>();

                            List<SceneChoreography> sceneChoreographies = new List<SceneChoreography>();
                            
                            reader.Read(); //StartArray
                            while (IsArrayOngoing(reader))
                            {
                                if(CheckForProperty("instanceJSONFileName", reader))
                                {
                                    instanceJSONFileName = (string)GetValueFromJSON(reader);
                                }

                                if(CheckForProperty("geologyInformation", reader))
                                {
                                    reader.Read(); //StartObject
                                    if(CheckForProperty("year", reader))
                                    {
                                        year = (int)(long)GetValueFromJSON(reader);
                                    }

                                    if(CheckForProperty("season", reader))
                                    {
                                        season = (Seasons)GetEnumerationFromJSONString(typeof(Seasons), (string)GetValueFromJSON(reader));
                                    }

                                    if(CheckForProperty("date", reader))
                                    {
                                        date = (int)(long)GetValueFromJSON(reader);
                                    }

                                    if(CheckForProperty("time", reader))
                                    {
                                        time = (Times)GetEnumerationFromJSONString(typeof(Times), (string)GetValueFromJSON(reader));
                                    }

                                    if (IsEndOfObject(reader))
                                    {

                                    }
                                }

                                if(CheckForProperty("isCinematic", reader))
                                {
                                    isCinematic = (bool)GetValueFromJSON(reader);
                                }

                                if(CheckForProperty("units", reader))
                                {
                                    string unitJSONFileName = "";
                                    int xPos = -255;
                                    int yPos = -255;
                                    int zPos = -255;

                                    reader.Read(); //StartArray
                                    while (IsArrayOngoing(reader))
                                    {
                                        if(CheckForProperty("unitJSONFileName", reader))
                                        {
                                            unitJSONFileName = (string)GetValueFromJSON(reader);
                                        }

                                        if(CheckForProperty("unitPosition", reader))
                                        {
                                            reader.Read(); //StartObject
                                            if (CheckForProperty("xPos", reader))
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

                                        }

                                        if (IsEndOfObject(reader))
                                        {
                                            unitsToPlace.Add(unitJSONFileName);
                                            unitPositions.Add(new Vector3Int(xPos, yPos, zPos));

                                            unitJSONFileName = "";
                                            xPos = -255;
                                            yPos = -255;
                                            zPos = -255;
                                        }
                                    }
                                }

                                if(CheckForProperty("sceneChorerographies", reader))
                                {
                                    Dialogue dialogue = new Dialogue();
                                    List<SceneAnimation> sceneAnimations = new List<SceneAnimation>();

                                    reader.Read(); //StartArray
                                    while (IsArrayOngoing(reader))
                                    {
                                        if(CheckForProperty("dialogue", reader))
                                        {
                                            string speakerID = "";
                                            string textID = "";
                                            string portraitFileName = "";
                                            int portraitEmote = -1;

                                            reader.Read(); //StartObject
                                            if (CheckForProperty("speakerID", reader))
                                            {
                                                speakerID = (string)GetValueFromJSON(reader);
                                            }

                                            if (CheckForProperty("textID", reader))
                                            {
                                                textID = (string)GetValueFromJSON(reader);
                                            }

                                            if (CheckForProperty("portraitFileName", reader))
                                            {
                                                portraitFileName = (string)GetValueFromJSON(reader);
                                            }

                                            if (CheckForProperty("portraitEmote", reader))
                                            {
                                                portraitEmote = (int)(long)GetValueFromJSON(reader);
                                            }

                                            if (IsEndOfObject(reader))
                                            {
                                                dialogue = new Dialogue(speakerID, textID, portraitFileName, portraitEmote);

                                                speakerID = "";
                                                textID = "";
                                                portraitFileName = "";
                                                portraitEmote = -1;
                                            }
                                        }

                                        if(CheckForProperty("animations", reader))
                                        {
                                            string unitName = "";
                                            int unitIndex = -1;
                                            int xPos = -255;
                                            int yPos = -255;
                                            int zPos = -255;
                                            int animationIndex = -1;


                                            reader.Read(); //StartArray
                                            while (IsArrayOngoing(reader))
                                            {
                                                if (CheckForProperty("unitName", reader))
                                                {
                                                    unitName = (string)GetValueFromJSON(reader);
                                                }

                                                if(CheckForProperty("unitIndex", reader))
                                                {
                                                    unitIndex = (int)(long)GetValueFromJSON(reader);
                                                }

                                                if (CheckForProperty("moveToTile", reader))
                                                {
                                                    reader.Read(); //StartObject
                                                    if (CheckForProperty("xPos", reader))
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
                                                }

                                                if (CheckForProperty("animationIndex", reader))
                                                {
                                                    animationIndex = (int)(long)GetValueFromJSON(reader);
                                                }

                                                if (IsEndOfObject(reader))
                                                {
                                                    sceneAnimations.Add(new SceneAnimation(unitName, unitIndex, new Vector3Int(xPos, yPos, zPos), animationIndex));

                                                    unitName = "";
                                                    unitIndex = -1;
                                                    xPos = -255;
                                                    yPos = -255;
                                                    zPos = -255;
                                                    animationIndex = -1;
                                                }
                                            }
                                        }

                                        if (IsEndOfObject(reader))
                                        {
                                            sceneChoreographies.Add(new SceneChoreography(dialogue, sceneAnimations));
                                        }
                                    }
                                }

                                if (IsEndOfObject(reader))
                                {
                                    scenes.Add(new Scene(instanceJSONFileName, year, season, date, time, isCinematic, unitsToPlace, unitPositions, sceneChoreographies));
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

            public List<Scene> GetScenes()
            {
                return scenes;
            }
            #endregion
        }

        /// <summary>
        /// Parses data from a JSON file into a <see cref="Quest"/> object.
        /// </summary>
        public class QuestJSONParser : JSONParser
        {
            public QuestJSONParser()
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

        /// <summary>
        /// Parses data from a JSON file into a <see cref="Status"/> object.
        /// </summary>
        public class StatusJSONParser : JSONParser
        {
            public StatusJSONParser(string statusJSONFileName)
            {
                JsonTextReader reader = new JsonTextReader(new StreamReader(UtilityFunctions.JSON_ASSETS_COMMON_STATUS_FOLDER_PATH + statusJSONFileName));
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
    }
}
