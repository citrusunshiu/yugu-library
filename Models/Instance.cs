using System;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;

namespace Models
{
    public abstract class Instance
    {
        public abstract string GeographyName { get; }

        /// <summary>
        /// The province that the instance is located in.
        /// </summary>
        public abstract Provinces Province { get; }

        /// <summary>
        /// The district that the instance is located in.
        /// </summary>
        public abstract Districts District { get; }

        /// <summary>
        /// The area that the instance is located in.
        /// </summary>
        public abstract Areas Area { get; }

        public abstract bool IsHostile { get; }

        public abstract List<UnitSpawner> UnitSpawners { get; }

        public abstract List<LoadingZone> LoadingZones { get; }

        public abstract List<EventMarker> EventMarkers { get; }

        public abstract List<WeatherGenerator> Weather { get; }

        /// <summary>
        /// The coordinates of the instance within the area.
        /// </summary>
        Vector3Int instanceCoordinates; //TODO: idk what this does; maybe remove?

        public void SetupInstance()
        {

        }
    }
}
