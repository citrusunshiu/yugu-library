using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

namespace YuguLibrary
{
    namespace Controllers
    {
        public class Geology
        {

            /// <summary>
            /// The current in-game year.
            /// </summary>
            public int year;

            /// <summary>
            /// The year's current season.
            /// </summary>
            public Seasons season;

            /// <summary>
            /// The season's current day.
            /// </summary>
            public int day;

            /// <summary>
            /// The day's current time.
            /// </summary>
            public Times time;

            /// <summary>
            /// The time progression of the current time of day.
            /// </summary>
            /// <remarks>
            /// Certain actions increment the time segment. When the time segment reaches 100, the time of day will 
            /// progress forward.
            /// </remarks>
            public int timeSegment; // after reaching some value (100?) it rolls over to the next time, if applicable

            Provinces currentProvince;

            Districts currentDistrict;

            Areas currentArea;

            string currentInstanceJSONFileName;

            Dictionary<string, WeatherConditions> weathers;

            bool isHostile;

            public Geology()
            {

            }

            public void SetCurrentInstance(Instance instance)
            {
                currentProvince = instance.GetProvince();
                currentDistrict = instance.GetDistrict();
                currentArea = instance.GetArea();
            }

            void GenerateWeather(string instanceJSONFileName)
            {

            }

            public void SetYear(int year)
            {
                this.year = year;
            }

            public void SetSeason(Seasons season)
            {
                this.season = season;
            }

            public void SetDay(int day)
            {
                this.day = day;
            }

            public void SetTimeOfDay(Times time)
            {
                this.time = time;
            }

            public void SetTimeSegment(int timeSegment)
            {
                this.timeSegment = timeSegment;
            }

        }
    }
}
