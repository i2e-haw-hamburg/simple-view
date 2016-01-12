#region usages

using UnityEngine;

#endregion

namespace Assets.Scripts.Tests
{
    #region usages

    using Assets.Scripts.ConstructionLogic;

    #endregion

    public class JointRotationTest : MonoBehaviour
    {
        #region Fields

        private Quaternion currentRot;

        #endregion

        // Use this for initialization

        #region Methods

        private void Rotate()
        {
            var joint = this.GetComponent<BlockJoint>();
            var wantedRot = Quaternion.Euler(this.currentRot.eulerAngles + (Vector3.up));
            joint.RotateJoint(wantedRot);
            this.currentRot = wantedRot;
        }

        private void Start()
        {
            var joint = this.GetComponent<BlockJoint>();
            this.currentRot = joint.EntityRotation;
            this.InvokeRepeating("Rotate", 0, 0.1f);
        }

        #endregion
    }
}