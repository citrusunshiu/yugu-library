using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuguLibrary
{
    namespace Enumerations
    {
        //TODO+: add descriptions from game concept document to enums, likely; unitroles/unitclassifications
        
        /// <summary>
        /// List of all controller buttons.
        /// </summary>
        public enum ControllerInputs
        {
            /// <summary>
            /// PS4: Cross, XBONE: A, SWITCH: B
            /// </summary>
            BottomFace,

            /// <summary>
            /// PS4: Circle, XBONE: B, SWITCH: A
            /// </summary>
            RightFace,

            /// <summary>
            /// PS4: Triangle, XBONE: Y, SWITCH: X
            /// </summary>
            LeftFace,

            /// <summary>
            /// PS4: Square, XBONE: X, SWITCH: Y
            /// </summary>
            TopFace,

            /// <summary>
            /// PS4: L1, XBONE: LB, SWITCH: L
            /// </summary>
            LeftBumper,

            /// <summary>
            /// PS4: R1, XBONE: RB, SWITCH: R
            /// </summary>
            RightBumper,

            /// <summary>
            /// 
            /// </summary>
            Start,

            /// <summary>
            /// 
            /// </summary>
            Select,

            LeftAnalogPress,
            RightAnalogPress,

            LeftAnalogAxes,
            RightAnalogAxes,

            DPadAxes,

            /// <summary>
            /// PS4: L2, XBONE: RT, SWITCH: ZL
            /// </summary>
            LeftTrigger,

            /// <summary>
            /// PS4: R2, XBONE: LT, SWITCH: ZR
            /// </summary>
            RightTrigger,

            None
        }

        /// <summary>
        /// List of all possible UI states.
        /// </summary>
        public enum UIScreens
        {
            StartScreen,
            StartScreen_FileSelect,
            GameScreen,
            GameMenu,
            GameMenu_Characters,
            GameMenu_Character_Detail,
            GameMenu_Character_Detail_Skill,
            GameMenu_UnitFormation,
            GameMenu_Travel,
            GameMenu_Requests,
            GameMenu_Progression,
            EncounterScreen,
            EncounterScreen_SelectAction,
            EncounterScreen_SkillMenu,
            EncounterScreen_SelectTarget,
            EncounterScreen_ConfirmAction,
            EncounterScreen_UnitStatus,
            EncounterScreen_UnitStatus_Detail,
            CutsceneScreen,
            CutsceneScreen_Dialogue_None,
            CutsceneScreen_Dialogue_Left,
            CutsceneScreen_Dialogue_Right,
        }

        public enum UIButtonModes
        {
            None,
            MainMenu_UnitSetup,
            Encounter_TileCursor,
            Cutscene,

        }

        public enum AnimationTypes
        {
            Idle,
            Idle_Dance,
            Hostile_Idle,
            Hostile_Idle_Dance,
            Walk,
            Run,
            Jump_Rise,
            Jump_Fall,
            Weak,
            Hit,
            Hit_Strong,
            Attack_1,
            Attack_2,
            Buff,
            Incapacitation
        }

        /// <summary>
        /// Flags that the system alerts during certain events to allow delegate functions to run, usually for the purpose
        /// of dynamic skill and mechanic logic.
        /// </summary>
        /// <remarks>
        /// Skill and mechanic logic are defined through delegate functions, and attached to units using 
        /// <see cref="HookFunction"/> objects with a defining DelegateFlag. The logic runs when the system alerts their
        /// defined flag.
        /// </remarks>
        public enum DelegateFlags
        {
            OnEncounterRoundStart,

            OnEncounterTurnStart,

            OnEncounterBeforeAttack,

            OnEncounterBeforeDefend,

            OnEncounterCollectAttackerModifiers,

            OnEncounterCollectDefenderModifiers,

            OnEncounterAfterAttack,

            OnEncounterAfterDefend,

            OnStatusApplied,

            OnStatusRemoved,

            OnEncounterTurnEnd,

            OnEncounterRoundEnd,

            OnEncounterCollectCastingHealModifiers,

            OnEncounterCollectReceivingHealModifiers,

            OnEncounterIncapacitation,

            OnApply,

            OnRemove
        };

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Order is the same way that they should be rendered; lower numbers first, higher numbers last
        /// </remarks>
        public enum TileIndicatorTypes
        {
            Aggro,
            Encounter,
            Movement,
            AllyTargetingSkill,
            EnemyTargetingSkill,
            LoadingZone,
        }

        /// <summary>
        /// The facing direction of a Unit object.
        /// </summary>
        /// <remarks>
        /// Used for determining sprite facing directions and directional calculations for attacks, mechanics and 
        /// skill effects.
        /// </remarks>
        public enum Directions
        {
            NW,
            NE,
            SE,
            SW,
            Up,
            Down
        };

        /// <summary>
        /// List of overworld enemy AI types.
        /// </summary>
        public enum EnemyTypes
        {
            Aggressive,
            Passive,
            Ranged,
            Ambushing
        };

        /// <summary>
        /// List of all unit roles.
        /// </summary>
        /// <remarks>
        /// Descriptor for unit information, and checked during certain targeting scenarios and damage calculations.
        /// </remarks>
        public enum UnitRoles
        {
            Tank,
            Melee,
            Ranged,
            Support
        };

        /// <summary>
        /// List of all unit classification types.
        /// </summary>
        /// <remarks>
        /// Descriptor for unit information, and checked during certain targeting scenarios and damage calculations.
        /// </remarks>
        public enum UnitClassifications
        {
            Regular,
            Midboss,
            Boss,
            Formidable
        };

        /// <summary>
        /// List of all stats a unit has.
        /// </summary>
        public enum UnitStats
        {
            HP,
            MP,
            MPRegen,
            PhysicalAttack,
            MagicalAttack,
            PhysicalDefense,
            MagicalDefense,
            Speed,
            StaggerThreshold,
            CriticalRate,
            CriticalResistance,
            Accuracy,
            Evasion,
            FireDamage,
            IceDamage,
            WindDamage,
            ElectricDamage,
            EarthDamage,
            WaterDamage,
            FireResistance,
            IceResistance,
            WindResistance,
            ElectricResistance,
            EarthResistance,
            WaterResistance,
        }

        /// <summary>
        /// List of all possible unit movement speeds.
        /// </summary>
        public enum SpeedTiers
        {
            VeryFast = 4,
            Fast = 5,
            Normal = 6,
            Slow = 10,
            VerySlow = 13
        }

        /// <summary>
        /// Grants special traits to <see cref="Models.OverworldObject"/> objects.
        /// </summary>
        public enum UnitAttributes
        {
            /// <summary>
            /// Allows the unit to skip ground-based tile checks.
            /// </summary>
            /// <seealso cref="Controllers.UnitDetectorController.Move(Models.OverworldObject, int, int, int)"/>
            Airborne,

            /// <summary>
            /// Allows the unit to skip unit-based collision checks.
            /// </summary>
            /// <seealso cref="Controllers.UnitDetectorController.Move(Models.OverworldObject, int, int, int)"/>
            Ghost,

            /// <summary>
            /// Unit is unaffected by <see cref="Statuses.Poison"/>, <see cref="Statuses.Bleed"/>, 
            /// <see cref="Statuses.Sleep"/>, <see cref="Statuses.Charm"/>, <see cref="Statuses.Fear"/>, 
            /// and <see cref="Statuses.EssentiaExtortion"/>.
            /// </summary>
            Nonorganic,

            /// <summary>
            /// Allows the unit to reveal invisible units within a certain range.
            /// </summary>
            SpecialSight,

            /// <summary>
            /// Unit cannot be afflicted with any <see cref="Ailments"/>.
            /// </summary>
            StatusImmune,

            /// <summary>
            /// Unit cannot be afflicted with any <see cref="Impairments"/>, except 
            /// <see cref="StatusEffects.Incapacitation"/>. Certain skills may bypass this effect.
            /// </summary>
            ImpairmentImmune
        };

        /// <summary>
        /// List of all possible directions a unit can attack from.
        /// </summary>
        public enum AttackingDirections
        {
            Front,
            Side,
            Back
        }

        /// <summary>
        /// List of resources that a skill may require to be used.
        /// </summary>
        public enum SkillResources
        {
            HP,
            MP,
            MirageEssence,
            _gsFirearms,
            _gsIce,
            _gsFire,
            _gsEarth,
            _gsWind,
            _gsElectric,
            _impArts,
            _luneVitality,
            _luneEssentia
        };

        /// <summary>
        /// List of status type categories.
        /// </summary>
        public enum StatusTypes
        {
            Ailment,
            Impairment,
            Beneficial,
            UniqueHelpful,
            UniqueHarmful,
            Neutral,
            Innate
        };

        /// <summary>
        /// List of all status effects.
        /// </summary>
        public enum StatusEffects
        {
            TestStatus,

            /// <summary>
            /// See <see cref="Statuses.Chill"/>.
            /// </summary>
            Chill,
            Shock,
            Burn,
            Poison,
            Bleed,
            Zombify,
            Doom,
            Fear,

            Stagger,
            Stun,
            Knockback,
            Grab,
            Seal,
            Freeze,
            Paralysis,
            Sleep,
            Charm,
            Confusion,
            Blind,
            Terrify,
            TimeStop,
            EssentiaExtortion,
            Ringout,
            Incapacitation,

            SuperArmor,
            Endure,
            Intangible,
            DamageReduction,
            Invisible,

            WaterDouse,
            MelodyOfProtectionBeneficial,
            MelodyOfProtectionChannel,
            CounterStanceRetaliation,
            ReactionaryExplosion
        };

        /// <summary>
        /// List of all possible skill types.
        /// </summary>
        public enum SkillTypes
        {
            Overworld,
            /// <summary>
            /// Identifier for Primary-type skills. One can be used per <see cref="UnitTurn"/>.
            /// </summary>
            Primary,

            /// <summary>
            /// Identifier for Auxiliary-type skills. One can be used per <see cref="UnitTurn"/>.
            /// </summary>
            Auxiliary,

            /// <summary>
            /// Identifier for Innate-type skills. Their effects can be toggled on/off through a UI menu.
            /// </summary>
            Innate,

            /// <summary>
            /// Identifier for Movement-type skills. One can be used per <see cref="UnitTurn"/>.
            /// </summary>
            Movement,
            None
        };

        /// <summary>
        /// 
        /// </summary>
        public enum HitAttributes
        {
            Physical,
            Magical,
            Fire,
            Ice,
            Wind,
            Earth,
            Electric,
            Water,
            Melee,
            Ranged,
            Projectile,
            Spellcast,
            Grab,
            Execute,
            Resuscitate,
            Disjoint,
            Guard,
            Channel,
            Restorative,
            Cleanse
        };

        /// <summary>
        /// 
        /// </summary>
        public enum Effectiveness
        {
            Weakness,
            Resistance,
            Immunity,
            Absorption
        };

        /// <summary>
        /// 
        /// </summary>
        public enum EncounterTypes
        {
            Advantage,
            Neutral,
            Disadvantage
        };

        /// <summary>
        /// 
        /// </summary>
        public enum EncounterEndstates
        {
            Victory,
            Flee,
            Defeat
        };

        /// <summary>
        /// 
        /// </summary>
        public enum TileTypes
        {
            Neutral,
            Gap,
            Wall,
            Slope,
            Water,
            Puddle,
            Ice,
            Flower,
            CleansingPuddle,
            AbyssalWater,
            PoisonSlime
        };

        /// <summary>
        /// 
        /// </summary>
        public enum TargetTypes
        {
            Enemy,
            Ally,
            Any,

            /// <summary>
            /// Targeting type used if the skill has a unique way of targeting.
            /// </summary>
            Unique,
            Self,
            Tile,
            Neutral
        };

        /// <summary>
        /// think of aggro and targeting, buffs and disruptions; heals, statuses
        /// </summary>
        public enum AISkillCategories
        {
            Aggro,
            Buff,
            Disruption,
            Heal,
            Status,
            Summon,
            Utility,
            Offensive,
            Defensive,
            Movement
        };

        public enum RequestStatuses
        {
            Available,
            Ongoing,
            Complete
        }

        public enum RequestTypes
        {
            MainStory,
            Mercenary
        }

        public enum Requests
        {

        }

        public enum Provinces
        {
            Unapolis,
            Magicadia,
            Eclime,
            Machis,
            Tetralight,
            Hiemperia,
            Novia,
            Omnis
        }

        public enum Districts
        {
            AristocratTreeDistrict,
            ElementalForest,
            CottongrazePlains,
            AvianRoost,
            DowntownUnapolis
        }

        public enum Areas
        {
            StoneTerrace,
            PowderPond,
            MatchfireWoods,
            VoltClearing,
            StormflareValley,
            IcestoneGrotto,
            RainbowRuins,
            TestArea,
            SilverlightForest,
            UnapolisDowntown
        }

        public enum Seasons
        {
            Spring,
            Summer,
            Autumn,
            Winter
        }

        public enum Times
        {
            EarlyMorning,
            Morning,
            Daytime,
            Afternoon,
            Evening,
            Night,
            LateNight
        }

        public enum WeatherConditions
        {
            Sun,
            PartialClouds,
            Cloudy,
            Rain,
            Sunshower,
            Windy,
            Snow,
            Hail,
            Fog,
            TestWeather
        }
    }
}