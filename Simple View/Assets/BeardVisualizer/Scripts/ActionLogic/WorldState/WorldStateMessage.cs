#region usages

using System.Net;

#endregion

namespace Assets.Scripts.ActionLogic
{
    #region usages

    using System;

    #endregion

    public class WorldStateMessage : IBeardEvent
    {
        #region Public Properties

        public IPEndPoint SenderEndpoint { get; private set; }

        public WorldState WorldState
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
            }
        }

        #endregion
    }
}
