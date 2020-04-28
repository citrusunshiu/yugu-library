using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Controllers
    {
        public class UnitDetector
        {
            /// <summary>
            /// Reference to the controller the unit detector is attached to.
            /// </summary>
            MonoBehaviour controllerReference;

            /// <summary>
            /// Reference to the tilemap used for overworld object movement.
            /// </summary>
            private Tilemap geography;

            /// <summary>
            /// Reference to the tilemap used for colored tile indicators.
            /// </summary>
            private Tilemap indicators;

            /// <summary>
            /// Reference to the current instance loaded.
            /// </summary>
            private Instance currentInstance;
            
            /// <summary>
            /// References to all effect areas in the current instance.
            /// </summary>
            private List<AreaOfEffect> aoes = new List<AreaOfEffect>();

            /// <summary>
            /// References to all <see cref="UnitSpawner"/> objects in the current instance.
            /// </summary>
            private List<UnitSpawner> unitSpawners = new List<UnitSpawner>();

            /// <summary>
            /// References to all <see cref="LoadingZone"/> objects in the current instance.
            /// </summary>
            private List<LoadingZone> loadingZones = new List<LoadingZone>();

            private List<OverworldObject> overworldObjects = new List<OverworldObject>();

            /// <summary>
            /// References to all special floor tiles in the current instance.
            /// </summary>
            private Dictionary<Vector3Int, List<SpecialTile>> specialTiles
                = new Dictionary<Vector3Int, List<SpecialTile>>();
            
            /// <summary>
            /// References to the active indicator tiles in the current instance.
            /// </summary>
            private Dictionary<TileIndicatorTypes, List<Vector3Int>> indicatorTiles =
                new Dictionary<TileIndicatorTypes, List<Vector3Int>>();

            /// <summary>
            /// Whether or not overworld AI patterns are currently running.
            /// </summary>
            public static bool overworldAIActivity = true;

            /// <summary>
            /// Whether or not the <see cref="aggroRadius"/> object will move with a unit.
            /// </summary>
            public static bool lockAggroRadius = false;

            /// <summary>
            /// The default size of the <see cref="aggroRadius"/> object.
            /// </summary>
            public static int baseAggroRadiusSize = 5;
            
            /// <summary>
            /// The <see cref="AreaOfEffect"/> object that will attract enemy units to its center.
            /// </summary>
            public static AreaOfEffect aggroRadius;

            public UnitDetector(Tilemap geography, Tilemap indicators, MonoBehaviour controllerReference)
            {
                this.geography = geography;
                this.indicators = indicators;
                this.controllerReference = controllerReference;
            }

            #region Functions

            /// <summary>
            /// Adds an overworld object to the unit detector at a given position.
            /// </summary>
            /// <param name="overworldObject">The overworld object to be added.</param>
            /// <param name="position">The position to place the overworld object at.</param>
            public void SpawnOverworldObject(OverworldObject overworldObject, Vector3Int position)
            {
                overworldObject.position = position;
                overworldObjects.Add(overworldObject);
                UtilityFunctions.SetSpriteDefaultPosition(overworldObject.overworldObjectCoordinator);
            }

            /// <summary>
            /// Removes an overworld object from the unit detector.
            /// </summary>
            /// <param name="overworldObject">The overworld object to be removed.</param>
            public void RemoveOverworldObject(OverworldObject overworldObject)
            {
                overworldObjects.Remove(overworldObject);
                overworldObject.overworldObjectCoordinator.StopAllCoroutines();
                GameObject.Destroy(overworldObject.overworldObjectCoordinator.gameObject);
            }

            #region Instance Functions
            /// <summary>
            /// Changes the current unit detector instance.
            /// </summary>
            /// <param name="instance">The instance that the unit detector will change to.</param>
            /// <param name="spawnPosition">The position where the player will be placed at.</param>
            public void LoadNewInstance(Instance instance, Vector3Int spawnPosition)
            {
                SwapGeography(instance.GetGeographyName());
            }
            #endregion

            #region Spatial Awareness Functions
            /// <summary>
            /// Translates a unit a specified value in the X, Y and Z planes.
            /// </summary>
            /// <param name="unit">The unit whose position will be translated.</param>
            /// <param name="x">Amount of tiles to travel in the X axis.</param>
            /// <param name="y">Amount of tiles to travel in the Y axis.</param>
            /// <param name="z">Amount of tiles to travel in the Z axis.</param>
            /// <returns>Returns true if the overworld object moved successfully, and false otherwise.</returns>
            public bool Move(OverworldObject overworldObject, int x, int y, int z)
            {
                Vector3Int zyTranslation = UtilityFunctions.TranslateZAsY(new Vector3Int(x, y, z));
                x = zyTranslation.x;
                y = zyTranslation.y;

                Vector3Int currentPosition = overworldObject.position;
                Vector3Int newPosition = new Vector3Int(currentPosition.x + x, currentPosition.y + y, currentPosition.z + z);

                Vector3Int highestFloorTile = GetHighestFloorTileAtPosition(newPosition);

                /* If not ascending or descending but the floor isn't directly below the overworld object, drop it down */
                /*
                if(z == 0 && !overworldObject.isJumping && newPosition.x != highestFloorTile.x + 1)
                {
                    x++;
                    y++;
                    z--;

                    newPosition.x++;
                    newPosition.y++;
                    newPosition.z--;
                }
                */

                Directions moveDirection;

                if (Math.Abs(x) > Math.Abs(y))
                {
                    if (x >= 0)
                    {
                        moveDirection = Directions.NE;
                    }
                    else
                    {
                        moveDirection = Directions.SW;
                    }
                }
                else if (Math.Abs(x) < Math.Abs(y))
                {
                    if (y >= 0)
                    {
                        moveDirection = Directions.NW;
                    }
                    else
                    {
                        moveDirection = Directions.SE;
                    }
                }
                else
                {
                    moveDirection = overworldObject.direction;
                }

                if (!overworldObject.isStationary && !geography.HasTile(newPosition) && highestFloorTile.z != -255)
                //if(true)
                {
                    Vector3Int oldPosition = overworldObject.position;
                    Vector3Int tileToPlace = newPosition;

                    overworldObject.position = tileToPlace;
                    overworldObject.direction = moveDirection;

                    //UtilityFunctions.SetSpriteDefaultPosition(overworldObject.overworldObjectCoordinator);

                    DelayMovements(overworldObject, z);

                    return true;
                }

                return false;
            }
            
            /// <summary>
            /// Finds the shortest traversable path from one location to another.
            /// </summary>
            /// <param name="overworldObject">Overworld object attempting to travel.</param>
            /// <param name="to">Location to pathfind to.</param>
            /// <returns>Returns the direction for the unit to move to next to reach the desired location, or returns 
            /// <see cref="Directions.Down"/> if there is none.</returns>
            public Directions PathfindTo(OverworldObject overworldObject, Vector3Int to)
            {
                return Directions.Down;
            }

            /// <summary>
            /// Teleports a given <see cref="OverworldObject"/> to specified location.
            /// </summary>
            /// <param name="overworldObject">OverworldObject attempting to travel.</param>
            /// <param name="to">Location to travel to.</param>
            /// <returns>Returns true if the overworld object moved successfully, and false otherwise.</returns>
            public bool BlinkTo(OverworldObject overworldObject, Vector3Int to)
            {
                return true;
            }



            #endregion

            public void FlagDelegate(DelegateFlags flag, HookBundle hookBundle, List<Unit> unitsAlerted)
            {
                foreach(Unit unit in unitsAlerted)
                {
                    unit.ExecuteDelegates(flag, hookBundle);
                }
            }

            public List<Unit> GetAllUnits()
            {
                List<Unit> units = new List<Unit>();

                foreach(OverworldObject overworldObject in overworldObjects)
                {
                    if(overworldObject is Unit)
                    {
                        units.Add((Unit)overworldObject);
                    }
                }

                return units;
            }
            
            /// <summary>
            /// Gets the highest floor tile at a given position.
            /// </summary>
            /// <param name="position">The position to search at.</param>
            /// <returns>Returns the highest floor tile found at the given position, or [-255, -255, -255] if none was found.</returns>
            private Vector3Int GetHighestFloorTileAtPosition(Vector3Int position)
            {
                /* Checking for tiles below the target location to move; when moving down in the Z axis, X/Y values need to 
                 * be incremented to match
                 * */

                Vector3Int p = position;
                Vector3Int highestFloorTile = new Vector3Int(-255, -255, -255);


                int i;
                for (i = 0; i < 10; i++)
                {
                    p.x++;
                    p.y++;
                    p.z--;
                    if (geography.HasTile(p))
                    {
                        highestFloorTile = p;
                        break;
                    }
                }

                return highestFloorTile;
            }

            /// <summary>
            /// Prevents movement from an <see cref="OverworldObject"/> for a specified amount of time.
            /// </summary>
            /// <param name="overworldObject">The overworldObject to have its movement delayed.</param>
            /// <param name="z">The amount of tiles travelled in the Z-axis.</param>
            private void DelayMovements(OverworldObject overworldObject, int z)
            {
                if (z == 0)
                {
                    overworldObject.canMove = false;
                    controllerReference.StartCoroutine(overworldObject.UnlockMovement());
                }

                if (z > 0)
                {
                    overworldObject.canAscend = false;
                    controllerReference.StartCoroutine(overworldObject.UnlockAscent());
                }
                else if (z < 0)
                {
                    overworldObject.canDescend = false;
                    controllerReference.StartCoroutine(overworldObject.UnlockDescent());
                }
            }

            private void SwapGeography(string name)
            {
                if (geography != null)
                {
                    Debug.Log(geography.gameObject.transform.parent.gameObject.name);
                    geography.ClearAllTiles();
                    geography.transform.SetParent(null);
                    geography.name += "(OLD)";
                }

                GameObject.Instantiate(Resources.Load(UtilityFunctions.GEOGRAPHY_FILE_PATH + name));

                geography = GameObject.Find(name + "(Clone)").GetComponentInChildren<Tilemap>();
                geography.gameObject.transform.parent.gameObject.name = "trashed";
                geography.transform.SetParent(GameObject.Find("Controller Hub").transform);
            }
            #endregion
        }
    }
}
