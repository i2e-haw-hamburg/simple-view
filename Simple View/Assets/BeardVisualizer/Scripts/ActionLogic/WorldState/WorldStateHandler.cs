#region usages

using System;

#endregion

namespace Assets.Scripts.ActionLogic
{
    public class WorldStateHandler : IWorldStateHandling
    {
        #region Public Methods and Operators

        public void ExecuteWorldStateUpdateFromClientSidePrediction(WorldStateMessage newWorldState)
        {
            throw new NotImplementedException();
        }

        public void ExecuteWorldStateUpdateFromServer(string newWorldState)
        {
            throw new NotImplementedException();
        }

        public WorldState GetLatestWorldStateExcludingClientSidePrediction()
        {
            throw new NotImplementedException();
        }

        public WorldState GetLatestWorldStateIncludingClientSidePrediction()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
