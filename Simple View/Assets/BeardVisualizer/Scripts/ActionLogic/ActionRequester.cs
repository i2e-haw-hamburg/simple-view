#region usages

using UnityEngine;

#endregion

namespace Assets.Scripts.ActionLogic
{
    #region usages

    using Assets.Scripts.Utilities;

    #endregion

    /// <summary>
    ///     Provides functionality for requesting actions. They will only be executed, if the <see cref="ActionExecutor" />
    ///     instance finds them valid. See <see cref="IBeardAction" /> for more info on actions.
    /// </summary>
    public abstract class ActionRequester : UnitySingleton<ActionRequester>
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Requests the execution of the specified action.
        /// </summary>
        /// <param name="action"></param>
        public abstract void RequestAction(IBeardAction action);

        #endregion
    }
}
