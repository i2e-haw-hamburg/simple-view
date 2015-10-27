#region usages

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Assets.Scripts.UserActions
{
    #region usages

    using System.Linq;
    using System.Net;

    using Assets.Scripts.ConstructionLogic;

    using UnityEngine;

    #endregion

    public class CreateNewBuildingPlanAtPosition : ConstructionEntityBasedAction
    {
        #region Constructors and Destructors

        public CreateNewBuildingPlanAtPosition(
            long id,
            Vector3 position,
            Quaternion rotation,
            IPEndPoint senderEndpoint,
            BuildingPlan buildingPlanType)
            : base(id, position, rotation, senderEndpoint)
        {
            this.BuildingPlanType = buildingPlanType;
        }

        public CreateNewBuildingPlanAtPosition(
            long id,
            Vector3 position,
            Quaternion rotation,
            BuildingPlan buildingPlanType)
            : base(id, position, rotation)
        {
            this.BuildingPlanType = buildingPlanType;
        }

        #endregion

        #region Public Properties

        public BuildingPlan BuildingPlanType { get; private set; }

        #endregion

        #region Public Methods and Operators

        public override void ExecuteAction()
        {
            var newBuildingPlanInstance =
                GameObject.Instantiate(this.BuildingPlanType, this.Position, this.Rotation) as BuildingPlan;
            newBuildingPlanInstance.ID = this.ID;
        }

        /// <summary>
        ///     This action is valid, if there exists no other entity with the same ID.
        /// </summary>
        /// <returns></returns>
        public override bool IsActionValid()
        {
            return ConstructionEntityManager.Instance.RegisteredConstructionEntities.All(x => x.ID != this.ID);
        }

        #endregion
    }
}
