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

    public class AttachBuildingBlockFromBuildingPlanToConstruct : ConstructionEntityBasedAction
    {
        #region Constructors and Destructors

        public AttachBuildingBlockFromBuildingPlanToConstruct(long id, Vector3 position, Quaternion rotation)
            : base(id, position, rotation)
        {
        }

        public AttachBuildingBlockFromBuildingPlanToConstruct(
            long id,
            Vector3 position,
            Quaternion rotation,
            IPEndPoint senderEndpoint)
            : base(id, position, rotation, senderEndpoint)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void ExecuteAction()
        {
            var buildingPlan =
                ConstructionEntityManager.Instance.RegisteredConstructionEntities.OfType<BuildingPlan>()
                                         .Single(x => x.ID == this.ID);
            buildingPlan.AttachInstanceOfBuiltBlockToConstruct();
        }

        /// <summary>
        ///     This action is valid, if there a BuildingBlock with the specified ID exists.
        /// </summary>
        /// <returns></returns>
        public override bool IsActionValid()
        {
            return
                ConstructionEntityManager.Instance.RegisteredConstructionEntities.OfType<BuildingPlan>()
                                         .Any(x => x.ID == this.ID);
        }

        #endregion
    }
}
