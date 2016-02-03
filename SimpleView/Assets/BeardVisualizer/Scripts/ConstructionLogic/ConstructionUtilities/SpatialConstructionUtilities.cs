#region usages

using System;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets.Scripts.ConstructionLogic.ConstructionUtilities
{
    #region usages

    using Assets.Scripts.ConstructionLogic.ConstructionUtilities.ThirdParty;

    #endregion

    /// <summary>
    ///     Provides functionality for solving common spatial problems for the constuction of the <see cref="Construct" />.
    /// </summary>
    public static class SpatialConstructionUtilities
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the two nearest joints that belong to the two specified BuildingBlocks. The algorithm will only take joints
        ///     into account that are not connected.
        ///     Be advised that the used algorithm has a complexity of O(n*m) where n is the number of joints of block0 and m is
        ///     the number of joints of block1.
        ///     It is expected that the number of joints on the two blocks will be relatively small, so the overhead of a more
        ///     complex algorithm would induce inefficiencies.
        ///     However if there are a lot of joints, or you need to calculate the distance between the joints very frequently,
        ///     think about using a <see cref="KDTree" />.
        /// </summary>
        /// <param name="block0"> The first block. </param>
        /// <param name="block1"> The second block. </param>
        /// <param name="joint1"> The nearest joint belonging to the first block. </param>
        /// <param name="joint2"> The nearest joint belonging to the second block. </param>
        public static void GetNearestUnconnectedJoints(
            BuildingBlock block0,
            BuildingBlock block1,
            out BlockJoint joint1,
            out BlockJoint joint2)
        {
            // Get all joints of both blocks that are not yet connected.
            var block1Joints = block0.BlockJoints.Where(x => !x.ConnectedJoint).ToList();
            var block2Joints = block1.BlockJoints.Where(x => !x.ConnectedJoint).ToList();

            // Initialize the variables to make the compiler happy. They always be reassigned in the algorithm however.
            joint1 = null;
            joint2 = null;

            // Check if the blocks have joints that we can work with.
            if (!block1Joints.Any())
            {
                throw new NoValidJointsFoundException(
                    "Failed to calculate the nearest two joints: The specified block in argument at index 0 (block0): "
                    + block0 + " has no available (not already connected) joints.");
            }
            if (!block2Joints.Any())
            {
                throw new NoValidJointsFoundException(
                    "Failed to calculate the nearest two joints: The specified block in argument at index 1 (block1): "
                    + block1 + " has no available (not already connected) joints.");
            }

            // Find the two blocks that are closest together.
            var currentBestDistance = Mathf.Infinity;

            // Check the distance of each joint of block1 to each joint of block2 and find the ones that are closest together.
            foreach (var block1Joint in block1Joints)
            {
                foreach (var block2Joint in block2Joints)
                {
                    var currentDistance = Vector3.Distance(block1Joint.EntityPosition, block2Joint.EntityPosition);

                    if (currentDistance < currentBestDistance)
                    {
                        currentBestDistance = currentDistance;
                        joint1 = block1Joint;
                        joint2 = block2Joint;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the two nearest joints that belong to the two specified BuildingBlocks. The algorithm will only take joints
        ///     into account that are not connected.
        ///     Be advised that the used algorithm has a complexity of O(n*m) where n is the number of joints of block0 and m is
        ///     the number of joints of block1.
        ///     It is expected that the number of joints on the two blocks will be relatively small, so the overhead of a more
        ///     complex algorithm would induce inefficiencies.
        ///     However if there are a lot of joints, or you need to calculate the distance between the joints very frequently,
        ///     think about using a <see cref="KDTree" />.
        ///     This version also takes a predicate which both joints must fullfill, for a connection to be considered valid.
        /// </summary>
        /// <param name="block0"> The first block. </param>
        /// <param name="block1"> The second block. </param>
        /// <param name="joint1"> The nearest joint belonging to the first block. </param>
        /// <param name="joint2"> The nearest joint belonging to the second block. </param>
        public static void GetNearestUnconnectedJointsWithPredicate(
            BuildingBlock block0,
            BuildingBlock block1,
            out BlockJoint joint1,
            out BlockJoint joint2,
            Func<BlockJoint, BlockJoint, bool> predicate)
        {
            // Get all joints of both blocks that are not yet connected.
            var block1Joints = block0.BlockJoints.Where(x => !x.ConnectedJoint).ToList();
            var block2Joints = block1.BlockJoints.Where(x => !x.ConnectedJoint).ToList();

            // Initialize the variables to make the compiler happy. They always be reassigned in the algorithm however.
            joint1 = null;
            joint2 = null;

            // Check if the blocks have joints that we can work with.
            if (!block1Joints.Any())
            {
                throw new NoValidJointsFoundException(
                    "Failed to calculate the nearest two joints: The specified block in argument at index 0 (block0): "
                    + block0 + " has no available (not already connected) joints.");
            }
            if (!block2Joints.Any())
            {
                throw new NoValidJointsFoundException(
                    "Failed to calculate the nearest two joints: The specified block in argument at index 1 (block1): "
                    + block1 + " has no available (not already connected) joints.");
            }

            // Find the two blocks that are closest together.
            var currentBestDistance = Mathf.Infinity;

            // Check the distance of each joint of block1 to each joint of block2 and find the ones that are closest together.
            foreach (var block1Joint in block1Joints)
            {
                foreach (var block2Joint in block2Joints)
                {
                    var currentDistance = Vector3.Distance(block1Joint.EntityPosition, block2Joint.EntityPosition);

                    if (currentDistance < currentBestDistance)
                    {
                        // Check if the blocks fulfill the predicate condition.
                        if (predicate(block1Joint, block2Joint))
                        {
                            currentBestDistance = currentDistance;
                            joint1 = block1Joint;
                            joint2 = block2Joint;
                        }
                    }
                }
            }

            if (joint1 == null || joint2 == null)
            {
                throw new NoValidJointsFoundException(
                    "Failed to find nearest block joints: There are no joints that fulfill the given predicate.");
            }
        }

        /// <summary>
        ///     Gets the next block in the construct to the specified block that has unconnected joints.
        /// </summary>
        /// <param name="blockToAttach">
        ///     The block from which position the next available block in the construct should be
        ///     searched.
        /// </param>
        /// <returns> The nearest available block to the specified building block. </returns>
        public static BuildingBlock GetNextAvailableBuildingBlockInConstruct(BuildingBlock blockToAttach)
        {
            var nextAvailableBlock =
                Construct.Instance.AllBuildingBlocksOfConstruct.Where(x => x.HasUnconnectedJoints)
                    .OrderBy(x => Vector3.Distance(x.EntityPosition, blockToAttach.EntityPosition))
                    .First();

            return nextAvailableBlock;
        }

        #endregion
    }
}