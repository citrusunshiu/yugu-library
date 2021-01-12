using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.HookBundles;
using YuguLibrary.Models;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Controllers
    {
        public class Encounter
        {
            /// <summary>
            /// Reference to the controller the encounter is attached to.
            /// </summary>
            private MonoBehaviour controllerReference;

            /// <summary>
            /// Whether or not the encounter is currently in progress.
            /// </summary>
            private bool encounterActive = false;

            /// <summary>
            /// The current round that the active encounter is on.
            /// </summary>
            private int currentRound;

            /// <summary>
            /// The size of the encounter area.
            /// </summary>
            private AreaOfEffect fieldSize;

            /// <summary>
            /// The area that the currently selected unit is able to move within.
            /// </summary>
            private AreaOfEffect currentUnitMoveBounds;

            /// <summary>
            /// The order that units within the encounter will take action.
            /// </summary>
            private static List<UnitTurn> turnOrder;

            private UnitTurn currentTurn;

            public Encounter(MonoBehaviour controllerReference)
            {
                this.controllerReference = controllerReference;
            }

            /// <summary>
            /// Adds a unit to the current encounter.
            /// </summary>
            /// <param name="unit">The unit to add.</param>
            public void AddUnitToEncounter(Unit unit)
            {
                AddUnitTurn(unit, false);
            }

            #region Encounter Logic
            /// <summary>
            /// Starts an encounter. 
            /// </summary>
            /// <param name="encounterType">The type of encounter that was engaged.</param>
            /// <param name="fieldSize">The size of the encounter zone.</param>
            /// <param name="units">The units initially engaged in the encounter.</param>
            public void BeginEncounter(EncounterTypes encounterType, AreaOfEffect fieldSize, List<Unit> units)
            {
                encounterActive = true;
                currentRound = 0;
                this.fieldSize = fieldSize;
                turnOrder = CreateTurnOrder(units);
                OrganizeTurnOrder();

                UtilityFunctions.GetActiveUnitDetector().AddIndicatorTiles(fieldSize.GetAllTiles(), TileIndicatorTypes.Encounter);


                foreach (Unit unit in units)
                {
                    Debug.Log(unit.GetType());
                }

                switch (encounterType)
                {
                    case EncounterTypes.Advantage:
                        GrantPriorityToAllOfType(TargetTypes.Ally);
                        break;
                    case EncounterTypes.Disadvantage:
                        GrantPriorityToAllOfType(TargetTypes.Enemy);
                        break;
                }

                //playerController.inputsDisabled = false;
                //uiController.PushUI(UIScreens.EncounterScreen, true);
                controllerReference.StartCoroutine(EncounterLoop());
            }

            /// <summary>
            /// Runs the game loop of an encounter.
            /// </summary>
            /// <returns></returns>
            IEnumerator EncounterLoop()
            {
                while (encounterActive)
                {
                    StartRound();
                    for (int i = 0; i < turnOrder.Count; i++)
                    {
                        UnitTurn unitTurn = turnOrder[i];

                        Debug.Log("start of turn loop");
                        StartTurn(unitTurn);
                        //Selecting actions (primary/auxiliary/...)
                        yield return new WaitUntil(() => unitTurn.isCompleted == true);

                        EndTurn(unitTurn);
                        Debug.Log("end of turn loop");
                    }
                    EndRound();
                }
            }

            /// <summary>
            /// Logic for the beginning of an encounter round.
            /// </summary>
            void StartRound()
            {
                currentRound++;
                Debug.Log(this + ":beginning of round #" + currentRound);
                RefreshTurnOrder();

                UtilityFunctions.GetActiveUnitDetector()
                    .FlagDelegate(DelegateFlags.OnEncounterRoundStart, new RoundStartHookBundle(), GetEncounterUnitList());
            }



            /// <summary>
            /// Logic for the beginning of an encounter turn.
            /// </summary>
            /// <param name="unitTurn">The <see cref="UnitTurn"/> of the current unit's turn.</param>
            void StartTurn(UnitTurn unitTurn)
            {
                UtilityFunctions.GetActiveUnitDetector()
                    .FlagDelegate(DelegateFlags.OnEncounterTurnStart, new TurnStartHookBundle(unitTurn), GetEncounterUnitList());

                unitTurn.isActive = true;
                currentTurn = unitTurn;
                currentUnitMoveBounds = new AreaOfEffect(unitTurn.unit.position, 5);
                UtilityFunctions.GetActiveUnitDetector().AddIndicatorTiles(currentUnitMoveBounds.GetAllTiles(), TileIndicatorTypes.Movement);

                Debug.Log(UtilityFunctions.GetUnitName(unitTurn) + ":turn started; waiting");

                if (CanUnitTakeAction(currentTurn))
                {
                    if (unitTurn.unit.GetTargetType() == TargetTypes.Enemy)
                    {
                        //CheckEncounterAI(unitTurn);
                    }
                    else
                    {
                        //playerController.SetPlayerUnit(currentTurn.unit);
                        //uiController.PushUI(UIScreens.EncounterScreen, true);
                    }
                }
                else
                {
                    Debug.Log("due to impairment, unit cannot take action");
                    EndTurn(currentTurn);
                }

            }

            /// <summary>
            /// Logic for the ending of an encounter turn.
            /// </summary>
            /// <param name="unitTurn">The <see cref="UnitTurn"/> of the current unit's turn.</param>
            void EndTurn(UnitTurn unitTurn)
            {
                Debug.Log(UtilityFunctions.GetUnitName(unitTurn) + ":turn ended; proceeding");
                unitTurn.isActive = false;
                //uiController.PushUI(UIScreens.EncounterScreen, true);
                UtilityFunctions.GetActiveUnitDetector()
                    .FlagDelegate(DelegateFlags.OnEncounterTurnEnd, new TurnEndHookBundle(unitTurn), GetEncounterUnitList());

                unitTurn.isCompleted = true;
            }

            /// <summary>
            /// Logic for the ending of an encounter round.
            /// </summary>
            void EndRound()
            {
                Debug.Log("end of round #" + currentRound);
                CheckEncounterEndstate();
                UtilityFunctions.GetActiveUnitDetector()
                    .FlagDelegate(DelegateFlags.OnEncounterRoundEnd, new RoundEndHookBundle(), GetEncounterUnitList());
                //CheckAggroDecay();
                AdvanceSurroundingEnemies();
                //CheckCCResistanceBarReset();
                //ReduceSkillCooldowns();
                //ReduceStatusDurations(1);
                //unitDetectorController.ReduceSpecialTileDurations(1);
            }

            /// <summary>
            /// Ends the current unit's turn.
            /// </summary>
            public void CompleteTurn()
            {
                currentTurn.isCompleted = true;
            }

            /// <summary>
            /// Ends the current encounter.
            /// </summary>
            /// <param name="encounterEndstate">The encounter's ending state.</param>
            void EndEncounter(EncounterEndstates encounterEndstate)
            {
                switch (encounterEndstate)
                {
                    case EncounterEndstates.Victory:
                        //aggro circle goes back to normal size; overworldai resumes
                        //playerController.ResetPlayerUnit();
                        CleanSlate();
                        //unitDetectorController.ResetAggroRadius();
                        CloseEncounter();
                        break;
                    case EncounterEndstates.Flee:
                        /* overworldai resumes, but aggro circle size stays(decreases at a rate of 1 tile / second);
                         * stats and statuses from the encounter do not change; lead character is changed to the one that 
                         * fled */
                        //playerController.SetPlayerUnit(currentTurn.unit);
                        CloseEncounter();
                        break;
                    case EncounterEndstates.Defeat:
                        CleanSlate();
                        TriggerDefeat();
                        break;
                }
            }

            /// <summary>
            /// Logic for ending an encounter and resuming overworld gameplay state.
            /// </summary>
            void CloseEncounter()
            {
                controllerReference.StopAllCoroutines();
                encounterActive = false;
                currentRound = 0;
                fieldSize = null;
                UtilityFunctions.GetActiveUnitDetector().RemoveUnitSetup();

                //playerController.inputsDisabled = false;
                //uiController.PushUI(UIScreens.GameScreen, true);
                UnitDetector unitDetector = UtilityFunctions.GetActiveUnitDetector();

                unitDetector.SetAutomaticAIActivity(true);
                //UnitDetectorController.MapAggroRadiusToUnit(playerController.GetPlayerUnit());
                //unitDetectorController.StartCoroutine(unitDetectorController.ReduceAggroRadius());
            }

            /// <summary>
            /// Moves all aggroed enemy units that are not currently part of the encounter 1 tile closer towards the 
            /// encounter's center.
            /// </summary>
            private void AdvanceSurroundingEnemies()
            {

            }

            /// <summary>
            /// Checks if all allied units or all enemy units have been incapacitated, triggering an encounter endstate if
            /// necessary.
            /// </summary>
            private void CheckEncounterEndstate()
            {
                bool allAlliesIncapacitated = true;
                bool allEnemiesIncapacitated = true;

                foreach (UnitTurn unitTurn in turnOrder)
                {
                    Unit unit = unitTurn.unit;
                    if (!unit.SearchStatuses(StatusEffects.Incapacitation))
                    {
                        if (unit.GetTargetType() == TargetTypes.Ally)
                        {
                            allAlliesIncapacitated = false;
                        }
                        else if (unit.GetTargetType() == TargetTypes.Enemy)
                        {
                            allEnemiesIncapacitated = false;
                        }
                    }

                }

                if (allAlliesIncapacitated)
                {
                    EndEncounter(EncounterEndstates.Defeat);
                    //game over state
                }
                else if (allEnemiesIncapacitated)
                {
                    Debug.Log("win?");
                    EndEncounter(EncounterEndstates.Victory);
                    //victory state
                }
            }

            /// <summary>
            /// Resets the statuses and stats of all units in an encounter.
            /// </summary>
            private void CleanSlate()
            {
                //TODO: cleanses all buffs/debuffs, and restores HP/MP values to full, resets aggro radius
                foreach (UnitTurn unitTurn in turnOrder)
                {
                    unitTurn.unit.ResetAllCooldowns();
                }
            }

            /// <summary>
            /// Logic for player's defeat.
            /// </summary>
            private void TriggerDefeat()
            {
                //game over (prompt retry/return to last safe zone/return to title)
            }

            /*
            /// <summary>
            /// Checks if any units in the current encounter should have parts of their aggro spread reduced.
            /// </summary>
            void CheckAggroDecay()
            {
                foreach (UnitTurn unitTurn in turnOrder)
                {
                    unitTurn.unit.aggroSpread.CheckAggroDecay(currentRound);
                }
            }

            void CheckCCResistanceBarReset()
            {
                foreach (UnitTurn unitTurn in turnOrder)
                {
                    unitTurn.unit.CheckCCResistanceBarReset(currentRound);
                }
            }

            void ReduceSkillCooldowns()
            {
                foreach (UnitTurn unitTurn in turnOrder)
                {
                    foreach (Skill skill in unitTurn.unit.GetAllSkills())
                    {
                        skill.currentCooldown--;
                    }
                }
            }

            void ReduceStatusDurations(int reductionAmount)
            {
                foreach (UnitTurn unitTurn in turnOrder)
                {
                    List<Status> statuses = unitTurn.unit.GetAllStatuses();

                    foreach (Status status in statuses)
                    {
                        status.ReduceDuration(reductionAmount);
                    }
                }
            }

            */
            #endregion

            public AreaOfEffect GetFieldSize()
            {
                return fieldSize;
            }

            public AreaOfEffect GetCurrentUnitMoveBounds()
            {
                return currentUnitMoveBounds;
            }

            /// <summary>
            /// Creates a list of <see cref="UnitTurn"/> objects from a list of <see cref="Models.OverworldObject"/> objects.
            /// </summary>
            /// <param name="unitList">The list of units to convert.</param>
            /// <returns>Returns a list of UnitTurn objects created from the Unit list object given.</returns>
            private List<UnitTurn> CreateTurnOrder(List<Unit> unitList)
            {
                List<UnitTurn> turnOrder = new List<UnitTurn>();
                foreach (Unit unit in unitList)
                {
                    UnitTurn unitTurn = new UnitTurn(unit);
                    turnOrder.Add(unitTurn);
                }
                return turnOrder;
            }

            /// <summary>
            /// Sorts the <see cref="turnOrder"/> using <see cref="UnitTurnSorter.Compare(UnitTurn, UnitTurn)"/>.
            /// </summary>
            private void OrganizeTurnOrder()
            {
                turnOrder.Sort(new UnitTurnSorter());
            }

            /// <summary>
            /// Resets the values of all <see cref="UnitTurn"/> objects in <see cref="turnOrder"/>.
            /// </summary>
            private void RefreshTurnOrder()
            {
                foreach (UnitTurn unitTurn in turnOrder)
                {
                    unitTurn.Reset();
                }
            }

            /// <summary>
            /// Retrieves the <see cref="UnitTurn"/> object from <see cref="turnOrder"/> that corresponds to the given Unit object.
            /// </summary>
            /// <param name="unit">The Unit object contained in the UnitTurn object to search for.</param>
            /// <returns>Returns the UnitTurn object associated with the given Unit object.</returns>
            private UnitTurn GetUnitTurn(Unit unit)
            {
                foreach (UnitTurn unitTurn in turnOrder)
                {
                    if (unitTurn.unit.Equals(unit))
                    {
                        return unitTurn;
                    }
                }

                return null;
            }

            /// <summary>
            /// Creates a prioritized <see cref="UnitTurn"/> for all units of a certain targeting type.
            /// </summary>
            /// <param name="targetType">The targeting type of the units to create a priority turn for.</param>
            private void GrantPriorityToAllOfType(TargetTypes targetType)
            {
                foreach (UnitTurn unitTurn in turnOrder)
                {
                    if (!unitTurn.isPriority && unitTurn.unit.GetTargetType() == targetType)
                    {
                        AddUnitTurn(unitTurn.unit, true);
                    }
                }
            }

            /// <summary>
            /// Creates a new <see cref="UnitTurn"/> and inserts it into <see cref="turnOrder"/>.
            /// </summary>
            /// <param name="u">The unit that will belong to the new UnitTurn object.</param>
            /// <param name="isPriority">Whether or not the unit turn is prioritized in the turn order.</param>
            private void AddUnitTurn(Unit unit, bool isPriority)
            {
                UnitTurn unitTurn = new UnitTurn(unit, isPriority);
                turnOrder.Add(unitTurn);
                OrganizeTurnOrder();
            }

            /// <summary>
            /// Removes a specified unit from the current encounter.
            /// </summary>
            /// <param name="unitTurn">The <see cref="UnitTurn"/> of the unit to be removed.</param>
            private void RemoveUnitFromEncounter(UnitTurn unitTurn)
            {
                //turnOrder.Remove(unitTurn);
                UtilityFunctions.GetActiveUnitDetector().RemoveOverworldObject(unitTurn.unit);
            }

            /// <summary>
            /// Checks if the unit is able to take action.
            /// </summary>
            /// <param name="unitTurn">The unit turn to check.</param>
            /// <returns>Returns true if the unit can take action, and false otherwise.</returns>
            private bool CanUnitTakeAction(UnitTurn unitTurn)
            {
                List<StatusEffects> blockingImpairments = new List<StatusEffects>
                {
                    StatusEffects.Stun,
                    StatusEffects.Grab,
                    StatusEffects.Freeze,
                    StatusEffects.Sleep,
                    StatusEffects.Charm,
                    StatusEffects.Confusion,
                    StatusEffects.Terrify,
                    StatusEffects.TimeStop,
                    StatusEffects.Incapacitation
                };

                //TODO: add the rest of the blocking impairments here
                foreach (StatusEffects impairment in blockingImpairments)
                {
                    if (unitTurn.unit.SearchStatuses(impairment))
                    {
                        return false;
                    }
                }

                return true;
            }

            private List<Unit> GetEncounterUnitList()
            {
                List<Unit> unitList = new List<Unit>();
                for (int i = 0; i < turnOrder.Count; i++)
                {
                    unitList.Add(turnOrder[i].unit);
                }

                return unitList;
            }

            public bool IsEncounterActive()
            {
                return encounterActive;
            }

            /// <summary>
            /// IComparer for properly ordering a list of <see cref="UnitTurn"/> objects for <see cref="Encounter"/>.
            /// </summary>
            class UnitTurnSorter : IComparer<UnitTurn>
            {
                //TODO: sort by isCompleted also?
                public int Compare(UnitTurn x, UnitTurn y)
                {
                    int result = 0;

                    Unit ux = x.unit;
                    Unit uy = y.unit;

                    if ((x.isPriority && y.isPriority) || !(x.isPriority && y.isPriority))
                    {
                        result = SpeedComparison(ux, uy) * -1;
                    }
                    else if (x.isPriority)
                    {
                        result = 1;
                    }
                    else if (y.isPriority)
                    {
                        result = -1;
                    }

                    if (x.isCompleted && !y.isCompleted)
                    {
                        result = 1;
                    }
                    else if (!x.isCompleted && y.isCompleted)
                    {
                        result = -1;
                    }

                    return result;
                }

                int SpeedComparison(Unit x, Unit y)
                {
                    int result = x.speed.CompareTo(y.speed);
                    if (result == 0)
                    {
                        return x.GetTargetType().CompareTo(y.GetTargetType());
                    }

                    return result;
                }
            }
        }
    }
}
