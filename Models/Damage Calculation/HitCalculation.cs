using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class HitCalculation
        {
            /// <summary>
            /// The unit delivering the hit.
            /// </summary>
            private Unit attackingUnit;

            /// <summary>
            /// The hit used for calculation.
            /// </summary>
            private Hit attackingHit;

            /// <summary>
            /// The unit being affected by the hit.
            /// </summary>
            private Unit defendingUnit;

            /// <summary>
            /// The value multipliers on the attacking hit.
            /// </summary>
            private Dictionary<string, float> modifiers;

            /// <summary>
            /// The statuses to be applied to the defending unit.
            /// </summary>
            /// <remarks>
            /// Key is the JSON file name of the status; value is the duration of the status.
            /// </remarks>
            private Dictionary<string, int> statuses;

            /// <summary>
            /// The value to modify the target unit's HP by, based on the calculation.
            /// </summary>
            private int damageResult;

            /// <summary>
            /// The value to add to the target unit's <see cref="AggroSpread"/>, based on the calculation.
            /// </summary>
            private float aggroResult;

            public HitCalculation(Unit attackingUnit, Hit attackingHit, Unit defendingUnit)
            {
                this.attackingUnit = attackingUnit;
                this.attackingHit = attackingHit;
                this.defendingUnit = defendingUnit;

                modifiers = CheckDamageModifiers();
                statuses = CheckStatuses();

                damageResult = CalculateDamage();
                aggroResult = CalculateAggro();
            }

            public void ApplyHitCalculation()
            {
                defendingUnit.AlterHP(this);
                defendingUnit.AlterAggro(this);
            }

            public Unit GetAttackingUnit()
            {
                return attackingUnit;
            }

            public Unit GetDefendingUnit()
            {
                return defendingUnit;
            }

            public int GetDamageResult()
            {
                return damageResult;
            }

            public float GetAggroResult()
            {
                return aggroResult;
            }

            private int CalculateDamage()
            {
                int attack, defense;

                //checking if calc should use physical or magical stats
                if (attackingHit.CheckAttribute(HitAttributes.Physical))
                {
                    attack = attackingUnit.physicalAttack;
                    defense = defendingUnit.physicalDefense;
                }
                else
                {
                    attack = attackingUnit.magicalAttack;
                    defense = defendingUnit.magicalDefense;
                }

                //setting modifier to make enemies take more damage
                int allyModifier = (defendingUnit.GetTargetType() == TargetTypes.Enemy) ? 1 : 1000;

                //damage calculation
                int damage = Mathf.RoundToInt((attackingHit.GetHitModifier() * ((1F + attack) / (1f + defense))) / allyModifier);



                //applying all modifiers
                foreach (float modifier in modifiers.Values)
                {
                    damage = Mathf.RoundToInt(damage * modifier);
                }

                return damage;
            }

            private Dictionary<string, int> CheckStatuses()
            {
                Dictionary<string, int> statuses = new Dictionary<string, int>();

                if (attackingHit.statuses != null)
                {
                    foreach (string status in attackingHit.statuses.Keys)
                    {
                        Debug.Log("rolling for status: " + status.GetType());
                        if (UtilityFunctions.GetRandomPercentage(attackingHit.statuses[status]))
                        {
                            Debug.Log("success; status applied");
                            statuses.Add(status, 5);
                        }
                        else
                        {
                            Debug.Log("fail; status not applied");
                        }
                    }
                }

                return statuses;
            }

            private Dictionary<string, float> CheckDamageModifiers()
            {
                Dictionary<string, float> modifiers = new Dictionary<string, float>();

                return modifiers;
            }

            /// <summary>
            /// Calculates the aggro to be generated from the hit.
            /// </summary>
            /// <returns>Returns the hit's aggro value.</returns>
            private float CalculateAggro()
            {   
                return attackingHit.GetAggroModifier() * (float)Math.Pow(damageResult, 2);
            }

        }
    }
}
