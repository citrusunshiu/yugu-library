using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TileIndicators;
using UnityEngine;
using UnityEngine.Tilemaps;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Controllers
    {
        /// <summary>
        /// Tracks and manages the positions and collision of <see cref="OverworldObject"/> and <see cref="Hitbox"/> objects in an 
        /// <see cref="Instance"/>.
        /// </summary>
        public class UnitDetector
        {
            #region Variables
            /// <summary>
            /// Reference to the controller the unit detector is attached to.
            /// </summary>
            private MonoBehaviour controllerReference;

            /// <summary>
            /// Reference to the tilemap used for overworld object movement and collision.
            /// </summary>
            private Tilemap geography;

            /// <summary>
            /// Reference to the tilemap used for colored tile indicators.
            /// </summary>
            private Tilemap indicators;

            /// <summary>
            /// References to all <see cref="UnitSpawner"/> objects in the current instance.
            /// </summary>
            private List<UnitSpawner> unitSpawners = new List<UnitSpawner>();

            /// <summary>
            /// References to all <see cref="LoadingZone"/> objects in the current instance.
            /// </summary>
            private List<LoadingZone> loadingZones = new List<LoadingZone>();

            /// <summary>
            /// References to all <see cref="OverworldObject"/> objects in the current instance.
            /// </summary>
            private List<OverworldObject> overworldObjects = new List<OverworldObject>();

            /// <summary>
            /// References to all <see cref="Hitbox"/> objects in the current instance.
            /// </summary>
            private List<Hitbox> hitboxes = new List<Hitbox>();

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
            /// Whether or not <see cref="Unit"/> objects will automatically receive new <see cref="UnitAIAction"/> commands to execute.
            /// </summary>
            private bool automaticAIActivity = true;
            #endregion

            #region Constructors
            /// <summary>
            /// Constructs a unit detector that uses a specified tilemap for collision checking.
            /// </summary>
            /// <param name="geography">Tthe tilemap used to determine overworld object movement and collision.</param>
            /// <param name="indicators">The tilemap used for colored tile indicators.</param>
            /// <param name="controllerReference">The controller the unit detector is attached to</param>
            public UnitDetector(Tilemap geography, Tilemap indicators, MonoBehaviour controllerReference)
            {
                this.geography = geography;
                this.indicators = indicators;
                this.controllerReference = controllerReference;
            }
            #endregion

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

                if(overworldObject is Unit)
                {
                    CheckHitboxesAtUnit((Unit)overworldObject);
                }
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

            /// <summary>
            /// Marks instance tiles with a specified tile indicator type.
            /// </summary>
            /// <param name="tilesToMark">The tiles to be marked with indicators.</param>
            /// <param name="indicatorType">The type of indicator to mark the tiles with.</param>
            public void AddIndicatorTiles(List<Vector3Int> tilesToMark, TileIndicatorTypes indicatorType)
            {
                if (indicatorTiles.ContainsKey(indicatorType))
                {
                    indicatorTiles[indicatorType] = tilesToMark;
                }
                else
                {
                    indicatorTiles.Add(indicatorType, tilesToMark);
                }

                MarkIndicatorTiles();
            }

            /// <summary>
            /// Removes all indicator tiles of a given type.
            /// </summary>
            /// <param name="indicatorType">The type of indicator tile to remove.</param>
            public void RemoveIndicatorTiles(TileIndicatorTypes indicatorType)
            {
                if (indicatorTiles.ContainsKey(indicatorType))
                {
                    indicatorTiles.Remove(indicatorType);
                }

                MarkIndicatorTiles();
            }

            /// <summary>
            /// Removes indicator tiles of all types from the current instance.
            /// </summary>
            public void WipeIndicatorTiles()
            {
                indicators.ClearAllTiles();
                indicatorTiles = new Dictionary<TileIndicatorTypes, List<Vector3Int>>();
            }

            /// <summary>
            /// Sets the value for whether or not non-playable units should automatically receive AI commands.
            /// </summary>
            /// <param name="automaticAIActivity"></param>
            public void SetAutomaticAIActivity(bool automaticAIActivity)
            {
                this.automaticAIActivity = automaticAIActivity;
            }

            /// <summary>
            /// Assigns unit AI commands to units that currently have none, and Runs unit AI commands for any idle units.
            /// </summary>
            public void AssignAndExecuteUnitAIs()
            {
                if (automaticAIActivity)
                {
                    AssignUnitAIs();
                    ExecuteUnitAIs();
                }
            }

            #region Instance Functions
            /// <summary>
            /// Changes the current unit detector instance.
            /// </summary>
            /// <param name="instance">The instance that the unit detector will change to.</param>
            /// <param name="spawnPosition">The position where the player will be placed at.</param>
            public void LoadNewInstance(Instance instance, Vector3Int spawnPosition, OverworldObject playerUnit)
            {
                SwapGeography(instance.GetGeographyName());

                unitSpawners = new List<UnitSpawner>();
                if (instance.GetUnitSpawners() != null)
                {
                    foreach (UnitSpawner unitSpawner in instance.GetUnitSpawners())
                    {
                        AddUnitSpawner(unitSpawner);
                    }
                }

                loadingZones = new List<LoadingZone>();
                if (instance.GetLoadingZones() != null)
                {
                    foreach (LoadingZone loadingZone in instance.GetLoadingZones())
                    {
                        loadingZones.Add(loadingZone);
                    }
                }

                MarkLoadingZones();

                UtilityFunctions.GetActiveGeology().SetCurrentInstance(instance);

                UtilityFunctions.GetActiveUnitDetector().SpawnOverworldObject(playerUnit, spawnPosition);
                UtilityFunctions.GetActivePlayer().SetCurrentOverworldObject(playerUnit);
                //UtilityFunctions.SetSpriteDefaultPosition(playerUnit.overworldObjectCoordinator);
            }

            /// <summary>
            /// Adds a unit spawneer to the current instance, and spawns the maximum allowed number of units.
            /// </summary>
            /// <param name="unitSpawner">UnitSpawner object to add.</param>
            public void AddUnitSpawner(UnitSpawner unitSpawner)
            {
                unitSpawners.Add(unitSpawner);
                unitSpawner.SpawnAllUnits();
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

                if (overworldObject.canTurn)
                {
                    overworldObject.direction = moveDirection;
                }

                if (IsTileOpen(newPosition, false) && !overworldObject.isStationary && !geography.HasTile(newPosition) && highestFloorTile.z != -255)
                {
                    Vector3Int oldPosition = overworldObject.position;
                    Vector3Int tileToPlace = newPosition;

                    overworldObject.position = tileToPlace;


                    DelayMovements(overworldObject, z);
                    if(overworldObject is Unit)
                    {
                        CheckHitboxesAtUnit((Unit)overworldObject);
                    }

                    //if unit moved was the player, check for loading zones
                    if (UtilityFunctions.GetActivePlayer().GetCurrentOverworldObject().Equals(overworldObject))
                    {
                        LoadingZone loadingZone = GetLoadingZoneAtPosition(overworldObject.position);

                        if (loadingZone != null /*&& !UtilityFunctions.GetActiveEncounter().IsEncounterActive()*/)
                        {
                            loadingZone.LoadZone(overworldObject);
                        }
                    }

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Finds the shortest traversable path from one location to another.
            /// </summary>
            /// <param name="unit">Unit attempting to travel.</param>
            /// <param name="to">Location to pathfind to.</param>
            /// <returns>Returns the direction for the unit to move to next to reach the desired location, or returns 
            /// <see cref="Directions.None"/> if there is none.</returns>
            public Directions PathfindTo(Unit unit, Vector3Int to)
            {
                Vector3Int from = unit.position;

                List<AStarNode> openList = new List<AStarNode>();
                List<AStarNode> closedList = new List<AStarNode>();

                AStarNode start = new AStarNode(null, from, 0, 0);

                openList.Add(start);

                AStarNode endNode = null;

                int passCount = 0;
                //keep searching for route
                while (openList.Count > 0)
                {
                    passCount++;

                    AStarNode shortestNode = null;

                    for (int i = 0; i < openList.Count; i++)
                    {
                        AStarNode node = openList[i];
                        if (shortestNode == null || shortestNode.GetTotalDistance() > node.GetTotalDistance())
                        {
                            shortestNode = node;
                        }
                    }


                    openList.Remove(shortestNode);

                    List<AStarNode> neighbours = new List<AStarNode>();

                    List<Vector3Int> positions = GetSurroundingTiles(unit, shortestNode.position);

                    //find valid tiles of movement for the unit
                    foreach (Vector3Int position in positions)
                    {
                        AStarNode n = new AStarNode(shortestNode, position, shortestNode.GetDistanceFromStart() + 1,
                            CalculateTileDistance(position, to));

                        neighbours.Add(n);
                    }

                    //checking for endpoint, adding valid tiles to openList for next pass
                    foreach (AStarNode neighbour in neighbours)
                    {
                        if (UtilityFunctions.CompareVector3Ints(neighbour.position, to))
                        {
                            endNode = neighbour;
                            break;
                        }

                        bool shouldSkip = false;

                        for (int i = 0; i < openList.Count; i++)
                        {
                            if (UtilityFunctions.CompareVector3Ints(neighbour.position, openList[i].position)
                                && neighbour.GetTotalDistance() >= openList[i].GetTotalDistance())
                            {
                                shouldSkip = true;
                            }
                        }

                        for (int i = 0; i < closedList.Count; i++)
                        {
                            if (UtilityFunctions.CompareVector3Ints(neighbour.position, closedList[i].position)
                                && neighbour.GetTotalDistance() >= closedList[i].GetTotalDistance())
                            {
                                shouldSkip = true;
                            }
                        }

                        if (shouldSkip)
                        {
                            continue;
                        }
                        else
                        {
                            openList.Add(neighbour);
                        }
                    }

                    if (endNode != null)
                    {
                        break;
                    }
                    else
                    {
                        closedList.Add(shortestNode);
                    }
                }

                //if there is a path, figure out how to move the unit 1 step closer
                if (endNode != null)
                {
                    AStarNode firstMovement = endNode;

                    while (firstMovement.parent != null)
                    {
                        AStarNode temp = firstMovement.parent;

                        if (temp.parent != null)
                        {
                            firstMovement = firstMovement.parent;
                        }
                        else
                        {
                            break;
                        }
                    }

                    Vector3Int moveTo = firstMovement.position;

                    //TODO: need to adjust for z-y offset;
                    int dx = unit.position.x - moveTo.x;
                    int dy = unit.position.y - moveTo.y;
                    int dz = unit.position.z - moveTo.z;

                    if (dx > 0)
                    {
                        return Directions.SW;
                    }
                    else if (dx < 0)
                    {
                        return Directions.NE;
                    }

                    if (dy > 0)
                    {
                        return Directions.SE;
                    }
                    else if (dy < 0)
                    {
                        return Directions.NW;
                    }

                    return Directions.Up;
                }
                else
                {
                    Debug.Log("no path found");
                    return Directions.None;
                }
            }

            /// <summary>
            /// Teleports a given <see cref="OverworldObject"/> to specified location.
            /// </summary>
            /// <param name="overworldObject">OverworldObject attempting to travel.</param>
            /// <param name="to">Location to travel to.</param>
            /// <returns>Returns true if the overworld object moved successfully, and false otherwise.</returns>
            public bool BlinkTo(OverworldObject overworldObject, Vector3Int to, bool ignoreUnits)
            {
                bool valid = false;
                if (ignoreUnits)
                {
                    valid = IsTileOpen(to, true);
                }
                else
                {
                    valid = IsTileOpen(to, false);
                }

                if (valid)
                {
                    Vector3Int oldPosition = overworldObject.position;
                    overworldObject.position = to;
                    SetSurroundingTileOpacity(oldPosition, overworldObject.position);
                    UtilityFunctions.SetSpriteDefaultPosition(overworldObject.overworldObjectCoordinator);
                    if (overworldObject == UtilityFunctions.GetActivePlayer().GetCurrentOverworldObject())
                    {
                        overworldObject.overworldObjectCoordinator.SetCameraPosition();
                    }

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns a list of all floor tiles at a given position.
            /// </summary>
            /// <param name="position">The position to search for tiles at.</param>
            /// <param name="minZ">The minimum Z-axis value to search through.</param>
            /// <param name="maxZ">The maximum Z-axis value to search through.</param>
            /// <returns>Returns a List of all positions where a tile was located.</returns>
            public List<Vector3Int> GetAllTilesAtPosition(Vector3Int position, int minZ, int maxZ)
            {
                List<Vector3Int> tiles = new List<Vector3Int>();

                for (int i = minZ; i <= maxZ; i++)
                {
                    Vector3Int tile = new Vector3Int(position.x - i, position.y - i, position.z + i);
                    if (IsTileOpen(tile, true))
                    {
                        tiles.Add(tile);
                    }
                }

                return tiles;
            }

            /// <summary>
            /// Returns all tiles surrounding a given position within the range of 10 tiles below and n tiles above, 
            /// where n = <see cref="OverworldObject.jumpHeight"/>.
            /// </summary>
            /// <param name="unit">The unit whose jump height should be checked.</param>
            /// <param name="position">The position to check for surrounding tiles at.</param>
            /// <returns>Returns a list of <see cref="Vector3Int"/> objects corresponding to all found surrounding tile locations.</returns>
            private List<Vector3Int> GetSurroundingTiles(Unit unit, Vector3Int position)
            {
                //when looking for surrounding tiles, should search 10 tiles down and unit.jumpheight tiles up
                List<Vector3Int> surroundingTiles = new List<Vector3Int>();

                Vector3Int nwTile = new Vector3Int(position.x, position.y + 1, position.z);
                Vector3Int neTile = new Vector3Int(position.x + 1, position.y, position.z);
                Vector3Int swTile = new Vector3Int(position.x - 1, position.y, position.z);
                Vector3Int seTile = new Vector3Int(position.x, position.y - 1, position.z);

                surroundingTiles.AddRange(GetAllTilesAtPosition(nwTile, 0, 0));
                surroundingTiles.AddRange(GetAllTilesAtPosition(neTile, 0, 0));
                surroundingTiles.AddRange(GetAllTilesAtPosition(swTile, 0, 0));
                surroundingTiles.AddRange(GetAllTilesAtPosition(seTile, 0, 0));

                return surroundingTiles;
            }


            /// <summary>
            /// Lowers the opacity of the tiles surrounding a unit's current position after they move.
            /// </summary>
            /// <param name="oldPosition">The unit's position before moving.</param>
            /// <param name="newPosition">The unit's position after moving.</param>
            /// <remarks>Sets the tile opacity around the unit's old tile back to normal if there are no other units nearby.</remarks>
            private void SetSurroundingTileOpacity(Vector3Int oldPosition, Vector3Int newPosition)
            {
                //remove opacity from old position; add it to new position
                //Debug.Log("TODO: set surrounding tile opacity");
            }

            /// <summary>
            /// Calculates the distance between 2 tiles, ignoring terrain and including diagonals.
            /// </summary>
            /// <param name="tile1">The first tile to check distance between.</param>
            /// <param name="tile2">The second tile to check distance between.</param>
            /// <returns>Returns an integer equal to the number of tiles required for tile1 to connect to tile2.</returns>
            private static int CalculateTileDistance(Vector3Int tile1, Vector3Int tile2)
            {
                int dx = (tile1.x > tile2.x) ? tile1.x - tile2.x : tile2.x - tile1.x;
                int dy = (tile1.y > tile2.y) ? tile1.y - tile2.y : tile2.y - tile1.y;

                return dx + dy;
            }

            /// <summary>
            /// Checks if a unit or terrain is at a specified location.
            /// </summary>
            /// <param name="position">Location to check</param>
            /// <param name="ignoreUnits">Whether or not units should be ignored while performing tile check.</param>
            /// <returns>Returns true if no terrain or units present, and false otherwise.</returns>
            public bool IsTileOpen(Vector3Int position, bool ignoreUnits)
            {
                // TODO: tilebelow check might break dropping off of the floor; check later n see
                //Vector3Int tileBelow = new Vector3Int(position.x + 1, position.y + 1, position.z - 1);

                Vector3Int tileBelow = GetHighestFloorTileAtPosition(position);

                if (ignoreUnits)
                {
                    if (geography.HasTile(tileBelow) && !geography.HasTile(position))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (geography.HasTile(tileBelow) && !geography.HasTile(position) && GetOverworldObjectsAtPosition(position).Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            /// <summary>
            /// Checks if a unit is currently in the air.
            /// </summary>
            /// <param name="unit">Unit to check.</param>
            /// <returns>Returns true if the unit is airborne, and false otherwise.</returns>
            public bool IsAirborne(OverworldObject overworldObject)
            {
                Vector3Int currentPosition = overworldObject.position;

                currentPosition.x++;
                currentPosition.y++;
                currentPosition.z--;
                if (geography.HasTile(currentPosition))
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Gets the <see cref="LoadingZone"/> at a given position, if it exists.
            /// </summary>
            /// <param name="position">The position of the LoadingZone.</param>
            /// <returns>Returns the LoadingZone at the location, or null if there is none.</returns>
            private LoadingZone GetLoadingZoneAtPosition(Vector3Int position)
            {
                foreach (LoadingZone loadingZone in loadingZones)
                {
                    if (UtilityFunctions.CompareVector3Ints(loadingZone.position, position))
                    {
                        return loadingZone;
                    }
                }
                return null;
            }

            /// <summary>
            /// Adds a hitbox to the unit detector, and begins timers for hitbox arming, activation, lingering, and removal.
            /// </summary>
            /// <param name="hitbox">The hitbox to be placed.</param>
            public void PlaceHitbox(Hitbox hitbox)
            {
                hitboxes.Add(hitbox);
                controllerReference.StartCoroutine(ProgressHitboxInterval(hitbox));
            }

            /// <summary>
            /// Increments a given <see cref="Hitbox"/> object's frame progress, and enables or disables the hitbox depending on the 
            /// Hitbox object's specifications.
            /// </summary>
            /// <param name="hitbox">The Hitbox object to progress the frame of.</param>
            /// <param name="frameProgress">The Hitbox object's current frame progress.</param>
            /// <returns></returns>
            private IEnumerator ProgressHitboxInterval(Hitbox hitbox, int frameProgress = 0)
            {
                yield return new WaitForSeconds(UtilityFunctions.FRAME_LENGTH);

                frameProgress++;
                ProgressHitboxVisual(hitbox, frameProgress);

                if(frameProgress == hitbox.GetLingerFrames())
                {
                    hitbox.SetIsActive(true);
                    CheckUnitsAtHitbox(hitbox);
                }

                if(frameProgress < hitbox.GetDelayFrames() + hitbox.GetLingerFrames())
                {
                    RepeatHitboxInterval(hitbox, frameProgress);
                }
                else
                {
                    hitbox.SetIsActive(false);
                    hitboxes.Remove(hitbox);
                }
            }

            /// <summary>
            /// Starts a new coroutine to rerun <see cref="ProgressHitboxInterval(Hitbox, int)"/>.
            /// </summary>
            /// <param name="hitbox">The Hitbox object to progress the frame of.</param>
            /// <param name="frameProgress">The Hitbox object's current frame progress.</param>
            private void RepeatHitboxInterval(Hitbox hitbox, int frameProgress)
            {
                controllerReference.StartCoroutine(ProgressHitboxInterval(hitbox, frameProgress));
            }

            /// <summary>
            /// Adjusts the color of a <see cref="Hitbox"/> object's tile indicator dependent on the frame progression of the hitbox, and 
            /// the hitbox's specified activation and ending timings.
            /// </summary>
            /// <param name="hitbox">The Hitbox object to progress the visual of.</param>
            /// <param name="frameProgress">The Hitbox object's current frame progress.</param>
            private void ProgressHitboxVisual(Hitbox hitbox, int frameProgress)
            {
                Vector3Int adjustedPosition = hitbox.GetPosition();
                adjustedPosition.z--;
                adjustedPosition.x++;
                adjustedPosition.y++;

                TileBase hitboxTile = ScriptableObject.CreateInstance<HitboxTile>();
                if (!indicators.HasTile(adjustedPosition))
                {
                    indicators.SetTile(adjustedPosition, hitboxTile);
                    indicators.SetTileFlags(adjustedPosition, TileFlags.None);
                }

                Color white = new Color(255F/255F, 255F/255F, 255F/255F, 0.5F);
                Color yellow = new Color(255F/255F, 234F/255F, 0F/255F, 0.5F);
                Color orange = new Color(255F/255F, 128F/255F, 0F/255F, 0.5F);
                Color red = new Color(255F/255F, 0F/255F, 0F/255F, 0.5F);
                Color magenta = new Color(255F/255F, 0F/255F, 191F/255F, 0.5F);

                if (frameProgress < hitbox.GetDelayFrames()/2) // white > yellow
                {
                    //indicators.SetColor(hitbox.GetPosition(), new Color());
                }
                else if(frameProgress < hitbox.GetDelayFrames()) // yellow > orange
                {
                    //indicators.SetColor(hitbox.GetPosition(), new Color());
                }
                else if(frameProgress < hitbox.GetDelayFrames() + (hitbox.GetLingerFrames() / 2)) // red > magenta
                {
                    indicators.SetColor(adjustedPosition, red);
                }
                else if(frameProgress < hitbox.GetDelayFrames() + hitbox.GetLingerFrames()) // magenta > white
                {
                    //indicators.SetColor(hitbox.GetPosition(), new Color());
                }
                else // remove
                {
                    indicators.SetTile(adjustedPosition, null);
                }
            }

            /// <summary>
            /// Executes a given hitbox on all units at its location.
            /// </summary>
            /// <param name="hitbox">The hitbox to be executed.</param>
            private void CheckUnitsAtHitbox(Hitbox hitbox)
            {
                //Debug.Log("searching for units at " + hitbox.GetPosition());
                List<OverworldObject> overworldObjects = GetOverworldObjectsAtPosition(hitbox.GetPosition());
                foreach (OverworldObject overworldObject in overworldObjects)
                {
                    if (overworldObject is Unit)
                    {
                        //Debug.Log("found: " + overworldObject);
                        hitbox.ExecuteHitbox((Unit)overworldObject);
                    }
                }
            }

            /// <summary>
            /// Executes all hitboxes at a given unit's position.
            /// </summary>
            /// <param name="unit">The unit to be affected.</param>
            private void CheckHitboxesAtUnit(Unit unit)
            {
                foreach(Hitbox hitbox in hitboxes)
                {
                    if(UtilityFunctions.CompareVector3Ints(hitbox.GetPosition(), unit.GetPosition()))
                    {
                        hitbox.ExecuteHitbox(unit);
                    }
                }
            }

            /// <summary>
            /// Gets all <see cref="OverworldObject"/> objects at a given position.
            /// </summary>
            /// <param name="position">The posititon to get all OverworldObjects at.</param>
            /// <returns>Returns a list of all OverworldObjects at the given position.</returns>
            public List<OverworldObject> GetOverworldObjectsAtPosition(Vector3Int position)
            {
                List<OverworldObject> owObjects = new List<OverworldObject>();

                for(int i = 0; i < overworldObjects.Count; i++)
                {
                    if(UtilityFunctions.CompareVector3Ints(overworldObjects[i].GetPosition(), position))
                    {
                        owObjects.Add(overworldObjects[i]);
                    }
                }

                return owObjects;
            }
            #endregion

            /// <summary>
            /// Spawns all units from a given <see cref="UnitSetup"/> relative to a given reference tile and rotation (specified by 
            /// direction).
            /// </summary>
            /// <remarks>
            /// The position that the units are placed relative to the reference tile will be rotated or flipped dependent 
            /// on the specified direction. UnitSetup positions are assumed to be facing <see cref="Directions.NE"/> by default.
            /// </remarks>
            /// <param name="unitSetup">The UnitSetup object containing the list of units to spawn and their relative positions.</param>
            /// <param name="centerTile">The position to place the units relative to.</param>
            /// <param name="direction">The rotation of the UnitSetup.</param>
            public void LoadUnitSetup(UnitSetup unitSetup, Vector3Int centerTile, Directions direction)
            {
                Dictionary<string, Vector3Int> units = unitSetup.GetUnitSetup(direction, centerTile);

                foreach (string unitJSONFileName in units.Keys)
                {
                    //50 is placeholder
                    
                    Unit allyUnit = new Unit(unitJSONFileName, 50, TargetTypes.Ally);

                    if (IsTileOpen(units[unitJSONFileName], false))
                    {
                        SpawnOverworldObject(allyUnit, units[unitJSONFileName]);
                    }
                    else
                    {
                        Debug.Log("tile already occupied; come back later to expand breadth search");
                        SpawnOverworldObject(allyUnit, units[unitJSONFileName]);
                    }

                }
            }

            /// <summary>
            /// Removes allied units spawned via unit setup from the field.
            /// </summary>
            public void RemoveUnitSetup()
            {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="flag"></param>
            /// <param name="hookBundle"></param>
            /// <param name="unitsAlerted"></param>
            public void FlagDelegate(DelegateFlags flag, HookBundle hookBundle, List<Unit> unitsAlerted)
            {
                foreach(Unit unit in unitsAlerted)
                {
                    unit.ExecuteDelegates(flag, hookBundle);
                }
            }

            /// <summary>
            /// Gets all <see cref="Unit"/> objects in the unit detector.
            /// </summary>
            /// <returns>Returns a list of all Unit objects in the unit detector.</returns>
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

                Vector3Int pos = position;
                Vector3Int highestFloorTile = new Vector3Int(-255, -255, -255);


                int i;
                for (i = 0; i < 10; i++)
                {
                    pos.x++;
                    pos.y++;
                    pos.z--;
                    if (geography.HasTile(pos))
                    {
                        highestFloorTile = pos;
                        break;
                    }
                }

                return highestFloorTile;
            }

            /// <summary>
            /// Prevents movement from an <see cref="OverworldObject"/> for an amount of time specified by 
            /// <see cref="OverworldObject.GetMovementDelay"/>, <see cref="OverworldObject.ascentDelay"/>, or 
            /// <see cref="OverworldObject.descentDelay"/>, dependent on how the overworld object moved.
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

            /// <summary>
            /// Changes the tilemap referenced by <see cref="geography"/> to a new specified tilemap.
            /// </summary>
            /// <param name="name">The prefab name of the tilemap to swap to.</param>
            private void SwapGeography(string name)
            {
                if (geography != null)
                {
                    geography.ClearAllTiles();
                    geography.transform.SetParent(null);
                    geography.name += "(OLD)";
                }

                GameObject.Instantiate(Resources.Load(UtilityFunctions.GEOGRAPHY_FILE_PATH + name));

                geography = GameObject.Find(name + "(Clone)").GetComponentInChildren<Tilemap>();
                geography.gameObject.transform.parent.gameObject.name = "trashed";
                geography.transform.SetParent(GameObject.Find("Controller Hub").transform);
            }

            /// <summary>
            /// Replaces the currently displayed indicator tiles with the values currently stored in <see cref="indicatorTiles"/>.
            /// </summary>
            private void MarkIndicatorTiles()
            {
                indicators.ClearAllTiles();

                for (int i = 0; i < Enum.GetNames(typeof(TileIndicatorTypes)).Length; i++)
                {
                    TileIndicatorTypes indicatorType = (TileIndicatorTypes)i;

                    if (indicatorTiles.ContainsKey(indicatorType))
                    {
                        List<Vector3Int> tilesToMark = indicatorTiles[indicatorType];

                        TileBase tileBase = GetTileFromIndicator(indicatorType);

                        foreach (Vector3Int tile in tilesToMark)
                        {

                            Vector3Int placement = new Vector3Int(tile.x + 1, tile.y + 1, tile.z - 1);

                            if (geography.HasTile(placement))
                            {
                                indicators.SetTile(placement, tileBase);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            /// <summary>
            /// Gets the tile associated with a given <see cref="TileIndicatorTypes"/> value.
            /// </summary>
            /// <param name="indicatorType">The tile indicator type to get the tile of.</param>
            /// <returns>Returns the tile associated with the given TileIndicatorTypes value.</returns>
            private TileBase GetTileFromIndicator(TileIndicatorTypes indicatorType)
            {
                switch (indicatorType)
                {
                    case TileIndicatorTypes.Movement:
                        return ScriptableObject.CreateInstance<MovementIndicator>();
                    case TileIndicatorTypes.LoadingZone:
                        return ScriptableObject.CreateInstance<LoadingZoneIndicator>();
                    case TileIndicatorTypes.Aggro:
                        return ScriptableObject.CreateInstance<AggroIndicator>();
                    case TileIndicatorTypes.Encounter:
                        return ScriptableObject.CreateInstance<EncounterIndicator>();
                    case TileIndicatorTypes.AllyTargetingSkill:
                        return ScriptableObject.CreateInstance<AllyTargetingSkillIndicator>();
                    case TileIndicatorTypes.EnemyTargetingSkill:
                        return ScriptableObject.CreateInstance<EnemyTargetingSkillIndicator>();
                    default:
                        return ScriptableObject.CreateInstance<MovementIndicator>();
                }
            }

            /// <summary>
            /// Adds the position of the current instance's loading zones to <see cref="indicatorTiles"/>.
            /// </summary>
            private void MarkLoadingZones()
            {
                List<Vector3Int> loadingZoneTiles = new List<Vector3Int>();
                foreach (LoadingZone loadingZone in loadingZones)
                {
                    loadingZoneTiles.Add(loadingZone.position);
                }

                AddIndicatorTiles(loadingZoneTiles, TileIndicatorTypes.LoadingZone);
            }

            /// <summary>
            /// Iterates through all <see cref="Unit"/> objects in the current instance, and runs their unit AI function to generate a 
            /// <see cref="UnitAIAction"/> to execute, if one is not already present on the unit.
            /// </summary>
            private void AssignUnitAIs()
            {

            }

            /// <summary>
            /// Iterates through all <see cref="Unit"/> objects in the current instance, and executes their current 
            /// <see cref="UnitAIAction"/>, if one is not already being executed for the unit.
            /// </summary>
            private void ExecuteUnitAIs()
            {

            }
            #endregion
        }
    }
}
