namespace Assets.Scripts.ActionLogic.Local
{
    #region usages

    using System;
    using System.Diagnostics;
    using Debug = UnityEngine.Debug;

    #endregion

    /// <summary>
    ///     Executes all actions from the local messaging system. This implementation is meant for a non distributed
    ///     environment!
    /// </summary>
    public class LocalActionExecutor : ActionExecutor
    {
        #region Methods

        protected override void OnNewAction(IBeardAction beardAction)
        {
            if (beardAction.IsActionValid())
            {
                beardAction.ExecuteAction();
            }
        }

        #endregion
    }
}