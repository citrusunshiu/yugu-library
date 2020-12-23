using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

namespace YuguLibrary
{
    namespace Models
    {
        public abstract class UnitAI
        {
            public UnitAI()
            {

            }

            public abstract UnitAIAction GetUnitAIAction();

            #region Functions

            #endregion
        }

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