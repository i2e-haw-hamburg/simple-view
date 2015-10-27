namespace Assets.Scripts.UserActions
{
    #region usages

    using System.Linq;
    using System.Net;

    using Assets.Scripts.ActionLogic;
    using Assets.Scripts.ConstructionLogic;

    using UnityEngine;

    #endregion

    public abstract class ConstructionEntityBasedAction : IBeardAction
    {
        #region Constructors and Destructors

        public ConstructionEntityBasedAction(long id, Vector3 position, Quaternion rotation)
        {
            this.Rotation = rotation;
            this.Position = position;
            this.ID = id;
        }

        public ConstructionEntityBasedAction(long id, Vector3 position, Quaternion rotation, IPEndPoint senderEndpoint)
        {
            this.ID = id;
            this.Position = position;
            this.Rotation = rotation;
            this.SenderEndpoint = senderEndpoint;
        }

        #endregion

        #region Public Properties

        public long ID { get; private set; }

        public Vector3 Position { get; private set; }

        public Quaternion Rotation { get; private set; }

        public IPEndPoint SenderEndpoint { get; private set; }

        #endregion

        #region Public Methods and Operators

        public abstract void ExecuteAction();

        public abstract bool IsActionValid();

        #endregion
    }
}
