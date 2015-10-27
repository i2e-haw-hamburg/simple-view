#region usages

using UnityEngine;

#endregion

namespace Assets.Scripts.Tests
{
    #region usages

    using Assets.Scripts.ConstructionLogic;

    #endregion

    public class ComplexAttachBuildingBlocksTest : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private BuildingBlock[] cfgAttachedBlocks;

        [SerializeField]
        private BuildingBlock cfgRootBlock;

        #endregion

        // Use this for initialization

        #region Methods

        private void Start()
        {
            foreach (var block in this.cfgAttachedBlocks)
            {
                block.AttachOtherBuildingBlock(this.cfgRootBlock);
            }
        }

        #endregion
    }
}
