#region usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Assets.Scripts.UserActions
{
    #region usages

    using System.Net;
    using Assets.Scripts.ConstructionLogic;
    using UnityEngine;

    #endregion

    public class BuildingPlanMoved : ConstructionEntityBasedAction
    {
        #region Constructors and Destructors

        public BuildingPlanMoved(long id, Vector3 position)
            : base(id, position, new Quaternion())
        {
        }

        public BuildingPlanMoved(long id, Vector3 position, IPEndPoint senderEndpoint)
            : base(id, position, new Quaternion(), senderEndpoint)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void ExecuteAction()
        {
            var entity = ConstructionEntityManager.Instance.GetEntityWithID(this.ID);

            entity.EntityPosition = this.Position;
        }

        /// <summary>
        ///     This action is valid, if there is a construction entity with the specified ID present.
        /// </summary>
        /// <returns></returns>
        public override bool IsActionValid()
        {
            return ConstructionEntityManager.Instance.GetEntityWithID(this.ID) != null;
        }

        #endregion
    }
}