﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Controllers;

namespace YuguLibrary
{
    namespace Utilities
    {
        public static class UtilityFunctions
        {
            #region Valid KeyCode Values
            public static KeyCode[] ValidKeyCodes =
            {
                KeyCode.Tilde,
                KeyCode.Alpha1,
                KeyCode.Alpha2,
                KeyCode.Alpha3,
                KeyCode.Alpha4,
                KeyCode.Tab,
                KeyCode.Q,
                KeyCode.W,
                KeyCode.E,
                KeyCode.R,
                KeyCode.CapsLock,
                KeyCode.A,
                KeyCode.S,
                KeyCode.D,
                KeyCode.F,
                KeyCode.LeftShift,
                KeyCode.Z,
                KeyCode.X,
                KeyCode.C,
                KeyCode.V,
                KeyCode.LeftControl,
                KeyCode.LeftAlt,
                KeyCode.Space,
            };
            #endregion

            #region File Paths
            public static readonly string JSON_ASSETS_FILE_PATH = Application.dataPath + "/Resources/JSON Assets/";
            public static readonly string JSON_ASSETS_UNIT_FOLDER_PATH = JSON_ASSETS_FILE_PATH + "/Units/";
            public static readonly string SPRITESHEET_FILE_PATH = "";
            public static readonly string UI_FILE_PATH = "Sprites/UI/";
            public static readonly string ICONS_FILE_PATH = UI_FILE_PATH + "Icons/";
            public static readonly string SPRITES_FILE_PATH = "Sprites/";
            public static readonly string BORDERS_FILE_PATH = SPRITES_FILE_PATH + "UI/Borders/";
            public static readonly string LOCKED_ICON_FILE_PATH = SPRITES_FILE_PATH + "/Misc/lock";
            public static readonly string GEOGRAPHY_FILE_PATH = "Prefabs/Geographies/";
            #endregion

            /// <summary>
            /// Gets the unit detector currently active in the scene.
            /// </summary>
            /// <returns>Returns the UnitDetector object from the current scene.</returns>
            public static UnitDetector GetActiveUnitDetector()
            {
                GameObject controllerHub = GameObject.Find("Controller Hub");
                UnitDetector unitDetector = controllerHub.GetComponent<UnitDetectorController>().unitDetector;
                return unitDetector;
            }

            /// <summary>
            /// Gets the player object currently active in the scene.
            /// </summary>
            /// <returns>Returns the Player object from the current scene.</returns>
            public static Player GetActivePlayer()
            {
                GameObject controllerHub = GameObject.Find("Controller Hub");
                Player player = controllerHub.GetComponent<PlayerController>().player;
                return player;
            }

            /// <summary>
            /// Sets a sprite to a position in the game world relative to its location.
            /// </summary>
            /// <param name="overworldObjectCoordinator">OverworldObjectCoordinator containing the sprite to be set.</param>
            public static void SetSpriteDefaultPosition(OverworldObjectCoordinator overworldObjectCoordinator)
            {
                Vector3Int currentPosition = overworldObjectCoordinator.overworldObject.position;
                float xpos = 0;
                float ypos = (currentPosition.z * 2.28f);

                //Debug.Log("x: " + currentPosition.x + "; y: " + currentPosition.y + "; z: " + currentPosition.z);

                //for every x: x += 1.225, y += 0.6125; if x is negative, equations are negated
                xpos += (currentPosition.x * 1.225f);
                ypos += (currentPosition.x * 0.6125f);

                //for every y: x -= 1.225, y += 0.6125; if y is negative, equations are negated
                xpos -= (currentPosition.y * 1.225f);
                ypos += (currentPosition.y * 0.6125f);
                float zpos = (currentPosition.z * 3.5f);

                Vector3 position = new Vector3(xpos, ypos, zpos);

                overworldObjectCoordinator.transform.position = position;
            }

            /// <summary>
            /// Subtracts the x and y positions of a <see cref="Vector3Int"/> by the z position to convert it for use
            /// with an isometric z-as-y tilemap.
            /// </summary>
            /// <param name="position">The Vector3Int to be converted.</param>
            /// <returns>Returns a new Vector3Int converted to z-as-y.</returns>
            public static Vector3Int TranslateZAsY(Vector3Int position)
            {
                Vector3Int translation = new Vector3Int(position.x, position.y, position.z);

                translation.x -= position.z;
                translation.y -= position.z;

                return translation;
            }
        }
    }
}