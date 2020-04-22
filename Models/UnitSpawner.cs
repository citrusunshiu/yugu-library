using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            public UnitSpawner()
            {

            }
        }
    }
}
