namespace Assets.Scripts.ConstructionLogic
{
    #region usages

    using System;

    using UnityEngine;

    using ConstructionUtilities;

    #endregion

    /// <summary>
    ///     Representation of physical blocks that are used by the user to control the system.
    /// </summary>
    public class BuildingPlan : ConstructionEntity
    {
        #region Fields

        [SerializeField]
        private BuildingBlock cfgBuiltBlock;

        [SerializeField]
        private Vector3 cfgBuiltBlockGhostOffset;

        private bool selectedByUser = false;

        /// <summary>
        /// The ghost block used by this BuildingPlan to visualize the position of the BuildingBlock that will be built.
        /// </summary>
        public BuildingBlock GhostBlock { get; private set; }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The block that is built by this plan.
        /// </summary>
        public BuildingBlock BuiltBlock
        {
            get
            {
                return this.cfgBuiltBlock;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Attaches a new instance of the <see cref="BuiltBlock" /> to the <see cref="Construct" />.
        /// </summary>
        public void AttachInstanceOfBuiltBlockToConstruct()
        {
            var newBlockInstance =
                Instantiate(
                    this.cfgBuiltBlock,
                    this.EntityPosition + this.transform.TransformDirection(this.cfgBuiltBlockGhostOffset),
                    this.EntityRotation) as BuildingBlock;
            
            var block = newBlockInstance;

            this.StartCoroutine(Construct.Instance.AttachBuildingBlockAnimated(block));
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            base.Initialize();

            this.GhostBlock =
                Instantiate(
                    this.cfgBuiltBlock,
                    this.EntityPosition + this.transform.TransformDirection(this.cfgBuiltBlockGhostOffset),
                    this.EntityRotation) as BuildingBlock;

            this.GhostBlock.transform.parent = this.transform;
        }

        public void Update()
        {
            var selected = this == ObjectTrackingAdapter.Instance.CurrentlySelectedEntity;

            if (this.selectedByUser != selected)
            {
                if (selected)
                {
                    this.GhostBlock.GetComponent<HighlightNearestJoints>().HighlightingActive = true;
                }
                else
                {
                    this.GhostBlock.GetComponent<HighlightNearestJoints>().HighlightingActive = false;
                }

                this.selectedByUser = selected;
            }
        }

        protected override void Dispose()
        {
            base.Dispose();

            if (this.GhostBlock)
            {
                var ghostBlockHighlight = this.GhostBlock.GetComponent<HighlightNearestJoints>();

                if (ghostBlockHighlight)
                {
                    ghostBlockHighlight.HighlightingActive = false;
                }
            }
        }

        #endregion
    }
}
