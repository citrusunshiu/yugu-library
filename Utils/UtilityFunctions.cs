using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Controllers;
using Mono.Data.Sqlite;
using System;
using YuguLibrary.Models;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UIConfirgurations;

namespace YuguLibrary
{
    namespace Utilities
    {
        public static class UtilityFunctions
        {
            public static float FRAME_LENGTH = 0.016667F;

            /// <summary>
            /// Random number generator for functions.
            /// </summary>
            private static System.Random random = new System.Random();

            private static PlayerFile currentFile;

            #region Valid KeyCode Values
            /// <summary>
            /// The list of valid keyboard keys for rmapping <see cref="OverworldObjectActions"/> to.
            /// </summary>
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
            /// <summary>
            /// Path to the project's string database to retrieve text from.
            /// </summary>
            public static readonly string SQLITE_LOCALIZATION_DB_FILE_PATH = Application.dataPath + "/Databases/text.db";

            /// <summary>
            /// Path to the project's root folder that stores all JSON assets.
            /// </summary>
            public static readonly string JSON_ASSETS_FILE_PATH = Application.dataPath + "/JSONsHubs/JSON Assets/";

            /// <summary>
            /// Path to the project's root folder that stores JSON assets for the <see cref="Unit"/> class.
            /// </summary>
            public static readonly string JSON_ASSETS_UNIT_FOLDER_PATH = JSON_ASSETS_FILE_PATH + "/Units/";

            /// <summary>
            /// Path to the project's root folder that stores JSON assets for the <see cref="Instance"/> class.
            /// </summary>
            public static readonly string JSON_ASSETS_INSTANCE_FOLDER_PATH = JSON_ASSETS_FILE_PATH + "/Instances/";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string JSON_ASSETS_COMMON_SKILL_FOLDER_PATH = JSON_ASSETS_FILE_PATH + "/Common Skills/";

            /// <summary>
            /// Path to the project's root folder that stores JSON assets for the <see cref="Status"/> class.
            /// </summary>
            public static readonly string JSON_ASSETS_COMMON_STATUS_FOLDER_PATH = JSON_ASSETS_FILE_PATH + "/Common Statuses/";

            /// <summary>
            /// Path to the project's root folder that stores JSON assets for the <see cref="Cutscene"/> class.
            /// </summary>
            public static readonly string JSON_ASSETS_CUTSCENE_FOLDER_PATH = JSON_ASSETS_FILE_PATH + "/Cutscenes/";

            /// <summary>
            /// Path to the project's root folder that stores UI screens.
            /// </summary>
            public static readonly string UI_FILE_PATH = "Prefabs/UI/";

            /// <summary>
            /// Path to the project's root folder that stores sprites for the UI.
            /// </summary>
            public static readonly string UI_SPRITES_FILE_PATH = "Sprites/UI/";
            
            /// <summary>
            /// Path to the project's root folder that stores UI icons.
            /// </summary>
            public static readonly string ICONS_FILE_PATH = UI_SPRITES_FILE_PATH + "Icons/";

            /// <summary>
            /// Path to the project's root folder that stores UI borders.
            /// </summary>
            public static readonly string BORDERS_FILE_PATH = SPRITES_FILE_PATH + "UI/Borders/";
            
            
            /// <summary>
            /// Path to the folder that stores the tilemaps used for instances.
            /// </summary>
            public static readonly string GEOGRAPHY_FILE_PATH = "Prefabs/Geographies/";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string UNIT_SPRITESHEET_FILE_PATH = "Sprites/Units/";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string SPRITES_FILE_PATH = "Sprites/";

            /// <summary>
            /// 
            /// </summary>
            public static readonly string LOCKED_ICON_FILE_PATH = SPRITES_FILE_PATH + "/Misc/lock";
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
            /// Gets the encounter currently active in the scene.
            /// </summary>
            /// <returns>Returns the Encounter object from the current scene.</returns>
            public static Encounter GetActiveEncounter()
            {
                GameObject controllerHub = GameObject.Find("Controller Hub");
                Encounter encounter = controllerHub.GetComponent<EncounterController>().encounter;
                return encounter;
            }

            /// <summary>
            /// Gets the UI manager currently active in the scene.
            /// </summary>
            /// <returns>Returns the UIManager object from the current scene.</returns>
            public static UIManager GetActiveUIManager()
            {
                GameObject controllerHub = GameObject.Find("Controller Hub");
                UIManager uiManager = controllerHub.GetComponent<UIController>().uiManager;
                return uiManager;
            }

            public static EventSystem GetActiveEventSystem()
            {
                GameObject controllerHub = GameObject.Find("Controller Hub");
                EventSystem eventSystem = controllerHub.GetComponent<UIController>().uiEventHandler;

                return eventSystem;
            }

            public static QuestManager GetActiveQuestManager()
            {
                GameObject controllerHub = GameObject.Find("Controller Hub");
                QuestManager questManager = controllerHub.GetComponent<QuestController>().questManager;
                return questManager;
            }

            public static Geology GetActiveGeology()
            {
                GameObject controllerHub = GameObject.Find("Controller Hub");
                Geology geology = controllerHub.GetComponent<GeologyController>().geology;
                return geology;
            }

            public static void SetCurrentFile(PlayerFile file)
            {
                currentFile = file;
            }

            public static PlayerFile GetCurrentFile()
            {
                return currentFile;
            }

            public static void LoadCurrentPlayerFile()
            {
                SceneManager.LoadScene("Instance");
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


                //for every x: x += 1.225, y += 0.6125; 
                xpos += (currentPosition.x * 1.225f);
                ypos += (currentPosition.x * 0.6125f);

                //for every y: x -= 1.225, y += 0.6125;
                xpos -= (currentPosition.y * 1.225f);
                ypos += (currentPosition.y * 0.6125f);
                float zpos = (currentPosition.z * 4f);

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

            /// <summary>
            /// Retrieves a string by its given ID from SQLite database.
            /// </summary>
            /// <param name="id">The ID of the string to retrieve.</param>
            /// <returns>Returns the string of the associated ID.</returns>
            public static string GetStringFromSQL(string id)
            {
                SqliteConnection connection = new SqliteConnection("URI=file:" + SQLITE_LOCALIZATION_DB_FILE_PATH);
                connection.Open();
                
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT text FROM Text WHERE textID='" + id + "'";

                SqliteDataReader reader = command.ExecuteReader();
                reader.Read();

                return reader.GetString(0);
            }

            /// <summary>
            /// Checks if 2 different <see cref="Vector3Int"/> objects have the same X, Y, and Z values.
            /// </summary>
            /// <param name="a">The first Vector3Int object to compare.</param>
            /// <param name="b">The second Vector3Int object to compare.</param>
            /// <returns>Returns true if both objects have the same X, Y, and Z values; returns false otherwise.</returns>
            public static bool CompareVector3Ints(Vector3Int a, Vector3Int b)
            {
                if (a.x == b.x && a.y == b.y && a.z == b.z)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Rolls a random integer between 1-100, and compares it against a specified passing rate.
            /// </summary>
            /// <param name="passChance">The success rate of the roll.</param>
            /// <returns>Returns true if passChance is higher than the number rolled, and false otherwise.</returns>
            public static bool GetRandomPercentage(int passChance)
            {

                int randint = random.Next(1, 101);

                if (passChance >= randint)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Returns the current Unix time.
            /// </summary>
            /// <returns>Returns the current Unix time.</returns>
            public static long UnixTimeNow()
            {
                var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                return (long)timeSpan.TotalSeconds;
            }

            public static string GetUnitName(UnitTurn unitTurn)
            {
                return unitTurn.unit.GetType() + " (" + unitTurn.GetHashCode() + ")";
            }
        }
    }
}
