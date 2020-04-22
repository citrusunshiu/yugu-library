using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox
{
    #region Variables
    /// <summary>
    /// The duration in frames (1/60 seconds) that is waited before the hitbox becomes active.
    /// </summary>
    private float delayFrames;

    /// <summary>
    /// The duration in frames (1/60 seconds) that the hitbox stays active for before being removed.
    /// </summary>
    private float lingerTime;

    /// <summary>
    /// The position where the hitbox is located.
    /// </summary>
    private Vector3Int position;
    #endregion

    #region Constructors
    public Hitbox()
    {

    }
    #endregion

    #region Functions
    #endregion
}
