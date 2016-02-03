#region usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Assets.Scripts.ConstructionLogic
{
    #region usages

    using UnityEngine;

    #endregion

    public class SquareBuildingBlock : BuildingBlock
    {
        #region Public Methods and Operators

        public BuildingBlock GetConnectedBlockInDirection(Vector3 direction)
        {
            foreach (var block in this.DirectlyConnectedBuildingBlocks)
            {
                var delta = this.transform.position - block.transform.position;

                if ((delta.normalized - direction.normalized).magnitude < 0.1f)
                {
                    return block;
                }
            }

            return null;
        }

        public bool HasConnectedBlockInDirection(Vector3 direction)
        {
            return this.GetConnectedBlockInDirection(direction) != null;
        }

        #endregion

        #region Methods

        protected override Func<BlockJoint, BlockJoint, bool> GetRestrictionPredicate()
        {
            Func<BlockJoint, BlockJoint, bool> predicate = (thisBlocksJoint, otherBlockJoint) =>
            {
                var thisJoint = thisBlocksJoint as SquareBlockJoint;
                // otherJoint is the construct
                var otherJoint = otherBlockJoint as SquareBlockJoint;
                var otherBlock = otherBlockJoint.OwningBlock as SquareBuildingBlock;

                // Allow only connections between SquareBuildingBlocks
                if (thisJoint == null || otherJoint == null || otherBlock == null)
                {
                    // SquareBuildingBlocks can only connect to other SquareBuildingBlocks...
                    return false;
                }

                // Do not connect to other connected blocks.
                if (otherJoint.ConnectedBlock != null)
                {
                    return false;
                }

                // Allow the connection to blocks that are on the ground level, as long as a joint is used, thats not below the ground level. The ground level is considered to be y == 0. Each block is 1 x 1 x 1 m. So the center of the block should be at y = 0.5, if it is located at the ground level.
                // WORKS!
                if (otherJoint.OwningBlock.EntityPosition.y < 0.1f
                    && otherJoint.OwningBlock.EntityPosition.y > -0.1f
                    && otherJoint.JointOrientation != SquareBlockJoint.BlockJointOrientation.DOWN)
                {
                    return true;
                }
                // Allow stacking blocks on top of each other.
                else if (thisJoint.JointOrientation == SquareBlockJoint.BlockJointOrientation.DOWN
                         && otherJoint.JointOrientation == SquareBlockJoint.BlockJointOrientation.UP)
                {
                    return true;
                }

                return false;
            };

            return predicate;
        }

        #endregion
    }
}