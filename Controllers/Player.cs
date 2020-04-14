using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

namespace YuguLibrary
{
    namespace Controllers
    {
        public class Player
        {
            #region Variables
            /// <summary>
            /// The player's saved data.
            /// </summary>
            private PlayerFile playerFile;

            /// <summary>
            /// The current overworld object being controlled by the user.
            /// </summary>
            private OverworldObject currentOverworldObject;

            /// <summary>
            /// Reference to the scene's camera.
            /// </summary>
            public Camera mainCamera;

            /// <summary>
            /// The movement speed of the scene's camera.
            /// </summary>
            public float cameraSpeed = 5;
            #endregion

            #region Constructors
            public Player(Camera mainCamera)
            {
                this.mainCamera = mainCamera;
            }
            #endregion

            #region Functions
            public void SetPlayerUnit(OverworldObject overworldObject)
            {
                currentOverworldObject = overworldObject;
            }

            private void RunAction(ControllerInputs input)
            {

            }

            /// <summary>
            /// Reads and handles controller inputs from the user.
            /// </summary>
            public void ReadInputs()
            {
                // Buttons
                if (Input.GetButtonDown("Bottom Face Button"))
                {
                    /* NOTES: Mapped to Overworld Skill 1 (Jump). Changes to Float while in midair if accessible by the 
                     * player’s character.
                     * */
                    Debug.Log("Bottom Face Button pressed");
                    RunAction(ControllerInputs.BottomFace);
                }

                if (Input.GetButtonDown("Right Face Button"))
                {
                    /* NOTES: Mapped to Overworld Skill 2 (Basic Attack). Changes to Interact when an interactable object 
                     * is in front of the player.
                     * 
                     * */
                    Debug.Log("Right Face Button pressed");
                    RunAction(ControllerInputs.RightFace);
                }

                if (Input.GetButtonDown("Left Face Button"))
                {
                    /* NOTES: Mapped to Overworld Skill 3.
                     * */
                    Debug.Log("Left Face Button pressed");
                    RunAction(ControllerInputs.LeftFace);
                }

                if (Input.GetButtonDown("Top Face Button"))
                {
                    /* NOTES: Mapped to Overworld Skill 4.
                     * */
                    Debug.Log("Top Face Button pressed");
                    RunAction(ControllerInputs.TopFace);
                }

                if (Input.GetButtonDown("Left Bumper"))
                {
                    /* NOTES: Toggles running. Double press to keep it permanently toggled until pressed again.
                     * 
                     * TODO+: Look into checking for double presses; not implemented yet.
                     * */
                    Debug.Log("Left Bumper pressed");
                    RunAction(ControllerInputs.LeftBumper);
                }

                if (Input.GetButtonDown("Right Bumper"))
                {
                    /* NOTES: Toggles swap between radar and map UI.
                     * 
                     * TODO+: not implemented yet
                     * */
                    Debug.Log("Right Bumper pressed");
                    RunAction(ControllerInputs.RightBumper);
                }

                if (Input.GetButtonDown("Start"))
                {
                    /* NOTES: Opens the game’s main menu.
                     * 
                     * TODO+: not implemented yet
                     * */
                    Debug.Log("Start pressed");
                    RunAction(ControllerInputs.Start);
                }

                if (Input.GetButtonDown("Select"))
                {
                    /* NOTES: No known use currently.
                     * */
                    Debug.Log("Select pressed");
                    RunAction(ControllerInputs.Select);
                }

                if (Input.GetButtonDown("Left Analog Press"))
                {
                    /* NOTES: No known use currently.
                     * */
                    Debug.Log("Left Analog Press pressed");
                    RunAction(ControllerInputs.LeftAnalogPress);
                }

                if (Input.GetButtonDown("Right Analog Press"))
                {
                    /* NOTES: No known use currently.
                     * */
                    Debug.Log("Right Analog Press pressed");
                    RunAction(ControllerInputs.RightAnalogPress);
                }

                // Joysticks/Triggers

                /* NOTES: Mapped to movement.
                 * 0-90 = NE
                 * 91-180 = NW
                 * 181-270 = SW
                 * 271-359 = SE.
                 * */
                float xAxisLeftAnalog = Input.GetAxisRaw("Left Analog Horizontal");
                float yAxisLeftAnalog = Input.GetAxisRaw("Left Analog Vertical");


                /* NOTES: Pans the camera around the field during encounters; no function outside of encounters.
                 * */
                float xAxisRightAnalog = Input.GetAxisRaw("Right Analog Horizontal");
                float yAxisRightAnalog = Input.GetAxisRaw("Right Analog Vertical");


                /* NOTES: Mapped to movement. Left analog stick is likely preferred due to isometric perspective; map to both for now.
                 *
                 * UP = NW 
                 * DOWN = SE
                 * LEFT = SW
                 * RIGHT = NE.
                 * 
                 * Alternative mapping (only reads diagonal inputs):
                 * UP-LEFT = NW
                 * UP-RIGHT = NE
                 * DOWN-LEFT = SW
                 * DOWN-RIGHT = SE.
                 * */
                float xAxisDPad = Input.GetAxisRaw("DPad Horizontal");
                float yAxisDPad = Input.GetAxisRaw("DPad Vertical");


                /* NOTES: No known use currently.
                 * */
                float leftTrigger = Input.GetAxisRaw("Left Trigger");

                /* NOTES: No known use currently.
                 * */
                float rightTrigger = Input.GetAxisRaw("Right Trigger");
                
                //if ((playerUnit.canMove && !inputsDisabled) || (inputsDisabled && currentOverworldObject is TileCursor))
                //{
                    CheckMovement(xAxisDPad, yAxisDPad);
                    CheckMovement(xAxisLeftAnalog, yAxisLeftAnalog * -1);
                //} 
                
            }

            /// <summary>
            /// Reads inputs for movement on the game's overworld.
            /// </summary>
            /// <param name="xAxis">The position of the input's x axis.</param>
            /// <param name="yAxis">The position of the input's y axis.</param>
            void CheckMovement(float xAxis, float yAxis)
            {
                if (xAxis > 0.25f && yAxis > 0.25f) // right && down
                {
                    if (currentOverworldObject.MoveSE())
                    {
                        Vector3 cp = mainCamera.transform.position;

                        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cp,
                        cameraSpeed * Time.deltaTime);
                    }

                }

                if (xAxis < -0.25f && yAxis < -0.25f) // left && up
                {
                    if (currentOverworldObject.MoveNW())
                    {
                        Vector3 cp = mainCamera.transform.position;

                        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cp,
                        cameraSpeed * Time.deltaTime);
                    }
                }

                if (yAxis > 0.25f && xAxis < -0.25f) // down && left
                {
                    if (currentOverworldObject.MoveSW())
                    {
                        Vector3 cp = mainCamera.transform.position;

                        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cp,
                        cameraSpeed * Time.deltaTime);
                    }

                }

                if (yAxis < -0.25f && xAxis > 0.25f) // up && right
                {
                    if (currentOverworldObject.MoveNE())
                    {
                        Vector3 cp = mainCamera.transform.position;

                        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cp,
                        cameraSpeed * Time.deltaTime);
                    }
                }
            }
            #endregion
        }

    }
}