using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YuguLibrary.Enumerations;
using YuguLibrary.Utilities;

public abstract class UIConfiguration
{
    protected UIScreens screen;
    protected string uiFileName;
    protected Button currentSelection;

    public abstract void LoadUI(GameObject uiContainer);

    public UIScreens GetScreen()
    {
        return screen;
    }

    public string GetUIFileName()
    {
        return uiFileName;
    }

    protected void LoadUIScreenFromPrefab(GameObject uiContainer)
    {
        Canvas uiScreen = Resources.Load<Canvas>(UtilityFunctions.UI_FILE_PATH + uiFileName);
        Canvas instantiatedUIScreen = GameObject.Instantiate(uiScreen, new Vector3(0, 0, 0), Quaternion.identity);
        instantiatedUIScreen.transform.SetParent(uiContainer.transform, false);
    }

    protected void SelectButton(GameObject uiContainer)
    {
        Button b = uiContainer.GetComponentInChildren<Button>();

        EventSystem uiEventHandler = UtilityFunctions.GetActiveEventSystem();
        if (b != null)
        {
            b.Select();
        }
    }
}
