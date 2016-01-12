#region usages

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Assets.Scripts.ConstructionLogic
{
    #region usages

    using System;
    using System.Collections;
    using Assets.Scripts.ConstructionLogic.ConstructionUtilities;
    using UnityEngine;

    #endregion

    public class Construct : ConstructionEntity
    {
        #region Static Fields

        private static Construct instance;

        #endregion

        #region Fields

        [SerializeField] private float cfgAttachAnimationTime = 0.5f;

        #endregion

        #region Public Properties

        public static Construct Instance
        {
            get
            {
                if (instance == null || !instance)
                {
                    throw new MissingReferenceException(
                        "Failed to get reference of Construct instance: There is no Construct in the scene.");
                }

                return instance;
            }
        }

        /// <summary>
        ///     Gets all BuildingBlocks this construct consists of.
        /// </summary>
        public IList<BuildingBlock> AllBuildingBlocksOfConstruct
        {
            get
            {
                // Keep track of all blocks that have not yet been processed.
                Queue<BuildingBlock> openList = new Queue<BuildingBlock>();

                // Keep track of all blocks that were processed.
                HashSet<BuildingBlock> closedList = new HashSet<BuildingBlock>();

                // The iteration starts with the root block.
                openList.Enqueue(this.RootBlock);

                // Iterate over all blocks of the construct.
                while (openList.Count > 0)
                {
                    // Get the next unprocessed block.
                    var currentBlock = openList.Dequeue();

                    // Check if the block has already been processed. If this is the case, we can stop here and look at the next one.
                    if (closedList.Contains(currentBlock))
                    {
                        continue;
                    }

                    // Mark the block as processed.
                    closedList.Add(currentBlock);

                    // Add all connected blocks that have not yet been processed to the closed list.
                    foreach (var block in currentBlock.DirectlyConnectedBuildingBlocks)
                    {
                        if (!closedList.Contains(block))
                        {
                            openList.Enqueue(block);
                        }
                    }
                }

                return closedList.ToList();
            }
        }

        /// <summary>
        ///     The root of the constructs block hierarchy. This is the first block that exists. All other blocks get attached to
        ///     this one.
        /// </summary>
        public BuildingBlock RootBlock { get; set; }

        #endregion

        #region Public Methods and Operators

        public void AttachBuildingBlock(BuildingBlock blockToAttach)
        {
            SpatialConstructionUtilities.GetNextAvailableBuildingBlockInConstruct(blockToAttach)
                .AttachOtherBuildingBlock(blockToAttach);
        }

        public IEnumerator AttachBuildingBlockAnimated(BuildingBlock blockToAttach)
        {
            if (!blockToAttach)
            {
                throw new ArgumentNullException();
            }

            yield return
                this.StartCoroutine(
                    blockToAttach.AttachOtherBuildingBlockAnimated(
                        SpatialConstructionUtilities.GetNextAvailableBuildingBlockInConstruct(blockToAttach),
                        this.cfgAttachAnimationTime));
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            base.Initialize();
            this.RootBlock = this.GetComponent<BuildingBlock>();

            if (this.RootBlock == null)
            {
                throw new Exception("No root block assigned!");
            }

            instance = this;
        }

        #endregion
    }
}