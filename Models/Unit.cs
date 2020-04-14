using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;

namespace YuguLibrary
{
    namespace Models
    {
        public abstract class Unit : OverworldObject
        {
            #region Variables
            /// <summary>
            /// Placeholder values for unit stats; will likely be tweaked in the future for game balance.
            /// </summary>
            #region Stat Scaling Values
            public static readonly float HP_SCALING_VERYHIGH = 40;
            public static readonly float HP_SCALING_HIGH = 30;
            public static readonly float HP_SCALING_AVERAGE = 20;
            public static readonly float HP_SCALING_LOW = 13.3F;
            public static readonly float HP_SCALING_VERYLOW = 10;

            public static readonly float MP_SCALING_VERYHIGH = 40;
            public static readonly float MP_SCALING_HIGH = 30;
            public static readonly float MP_SCALING_AVERAGE = 20;
            public static readonly float MP_SCALING_LOW = 13.3F;
            public static readonly float MP_SCALING_VERYLOW = 10;

            public static readonly float MPREGEN_SCALING_VERYHIGH = 4;
            public static readonly float MPREGEN_SCALING_HIGH = 3;
            public static readonly float MPREGEN_SCALING_AVERAGE = 2;
            public static readonly float MPREGEN_SCALING_LOW = 1.33F;
            public static readonly float MPREGEN_SCALING_VERYLOW = 1;

            public static readonly float PHYSICALATTACK_SCALING_VERYHIGH = 10;
            public static readonly float PHYSICALATTACK_SCALING_HIGH = 7.5F;
            public static readonly float PHYSICALATTACK_SCALING_AVERAGE = 5;
            public static readonly float PHYSICALATTACK_SCALING_LOW = 3.33F;
            public static readonly float PHYSICALATTACK_SCALING_VERYLOW = 2.5F;

            public static readonly float MAGICALATTACK_SCALING_VERYHIGH = 10;
            public static readonly float MAGICALATTACK_SCALING_HIGH = 7.5F;
            public static readonly float MAGICALATTACK_SCALING_AVERAGE = 5;
            public static readonly float MAGICALATTACK_SCALING_LOW = 3.33F;
            public static readonly float MAGICALATTACK_SCALING_VERYLOW = 2.5F;

            public static readonly float PHYSICALDEFENSE_SCALING_VERYHIGH = 6;
            public static readonly float PHYSICALDEFENSE_SCALING_HIGH = 4.5F;
            public static readonly float PHYSICALDEFENSE_SCALING_AVERAGE = 3;
            public static readonly float PHYSICALDEFENSE_SCALING_LOW = 2;
            public static readonly float PHYSICALDEFENSE_SCALING_VERYLOW = 1.5F;

            public static readonly float MAGICALDEFENSE_SCALING_VERYHIGH = 6;
            public static readonly float MAGICALDEFENSE_SCALING_HIGH = 4.5F;
            public static readonly float MAGICALDEFENSE_SCALING_AVERAGE = 3;
            public static readonly float MAGICALDEFENSE_SCALING_LOW = 2;
            public static readonly float MAGICALDEFENSE_SCALING_VERYLOW = 1.5F;

            public static readonly float SPEED_SCALING_VERYHIGH = 8;
            public static readonly float SPEED_SCALING_HIGH = 6;
            public static readonly float SPEED_SCALING_AVERAGE = 4;
            public static readonly float SPEED_SCALING_LOW = 2.67F;
            public static readonly float SPEED_SCALING_VERYLOW = 2;

            public static readonly float STAGGERTHRESHOLD_SCALING_VERYHIGH = 10;
            public static readonly float STAGGERTHRESHOLD_SCALING_HIGH = 7.5F;
            public static readonly float STAGGERTHRESHOLD_SCALING_AVERAGE = 5;
            public static readonly float STAGGERTHRESHOLD_SCALING_LOW = 3.33F;
            public static readonly float STAGGERTHRESHOLD_SCALING_VERYLOW = 2.5F;
            #endregion

            /// <summary>
            /// The unit's displayed name.
            /// </summary>
            public abstract string Name { get; }
            
            /// <summary>
            /// The unit's role type.
            /// </summary>
            /// <remarks>
            /// Descriptor for unit information; checked during certain targeting scenarios and damage calculations.
            /// </remarks>
            public abstract UnitRoles Role { get; }

            /// <summary>
            /// The unit's classification type.
            /// </summary>
            /// <remarks>
            /// Descriptor for unit information; checked during certain targeting scenarios and damage calculations.
            /// </remarks>
            public abstract UnitClassifications Classification { get; }

            /// <summary>
            /// The unit's target type.
            /// </summary>
            /// <remarks>
            /// Descriptor for unit information, and checked during certain targeting scenarios and calculations.
            /// </remarks>
            public TargetTypes targetType;
            
            /// <summary>
            /// Tracks additional skill resources possessed by the unit.
            /// </summary>
            /// <remarks>
            /// Value is an integer to store the amount of resource available. Certain skills may require special 
            /// resources in addition to or in place of MP.
            /// </remarks>
            private Dictionary<SkillResources, int> specialResources = new Dictionary<SkillResources, int>();
            
            /// <summary>
            /// The list of attributes that the unit currently has.
            /// </summary>
            /// <remarks>
            /// Size is equivalent to the length of <see cref="UnitAttributes"/>. Toggle an attribute’s index 
            /// value to true to apply the respective attribute.
            /// </remarks>
            private bool[] attributes = new bool[Enum.GetNames(typeof(UnitAttributes)).Length];

            /// <summary>
            /// Tracks the unit's special interactions with skill attributes.
            /// </summary>
            /// <remarks>
            /// Used to calculate damage modifiers during <see cref="EncounterController.CollectMiscellaneousModifiers()"/>.
            /// </remarks>
            private Dictionary<HitAttributes, Effectiveness> skillInteractions = new Dictionary<HitAttributes, Effectiveness>();

            /// <summary>
            /// Tracks the unit's current statuses.
            /// </summary>
            private Dictionary<StatusEffects, Status> statuses = new Dictionary<StatusEffects, Status>();

            /// <summary>
            /// List of the skills that the unit has access to.
            /// </summary>
            private List<Skill> skills = new List<Skill>();

            /// <summary>
            /// Tracks the unit's aggro towards enemy units.
            /// </summary>
            private AggroSpread aggroSpread = new AggroSpread();

            /// <summary>
            /// Tracks the unit's current resistance to impairment-type effects.
            /// </summary>
            private CCResistanceBar ccResistanceBar;

            /// <summary>
            /// List of <see cref="HookFunction"/> objects to search through and execute during specific 
            /// <see cref="DelegateFlags"/> events.
            /// </summary>
            private List<HookFunction> hookFunctions = new List<HookFunction>();

            /// <summary>
            /// List of <see cref="OverworldAI"/> objects currently attached to the unit.
            /// </summary>
            /// <remarks>
            /// Determines how the unit will interact with other units and tiles in the overworld.
            /// </remarks>
            private List<OverworldAI> overworldAIs = new List<OverworldAI>();

            /// <summary>
            /// The unit's current <see cref="OverworldAIAction"/>.
            /// </summary>
            /// <remarks>
            /// When determined, the unit will execute the action when they are free.
            /// </remarks>
            private OverworldAIAction currentOverworldAIAction;

            /// <summary>
            /// Whether or not the unit is currently executing an <see cref="OverworldAIAction"/>.
            /// </summary>
            private bool isExecutingOverworldAIAction = false;

            private List<OverworldSkill> overworldSkills = new List<OverworldSkill>();

            #region Unit Stats
            /// <summary>
            /// The unit's HP scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's HP stat (<see cref="level"/> * hpScaling).
            /// </remarks>
            protected abstract float HPScaling { get; }

            /// <summary>
            /// The unit's MP scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's MP stat (<see cref="level"/> * mpScaling).
            /// </remarks>
            protected abstract float MPScaling { get; }

            /// <summary>
            /// The unit's MP Regen scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's MP Regen stat (<see cref="level"/> * mpRegenScaling).
            /// </remarks>
            protected abstract float MPRegenScaling { get; }

            /// <summary>
            /// The unit's Physical Attack scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Physical Attack stat (<see cref="level"/> * physicalAttackScaling).
            /// </remarks>
            protected abstract float PhysicalAttackScaling { get; }

            /// <summary>
            /// The unit's Magical Attack scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Magical Attack stat (<see cref="level"/> * magicalAttackScaling).
            /// </remarks>
            protected abstract float MagicalAttackScaling { get; }

            /// <summary>
            /// The unit's Physical Defense scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Physical Defense stat (<see cref="level"/> * physicalDefenseScaling).
            /// </remarks>
            protected abstract float PhysicalDefenseScaling { get; }

            /// <summary>
            /// The unit's Magical Defense scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Magical Defense stat (<see cref="level"/> * magicalDefenseScaling).
            /// </remarks>
            protected abstract float MagicalDefenseScaling { get; }

            /// <summary>
            /// The unit's Speed scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Speed stat (<see cref="level"/> * speedScaling).
            /// </remarks>
            protected abstract float SpeedScaling { get; }

            /// <summary>
            /// The unit's Stagger Threshold scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Stagger Threshold stat (<see cref="level"/> * staggerThresholdScaling).
            /// </remarks>
            protected abstract float StaggerThresholdScaling { get; }

            /// <summary>
            /// The unit's level.
            /// </summary>
            /// <remarks>
            /// Used to scale the unit’s <see cref="hp"/>, <see cref="mp"/>, <see cref="mpRegen"/>, 
            /// <see cref="physicalAttack"/>, <see cref="magicalAttack"/>, <see cref="physicalDefense"/>, and 
            /// <see cref="magicalDefense"/> stats.
            /// </remarks>
            public int level = 1;

            /// <summary>
            /// The unit's max HP stat.
            /// </summary>
            /// <remarks>
            /// Value cannot drop below 0.
            /// </remarks>
            public int hp;

            /// <summary>
            /// The unit's current HP.
            /// </summary>
            /// <remarks>
            /// Value is initialized to be equal to <see cref="hp"/>. Units lose HP whenever they take damage from hits. 
            /// If their currentHP stat becomes 0, they are inflicted with <see cref="Statuses.Incapacitated"/>.
            /// </remarks>
            public int currentHP;

            /// <summary>
            /// The unit's max MP stat.
            /// </summary>
            /// <remarks>
            /// Value cannot drop below 0.
            /// </remarks>
            public int mp;

            /// <summary>
            /// The unit's current MP.
            /// </summary>
            /// <remarks>
            /// Value is initialized to be equal to <see cref="mp"/>. Units spend MP whenever they use skills that require it. 
            /// Units cannot use skills or actions that require more MP than they currently have.
            /// </remarks>
            public int currentMP;

            /// <summary>
            /// The unit's MP Regen stat.
            /// </summary>
            /// <remarks>
            /// Units restore an amount of <see cref="currentMP"/> equivalent to their mpRegen stat at the beginning of their 
            /// turn. A unit’s currentMP stat cannot exceed its mp stat.
            /// </remarks>
            public int mpRegen;

            /// <summary>
            /// The unit's Physical Attack stat.
            /// </summary>
            /// <remarks>
            /// Used for <see cref="EncounterController.CalculateDamage(Skill, OverworldObject, OverworldObject)"/> when the unit attacks with a 
            /// Physical-type hit. Stat cannot drop below 0.
            /// </remarks>
            public int physicalAttack;

            /// <summary>
            /// The unit's Magical Attack stat.
            /// </summary>
            /// <remarks>
            /// Used for <see cref="EncounterController.CalculateDamage(Skill, OverworldObject, OverworldObject)"/> when the unit attacks with a 
            /// Magical-type hit. Stat cannot drop below 0.
            /// </remarks>
            public int magicalAttack;

            /// <summary>
            /// The unit's Physical Defense stat.
            /// </summary>
            /// <remarks>
            /// Used for <see cref="EncounterController.CalculateDamage(Skill, OverworldObject, OverworldObject)"/> when the unit is attacked by a 
            /// Physical-type hit. Stat cannot drop below 0.
            /// </remarks>
            public int physicalDefense;

            /// <summary>
            /// The unit's Magical Defense stat.
            /// </summary>
            /// <remarks>
            /// Used for <see cref="EncounterController.CalculateDamage(Skill, OverworldObject, OverworldObject)"/> when the unit is attacked by a 
            /// Magical-type hit. Stat cannot drop below 0.
            /// </remarks>
            public int magicalDefense;

            /// <summary>
            /// The unit's chance to achieve a critical hit.
            /// </summary>
            /// <remarks>
            /// Set to 0 by default; used in <see cref="EncounterController.RollCritical(OverworldObject, OverworldObject)"/>.
            /// </remarks>
            public int criticalRate;

            /// <summary>
            /// The unit's chance to defend against a critical hit.
            /// </summary>
            /// <remarks>
            /// Set to 0 by default; used in <see cref="EncounterController.RollCritical(OverworldObject, OverworldObject)"/>. Subtracts from the
            /// attacking unit's <see cref="criticalRate"/> stat during the critical hit calculation.
            /// </remarks>
            public int criticalResistance;

            /// <summary>
            /// The unit's chance to connect a hit on an enemy.
            /// </summary>
            /// <remarks>
            /// Set to 100 by default; used in <see cref="EncounterController.RollDodge(OverworldObject, OverworldObject)"/>.
            /// </remarks>
            public int accuracy;

            /// <summary>
            /// The unit's chance to evade a hit from an enemy.
            /// </summary>
            /// <remarks>
            /// Set to 0 by default; used in <see cref="EncounterController.RollDodge(OverworldObject, OverworldObject)"/>. Subtracts from the 
            /// attacking unit's accuracy stat during dodge calculation.
            /// </remarks>
            public int evasion;

            /// <summary>
            /// The unit's Speed stat.
            /// </summary>
            /// <remarks>
            /// Determines the unit's place in the encounter turn order.
            /// </remarks>
            public int speed;

            /// <summary>
            /// The unit's Stagger Threshold stat.
            /// </summary>
            /// <remarks>
            /// The minimum amount of damage that the unit must be hit with to become inflicted with 
            /// <see cref="Statuses.Stagger"/>.
            /// </remarks>
            public int staggerThreshold;

            /// <summary>
            /// Thye unit's Fire Damage stat.
            /// </summary>
            /// <remarks>
            /// Multiplies the damage of Fire-type hits from the unit during 
            /// <see cref="EncounterController.CalculateElement"/>. Scales at a rate of n per +100 Fire Damage, and 
            /// 1/n per -100 Fire Damage.
            /// </remarks>
            public int fireDamage;

            /// <summary>
            /// Thye unit's Ice Damage stat.
            /// </summary>
            /// <remarks>
            /// Multiplies the damage of Ice-type hits from the unit during 
            /// <see cref="EncounterController.CalculateElement"/>. Scales at a rate of n per +100 Ice Damage, and 
            /// 1/n per -100 Ice Damage.
            /// </remarks>
            public int iceDamage;

            /// <summary>
            /// Thye unit's Wind Damage stat.
            /// </summary>
            /// <remarks>
            /// Multiplies the damage of Wind-type hits from the unit during 
            /// <see cref="EncounterController.CalculateElement"/>. Scales at a rate of n per +100 Wind Damage, and 
            /// 1/n per -100 Wind Damage.
            /// </remarks>
            public int windDamage;

            /// <summary>
            /// Thye unit's Earth Damage stat.
            /// </summary>
            /// <remarks>
            /// Multiplies the damage of Earth-type hits from the unit during 
            /// <see cref="EncounterController.CalculateElement"/>. Scales at a rate of n per +100 Earth Damage, and 
            /// 1/n per -100 Earth Damage.
            /// </remarks>
            public int earthDamage;

            /// <summary>
            /// Thye unit's Electric Damage stat.
            /// </summary>
            /// <remarks>
            /// Multiplies the damage of Electric-type hits from the unit during 
            /// <see cref="EncounterController.CalculateElement"/>. Scales at a rate of n per +100 Electric Damage, and 
            /// 1/n per -100 Electric Damage.
            /// </remarks>
            public int electricDamage;

            /// <summary>
            /// Thye unit's Water Damage stat.
            /// </summary>
            /// <remarks>
            /// Multiplies the damage of Water-type hits from the unit during 
            /// <see cref="EncounterController.CalculateElement"/>. Scales at a rate of n per +100 Water Damage, and 
            /// 1/n per -100 Water Damage.
            /// </remarks>
            public int waterDamage;

            /// <summary>
            /// The unit's Fire Resistance stat.
            /// </summary>
            /// <remarks>
            /// Subtracts from an attacker’s Fire Damage stat while defending against Fire-type hits.
            /// </remarks>
            public int fireResistance;

            /// <summary>
            /// The unit's Ice Resistance stat.
            /// </summary>
            /// <remarks>
            /// Subtracts from an attacker’s Ice Damage stat while defending against Ice-type hits.
            /// </remarks>
            public int iceResistance;

            /// <summary>
            /// The unit's Wind Resistance stat.
            /// </summary>
            /// <remarks>
            /// Subtracts from an attacker’s Wind Damage stat while defending against Wind-type hits.
            /// </remarks>
            public int windResistance;

            /// <summary>
            /// The unit's Earth Resistance stat.
            /// </summary>
            /// <remarks>
            /// Subtracts from an attacker’s Earth Damage stat while defending against Earth-type hits.
            /// </remarks>
            public int earthResistance;

            /// <summary>
            /// The unit's Electric Resistance stat.
            /// </summary>
            /// <remarks>
            /// Subtracts from an attacker’s Electric Damage stat while defending against Electric-type hits.
            /// </remarks>
            public int electricResistance;

            /// <summary>
            /// The unit's Water Resistance stat.
            /// </summary>
            /// <remarks>
            /// Subtracts from an attacker’s Water Damage stat while defending against Water-type hits.
            /// </remarks>
            public int waterResistance;
            #endregion

            #region Encounter-specific Variables
            /// <summary>
            /// The number of tiles outward a unit is legally allowed to move during their turn.
            /// </summary>
            public abstract int BaseMovementRadius { get; }

            /// <summary>
            /// List of <see cref="EncounterAI"/> scripts currently attached to the unit.
            /// </summary>
            /// <remarks>
            /// Determines how the unit will prioritize skill usage and targets during an encounter.
            /// </remarks>
            private List<EncounterAI> encounterAIs = new List<EncounterAI>();

            /// <summary>
            /// The unit's current <see cref="EncounterAIAction"/>.
            /// </summary>
            /// <remarks>
            /// When determined, the unit will execute the action on their turn.
            /// </remarks>
            private EncounterAIAction currentEncounterAIAction;
            #endregion
            #endregion

            #region Constructors
            public Unit(int level, TargetTypes targetType) : base()
            {
                GameObject g = (GameObject)Resources.Load("Prefabs/Overworld Object");
                GameObject overworldObject = GameObject.Instantiate(g);

                overworldObjectCoordinator = overworldObject.GetComponent<OverworldObjectCoordinator>();
                overworldObjectCoordinator.overworldObject = this;
                overworldObjectCoordinator.AttachAnimationScript(AnimationScript);

                this.level = level;
                this.targetType = targetType;

                ccResistanceBar = new CCResistanceBar(this);
                InitializeUnitStats();
                InitializeOverworldSkills();
                InitializeSkills();
                InitializeOverworldAIs();
                InitializeEncounterAIs();
            }
            #endregion

            #region Functions
            protected abstract void InitializeOverworldSkills();
            protected abstract void InitializeSkills();
            protected abstract void InitializeOverworldAIs();
            protected abstract void InitializeEncounterAIs();
            
            protected void InitializeUnitStats()
            {
                hp = level * (int)HPScaling;
                if (targetType == TargetTypes.Enemy)
                {
                    hp *= 1000;
                }

                mp = level * (int)MPScaling;

                currentHP = hp;
                currentMP = mp;


                mpRegen = level * (int)MPRegenScaling;
                physicalAttack = level * (int)PhysicalAttackScaling;
                magicalAttack = level * (int)MagicalAttackScaling;
                physicalDefense = level * (int)PhysicalDefenseScaling;
                magicalDefense = level * (int)MagicalDefenseScaling;
                speed = level * (int)SpeedScaling;
                staggerThreshold = level * (int)StaggerThresholdScaling;
            }

            /// <summary>
            /// Changes specified index in <see cref="attributes"/> to true.
            /// </summary>
            /// <param name="attribute"><see cref="UnitAttributes"/> value to apply.</param>
            public void ApplyAttribute(UnitAttributes attribute)
            {
                attributes[(int)attribute] = true;
            }

            /// <summary>
            /// Changes specified index in <see cref="attributes"/> to false.
            /// </summary>
            /// <param name="attribute"><see cref="UnitAttributes"/> value to remove.</param>
            public void RemoveAttribute(UnitAttributes attribute)
            {
                attributes[(int)attribute] = false;
            }

            /// <summary>
            /// Checks whether or not a given <see cref="UnitAttributes"/> exists on the unit.
            /// </summary>
            /// <param name="attribute">The UnitAttributes value to search for.</param>
            /// <returns>Returns true if the unit has the attribute, and false otherwise.</returns>
            public bool HasAttribute(UnitAttributes attribute)
            {
                return attributes[(int)attribute];
            }

            /// <summary>
            /// Adds a skill interaction to a unit.
            /// </summary>
            /// <param name="hitAttribute">The <see cref="HitAttributes"/> value to be added.</param>
            /// <param name="effectiveness">The <see cref="Effectiveness"/> value attributed to the HitAttribute value.</param>
            public void AddSkillInteraction(HitAttributes hitAttribute, Effectiveness effectiveness)
            {
                skillInteractions.Add(hitAttribute, effectiveness);
            }

            /// <summary>
            /// Checks if the unit has an interaction with the given <see cref="HitAttributes"/> value.
            /// </summary>
            /// <param name="hitAttribute">The HitAttributes value to check.</param>
            /// <returns>Returns true if a skill interaction exists with the HitAttributes value, and false otherwise.</returns>
            public bool SearchSkillInteractions(HitAttributes hitAttribute)
            {
                return skillInteractions.ContainsKey(hitAttribute);
            }

            /// <summary>
            /// Removes a given skill interaction from the unit.
            /// </summary>
            /// <param name="hitAttribute">The <see cref="HitAttributes"/> value to be removed.</param>
            public void RemoveSkillInteraction(HitAttributes hitAttribute)
            {
                skillInteractions.Remove(hitAttribute);
            }

            /// <summary>
            /// Gets the skill interaction for the given <see cref="HitAttributes"/> value.
            /// </summary>
            /// <param name="hitAttribute">The HitAttributes value to get the skill interaction for.</param>
            /// <returns>Returns an <see cref="Effectiveness"/> value dependent on the interaction with the HitAttributes value.</returns>
            public Effectiveness GetSkillInteraction(HitAttributes hitAttribute)
            {
                return skillInteractions[hitAttribute];
            }
            #endregion
        }
    }
}
