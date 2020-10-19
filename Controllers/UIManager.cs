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

            Stack<YuguUIScreens> uiStack;

            YuguUIScreens currentUIScreen;

            public UIManager()
            {
                uiStack = new Stack<YuguUIScreens>();
            }

            public void PushUI(YuguUIScreens uiScreen, bool clearsStack)
            {

            }

            public void PopUI()
            {

            }
        }
    }
}