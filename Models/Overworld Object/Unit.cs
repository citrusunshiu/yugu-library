using System;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class Unit : OverworldObject
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
            /// The file path to the JSON containing the unit's data.
            /// </summary>
            private string unitJSONFilePath;

            /// <summary>
            /// The localized database ID containing the unit's displayed name.
            /// </summary>
            private string nameID;

            /// <summary>
            /// The unit's role type.
            /// </summary>
            /// <remarks>
            /// Descriptor for unit information; checked during certain targeting scenarios and damage calculations.
            /// </remarks>
            private UnitRoles role;

            /// <summary>
            /// The unit's classification type.
            /// </summary>
            /// <remarks>
            /// Descriptor for unit information; checked during certain targeting scenarios and damage calculations.
            /// </remarks>
            private UnitClassifications classification;

            /// <summary>
            /// The unit's target type.
            /// </summary>
            /// <remarks>
            /// Descriptor for unit information, and checked during certain targeting scenarios and calculations.
            /// </remarks>
            private TargetTypes targetType;
            
            /// <summary>
            /// Tracks additional skill resources possessed by the unit.
            /// </summary>
            /// <remarks>
            /// Value is an integer to store the amount of resource available. Certain skills may require special 
            /// resources in addition to or in place of MP.
            /// </remarks>
            private Dictionary<SkillResources, SkillResource> skillResources = new Dictionary<SkillResources, SkillResource>();

            #region Unit Stats
            /// <summary>
            /// The unit's HP scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's HP stat (<see cref="level"/> * hpScaling).
            /// </remarks>
            private float hpScaling;

            /// <summary>
            /// The unit's MP scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's MP stat (<see cref="level"/> * mpScaling).
            /// </remarks>
            private float mpScaling;

            /// <summary>
            /// The unit's MP Regen scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's MP Regen stat (<see cref="level"/> * mpRegenScaling).
            /// </remarks>
            private float mpRegenScaling;

            /// <summary>
            /// The unit's Physical Attack scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Physical Attack stat (<see cref="level"/> * physicalAttackScaling).
            /// </remarks>
            private float physicalAttackScaling;

            /// <summary>
            /// The unit's Magical Attack scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Magical Attack stat (<see cref="level"/> * magicalAttackScaling).
            /// </remarks>
            private float magicalAttackScaling;

            /// <summary>
            /// The unit's Physical Defense scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Physical Defense stat (<see cref="level"/> * physicalDefenseScaling).
            /// </remarks>
            private float physicalDefenseScaling;

            /// <summary>
            /// The unit's Magical Defense scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Magical Defense stat (<see cref="level"/> * magicalDefenseScaling).
            /// </remarks>
            private float magicalDefenseScaling;

            /// <summary>
            /// The unit's Speed scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Speed stat (<see cref="level"/> * speedScaling).
            /// </remarks>
            private float speedScaling;

            /// <summary>
            /// The unit's Stagger Threshold scaling.
            /// </summary>
            /// <remarks>
            /// Used to calculate the unit's Stagger Threshold stat (<see cref="level"/> * staggerThresholdScaling).
            /// </remarks>
            private float staggerThresholdScaling;

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

            /// <summary>
            /// The unit's Attack Speed stat.
            /// </summary>
            /// <remarks>
            /// Higher values decrease the animation time of certain skills. (100 attack speed = 2x speed)
            /// </remarks>
            public float attackSpeed;
            #endregion

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
            private Dictionary<string, HookFunction> hookFunctions = new Dictionary<string, HookFunction>();

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
            private bool isExecutingOverworldAIAction;

            private List<string> hitboxImmunities;

            #region Encounter-specific Variables
            /// <summary>
            /// The number of tiles outward a unit is legally allowed to move during their turn.
            /// </summary>
            private int baseMovementRadius;

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
            /// <summary>
            /// Creates a new unit instance from a JSON data file.
            /// </summary>
            /// <param name="unitJSONFileName">The name of the JSON file, relative to the 
            /// "Assets/Resources/JSON Assets/Units/" directory.</param>
            /// <param name="level">The unit's level.</param>
            /// <param name="targetType">The unit's targeting type.</param>
            public Unit(string unitJSONFileName, int level, TargetTypes targetType) : base()
            {
                UnitJSONParser unitJSONParser = new UnitJSONParser(unitJSONFileName);


                GameObject g = (GameObject)Resources.Load("Prefabs/Overworld Object");
                GameObject overworldObject = GameObject.Instantiate(g);

                overworldObjectCoordinator = overworldObject.GetComponent<OverworldObjectCoordinator>();
                overworldObjectCoordinator.overworldObject = this;

                this.level = level;
                this.targetType = targetType;

                InitializeUnitBaseValues(unitJSONParser);
                InitializeStats();
                InitializeUnitFunctionality(unitJSONParser);


            }
            #endregion

            #region Functions
            public void SetAnimation(int animationIndex, int framesGiven, bool isLooping)
            {
                overworldObjectCoordinator.SetAnimation(animationIndex, framesGiven, isLooping);
            }

            /// <summary>
            /// Modifies the unit's current HP using a value provided from a hit calculation.
            /// </summary>
            /// <param name="calculation">The hit calculation that will modify the unit's current HP.</param>
            public void AlterHP(HitCalculation calculation)
            {
                int alterAmount = calculation.GetDamageResult();
                
                currentHP -= alterAmount;
                
                if(alterAmount >= 0) //for damage
                {
                    if(currentHP <= 0) //is incapacitated
                    {

                    }
                    else
                    {
                        
                    }
                }
                else //for healing
                {
                    if (currentHP > hp)
                    {
                        currentHP = hp;
                    }
                }
            }

            /// <summary>
            /// Modifies the unit's aggro distribution using a value provided from a hit calculatiotn.
            /// </summary>
            /// <param name="calculation">The hit calculation that will modify the unit's aggro.</param>
            public void AlterAggro(HitCalculation calculation)
            {
                aggroSpread.InsertAggro(calculation.GetAttackingUnit(), calculation.GetAggroResult());
            }

            /// <summary>
            /// Applies a status effect to the unit.
            /// </summary>
            /// <param name="status">The status effect to be applied.</param>
            /// <returns>Returns true if the status applied successfully; returns false if it fails.</returns>
            public bool ApplyStatus(Status status)
            {
                status.SetAttachedUnit(this);
                statuses.Add(status.GetStatusID(), status);

                foreach(HookFunction hookFunction in status.GetHookFunctions())
                {
                    AddHookFunction(hookFunction);
                }

                return true;
            }

            /// <summary>
            /// Removes a specified <see cref="Status"/> from the unit's <see cref="statuses"/> attribute.
            /// </summary>
            /// <param name="statusType">Enum value of the status to remove.</param>
            /// <returns>Returns true if the status was removed successfully; returns false if it fails.</returns>
            /// <seealso cref="Ailments"/> <seealso cref="Impairments"/> 
            /// <seealso cref="BeneficialEffects"/> <seealso cref="SkillEffects"/>
            public bool RemoveStatus(StatusEffects statusEffect)
            {
                Status status = statuses[statusEffect];

                foreach(HookFunction hookFunction in status.GetHookFunctions())
                {
                    RemoveHookFunction(hookFunction.GetHookFunctionID());
                }

                statuses.Remove(statusEffect);

                return true;
            }

            /// <summary>
            /// Searches for all hook functions of a given delegate flag, and executes their logic.
            /// </summary>
            /// <param name="flag">The delegate flag to search for.</param>
            /// <param name="hookBundle">The hook bundle to pass into the hook functions.</param>
            public void ExecuteDelegates(DelegateFlags flag, HookBundle hookBundle)
            {
                foreach(HookFunction hookFunction in hookFunctions.Values)
                {
                    if (hookFunction.GetDelegateType() == flag)
                    {
                        hookFunction.ExecuteFunction(hookBundle);
                    }
                }
            }

            /// <summary>
            /// Adds a skill resource to the unit.
            /// </summary>
            /// <param name="resource">The skill resource</param>
            public void AddSkillResource(SkillResources resourceValue, SkillResource resource)
            {
                skillResources.Add(resourceValue, resource);
            }

            /// <summary>
            /// Adds a skill to a unit, and links the unit to the skill.
            /// </summary>
            /// <param name="skill">The skill to be added to and linked with the unit.</param>
            public void AddSkill(Skill skill)
            {
                skills.Add(skill);
                skill.AttachSkillToUnit(this);
            }

            /// <summary>
            /// Adds an overworld AI pattern to the unit.
            /// </summary>
            /// <param name="overworldAI">The overworld AI pattern to be added.</param>
            public void AddOverworldAI(OverworldAI overworldAI)
            {
                overworldAIs.Add(overworldAI);
            }

            /// <summary>
            /// Adds an encounter AI pattern to the unit.
            /// </summary>
            /// <param name="encounterAI">The encounter AI pattern to be added.</param>
            public void AddEncounterAI(EncounterAI encounterAI)
            {
                encounterAIs.Add(encounterAI);
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

            public TargetTypes GetTargetType()
            {
                return targetType;
            }

            public void AddHitboxImmunity(string hitboxGroupID)
            {
                hitboxImmunities.Add(hitboxGroupID);
            }

            public bool SearchHitboxImmunity(string hitboxGroupID)
            {
                return hitboxImmunities.Contains(hitboxGroupID);
            }

            /// <summary>
            /// Sets the values for the unit using information from the unit's JSON data.
            /// </summary>
            /// <param name="unitJSONParser">The JSON parser containing the unit's data.</param>
            private void InitializeUnitBaseValues(UnitJSONParser unitJSONParser)
            {
                nameID = unitJSONParser.GetNameID();
                animationScript = unitJSONParser.GetAnimationScript();
                overworldObjectCoordinator.AttachAnimationScript(animationScript);
                role = unitJSONParser.GetRole();
                classification = unitJSONParser.GetClassification();
                speedTier = unitJSONParser.GetSpeedTier();

                hpScaling = unitJSONParser.GetHPScaling();
                mpScaling = unitJSONParser.GetMPScaling();
                mpRegenScaling = unitJSONParser.GetMPRegenScaling();
                physicalAttackScaling = unitJSONParser.GetPhysicalAttackScaling();
                magicalAttackScaling = unitJSONParser.GetMagicalAttackScaling();
                physicalDefenseScaling = unitJSONParser.GetPhysicalDefenseScaling();
                magicalDefenseScaling = unitJSONParser.GetMagicalDefenseScaling();
                staggerThresholdScaling = unitJSONParser.GetStaggerThresholdScaling();
                speedScaling = unitJSONParser.GetSpeedScaling();

                List<SkillResource> skillResources = unitJSONParser.GetSkillResources();

                foreach (SkillResource container in skillResources)
                {
                    AddSkillResource(container.GetResourceValue(), container);
                }
            }

            /// <summary>
            /// Sets the unit's stats, dependent on its level, target type and stat scaling.
            /// </summary>
            private void InitializeStats()
            {
                hp = level * (int)hpScaling;
                if (targetType == TargetTypes.Enemy)
                {
                    hp *= 1000;
                }

                mp = level * (int)mpScaling;

                currentHP = hp;
                currentMP = mp;


                mpRegen = level * (int)mpRegenScaling;
                physicalAttack = level * (int)physicalAttackScaling;
                magicalAttack = level * (int)magicalAttackScaling;
                physicalDefense = level * (int)physicalDefenseScaling;
                magicalDefense = level * (int)magicalDefenseScaling;
                speed = level * (int)speedScaling;
                staggerThreshold = level * (int)staggerThresholdScaling;
            }

            /// <summary>
            /// Attaches skills, actions, and AI to the unit using informatiotn from the unit's JSON data.
            /// </summary>
            /// <param name="unitJSONParser">The JSON parser containing the unit's data.</param>
            private void InitializeUnitFunctionality(UnitJSONParser unitJSONParser)
            {
                List<Skill> skills = unitJSONParser.GetSkills();
                foreach (Skill skill in skills)
                {
                    AddSkill(skill);
                }

                List<OverworldObjectAction> actions = unitJSONParser.GetActions();
                foreach (OverworldObjectAction action in actions)
                {
                    AddOverworldObjectAction(action);
                }

                List<OverworldAI> overworldAIs = unitJSONParser.GetOverworldAIs();
                foreach (OverworldAI overworldAI in overworldAIs)
                {
                    AddOverworldAI(overworldAI);
                }

                List<EncounterAI> encounterAIs = unitJSONParser.GetEncounterAIs();
                foreach (EncounterAI encounterAI in encounterAIs)
                {
                    AddEncounterAI(encounterAI);
                }
            }

            /// <summary>
            /// Adds a hook function to the unit.
            /// </summary>
            /// <param name="hookFunction">The hook function to be added.</param>
            private void AddHookFunction(HookFunction hookFunction)
            {
                hookFunctions.Add(hookFunction.GetHookFunctionID(), hookFunction);
                hookFunction.SetAttachedUnit(this);
                hookFunction.CheckApplicationEffect();

            }

            /// <summary>
            /// Removes a hook function from the unit.
            /// </summary>
            /// <param name="hookFunctionId">The hook function to be removed.</param>
            private void RemoveHookFunction(string hookFunctionId)
            {
                HookFunction hookFunction = hookFunctions[hookFunctionId];
                hookFunction.CheckRemovalEffect();
                hookFunctions.Remove(hookFunctionId);
            }
            #endregion
        }

        public class SkillResource
        {
            SkillResources resource;
            int maxValue;
            int currentValue;

            public SkillResource(SkillResources resource, int maxValue)
            {
                this.resource = resource;
                this.maxValue = maxValue;
            }

            public SkillResources GetResourceValue()
            {
                return resource;
            }

            public int GetMaxValue()
            {
                return maxValue;
            }

            public int GetCurrentValue()
            {
                return currentValue;
            }
        }
    }
}
