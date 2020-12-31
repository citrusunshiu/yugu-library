using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;

namespace YuguLibrary
{
    namespace Controllers
    {
        public class UIManager
        {

            private Stack<UIConfiguration> uiStack;

            private UIConfiguration currentUI;

            public UIManager()
            {
                uiStack = new Stack<UIConfiguration>();
            }

            public void PushUI(UIConfiguration ui, bool clearsStack)
            {
                uiStack.Push(ui);
                ui.LoadUI(GameObject.Find("UI Container"));
            }

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