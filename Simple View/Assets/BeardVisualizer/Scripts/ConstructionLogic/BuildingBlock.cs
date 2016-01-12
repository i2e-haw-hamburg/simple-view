#region usages

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Assets.Scripts.ConstructionLogic
{
    #region usages

    using Assets.Scripts.ConstructionLogic.ConstructionUtilities;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     BuildingBlocks are combined to build the <see cref="Construct" />. The construct consists of a set of
    ///     BuildingBlocks. Each BuildingBlock has a set of <see cref="BlockJoint" />. BuildingBlocks can be attached to the
    ///     <see cref="Construct" /> using the respective <see cref="BlockJoint" />.
    /// </summary>
    public class BuildingBlock : ConstructionEntity
    {
        #region Fields

        private List<BlockJoint> joints;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The joints that belong to this block.
        /// </summary>
        public IList<BlockJoint> BlockJoints
        {
            get { return this.joints.AsReadOnly(); }
        }

        /// <summary>
        ///     All <see cref="BuildingBlock" /> that are directly connected to one of this blocks joints (
        ///     <see cref="BlockJoints" />).
        /// </summary>
        public IList<BuildingBlock> DirectlyConnectedBuildingBlocks
        {
            get { return this.joints.Select(x => x.ConnectedBlock).Where(x => x != null).ToList(); }
        }

        /// <summary>
        ///     True if there are one or more joints in the <see cref="BlockJoints" /> list that are not connected to any other
        ///     BuildingBlock, otherwise false.
        /// </summary>
        public bool HasUnconnectedJoints
        {
            get { return this.joints.Any(x => !x.ConnectedJoint); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds an existing joint to this block. The added joint will be reinitialized accordingly.
        /// </summary>
        /// <param name="joint"></param>
        public void AddJoint(BlockJoint joint)
        {
            if (joint == null)
            {
                throw new ArgumentNullException();
            }

            joint.OwningBlock = this;
            joint.transform.parent = this.transform;
            this.joints.Add(joint);
        }

        /// <summary>
        ///     Instantly attaches another BuildingBlock to this one. The two nearest joints of each block are choosen. The other
        ///     block is then rotated and moved to the appropriate position.
        /// </summary>
        /// <param name="otherBlock"> The block that will be attached to this one. </param>
        public void AttachOtherBuildingBlock(BuildingBlock otherBlock)
        {
            BlockJoint ownJoint;
            BlockJoint otherJoint;

            SpatialConstructionUtilities.GetNearestUnconnectedJointsWithPredicate(
                this,
                otherBlock,
                out ownJoint,
                out otherJoint,
                this.GetRestrictionPredicate());

            otherJoint.AttachBlockJoint(ownJoint);
        }

        /// <summary>
        ///     Attaches another BuildingBlock to this one. The movement and rotation will be animated. The two nearest joints of
        ///     each block are choosen. The other block is then rotated and moved to the appropriate position.
        /// </summary>
        /// <param name="otherBlock"> The block that will be attached to this one. </param>
        /// <param name="animationTime">
        ///     The total time the attach process takes. The complete animation (rotation and movement)
        ///     will take this time to complete.
        /// </param>
        public IEnumerator AttachOtherBuildingBlockAnimated(BuildingBlock otherBlock, float animationTime)
        {
            BlockJoint ownJoint = null;
            BlockJoint otherJoint = null;

            bool valid = true;

            try
            {
                SpatialConstructionUtilities.GetNearestUnconnectedJointsWithPredicate(
                    this,
                    otherBlock,
                    out ownJoint,
                    out otherJoint,
                    this.GetRestrictionPredicate());
            }
            catch (NoValidJointsFoundException)
            {
                valid = false;
            }

            if (!valid)
            {
                yield return 0;
                Destroy(this.gameObject);
            }
            else
            {
                yield return this.StartCoroutine(otherJoint.AttachBlockJointAnimated(ownJoint, animationTime));
            }
        }

        /// <summary>
        ///     Removes the specified joint from the block.
        /// </summary>
        /// <param name="joint"></param>
        public void RemoveJoint(BlockJoint joint)
        {
            if (joint == null)
            {
                throw new ArgumentNullException();
            }

            if (!this.joints.Contains(joint))
            {
                throw new ArgumentException(
                    "Failed to remove joint: Joint: " + joint + " is not connected to this BuildingBlock.");
            }

            joint.OwningBlock = null;
            joint.transform.parent = null;
            this.joints.Remove(joint);
        }

        #endregion

        #region Methods

        protected virtual Func<BlockJoint, BlockJoint, bool> GetRestrictionPredicate()
        {
            Func<BlockJoint, BlockJoint, bool> predicate = (thisBlocksJoint, otherBlockJoint) => { return true; };

            return predicate;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.joints = new List<BlockJoint>();

            foreach (var joint in this.GetComponentsInChildren<BlockJoint>())
            {
                this.AddJoint(joint);
            }
        }

        #endregion
    }
}