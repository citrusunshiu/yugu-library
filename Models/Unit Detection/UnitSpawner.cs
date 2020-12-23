using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class UnitSpawner
        {
            /// <summary>
            /// The name of the JSON file containing the unit to be created.
            /// </summary>
            string unitJSONFileName;

            /// <summary>
            /// The max amount of units that can be on the map that are created by the <see cref="UnitSpawner"/> object.
            /// </summary>
            int maxUnits;

            /// <summary>
            /// The level of the units that are created.
            /// </summary>
            int unitLevel;

            /// <summary>
            /// The position of the <see cref="UnitSpawner"/>.
            /// </summary>
            Vector3Int position;

            /// <summary>
            /// The spawn range that units created can be placed in.
            /// </summary>
            AreaOfEffect spawnRange;

            /// <summary>
            /// The time between unit spawns.
            /// </summary>
            float spawnTime;

            /// <summary>
            /// Creates a new <see cref="UnitSpawner"/>.
            /// </summary>
            /// <param name="maxUnits">The maximum unit count for the UnitSpawner.</param>
            /// <param name="spawnRange">The range of the UnitSpawner.</param>
            /// <param name="unitLevel">The level of the units created by the UnitSpawner.</param>
            /// <param name="spawnTime">The spawn time of units created by the UnitSpawner.</param>
            public UnitSpawner(string unitJSONFileName, int maxUnits, AreaOfEffect spawnRange, int unitLevel, float spawnTime = 60f)
            {
                this.unitJSONFileName = unitJSONFileName;
                this.maxUnits = maxUnits;
                this.spawnRange = spawnRange;
                this.spawnTime = spawnTime;
                this.unitLevel = unitLevel;
            }

            /// <summary>
            /// Spawns the maximum allowed amount of units at once.
            /// </summary>
            /// <remarks>
            /// Number of units spawned is dictated by <see cref="maxUnits"/>.
            /// </remarks>
            public void SpawnAllUnits()
            {
                int spawnCount = 0;
                int tryCount = 0;
                while (spawnCount < maxUnits && tryCount < 100)
                {
                    tryCount++;
                    if (SpawnUnit())
                    {
                        spawnCount++;
                    }
                }

                Debug.Log("done spawning " + unitJSONFileName + ". (" + maxUnits + "/" + spawnCount + " units spawned. total tries: "
                    + tryCount + ")");
            }

            /// <summary>
            /// Attempts to spawn a single unit into the instance.
            /// </summary>
            /// <returns>Returns true if the unit was successfully created and placed, and false otherwise.</returns>
            bool SpawnUnit()
            {
                Vector3Int tile = spawnRange.RandomTile();
                if (UtilityFunctions.GetActiveUnitDetector().IsTileOpen(tile, false))
                {
                    Unit newUnit = new Unit(unitJSONFileName, unitLevel, TargetTypes.Enemy);
                    newUnit.SetUnitSpawner(this);
                    UtilityFunctions.GetActiveUnitDetector().SpawnOverworldObject(newUnit, tile);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Spawns a unit into the instance after waiting a certain duration.
            /// </summary>
            /// <remarks>
            /// Wait time is dictated by <see cref="spawnTime"/>.
            /// </remarks>
            /// <returns></returns>
            public IEnumerator RespawnUnit()
            {
                yield return new WaitForSeconds(spawnTime);
                SpawnUnit();
            }
        }
    }
}
