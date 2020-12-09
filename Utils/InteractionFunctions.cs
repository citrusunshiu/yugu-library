using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Models;

public static class InteractionFunctions
{

    public static void SummonUnit(Unit unit, string unitJSONFileName)
    {

    }

    public static void SpawnOverworldObject(Unit unit, string unitJSONFileName)
    {

    }

    /// <summary>
    /// Checks if a given unit type is present in the current map.
    /// </summary>
    /// <param name="unitID">The JSON file name of the unit to search for.</param>
    /// <returns>Returns a reference to the unit found, or null otherwise.</returns>
    public static Unit SearchForUnit(string unitID)
    {
        Unit unit;
        return null;
    }

    /// <summary>
    /// Checks the tile distance between two units.
    /// </summary>
    /// <param name="unit1">The first unit to compare distance.</param>
    /// <param name="unit2">The second unit to compare distance.</param>
    /// <returns>Returns the total tile movement distance between the units.</returns>
    public static int CheckDistanceBetweenUnits(Unit unit1, Unit unit2)
    {
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit">The unit to find a target for.</param>
    /// <returns>Returns the unit with the highest aggro to the given unit.</returns>
    public static Unit GetAggroBasedTarget(Unit unit)
    {
        Unit target;
        return null;
    }

    public static void RemoveHarmfulEffects(Unit unit)
    {

    }

    public static void RemoveBeneficialEffects(Unit unit)
    {

    }
}
