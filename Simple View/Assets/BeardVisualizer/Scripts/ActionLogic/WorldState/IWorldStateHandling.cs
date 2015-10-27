namespace Assets.Scripts.ActionLogic
{
    public interface IWorldStateHandling
    {
        #region Public Methods and Operators

        void ExecuteWorldStateUpdateFromClientSidePrediction(WorldStateMessage newWorldState);

        void ExecuteWorldStateUpdateFromServer(string newWorldState);

        WorldState GetLatestWorldStateExcludingClientSidePrediction();

        WorldState GetLatestWorldStateIncludingClientSidePrediction();

        #endregion
    }
}
