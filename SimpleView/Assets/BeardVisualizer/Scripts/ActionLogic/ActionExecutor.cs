#region usages

using UnityEngine;

#endregion

namespace Assets.Scripts.ActionLogic
{
    #region usages

    using System;
    using Assets.Scripts.Utilities;

    #endregion

    /// <summary>
    ///     Validates and executes actions if they are valid.
    ///     All actions inherit from <see cref="IBeardEvent" /> and can therefore be sent over the message system.
    /// </summary>
    public abstract class ActionExecutor : UnitySingleton<ActionExecutor>
    {
        #region Public Properties

        public IWorldStateHandling IWorldStateHandling
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            base.Initialize();
            MessagingSystem.Instance.SubscribeToEvent<IBeardAction>(this.OnNewAction);
        }

        protected abstract void OnNewAction(IBeardAction beardAction);

        #endregion
    }
}