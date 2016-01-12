#region usages

using System.Linq;
using Assets.Scripts.ConstructionLogic;
using UnityEngine;
using System.Collections;

#endregion

public class AttachToConstructOnClick : MonoBehaviour
{
    #region Fields

    [SerializeField] private KeyCode cfgKeyToAttach = KeyCode.Space;

    private BuildingPlan plan;

    #endregion

    #region Methods

    private void Start()
    {
        this.plan = this.GetComponent<BuildingPlan>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(this.cfgKeyToAttach))
        {
            if (ObjectTrackingAdapter.Instance.CurrentlySelectedEntity.ID == this.plan.ID)
            {
                this.plan.AttachInstanceOfBuiltBlockToConstruct();
            }
        }
    }

    #endregion
}