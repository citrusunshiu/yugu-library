using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

namespace YuguLibrary
{
    namespace Models
    {
        /// <summary>
        /// Base class for defining unit AI.
        /// </summary>
        public abstract class UnitAI
        {
            public UnitAI()
            {

            }


            #region Functions
            /// <summary>
            /// Gets a <see cref="UnitAIAction"/> object that may dynamically change dependent on the current game state.
            /// </summary>
            /// <returns>Returns a UnitAIAction object as determined by the function's logic.</returns>
            public abstract UnitAIAction GetUnitAIAction();
            #endregion
        }

        /// <summary>
        /// Contains functions that generate <see cref="UnitAIAction"/> objects for unit AI functionality.
        /// </summary>
        public static class UnitAILogic
        {
            public static UnitAIAction RandomMove(Unit unit)
            {
                UnitAIAction unitAIAction;
                return null;
            }
            public static UnitAIAction RandomSkill(Unit unit, SkillTypes skillType, Unit target)
            {
                UnitAIAction unitAIAction;
                return null;
            }

            public static UnitAIAction MoveToUnit(Unit unit, Unit target)
            {
                UnitAIAction unitAIAction;
                return null;
            }
            
            public static UnitAIAction Flee(Unit unit)
            {
                UnitAIAction unitAIAction;
                return null;
            }
            
            public static UnitAIAction UseSkill(Unit unit, Skill skill, Unit target)
            {
                UnitAIAction unitAIAction;
                return null;
            }
        }

        /// <summary>
        /// Sends commands for how a unit will move and what actions it will take.
        /// </summary>
        public class UnitAIAction
        {
            public UnitAIAction()
            {

            }
        }

    }
}