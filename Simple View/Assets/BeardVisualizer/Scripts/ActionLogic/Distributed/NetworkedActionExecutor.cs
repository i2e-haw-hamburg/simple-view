namespace Assets.Scripts.ActionLogic.Distributed
{
    #region usages

    using System;

    #endregion

    public class NetworkedActionExecutor : ActionExecutor
    {
        #region Methods

        protected override void OnNewAction(IBeardAction beardAction)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}