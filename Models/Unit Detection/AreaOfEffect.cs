using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    namespace Models
    {
        public class AreaOfEffect
        {
            #region Variables
            /// <summary>
            /// Size of the effect area.
            /// </summary>
            Vector3Int[,] range;

            public int radius;

            /// <summary>
            /// The amount of tiles up/down that the area of effect will search when checking for tiles within its radius.
            /// </summary>
            int zRange;

            public Vector3Int center;

            /// <summary>
            /// Unit that the effect area belongs to.
            /// </summary>
            public Unit caster;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates an AreaOfEffect of a given size and shape at a specified location
            /// </summary>
            /// <param name="center">The center of the effect area.</param>
            /// <param name="radius">The radius of the effect area.</param>
            /// <param name="shape">The shape of the effect area.</param>
            public AreaOfEffect(Vector3Int center, int radius, int zRange = 5)
            {
                this.zRange = zRange;
                this.radius = radius;
                this.center = center;
                int r = (radius * 2) + 1;

                range = new Vector3Int[r, r];

                int yr = radius * -1;

                for (int i = 0; i < r; i++)
                {
                    int xr = radius * -1;
                    for (int j = 0; j < r; j++)
                    {
                        range[i, j] = new Vector3Int(center.x + xr, center.y + yr, center.z);
                        xr++;
                    }
                    yr++;
                }
            }
            #endregion

            #region Functions
            /// <summary>
            /// Translates the positions in a given AreaOfEffect's <see cref="range"/> by a given value.
            /// </summary>
            /// <param name="range">The range to be translated.</param>
            /// <param name="translation">The values to translate the range by.</param>
            public void TranslateAreaOfEffect(Vector3Int translation)
            {
                //z as y offset
                translation.x -= translation.z;
                translation.y -= translation.z;

                for (int i = 0; i < range.GetLength(0); i++)
                {
                    for (int j = 0; j < range.GetLength(1); j++)
                    {

                        range[i, j].x += translation.x;

                        range[i, j].y += translation.y;

                        range[i, j].z += translation.z;
                    }
                }

                center.x += translation.x;
                center.y += translation.y;
                center.z += translation.z;

                //TODO: translate center tile as well?
            }

            public List<Vector3Int> GetAllTiles()
            {
                //refactor to account for z range
                List<Vector3Int> allTiles = new List<Vector3Int>();

                for (int i = 0; i < range.GetLength(0); i++)
                {
                    for (int j = 0; j < range.GetLength(1); j++)
                    {
                        allTiles.AddRange(UtilityFunctions.GetActiveUnitDetector().GetAllTilesAtPosition(range[i, j], zRange * -1, zRange));
                    }
                }

                return allTiles;
            }

            /// <summary>
            /// Checks if a given location is within the effect area.
            /// </summary>
            /// <param name="location">The location to check for.</param>
            /// <returns>Returns true if the location is within the effect area, and false otherwise.</returns>
            public bool CheckForLocation(Vector3Int location)
            {
                List<Vector3Int> allTiles = GetAllTiles();

                foreach (Vector3Int tile in allTiles)
                {
                    if (UtilityFunctions.CompareVector3Ints(location, tile))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Gets all units within the effect area.
            /// </summary>
            /// <returns>Returns a list of units within the effect area.</returns>
            public List<Unit> GetUnitsInRange()
            {
                List<Unit> units = new List<Unit>();


                for (int i = 0; i < range.GetLength(0); i++)
                {
                    for (int j = 0; j < range.GetLength(1); j++)
                    {
                        Vector3Int targetTile = range[i, j];
                        List<OverworldObject> overworldObjects 
                            = UtilityFunctions.GetActiveUnitDetector().GetOverworldObjectsAtPosition(targetTile);

                        foreach (OverworldObject overworldObject in overworldObjects)
                        {
                            if (overworldObject is Unit)
                            {
                                units.Add((Unit)overworldObject);
                            }
                        }
                    }
                }

                return units;
            }

            /// <summary>
            /// Generates a random position within the effect area.
            /// </summary>
            /// <returns>Returns a random <see cref="Vector3Int"/> object with x and y coordinates within the effect 
            /// area.</returns>
            public Vector3Int RandomTile()
            {
                //returns random tile from aoe. index 0 is x; 1 is y
                int[] tileIndices = new int[2];
                int xmin = 0;
                int xmax = range.GetLength(0);
                int ymin = 0;
                int ymax = range.GetLength(1);


                System.Random random = new System.Random();

                tileIndices[0] = random.Next(xmin, xmax);
                tileIndices[1] = random.Next(ymin, ymax);

                Vector3Int randomTile = range[tileIndices[0], tileIndices[1]];

                return randomTile;
            }

            /// <summary>
            /// Types of shapes that can be created in an AreaOfEffect object.
            /// </summary>
            public enum AreaOfEffectShapes
            {

            }
            #endregion
        }
    }
}
