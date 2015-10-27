namespace Assets.Scripts.ConstructionLogic.ConstructionUtilities
{
    using System;

    public class NoValidJointsFoundException : Exception
    {
        #region Constructors and Destructors

        public NoValidJointsFoundException()
        {
        }

        public NoValidJointsFoundException(string message)
            : base(message)
        {
        }

        public NoValidJointsFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}
