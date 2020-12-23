using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

namespace YuguLibrary
{
    /// <summary>
    /// Specifies information to pass to a <see cref="HookFunction"/>.
    /// </summary>
    /// <remarks>
    /// Subclasses exist to add clarity when defining a new hook function.
    /// </remarks>
    public abstract class HookBundle
    {

    }

    public class IncapacitationHookBundle : HookBundle
    {
        public IncapacitationHookBundle()
        {

        }
    }

    public class StatusAppliedHookBundle : HookBundle
    {
        /// <summary>
        /// The status to be applied.
        /// </summary>
        public Status status;

        /// <summary>
        /// The unit turn of the unit to be afflicted by the status.
        /// </summary>
        public Unit target;

        public StatusAppliedHookBundle(Status status, Unit target)
        {
            this.status = status;
            this.target = target;
        }
    }

    public class StatusRemovedHookBundle : HookBundle
    {
        /// <summary>
        /// The status to be removed.
        /// </summary>
        public StatusEffects statusEffect;

        /// <summary>
        /// The unit turn of the unit afflicted by the status.
        /// </summary>
        public Unit target;

        public StatusRemovedHookBundle(StatusEffects statusEffect, Unit target)
        {
            this.statusEffect = statusEffect;
            this.target = target;
        }
    }

    #region Encounter Hook Bundles
    public class RoundStartHookBundle : HookBundle
    {
        public RoundStartHookBundle()
        {

        }
    }

    public class RoundEndHookBundle : HookBundle
    {
        public RoundEndHookBundle()
        {

        }
    }

    public class TurnStartHookBundle : HookBundle
    {
        /// <summary>
        /// The <see cref="UnitTurn"/> object of the unit whose turn is about to start.
        /// </summary>
        public UnitTurn unitTurn;

        public TurnStartHookBundle(UnitTurn unitTurn)
        {
            this.unitTurn = unitTurn;
        }
    }

    public class TurnEndHookBundle : HookBundle
    {
        /// <summary>
        /// The <see cref="UnitTurn"/> object of the unit whose turn is about to end.
        /// </summary>
        public UnitTurn unitTurn;

        public TurnEndHookBundle(UnitTurn unitTurn)
        {
            this.unitTurn = unitTurn;
        }
    }
    #endregion
}
