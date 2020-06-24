using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

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

    private Vector3Int basePosition;

    /// <summary>
    /// The name of the function contained in <see cref="SkillHub"/> associated with the hitbox object's logic.
    /// </summary>
    private string hitFunctionName;

    private int hitIndex;

    private Unit unit;

    /// <summary>
    /// The skill that the hitbox is attached to.
    /// </summary>
    private Skill skill;

    /// <summary>
    /// The unique identifier for the hitbox's hitbox group.
    /// </summary>
    /// <remarks>
    /// Used to ensure units cannot be hit by hitboxes in the same group more than once.
    /// </remarks>
    private string hitboxGroupID;

    private bool isActive;

    private string hitboxIndicatorTilemapName;
    #endregion

    #region Constructors
    public Hitbox(int startFrame, int delayFrames, int lingerFrames, Vector3Int position, string hitFunctionName, int hitIndex)
    {
        this.startFrame = startFrame;
        this.delayFrames = delayFrames;
        this.lingerFrames = lingerFrames;
        this.position = position;
        this.basePosition = position;
        this.hitFunctionName = hitFunctionName;
        this.hitIndex = hitIndex;
    }
    #endregion

    #region Functions
    /// <summary>
    /// Modifies <see cref="startFrame"/> and <see cref="delayFrames"/> to be sped up or slowed down dependent on a 
    /// given attack speed value.
    /// </summary>
    /// <param name="attackSpeed">The attack speed value to modify the frames by.</param>
    public void AdjustToAttackSpeed(float attackSpeed)
    {
        // not done yet
    }

    private bool TryHitbox(Unit target)
    {
        if(unit.GetTargetType() != target.GetTargetType())
        {
            return target.SearchHitboxImmunity(hitboxGroupID);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Sets the hitbox's position relative to the casting unit's position and direction.
    /// </summary>
    public void AdjustPositionToUnit()
    {
        
        Debug.Log(unit.position);
        position = unit.position;
        switch (unit.direction)
        {
            case Directions.NW:
                position.y += basePosition.x;
                position.x += basePosition.y;
                break;
            case Directions.NE:
                position.y -= basePosition.y;
                position.x += basePosition.x;
                break;
            case Directions.SW:
                position.y += basePosition.y;
                position.x -= basePosition.x;
                break;
            case Directions.SE:
                position.y -= basePosition.x;
                position.x -= basePosition.y;
                break;
        }
    }

    public void ExecuteHitbox(Unit target)
    {
        if (TryHitbox(target))
        {
            Debug.Log(hitFunctionName);
            if (hitFunctionName.Equals(""))
            {
                RunHitboxDefault(target);
            }
            else
            {
                MethodInfo method = GetType().GetMethod(hitFunctionName, BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(skill, new object[] { target });
            }

            target.AddHitboxImmunity(hitboxGroupID);
        }

    }

    private void RunHitboxDefault(Unit target)
    {
        HitCalculation hitCalculation = new HitCalculation(unit, skill.GetHits()[hitIndex], target);
        hitCalculation.ApplyHitCalculation();
    }

    public void SetHitboxGroupID(string hitboxGroupID)
    {
        this.hitboxGroupID = hitboxGroupID;
    }

    public Vector3Int GetPosition()
    {
        return position;
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;
    }

    public int GetStartFrame()
    {
        return startFrame;
    }

    public int GetDelayFrames()
    {
        return delayFrames;
    }

    public int GetLingerFrames()
    {
        return lingerFrames;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetIsActive(bool isActive)
    {
        this.isActive = isActive;
    }
    #endregion
}
