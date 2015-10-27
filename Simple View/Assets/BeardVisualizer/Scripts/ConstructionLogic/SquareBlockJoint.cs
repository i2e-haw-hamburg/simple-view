#region usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Assets.Scripts.ConstructionLogic
{
    #region usages

    using System.Collections;

    using Holoville.HOTween;

    using UnityEngine;

    #endregion

    public class SquareBlockJoint : BlockJoint
    {
        #region Enums

        public enum BlockJointOrientation
        {
            UP,

            DOWN,

            SIDEWAYS
        }

        #endregion

        #region Public Properties

        public BlockJointOrientation JointOrientation
        {
            get
            {
                if (Mathf.Abs(this.EntityPosition.y - this.OwningBlock.EntityPosition.y) < 0.1f)
                {
                    return BlockJointOrientation.SIDEWAYS;
                }
                if (this.EntityPosition.y > this.OwningBlock.EntityPosition.y)
                {
                    return BlockJointOrientation.UP;
                }
                else if (this.EntityPosition.y < this.OwningBlock.EntityPosition.y)
                {
                    return BlockJointOrientation.DOWN;
                }
                else
                {
                    return BlockJointOrientation.SIDEWAYS;
                }
            }
        }

        #endregion

        private void Update()
        {
            switch(JointOrientation) {
                case BlockJointOrientation.UP:
                    Debug.DrawLine(this.EntityPosition, this.EntityPosition + transform.forward, Color.red);
                    break;
                case BlockJointOrientation.DOWN:
                    Debug.DrawLine(this.EntityPosition, this.EntityPosition + transform.forward, Color.blue);
                    break;
                case BlockJointOrientation.SIDEWAYS:
                    Debug.DrawLine(this.EntityPosition, this.EntityPosition + transform.forward, Color.yellow);
                    break;
            }
            
        }

        #region Public Methods and Operators

        public override IEnumerator AttachBlockJointAnimated(BlockJoint otherJoint, float animationTime)
        {
            //yield return this.StartCoroutine(base.AttachBlockJointAnimated(otherJoint, animationTime));
            yield return 0;

            this.ConnectedJoint = otherJoint;
            otherJoint.ConnectedJoint = this;

            // Snap the other blocks rotation to 90 degree angles.
            var roundedEntityRot = otherJoint.OwningBlock.EntityRotation.eulerAngles;
            //roundedEntityRot.x = Mathf.Round(roundedEntityRot.x / 90.0f) * 90.0f;
            roundedEntityRot.x = 0.0f;
            roundedEntityRot.y = Mathf.Round(roundedEntityRot.y / 90.0f) * 90.0f;
            //roundedEntityRot.z = Mathf.Round(roundedEntityRot.z / 90.0f) * 90.0f;
            roundedEntityRot.z = 0;
            var targetRot = Quaternion.Euler(roundedEntityRot);

            HOTween.To(
                otherJoint.OwningBlock,
                animationTime / 2.0f,
                new TweenParms().Prop("EntityRotation", targetRot).Ease(EaseType.EaseOutCubic));

            HOTween.To(
                otherJoint.OwningBlock,
                animationTime / 2.0f,
                new TweenParms().Prop(
                    "EntityPosition",
                    this.OwningBlock.EntityPosition + (this.EntityPosition - this.OwningBlock.EntityPosition) * 2.0f)
                                .Ease(EaseType.EaseOutCubic).OnComplete(this.OnPositionReached));
        }

        #endregion

        private void OnPositionReached()
        {
            var jointsOfOwningBlock = this.OwningBlock.BlockJoints;
            var allJoints = ConstructionEntityManager.Instance.RegisteredConstructionEntities.OfType<BlockJoint>();

            // TODO: Complexity of algorithm way too high!
            foreach(BlockJoint jointOfOwningBlock in jointsOfOwningBlock) {
                foreach(BlockJoint globalJoint in allJoints) {
                    if (jointOfOwningBlock == globalJoint)
                    {
                        continue;
                    }

                    if (Vector3.Distance(jointOfOwningBlock.EntityPosition, globalJoint.EntityPosition) < 0.1f)
                    {
                        jointOfOwningBlock.ConnectedJoint = globalJoint;
                        globalJoint.ConnectedJoint = jointOfOwningBlock;
                    }
                }
            }
        }
    }
}
