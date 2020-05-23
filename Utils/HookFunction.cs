using System;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

namespace YuguLibrary
{
    /// <summary>
    /// Grants <see cref="Unit"/> objects dynamic functionality at certain points during encounters.
    /// </summary>
    /// <remarks>
    /// Defines a specific <see cref="DelegateFlags"/> value and a snippet of code to attach to a Unit object.
    /// The code snippet is ran whenever the associated delegate flag function is ran by <see cref="EncounterController"/>.
    /// </remarks>
    public class HookFunction
    {
        #region Variables
        /// <summary>
        /// The identifier of the hook function.
        /// </summary>
        private string hookFunctionId;

        /// <summary>
        /// The <see cref="DelegateFlags"/> value that the function is specified to run on.
        /// </summary>
        private DelegateFlags delegateType;

        /// <summary>
        /// The code snippet that runs on the specified <see cref="DelegateFlags"/> value.
        /// </summary>
        /// <remarks>
        /// Requires a certain amount of information from the location at which it is called, passed to it by a 
        /// <see cref="HookBundle"/> object.
        /// </remarks>
        private Func<HookBundle, object> returnLogic;

        /// <summary>
        /// The unit that the hook function is attached to.
        /// </summary>
        private Unit attachedUnit;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor to initialize all instance variables.
        /// </summary>
        /// <param name="delegateType">The HookFunction's <see cref="delegateType"/>.</param>
        /// <param name="returnLogic">The HookFunction's <see cref="returnLogic"/>.</param>
        public HookFunction(string hookFunctionId, DelegateFlags delegateType, Func<HookBundle, object> returnLogic)
        {
            this.hookFunctionId = hookFunctionId;
            this.delegateType = delegateType;
            this.returnLogic = returnLogic;
        }
        #endregion

        #region Functions
        public void ExecuteFunction(HookBundle hookBundle)
        {
            returnLogic.Invoke(hookBundle);
        }

        public string GetHookFunctionID()
        {
            return hookFunctionId;
        }

        public DelegateFlags GetDelegateType()
        {
            return delegateType;
        }

        public void SetAttachedUnit(Unit attachedUnit)
        {
            this.attachedUnit = attachedUnit;
        }

        public void CheckApplicationEffect()
        {
            if (delegateType == DelegateFlags.OnApply)
            {
                ExecuteFunction(null);
            }
        }
        public void CheckRemovalEffect()
        {
            if (delegateType == DelegateFlags.OnRemove)
            {
                ExecuteFunction(null);
            }
        }
        #endregion
    }
}