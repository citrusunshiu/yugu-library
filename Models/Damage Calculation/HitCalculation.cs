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
            private Unit attackingUnit;
            private Hit attackingHit;

            private Unit defendingUnit;

            private Dictionary<string, float> modifiers;
            private Dictionary<string, int> statuses;

            private int damageResult;
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

            private float CalculateAggro()
            {   
                return attackingHit.GetAggroModifier() * (float)Math.Pow(damageResult, 2);
            }

        }
    }
}
