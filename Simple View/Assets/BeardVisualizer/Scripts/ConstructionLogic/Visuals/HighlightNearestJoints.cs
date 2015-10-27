#region usages

using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.ConstructionLogic;
using Assets.Scripts.ConstructionLogic.ConstructionUtilities;

using UnityEngine;

using System.Collections;

#endregion

public class HighlightNearestJoints : MonoBehaviour
{
    #region Fields

    private BuildingBlock block;

    private BlockJoint currentlyHighlightedJoint1 = null;

    private BlockJoint currentlyHighlightedJoint2 = null;

    private bool highlightingActive = false;

    #endregion

    #region Public Properties

    /// <summary>
    ///     Indicates if the block joints will be highlighted.
    /// </summary>
    public bool HighlightingActive
    {
        get
        {
            return this.highlightingActive;
        }
        set
        {
            if (this.highlightingActive && !value)
            {
                if (this.currentlyHighlightedJoint1)
                {
                    this.currentlyHighlightedJoint1.GetComponent<BlockJointHighlight>().UnHighlightAnimated();
                }

                if (this.currentlyHighlightedJoint2)
                {
                    this.currentlyHighlightedJoint2.GetComponent<BlockJointHighlight>().UnHighlightAnimated();
                }
            }

            this.highlightingActive = value;
        }
    }

    #endregion

    #region Methods

    private void Start()
    {
        this.block = this.GetComponent<BuildingBlock>();
        this.HighlightingActive = false;

        // Add JointHighlighting behaviours to all joints if the don't have any.
        foreach (var joint in this.block.BlockJoints)
        {
            if (!joint.GetComponent<BlockJointHighlight>())
            {
                joint.gameObject.AddComponent<BlockJointHighlight>();
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!this.HighlightingActive)
        {
            return;
        }

        var nextBlock = SpatialConstructionUtilities.GetNextAvailableBuildingBlockInConstruct(this.block);

        if (!nextBlock)
        {
            return;
        }

        BlockJoint joint1;
        BlockJoint joint2;
        SpatialConstructionUtilities.GetNearestUnconnectedJoints(this.block, nextBlock, out joint1, out joint2);

        if (joint1 != this.currentlyHighlightedJoint1)
        {
            if (this.currentlyHighlightedJoint1)
            {
                this.currentlyHighlightedJoint1.GetComponent<BlockJointHighlight>().UnHighlightAnimated();
            }

            this.currentlyHighlightedJoint1 = joint1;
            joint1.GetComponent<BlockJointHighlight>().HighlightAnimated();
        }

        if (joint2 != this.currentlyHighlightedJoint2)
        {
            if (this.currentlyHighlightedJoint2)
            {
                this.currentlyHighlightedJoint2.GetComponent<BlockJointHighlight>().UnHighlightAnimated();
            }

            this.currentlyHighlightedJoint2 = joint2;
            joint2.GetComponent<BlockJointHighlight>().HighlightAnimated();
        }
    }

    #endregion
}
