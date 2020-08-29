using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuguLibrary
{
    namespace Models
    {
        public class UnitTurn
        {
            #region Variables
            /// <summary>
            /// The unit of the associated turn.
            /// </summary>
            public Unit unit;

            /// <summary>
            /// Whether the unit's turn was granted by priority.
            /// </summary>
            public bool isPriority;

            /// <summary>
            /// Whether the unit is able to act on their turn.
            /// </summary>
            bool isDebilitated;

            /// <summary>
            /// Whether the unit's turn is currently being executed.
            /// </summary>
            public bool isActive;

            /// <summary>
            /// Whether the unit's turn has been completed.
            /// </summary>
            public bool isCompleted;

            /// <summary>
            /// Whether the unit's primary action is available for use.
            /// </summary>
            /// <remarks>
            /// Units may only use one skill with the <see cref="Enumerations.SkillTypes.Primary"/> attribute per turn, 
            /// classified as a primary action.
            /// </remarks>
            public bool primaryAvailable = true;

            /// <summary>
            /// Whether the unit's auxiliary action is available for use.
            /// </summary>
            /// <remarks>
            /// Units may only use one skill with the <see cref="Enumerations.SkillTypes.Auxiliary"/> attribute per turn, 
            /// classified as a auxiliary action.
            /// </remarks>
            public bool auxiliaryAvailable = true;

            /// <summary>
            /// Whether the unit's movement action is available for use.
            /// </summary>
            /// <remarks>
            /// Units may only use one skill with the <see cref="Enumerations.SkillTypes.Movement"/> attribute per turn, 
            /// classified as a movement action.
            /// </remarks>
            public bool movementAvailable = true;
            #endregion

            #region Constructors
            public UnitTurn(Unit unit)
            {
                this.unit = unit;
            }

            public UnitTurn(Unit unit, bool isPriority)
            {
                this.unit = unit;
                this.isPriority = isPriority;
            }
            #endregion

            #region Functions
            /// <summary>
            /// Resets the state of the <see cref="UnitTurn"/> to defaults.
            /// </summary>
            public void Reset()
            {
                primaryAvailable = true;
                auxiliaryAvailable = true;
                movementAvailable = true;
                isCompleted = false;
            }
            #endregion
        }
    }
}
