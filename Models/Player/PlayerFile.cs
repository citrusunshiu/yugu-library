﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

public class PlayerFile
{
    int year;
    Seasons season;
    int day;
    Times time;
    int timeSegment;
    WeatherConditions weather;
    string currentInstanceJSONFileName;
    Vector3Int playerPosition;
    List<PlayerUnit> playerUnits;
    List<UnitSetup> unitSetups;
    List<Quest> questProgression;

    public PlayerFile()
    {

    }

    public PlayerFile(int year, Seasons season, int day, Times time, int timeSegment,
        WeatherConditions weather, string currentInstanceJSONFileName, Vector3Int playerPosition, List<PlayerUnit> playerUnits,
        List<Quest> questProgression, List<UnitSetup> unitSetups)
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