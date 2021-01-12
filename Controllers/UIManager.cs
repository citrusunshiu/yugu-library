using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;

namespace YuguLibrary
{
    namespace Controllers
    {
        /// <summary>
        /// Tracks UI history and loads UI screens.
        /// </summary>
        public class UIManager
        {
            /// <summary>
            /// The stack containing the order that the UI was traversed in.
            /// </summary>
            private Stack<UIConfiguration> uiStack;

            /// <summary>
            /// Constructs a UI manager with an empty UI stack.
            /// </summary>
            public UIManager()
            {
                uiStack = new Stack<UIConfiguration>();
            }

            /// <summary>
            /// Pushes a given UI screen onto <see cref="uiStack"/>, and loads the UI screen.
            /// </summary>
            /// <param name="ui">The UI screen to be added and loaded to the UI stack.</param>
            /// <param name="clearsStack">Whether or not the UI stack should be cleared before pushing the UI screen.</param>
            public void PushUI(UIConfiguration ui, bool clearsStack)
            {
                if (clearsStack)
                {
                    uiStack.Clear();
                }

                uiStack.Push(ui);
                ui.LoadUI(GameObject.Find("UI Container"));
            }

            /// <summary>
            /// Pops the latest UI screen from <see cref="uiStack"/>, and loads the new UI screen at the top of the stack.
            /// </summary>
            public void PopUI()
            {
                if(uiStack.Count > 1)
                {
                    uiStack.Pop();
                    uiStack.Peek().LoadUI(GameObject.Find("UI Container"));
                }
            }
        }
    }
}