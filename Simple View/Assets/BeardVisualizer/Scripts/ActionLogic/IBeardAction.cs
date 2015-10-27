namespace Assets.Scripts.ActionLogic
{
    /// <summary>
    ///     An action that has been carried out by a user. The effects of this action are only executed, if they are valid.
    ///     Implementing actions must contain all parameters needed for the execution of the action.
    ///     See <see cref="ActionRequester" /> and <see cref="ActionExecutor" /> for more details. The action execution and
    ///     validation can be distributed. The logic to validate and execute the action are implemented in the specific
    ///     message. The requesting (<see cref="ActionRequester" />) and executing (<see cref="ActionExecutor" />) must use the
    ///     same implementation of the action for this concept to work.
    /// </summary>
    /// <remarks>
    ///     Since all logic specific to an action is implemented in the respective action class, the system is fairly easy to
    ///     extend. For distribution purposes protobuf may be used to send the messages over the network. Make sure all
    ///     parameters get serialized correctly if you do this.
    /// </remarks>
    public interface IBeardAction : IBeardEvent
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Executes the effects of this action. The specific effects are totally determined by the executing class.
        /// </summary>
        void ExecuteAction();

        /// <summary>
        ///     Checks if this action can and should be executed in the current context. This method can be used on the server to
        ///     prevent users from executing illegal commands.
        /// </summary>
        /// <returns>
        ///     True, if its save to execute the action, otherwise false.
        /// </returns>
        bool IsActionValid();

        #endregion
    }
}
