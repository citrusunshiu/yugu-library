using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

namespace YuguLibrary
{
    public abstract class Status
    {
        #region Variables
        private string nameID;

        private string descriptionID;

        private string longDescriptionID;

        private string iconFilePath;

        /// <summary>
        /// The status' type category.
        /// </summary>
        private StatusTypes statusType;

        /// <summary>
        /// The status' identifier.
        /// </summary>
        private StatusEffects statusId;

        /// <summary>
        /// Whether the status stacks.
        /// </summary>
        /// <remarks>
        /// If the attribute is set to false, no more than 1 item can be inserted into the <see cref="stacks"/> attribute.
        /// </remarks>
        private bool isStackable;

        private string statusFunctionName;

        /// <summary>
        /// List of <see cref="HookFunction"/> objects to attach to a unit when the status is applied.
        /// </summary>
        private List<HookFunction> hookFunctions;

        /// <summary>
        /// List of all instances of the status stacked onto the owner unit.
        /// </summary>
        /// <seealso cref="StatusStack"/>
        public List<StatusStack> stacks = new List<StatusStack>();

        private Unit attachedUnit;


        #endregion

        #region Constructors
        public Status(string statusJSONFileName, int duration)
        {
            
        }
        #endregion


        #region Functions
        public StatusEffects GetStatusID()
        {
            return statusId;
        }

        public List<HookFunction> GetHookFunctions()
        {
            return hookFunctions;
        }

        public void SetAttachedUnit(Unit attachedUnit)
        {
            this.attachedUnit = attachedUnit;
        }
        #endregion

    }

    /// <summary>
    /// An object utilized by <see cref="Status"/> to track an instance of a status.
    /// </summary>
    public class StatusStack
    {
        #region Variables
        /// <summary>
        /// Unit that applied the status stack.
        /// </summary>
        public Unit inflicter;

        /// <summary>
        /// Number of turns that the status stack lasts.
        /// </summary>
        public int duration;

        /// <summary>
        /// The skill associated with the status stack.
        /// </summary>
        /// <remarks>
        /// Only used if the status inflicts damage; stores the hits and damage modifiers of the status. 
        /// This is not used to store the skill that inflicted the status.
        /// </remarks>
        Skill skill;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="StatusStack"/> with a specified duration.
        /// </summary>
        /// <param name="duration">The StatusStack's <see cref="duration"/>.</param>
        public StatusStack(int duration)
        {
            this.duration = duration;
        }

        /// <summary>
        /// Creates a new <see cref="StatusStack"/> with a specified duration and inflicter.
        /// </summary>
        /// <param name="inflicter">The StatusStack's <see cref="inflicter"/>.</param>
        /// <param name="duration">The StatusStack's <see cref="duration"/>.</param>
        public StatusStack(Unit inflicter, int duration)
        {
            this.inflicter = inflicter;
            this.duration = duration;
        }

        /// <summary>
        /// Creates a new <see cref="StatusStack"/> with a specified duration, inflicter, and skill value.
        /// </summary>
        /// <param name="inflicter">The StatusStack's <see cref="inflicter"/>.</param>
        /// <param name="duration">The StatusStack's <see cref="duration"/>.</param>
        /// <param name="skill">The StatusStack's <see cref="skill"/>.</param>
        public StatusStack(Unit inflicter, int duration, Skill skill)
        {
            this.inflicter = inflicter;
            this.duration = duration;
            this.skill = skill;
        }
        #endregion
    }
}
