using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class LoadingZone
        {
            public Vector3Int position;
            string instanceJSONFileName;
            Vector3Int spawnLocation;

            public LoadingZone(Vector3Int position, string instanceJSONFileName, Vector3Int spawnLocation)
            {
                this.position = position;
                this.instanceJSONFileName = instanceJSONFileName;
                this.spawnLocation = spawnLocation;
            }

            public void LoadZone(OverworldObject playerUnit)
            {
                Debug.Log("LOADING AREA: " + instanceJSONFileName);
                Instance instance = new Instance(instanceJSONFileName);
                UtilityFunctions.GetActiveUnitDetector().LoadNewInstance(instance, spawnLocation, playerUnit);
            }
        }

    }
}