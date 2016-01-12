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

    public class BuildingPlanRotated : ConstructionEntityBasedAction
    {
        #region Constructors and Destructors

        public BuildingPlanRotated(long id, Quaternion rotation)
            : base(id, Vector3.zero, rotation)
        {
        }

        public BuildingPlanRotated(long id, Quaternion rotation, IPEndPoint senderEndpoint)
            : base(id, Vector3.zero, rotation, senderEndpoint)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void ExecuteAction()
        {
            var entity = ConstructionEntityManager.Instance.GetEntityWithID(this.ID);

            entity.EntityRotation = this.Rotation;
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