﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using YuguLibrary.Controllers;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;
using YuguLibrary.Utilities;

public class PlayerFile
{
    int year;
    Seasons season;
    int day;
    Times time;
    int timeSegment;
    Dictionary<string, WeatherConditions> weather;
    string currentInstanceJSONFileName;
    Vector3Int playerPosition;
    List<PlayerUnit> playerUnits;
    List<UnitSetup> unitSetups;
    List<Quest> questProgression;

    string leadUnitJSONFileName;

    public PlayerFile()
    {
        
    }

    public PlayerFile(int year, Seasons season, int day, Times time, int timeSegment,
        Dictionary<string, WeatherConditions> weather, string currentInstanceJSONFileName, Vector3Int playerPosition, List<PlayerUnit> playerUnits,
        List<Quest> questProgression, List<UnitSetup> unitSetups, string leadUnitJSONFileName)
    {
        this.year = year;
        this.season = season;
        this.day = day;
        this.time = time;
        this.timeSegment = timeSegment;
        this.weather = weather;
        this.currentInstanceJSONFileName = currentInstanceJSONFileName;
        this.playerPosition = playerPosition;
        this.questProgression = questProgression;
        this.playerUnits = playerUnits;
        this.unitSetups = unitSetups;
        this.leadUnitJSONFileName = leadUnitJSONFileName;
    }

    public void SaveToFile(string fileName)
    {
        Debug.Log("saving file to " + Application.persistentDataPath + "/" + fileName);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Application.persistentDataPath + "/" + fileName, FileMode.Create);
        SerializablePlayerFile serializablePlayerFile = ConvertToSerializablePlayerFile();
        binaryFormatter.Serialize(fileStream, serializablePlayerFile);
        fileStream.Close();
    }

    public static PlayerFile LoadFromFile(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Application.persistentDataPath + "/testSave.ygs", FileMode.Create);
            SerializablePlayerFile serializablePlayerFile = (SerializablePlayerFile)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();

            PlayerFile playerFile = serializablePlayerFile.ConvertToPlayerFile();
            return playerFile;
        }
        else
        {
            return null;
        }
    }

    public SerializablePlayerFile ConvertToSerializablePlayerFile()
    {
        return new SerializablePlayerFile(this);
    }

    public static PlayerFile CreateNewFile()
    {
        int year = 1;
        Seasons season = Seasons.Spring;
        int day = 30;
        Times time = Times.Night;
        int timeSegment = 0;
        Dictionary<string, WeatherConditions> weather = new Dictionary<string, WeatherConditions>();
        string currentInstanceJSONFileName = "02_Magicadia/Aristocrat Tree District/Silverlight Forest/silverlight-forest_1.json";
        Vector3Int playerPosition = new Vector3Int(0, 0, 1);
        List<Quest> questProgression = new List<Quest>();
        List<PlayerUnit> playerUnits = new List<PlayerUnit>();
        List<UnitSetup> unitSetups = new List<UnitSetup>();
        string leadUnitJSONFileName = "Sample/Sample Unit/unit.json";

        PlayerFile playerFile = new PlayerFile(year, season, day, time, timeSegment, weather, currentInstanceJSONFileName, playerPosition,
            playerUnits, questProgression, unitSetups, leadUnitJSONFileName);

        return playerFile;
    }

    public static PlayerFile CreateTestFile()
    {
        int year = 5;
        Seasons season = Seasons.Summer;
        int day = 89;
        Times time = Times.EarlyMorning;
        int timeSegment = 0;
        Dictionary<string, WeatherConditions> weather = new Dictionary<string, WeatherConditions>();
        string currentInstanceJSONFileName = "00_TEST/Dummy Room/dummy-room_1.json";
        Vector3Int playerPosition = new Vector3Int(0, 0, 1);
        List<Quest> questProgression = new List<Quest>();
        List<PlayerUnit> playerUnits = new List<PlayerUnit>();
        List<UnitSetup> unitSetups = new List<UnitSetup>();
        string leadUnitJSONFileName = "Sample/Sample Unit/unit.json";

        PlayerFile playerFile = new PlayerFile(year, season, day, time, timeSegment, weather, currentInstanceJSONFileName, playerPosition,
            playerUnits, questProgression, unitSetups, leadUnitJSONFileName);

        return playerFile;
    }

    public void LoadGameState()
    {
        Debug.Log("gamestate loading");
        Instance instance = new Instance(currentInstanceJSONFileName);
        Unit unit = new Unit(leadUnitJSONFileName, 1, TargetTypes.Ally);
        Geology geology = UtilityFunctions.GetActiveGeology();
        geology.SetYear(year);
        geology.SetSeason(season);
        geology.SetDay(day);
        geology.SetTimeOfDay(time);
        geology.SetTimeSegment(timeSegment);
        geology.SetCurrentInstance(instance);

        UnitDetector unitDetector = UtilityFunctions.GetActiveUnitDetector();

        unitDetector.LoadNewInstance(instance, playerPosition, unit);
    }
}

[Serializable]
public class SerializablePlayerFile
{
    public SerializablePlayerFile()
    {

    }

    public SerializablePlayerFile(PlayerFile playerFile)
    {

    }

    public PlayerFile ConvertToPlayerFile()
    {
        return null;
    }
}

public class PlayerUnit
{
    string unitJSONFileName;
    int unitLevel;
    int unitProgressionPoint;
    bool isLead;
}

public class UnitSetup
{
    Dictionary<string, Vector3Int> unitPositions = new Dictionary<string, Vector3Int>();

    public void AddUnit(string unitJSONFileName, Vector3Int position)
    {
        if (unitPositions.ContainsKey(unitJSONFileName))
        {
            unitPositions[unitJSONFileName] = position;
        }
        else
        {
            unitPositions.Add(unitJSONFileName, position);
        }
    }

    public void RemoveUnit(string unitJSONFileName)
    {
        unitPositions.Remove(unitJSONFileName);
    }

    public Dictionary<string, Vector3Int> GetUnitSetup(Directions facingDirection, Vector3Int centerTile)
    {
        Dictionary<string, Vector3Int> rotatedPositions = unitPositions;

        switch (facingDirection)
        {
            case Directions.NW:
                rotatedPositions = AlterPositions(AlterTypes.Counterclockwise, centerTile);
                break;
            case Directions.NE: //cw 1
                rotatedPositions = AlterPositions(AlterTypes.None, centerTile);
                break;
            case Directions.SE: // flip y coord
                rotatedPositions = AlterPositions(AlterTypes.Clockwise, centerTile);
                break;
            case Directions.SW: //ccw 1
                rotatedPositions = AlterPositions(AlterTypes.Flip, centerTile);
                break;
        }

        return rotatedPositions;
    }

    Dictionary<string, Vector3Int> AlterPositions(AlterTypes type, Vector3Int centerTile)
    {
        Dictionary<string, Vector3Int> alteredPositions = new Dictionary<string, Vector3Int>();

        foreach (string unitJSONFileName in unitPositions.Keys)
        {
            Vector3Int position = unitPositions[unitJSONFileName];

            switch (type)
            {
                case AlterTypes.Clockwise:
                    position.x *= -1;
                    int tempx = position.x;
                    position.x = position.y;
                    position.y = tempx;
                    break;
                case AlterTypes.Counterclockwise:
                    position.y *= -1;
                    int tempy = position.y;
                    position.y = position.x;
                    position.x = tempy;
                    break;
                case AlterTypes.Flip:
                    position.y *= -1;
                    break;
                case AlterTypes.None:
                    break;
            }

            position.x += centerTile.x;
            position.y += centerTile.y;

            alteredPositions.Add(unitJSONFileName, position);
            //alteredPositions[unit] = position;
        }

        return alteredPositions;
    }

    enum AlterTypes
    {
        Clockwise,
        Counterclockwise,
        Flip,
        None
    }
}