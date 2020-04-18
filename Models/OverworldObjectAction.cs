using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

namespace YuguLibrary
{
    namespace Models
    {
        public abstract class OverworldObjectAction
        {
            #region Variables
            /// <summary>
            /// The controller button that the OverworldObjectAction is mapped to.
            /// </summary>
            private ControllerInputs controllerInput;

            /// <summary>
            /// The keyboard key that the OverworldObjectAction is mapped to. 
            /// </summary>
            private KeyCode keyboardInput;

            /// <summary>
            /// The UI button mode that the OverworldObjectAction is active on.
            /// </summary>
            /// <remarks>
            /// OverworldObjectActions can only be executed when their uiButtonMode value matches with the system's.
            /// </remarks>
            private UIButtonModes uiButtonMode;

            /// <summary>
            /// The priority of the OverworldSkill.
            /// </summary>
            /// <remarks>If multiple OverworldSkills are mapped to the same butotn, the higher priority skill will be 
            /// used.</remarks>
            private int priority;

            /// <summary>
            /// The overworld object that the OverworldObjectAction object is attached to.
            /// </summary>
            private OverworldObject overworldObject;

            /// <summary>
            /// The file path to the overworld object action's display icon.
            /// </summary>
            public abstract string IconFilePath { get; }
            #endregion

            #region Constructors
            public OverworldObjectAction(ControllerInputs input, OverworldObject overworldObject)
            {
                controllerInput = input;
                this.overworldObject = overworldObject;
            }

            public OverworldObjectAction(KeyCode input, OverworldObject overworldObject)
            {
                keyboardInput = input;
                this.overworldObject = overworldObject;
            }

            public OverworldObjectAction(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject)
            {
                this.controllerInput = controllerInput;
                this.keyboardInput = keyboardInput;
                this.overworldObject = overworldObject;
            }
            #endregion

            #region Functions
            /// <summary>
            /// Runs the functionality associated with the overworld object action.
            /// </summary>
            public abstract void ExecuteAction();

            /// <summary>
            /// Gets the controller input associated with the overworld object action.
            /// </summary>
            /// <returns>Returns the controller input associated with the overworld object action.</returns>
            public ControllerInputs GetControllerInput()
            {
                return controllerInput;
            }

            /// <summary>
            /// Sets the controller button associated with activating the overworld object action.
            /// </summary>
            /// <param name="controllerInput">The controller button to set the action to.</param>
            public void SetControllerInput(ControllerInputs controllerInput)
            {
                this.controllerInput = controllerInput;
            }
            
            /// <summary>
            /// Gets the keyboard input associated with the overworld object action.
            /// </summary>
            /// <returns>Returns the keyboard input associated with the overworld object action.</returns>
            public KeyCode GetKeyboardInput()
            {
                return keyboardInput;
            }

            /// <summary>
            /// Sets the keyboard input associated with activating the overworld object action.
            /// </summary>
            /// <param name="keyboardInput">The keyboard input to set the action to.</param>
            public void SetKeyboardInput(KeyCode keyboardInput)
            {
                this.keyboardInput = keyboardInput;
            }

            /// <summary>
            /// Gets the priority associated with the overworld object action.
            /// </summary>
            /// <returns>Returns the priority number of the overworld object action.</returns>
            public int GetPriority()
            {
                return priority;
            }

            /// <summary>
            /// Sets the overworld object action's priority value.
            /// </summary>
            /// <param name="priority">The value to set the priority to.</param>
            public void SetPriority(int priority)
            {
                this.priority = priority;
            }
            #endregion
        }
    }

    namespace OverworldObjectActions
    {
        public class TestOWOAction : OverworldObjectAction
        {
            public TestOWOAction(ControllerInputs input, OverworldObject overworldObject) : base(input, overworldObject)
            {

            }

            public TestOWOAction(KeyCode input, OverworldObject overworldObject) : base(input, overworldObject)
            {

            }

            public TestOWOAction(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject) : base(controllerInput, keyboardInput, overworldObject)
            {
            }

            public override string IconFilePath => throw new System.NotImplementedException();

            public override void ExecuteAction()
            {
                Debug.Log("TestOWOAction:ExecuteAction running");
            }
        }

        public class Interact : OverworldObjectAction
        {
            public Interact(ControllerInputs input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public Interact(KeyCode input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public Interact(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject) : base(controllerInput, keyboardInput, overworldObject)
            {
            }

            public override string IconFilePath => throw new System.NotImplementedException();

            public override void ExecuteAction()
            {
                throw new System.NotImplementedException();
            }
        }

        public class Jump : OverworldObjectAction
        {
            public Jump(ControllerInputs input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public Jump(KeyCode input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public Jump(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject) : base(controllerInput, keyboardInput, overworldObject)
            {
            }

            //if owo type is valid for floating, pressing jump while jumping toggles float
            public override string IconFilePath => throw new System.NotImplementedException();

            public override void ExecuteAction()
            {
                throw new System.NotImplementedException();
            }
        }

        public class FloatAscend : OverworldObjectAction
        {
            public FloatAscend(ControllerInputs input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public FloatAscend(KeyCode input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public FloatAscend(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject) : base(controllerInput, keyboardInput, overworldObject)
            {
            }

            public override string IconFilePath => throw new System.NotImplementedException();

            public override void ExecuteAction()
            {
                throw new System.NotImplementedException();
            }
        }

        public class FloatDescend : OverworldObjectAction
        {
            public FloatDescend(ControllerInputs input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public FloatDescend(KeyCode input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public FloatDescend(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject) : base(controllerInput, keyboardInput, overworldObject)
            {
            }

            public override string IconFilePath => throw new System.NotImplementedException();

            public override void ExecuteAction()
            {
                throw new System.NotImplementedException();
            }
        }

        public class ToggleRun : OverworldObjectAction
        {
            public ToggleRun(ControllerInputs input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public ToggleRun(KeyCode input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public ToggleRun(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject) : base(controllerInput, keyboardInput, overworldObject)
            {
            }

            public override string IconFilePath => throw new System.NotImplementedException();

            public override void ExecuteAction()
            {
                throw new System.NotImplementedException();
            }
        }

        public class ToggleStationary : OverworldObjectAction
        {
            public ToggleStationary(ControllerInputs input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public ToggleStationary(KeyCode input, OverworldObject overworldObject) : base(input, overworldObject)
            {
            }

            public ToggleStationary(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject) : base(controllerInput, keyboardInput, overworldObject)
            {
            }

            public override string IconFilePath => throw new System.NotImplementedException();

            public override void ExecuteAction()
            {
                throw new System.NotImplementedException();
            }
        }

        public class UseSkill : OverworldObjectAction
        {
            Skill skill;

            public UseSkill(ControllerInputs input, OverworldObject overworldObject, Skill skill) : base(input, overworldObject)
            {
                this.skill = skill;
            }

            public UseSkill(KeyCode input, OverworldObject overworldObject, Skill skill) : base(input, overworldObject)
            {
                this.skill = skill;
            }

            public UseSkill(ControllerInputs controllerInput, KeyCode keyboardInput, OverworldObject overworldObject, Skill skill) : base(controllerInput, keyboardInput, overworldObject)
            {
                this.skill = skill;
            }

            public override string IconFilePath => throw new System.NotImplementedException();

            public override void ExecuteAction()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
