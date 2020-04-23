using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox
{
    #region Variables

    /// <summary>
    /// The frame number to activate the hitbox on.
    /// </summary>
    private int startFrame;

    /// <summary>
    /// The duration in frames (1/60 seconds) that is waited before the hitbox becomes active.
    /// </summary>
    private int delayFrames;

    /// <summary>
    /// The duration in frames (1/60 seconds) that the hitbox stays active for before being removed.
    /// </summary>
    private int lingerFrames;

    /// <summary>
    /// The position where the hitbox is located.
    /// </summary>
    private Vector3Int position;

    private string hitFunctionName;

    private int hitIndex;
    #endregion

    #region Constructors
    public Hitbox(int startFrame, int delayFrames, int lingerFrames, Vector3Int position, string hitFunctionName, int hitIndex)
    {
        this.startFrame = startFrame;
        this.delayFrames = delayFrames;
        this.lingerFrames = lingerFrames;
        this.position = position;
        this.hitFunctionName = hitFunctionName;
        this.hitIndex = hitIndex;
    }
    #endregion

    #region Functions
    #endregion
}
