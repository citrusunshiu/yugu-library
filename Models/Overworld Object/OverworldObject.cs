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
        public abstract class OverworldObject
        {
            #region Variables
            /// <summary>
            /// The <see cref="OverworldObjectCoordinator"/> associated with the overworld object.
            /// </summary>
            public OverworldObjectCoordinator overworldObjectCoordinator;

            /// <summary>
            /// The overworld object's current position.
            /// </summary>
            public Vector3Int position;

            /// <summary>
            /// The facing direction of the unit.
            /// </summary>
            /// <remarks>
            /// Determines results for directional calculations.
            /// </remarks>
            public Directions direction;

            /// <summary>
            /// The overworld object's speed tier.
            /// </summary>
            /// <remarks>
            /// Determines how fast the unit is able to move in the overworld.
            /// </remarks>
            public SpeedTiers speedTier;

            /// <summary>
            /// The <see cref="Models.AnimationScript"/> used to display the unit's visuals.
            /// </summary>
            public AnimationScript animationScript;
            
            private List<OverworldObjectAction> owoActions = new List<OverworldObjectAction>();



            /// <summary>
            /// The number of frames it takes for the overworld object to walk across 1 tile.
            /// </summary>
            public float walkFrames;

            /// <summary>
            /// Amount of delay (in seconds) that the unit undergoes while walking.
            /// </summary>
            /// <remarks>
            /// Delay is enacted when moving across tiles in the X and Y axes. Lower moveSpeed values makes the unit move 
            /// faster. For player units, the delay activates after moving to a new tile. For non-player units, the delay 
            /// activates before trying to move; if the tile becomes occupied after the delay ends, the unit does not move.
            /// </remarks>
            public float walkDelay;


            /// <summary>
            /// The number of frames it takes for the overworld object to run across 1 tile.
            /// </summary>
            public float runFrames;

            /// <summary>
            /// Amount of delay (in seconds) that the unit undergoes during movement.
            /// </summary>
            /// <remarks>
            /// Delay is enacted when moving across tiles in the X and Y axes. Lower moveSpeed values makes the unit move 
            /// faster. For player units, the delay activates after moving to a new tile. For non-player units, the delay 
            /// activates before trying to move; if the tile becomes occupied after the delay ends, the unit does not move.
            /// </remarks>
            public float runDelay;


            /// <summary>
            /// The number of frames it takes for the overworld object to ascend up 1 tile.
            /// </summary>
            public float ascentFrames;

            /// <summary>
            /// Amount of delay (in seconds) that the unit undergoes during jump ascent.
            /// </summary>
            /// <remarks>
            /// Delay is enacted when moving up tiles in the Z axis while airborne.
            /// </remarks>
            public float ascentDelay;

            /// <summary>
            /// The number of frames it takes for the overworld object to descend down 1 tile.
            /// </summary>
            public float descentFrames;

            /// <summary>
            /// Amount of delay (in seconds) that the unit undergoes during jump descent.
            /// </summary>
            /// <remarks>
            /// Delay is enacted when moving down tiles in the Z axis.
            /// </remarks>
            public float descentDelay;

            /// <summary>
            /// Amount of tiles upwards that the overworld object can travel during a jump.
            /// </summary>
            public int jumpHeight = 2;

            /// <summary>
            /// The overworld object's current progression in their jump.
            /// </summary>
            public int currentJump = 0;

            /// <summary>
            /// Whether or not the overworld object is able to currently move.
            /// </summary>
            public bool canMove = true;


            /// <summary>
            /// Whether or not the overworld object is able to currently ascend.
            /// </summary>
            public bool canAscend = true;

            /// <summary>
            /// Whether or not the overworld object is able to currently descend.
            /// </summary>
            public bool canDescend = true;

            /// <summary>
            /// Whether or not the overworld object is currently jumping.
            /// </summary>
            public bool isJumping = false;

            /// <summary>
            /// 
            /// </summary>
            public bool canFloat = false;

            /// <summary>
            /// Whether or not the overworld object is currently floating.
            /// </summary>
            public bool isFloating = false;

            /// <summary>
            /// Whether or not the overworld object is currently running.
            /// </summary>
            public bool isRunning = true;

            /// <summary>
            /// Whether or not the overworld object is currently stationary.
            /// </summary>
            /// <remarks>
            /// Stationary overworld objects will not move when a direction is pressed, but they will turn to face that direction.
            /// </remarks>
            public bool isStationary = false;

            /// <summary>
            /// 
            /// </summary>
            public bool manualMovementToggle = false;

            /// <summary>
            /// Whether or not the overworld object can change its facing direction.
            /// </summary>
            public bool canTurn = true;

            /// <summary>
            /// Whether or not the overworld object's movement is currently being unlocked.
            /// </summary>
            /// <remarks>
            /// Tthe overworld object will not be able to move in another direction until the time specified by 
            /// <see cref="walkDelay"/> or <see cref="runDelay"/> has passed.
            /// </remarks>
            public bool unlockingMovement = false;

            /// <summary>
            /// Whether or not the overworld object's ascent is currently being unlocked.
            /// </summary>
            /// <remarks>
            /// Tthe overworld object will not be able to move in another direction until the time specified by 
            /// <see cref="ascentDelay"/> has passed.
            /// </remarks>
            public bool unlockingAscent = false;

            /// <summary>
            /// Whether or not the overworld object's descent is currently being unlocked.
            /// </summary>
            /// <remarks>
            /// Tthe overworld object will not be able to move in another direction until the time specified by 
            /// <see cref="descentDelay"/> has passed.
            /// </remarks>
            public bool unlockingDescent = false;
            #endregion

            #region Constructors
            public OverworldObject()
            {

            }
            #endregion

            #region Functions
            public Vector3Int GetPosition()
            {
                return position;
            }

            public void AddOverworldObjectAction(OverworldObjectAction overworldObjectAction)
            {
                owoActions.Add(overworldObjectAction);
            }

            public void RemoveOverworldObjectAction(OverworldObjectAction overworldObjectAction)
            {
                owoActions.Remove(overworldObjectAction);
            }

            public OverworldObjectAction GetOverworldObjectAction(ControllerInputs input)
            {
                OverworldObjectAction overworldObjectAction = null;

                foreach(OverworldObjectAction action in owoActions)
                {
                    if(action.GetControllerInput() == input && 
                        (overworldObjectAction == null || action.GetPriority() > overworldObjectAction.GetPriority()))
                    {
                        overworldObjectAction = action;
                    }
                }

                return overworldObjectAction;
            }

            public OverworldObjectAction GetOverworldObjectAction(KeyCode input)
            {
                OverworldObjectAction overworldObjectAction = null;

                foreach (OverworldObjectAction action in owoActions)
                {
                    if (action.GetKeyboardInput() == input &&
                        (overworldObjectAction == null || action.GetPriority() > overworldObjectAction.GetPriority()))
                    {
                        overworldObjectAction = action;
                    }
                }

                return overworldObjectAction;
            }

            /// <summary>
            /// Sets the number of frames the overworld object should take for different forms of movement, and 
            /// calculates the delay (in seconds) based on the number of frames.
            /// </summary>
            /// <param name="walkFrames">The number of frames the overworld object should take to walk 1 tile.</param>
            /// <param name="ascentFrames">The number of frames the overworld object should take to ascend 1 tile.</param>
            /// <param name="descentFrames">The number of frames the overworld object should take to descend 1 tile.</param>
            protected void CalculateFrameSpeeds(int walkFrames, int ascentFrames, int descentFrames)
            {
                this.walkFrames = walkFrames;
                walkDelay = walkFrames / 60F;

                //runFrames = (int)speedTier;
                runDelay = runFrames / 60F;

                //this.ascentFrames = ascentFrames;
                ascentDelay = ascentFrames / 60F;

                //this.descentFrames = descentFrames;
                descentDelay = descentFrames / 60F;
            }

            /// <summary>
            /// Gets the appropriate movement delay based on the overworld object's current movement state.
            /// </summary>
            /// <returns>Returns <see cref="runDelay"/> if <see cref="isRunning"/> is true, and returns 
            /// <see cref="walkDelay"/> otherwise.</returns>
            public float GetMovementDelay()
            {
                return isRunning ? runDelay : walkDelay;
            }

            /// <summary>
            /// Gets the appropriate movement frames based on the overworld object's current movement state.
            /// </summary>
            /// <returns>Returns <see cref="runFrames"/> if <see cref="isRunning"/> is true, and returns 
            /// <see cref="walkFrames"/> otherwise.</returns>
            public float GetTotalMovementFrames()
            {
                return isRunning ? runFrames : walkFrames;
            }

            public bool Jump()
            {
                if (!isJumping)
                {
                    overworldObjectCoordinator.StartCoroutine(ProgressJump());
                }
                return true;
            }

            private IEnumerator ProgressJump()
            {
                isJumping = true;
                while(currentJump < jumpHeight)
                {
                    MoveInDirection(Directions.Up);
                    currentJump++;
                    yield return new WaitUntil(() => canAscend == true);
                }
                while(currentJump > 0)
                {
                    MoveInDirection(Directions.Down);
                    currentJump--;
                    yield return new WaitUntil(() => canDescend == true);
                }
                isJumping = false;
                currentJump = 0;
                yield return null;
            }

            private IEnumerator FallLoop()
            {
                while (UtilityFunctions.GetActiveUnitDetector().IsAirborne(this))
                {
                    MoveDown();
                    yield return new WaitUntil(() => canDescend == true);
                }
                yield return null;
            }

            /// <summary>
            /// Moves the overworld object 1 tile in the given direction.
            /// </summary>
            /// <param name="direction">The direction for the overworld object to move in.</param>
            /// <returns>Returns true if the overworld object moved successfully, and false otherwise.</returns>
            public bool MoveInDirection(Directions direction)
            {
                //for gravity
                if (!isJumping)
                {
                    overworldObjectCoordinator.StartCoroutine(FallLoop());
                    //MoveDown();
                }

                switch (direction)
                {
                    case Directions.NW:
                        return MoveNW();

                    case Directions.NE:
                        return MoveNE();

                    case Directions.SW:
                        return MoveSW();

                    case Directions.SE:
                        return MoveSE();

                    case Directions.Up:
                        return MoveUp();
                    
                    case Directions.Down:
                        return MoveDown();
                    
                    default:
                        return false;
                }
            }

            /// <summary>
            /// Moves the overworld object 1 tile forward in the northwest direction.
            /// </summary>
            /// <returns>Returns true if the unit moved successfully, and false otherwise.</returns>
            private bool MoveNW()
            {
                if (canMove && UtilityFunctions.GetActiveUnitDetector().Move(this, 0, 1, 0))
                {
                    direction = Directions.NW;
                    overworldObjectCoordinator.FrameMove(Directions.NW);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Moves the overworld object 1 tile forward in the northeast direction.
            /// </summary>
            /// <returns>Returns true if the unit moved successfully, and false otherwise.</returns>
            private bool MoveNE()
            {
                if (canMove && UtilityFunctions.GetActiveUnitDetector().Move(this, 1, 0, 0))
                {
                    direction = Directions.NE;
                    overworldObjectCoordinator.FrameMove(Directions.NE);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Moves the overworld object 1 tile forward in the southeast direction.
            /// </summary>
            /// <returns>Returns true if the unit moved successfully, and false otherwise.</returns>
            private bool MoveSE()
            {
                if (canMove && UtilityFunctions.GetActiveUnitDetector().Move(this, 0, -1, 0))
                {
                    direction = Directions.SE;
                    overworldObjectCoordinator.FrameMove(Directions.SE);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Moves the overworld object 1 tile forward in the southwest direction.
            /// </summary>
            /// <returns>Returns true if the unit moved successfully, and false otherwise.</returns>
            private bool MoveSW()
            {
                if (canMove && UtilityFunctions.GetActiveUnitDetector().Move(this, -1, 0, 0))
                {
                    direction = Directions.SW;
                    overworldObjectCoordinator.FrameMove(Directions.SW);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Moves the overworld object 1 tile upward.
            /// </summary>
            /// <returns>Returns true if the unit moved successfully, and false otherwise.</returns>
            private bool MoveUp()
            {
                if (canAscend && UtilityFunctions.GetActiveUnitDetector().Move(this, 0, 0, 1))
                {
                    overworldObjectCoordinator.FrameMove(Directions.Up);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Moves the overworld object 1 tile downward.
            /// </summary>
            /// <returns>Returns true if the unit moved successfully, and false otherwise.</returns>
            private bool MoveDown()
            {
                if (canDescend && UtilityFunctions.GetActiveUnitDetector().Move(this, 0, 0, -1))
                {
                    overworldObjectCoordinator.FrameMove(Directions.Down);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Sets <see cref="canMove"/> to true after the seconds specified by <see cref="walkDelay"/> or 
            /// <see cref="runDelay"/> have passed.
            /// </summary>
            /// <remarks>Whether walkDelay or runDelay is used is dependent on the value of <see cref="isRunning"/>.</remarks>
            /// <returns></returns>
            public IEnumerator UnlockMovement()
            {
                if (!unlockingMovement)
                {
                    unlockingMovement = true;
                    yield return new WaitForSeconds(GetMovementDelay());
                    canMove = true;
                    unlockingMovement = false;
                }
                else
                {
                    yield return null;
                }

            }

            /// <summary>
            /// Sets <see cref="canAscend"/> to true after the seconds specified by <see cref="ascentDelay"/> have passed.
            /// </summary>
            /// <returns></returns>
            public IEnumerator UnlockAscent()
            {
                if (!unlockingAscent)
                {
                    unlockingAscent = true;
                    yield return new WaitForSeconds(ascentDelay);
                    canAscend = true;
                    unlockingAscent = false;
                    //Jump();
                }
                else
                {
                    yield return null;
                }
            }

            /// <summary>
            /// Sets <see cref="canDescend"/> to true after the seconds specified by <see cref="descentDelay"/> have passed.
            /// </summary>
            /// <returns></returns>
            public IEnumerator UnlockDescent()
            {
                if (!unlockingDescent)
                {
                    unlockingDescent = true;
                    yield return new WaitForSeconds(descentDelay);
                    canDescend = true;
                    unlockingDescent = false;
                    //Fall();
                }
                else
                {
                    yield return null;
                }

            }
            #endregion
        }
    }
}