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
    using System.Xml.Schema;
    using Assets.Scripts.ConstructionLogic;
    using UnityEngine;

    #endregion

    public class RemoveBuildingPlan : ConstructionEntityBasedAction
    {
        #region Constructors and Destructors

        public RemoveBuildingPlan(Quaternion rotation, Vector3 position, long id)
            : base(id, position, rotation)
        {
        }

        public RemoveBuildingPlan(long id, Vector3 position, Quaternion rotation, IPEndPoint senderEndpoint)
            : base(id, position, rotation, senderEndpoint)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void ExecuteAction()
        {
            ConstructionEntityManager.Instance.GetEntityWithID(this.ID).Destroy();
        }

        public override bool IsActionValid()
        {
            return ConstructionEntityManager.Instance.GetEntityWithID(this.ID) != null;
        }

        #endregion
    }
}