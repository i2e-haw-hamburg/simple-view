namespace Assets.Scripts.ActionLogic.Local
{
    #region usages

    using System.Diagnostics;

    #endregion

    /// <summary>
    ///     Requests the local execution of an action. This is meant for a non distributed environment only!
    /// </summary>
    public class LocalActionRequester : ActionRequester
    {
        #region Public Methods and Operators

        public override void RequestAction(IBeardAction action)
        {
            MessagingSystem.Instance.BroadcastEvent(action);
        }

        #endregion
    }
}
