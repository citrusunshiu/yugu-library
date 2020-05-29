using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
    }
}
