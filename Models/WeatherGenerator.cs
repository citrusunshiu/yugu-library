using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class WeatherGenerator
        {
            /// <summary>
            /// Random number generator for functions.
            /// </summary>
            private static System.Random random = new System.Random();

            /// <summary>
            /// Stores the types of weather conditions that can appear, and their chance of appearing.
            /// </summary>
            /// <remarks>
            /// The chances do not need to add up to 100; the appearance rate of any given weather condition is calculated as
            /// weatherChances[WeatherConditions.Value] / <see cref="weatherChanceSum"/>.
            /// </remarks>
            private Dictionary<WeatherConditions, int> weatherChances;

            /// <summary>
            /// The sum of all weights held in <see cref="weatherChances"/>.
            /// </summary>
            private int weatherChanceSum;

            public WeatherGenerator(Dictionary<WeatherConditions, int> weatherChances)
            {
                this.weatherChances = weatherChances;

                foreach (int value in weatherChances.Values)
                {
                    weatherChanceSum += value;
                }
            }

            /// <summary>
            /// Generates a random weather condition among those specified by <see cref="weatherChances"/>.
            /// </summary>
            /// <returns>Returns a random weather condition.</returns>
            public WeatherConditions GenerateWeather()
            {
                int rng = random.Next(0, weatherChanceSum);
                int checker = 0;

                foreach (WeatherConditions weatherCondition in weatherChances.Keys)
                {
                    if (rng >= checker && rng <= (checker + weatherChances[weatherCondition]))
                    {
                        return weatherCondition;
                    }
                    else
                    {
                        checker += weatherChances[weatherCondition];
                    }
                }

                return WeatherConditions.TestWeather;
            }
        }
    }
}