#region usages

using UnityEngine;

#endregion

namespace Assets.Scripts.Tests
{
    #region usages

    using Assets.Scripts.ConstructionLogic;

    #endregion

    public class AttachBuildingBlocksTest : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private BuildingBlock cfgAttachBlock;

        [SerializeField]
        private BuildingBlock cfgRootBlock;

        #endregion

        // Use this for initialization

        #region Methods

        private void Start()
        {
            this.cfgAttachBlock.AttachOtherBuildingBlock(this.cfgRootBlock);
        }

        #endregion
    }
}
