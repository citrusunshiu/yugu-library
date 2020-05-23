﻿using System.Collections.Generic;

namespace YuguLibrary
{
    namespace Models
    {
        /// <summary>
        /// Calculates targets for enemies to prioritize for certain skills.
        /// </summary>
        /// <remarks>
        /// Enemy hits generate aggro via <see cref="EncounterController.CalculateAggro"/>, and are stored within 
        /// <see cref="aggroShares"/>. The enemies with the highest aggro values are stored in <see cref="topAggros"/> 
        /// for enemy targeting if needed. 
        /// </remarks>
        public class AggroSpread
        {
            #region Variables
            readonly float AUTO_REMOVAL_VALUE = 0.05f;

            /// <summary>
            /// The sum of all values in <see cref="aggroShares"/>.
            /// </summary>
            /// <remarks>
            /// Used to calculate the percentage of aggro held by units.
            /// </remarks>
            float aggroSum;

            /// <summary>
            /// The aggro value form enemy units that have recently attacked a unit.
            /// </summary>
            /// <remarks>
            /// When a unit is attacked, an aggro value is calculated using the damage inflicted to the defending unit 
            /// using <see cref="EncounterController.CalculateAggro"/>.
            /// </remarks>
            private Dictionary<Unit, float> aggroShares = new Dictionary<Unit, float>();


            public Dictionary<Unit, int> attackHistory = new Dictionary<Unit, int>();

            /// <summary>
            /// A list of units with the highest aggro value.
            /// </summary>
            /// <remarks>
            /// Units that are within 10% of the highest aggro value are added to the list.
            /// </remarks>
            List<Unit> topAggros = new List<Unit>();
            #endregion

            #region Functions
            /// <summary>
            /// Inserts or appends to a value in <see cref="aggroShares"/>.
            /// </summary>
            /// <remarks>
            /// Searches for the unit’s key, and adds the given value to the current value stored. If the entry does not
            /// exist, a new entry is added using the parameters given.
            /// </remarks>
            /// <param name="unit">The attacking unit that generated the aggro.</param>
            /// <param name="value">The amount of aggro that was generated by the attacking unit.</param>
            public void InsertAggro(Unit unit, float value)
            {
                if (aggroShares.ContainsKey(unit))
                {
                    aggroShares[unit] += value;
                }
                else
                {
                    aggroShares.Add(unit, value);
                }
            }

            /// <summary>
            /// Modifies a specified unit's aggro by a modifier.
            /// </summary>
            /// <param name="unit">The unit aggro to be modified.</param>
            /// <param name="modifier">The value to multiply the unit's aggro by.</param>
            public void ModifyAggro(Unit unit, float modifier)
            {
                if (aggroShares.ContainsKey(unit))
                {
                    aggroShares[unit] *= modifier;
                }
            }

            /// <summary>
            /// Checks if a unit's aggro generation is low enough to be removed.
            /// </summary>
            /// <remarks>
            /// If the difference is less than 5% of <see cref="aggroSum"/>, the entry is removed. 
            /// </remarks>
            /// <param name="unit">The unit whose aggro is to be checked.</param>
            void CheckAggroRemoval(Unit unit)
            {
                if (aggroShares.ContainsKey(unit) && aggroShares[unit] / aggroSum < AUTO_REMOVAL_VALUE)
                {
                    aggroShares.Remove(unit);
                }
            }

            /// <summary>
            /// Subtracts from an aggro value in <see cref="aggroShares"/>.
            /// </summary>
            /// <remarks>
            /// Searches for the unit’s key, and subtracts the given value from the current value stored.
            /// </remarks>
            /// <param name="unit">The key to search for.</param>
            /// <param name="value">The amount of aggro to subtract from the retrieved entry.</param>
            void SubtractAggro(Unit unit, float value)
            {
                if (aggroShares.ContainsKey(unit))
                {
                    aggroShares[unit] -= value;
                }
            }
            #endregion
        }

    }
}