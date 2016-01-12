#region usages

using System;
using System.Collections;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets.Scripts.Tests
{
    #region usages

    using Assets.Scripts.ConstructionLogic;
    using Random = UnityEngine.Random;

    #endregion

    public class AnimatedComplexAttachBuildingBlocksTest : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Wave[] cfgAttachedBlocksWaves;

        [SerializeField] private BuildingBlock cfgRootBlock;

        #endregion

        // Use this for initialization

        #region Methods

        private IEnumerator Start()
        {
            float animationTime = 1.5f;

            BuildingBlock[] availableBlocks = new[] {this.cfgRootBlock};

            this.cfgRootBlock.EntityRotation = Random.rotation;

            foreach (var wave in this.cfgAttachedBlocksWaves)
            {
                foreach (var block in wave.Blocks)
                {
                    block.EntityPosition = Random.onUnitSphere*Random.Range(3.0f, 10.0f);
                    block.EntityRotation = Random.rotation;
                }
            }

            for (int i = 0; i < this.cfgAttachedBlocksWaves.Length; i++)
            {
                yield return new WaitForSeconds(animationTime);

                foreach (var block in this.cfgAttachedBlocksWaves[i].Blocks)
                {
                    var freeBlocks = availableBlocks.Where(x => x.HasUnconnectedJoints).ToList();
                    this.StartCoroutine(
                        block.AttachOtherBuildingBlockAnimated(
                            freeBlocks[Random.Range(0, freeBlocks.Count)],
                            animationTime));
                    yield return new WaitForSeconds(0.5f);
                }

                availableBlocks = this.cfgAttachedBlocksWaves[i].Blocks;
            }

            yield return new WaitForSeconds(2.0f);

            Application.LoadLevel(Application.loadedLevel);
        }

        #endregion

        [Serializable]
        public class Wave
        {
            #region Fields

            public BuildingBlock[] Blocks;

            #endregion
        }
    }
}