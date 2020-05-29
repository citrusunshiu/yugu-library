using System;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class Instance
        {
            #region Variables
            /// <summary>
            /// The localized database ID containing the instance's name.
            /// </summary>
            private string nameID;

            /// <summary>
            /// The name of the geography prefab.
            /// </summary>
            private string geographyName;

            /// <summary>
            /// The province that the instance is located in.
            /// </summary>
            private Provinces province;

            /// <summary>
            /// The district that the instance is located in.
            /// </summary>
            private Districts district;

            /// <summary>
            /// The area that the instance is located in.
            /// </summary>
            private Areas area;

            /// <summary>
            /// The coordinates of the instance within the whole area's map grid.
            /// </summary>
            /// <remarks>
            /// Used to display the instance's location on the detailed map.
            /// </remarks>
            private Vector3Int instanceCoordinates;

            /// <summary>
            /// Whether or not units of opposing <see cref="TargetTypes"/> values can target each other in the area.
            /// </summary>
            private bool isHostile;

            /// <summary>
            /// A list of all UnitSpawner objects present in the instance.
            /// </summary>
            private List<UnitSpawner> unitSpawners;

            /// <summary>
            /// A list of all LoadingZone objects present in the instance.
            /// </summary>
            private List<LoadingZone> loadingZones;

            /// <summary>
            /// A list of all EventMarker objects present in the instance.
            /// </summary>
            private List<EventMarker> eventMarkers;

            /// <summary>
            /// A list of all WeatherGenerator objects for the instance.
            /// </summary>
            private List<WeatherGenerator> weather;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance from a JSON data file.
            /// </summary>
            /// <param name="instanceJSONFileName">The name of the JSON file, relative to the 
            /// "Assets/Resources/JSON Assets/Instances/" directory.</param>
            public Instance(string instanceJSONFileName)
            {
                InstanceJSONParser instanceJSONParser = new InstanceJSONParser(instanceJSONFileName);

                InitializeInstanceValues(instanceJSONParser);
            }
            #endregion

            #region Functions
            /// <summary>
            /// Sets the instance object's values using data from a parsed JSON file.
            /// </summary>
            /// <param name="instanceJSONParser">The InstanceJSONParser object containing the parsed JSON data.</param>
            private void InitializeInstanceValues(InstanceJSONParser instanceJSONParser)
            {
                nameID = instanceJSONParser.GetNameID();
                geographyName = instanceJSONParser.GetGeographyName();
                province = instanceJSONParser.GetProvince();
                district = instanceJSONParser.GetDistrict();
                area = instanceJSONParser.GetArea();
                instanceCoordinates = instanceJSONParser.GetInstanceCoordinates();
                isHostile = instanceJSONParser.GetIsHostile();
                unitSpawners = instanceJSONParser.GetUnitSpawners();
                loadingZones = instanceJSONParser.GetLoadingZones();
                eventMarkers = instanceJSONParser.GetEventMarkers();
                weather = instanceJSONParser.GetWeather();
            }

            #region Getters & Setters
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

            public bool IsHostile()
            {
                return isHostile;
            }

            public List<UnitSpawner> GetUnitSpawners()
            {
                return unitSpawners;
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
            #endregion
        }
    }

}