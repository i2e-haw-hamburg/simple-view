#region usages

using Assets.Scripts.ConstructionLogic;

using UnityEngine;

using System.Collections;

#endregion

public class AttachToConstructInOrder : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private BuildingBlock[] cfgBlocksToAttach;

    #endregion

    // Use this for initialization

    #region Methods

    private IEnumerator Start()
    {
        for (int i = 0; i < this.cfgBlocksToAttach.Length; i++)
        {
            yield return new WaitForSeconds(1.0f);
            yield return this.StartCoroutine(Construct.Instance.AttachBuildingBlockAnimated(this.cfgBlocksToAttach[i]));
        }
    }

    #endregion
}
