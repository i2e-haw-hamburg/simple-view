#region usages

using System;
using System.Collections;

using Holoville.HOTween;

using UnityEngine;

#endregion

namespace Assets.Scripts.ConstructionLogic
{
    #region usages

    using System.Runtime.CompilerServices;

    #endregion

    /// <summary>
    ///     Provides functionality to attach other <see cref="BuildingBlock" /> via their joints.
    /// </summary>
    public class BlockJoint : ConstructionEntity
    {
        #region Public Properties

        /// <summary>
        ///     The block this joint is connected to, if any.
        /// </summary>
        public BuildingBlock ConnectedBlock
        {
            get
            {
                if (this.ConnectedJoint)
                {
                    return this.ConnectedJoint.OwningBlock;
                }

                return null;
            }
        }

        /// <summary>
        ///     The other joint this joint is connected to, if any.
        /// </summary>
        public BlockJoint ConnectedJoint { get; set; }

        /// <summary>
        ///     The rotation newly connected joints must be rotated to, to connect to this joint.
        /// </summary>
        public Quaternion JointConnectionConstraintRotation
        {
            get
            {
                return Quaternion.LookRotation(-this.transform.forward, this.transform.up);
            }
        }

        /// <summary>
        ///     The block this joint is a part of.
        /// </summary>
        public BuildingBlock OwningBlock { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Attaches another joint to this one. The other joint will be rotated to match the
        ///     <see cref="JointConnectionConstraintRotation" />.
        /// </summary>
        /// <param name="otherJoint"></param>
        public void AttachBlockJoint(BlockJoint otherJoint)
        {
            if (this.ConnectedBlock)
            {
                throw new FailedToConnectBlockException(
                    "Failed to connect block: There is already a block connected to this joint. You need to disconnect the connected block first, before connecting a new one. The currently connected block is: "
                    + this.ConnectedBlock + " The joint that you tried to connect is: " + otherJoint);
            }

            this.ConnectedJoint = otherJoint;
            otherJoint.ConnectedJoint = this;

            var deltaToOwningBlock = otherJoint.LocalEntityPosition;
            otherJoint.RotateJoint(this.JointConnectionConstraintRotation);

            otherJoint.OwningBlock.EntityPosition = this.EntityPosition
                                                    - otherJoint.transform.forward * deltaToOwningBlock.magnitude;
        }

        /// <summary>
        ///     Attaches another joint to this one. The other joint will be rotated to match the
        ///     <see cref="JointConnectionConstraintRotation" />.
        ///     The block will be animated to the new position.
        /// </summary>
        /// <param name="otherJoint"></param>
        /// <param name="animationTime"> The time the animation takes to complete. </param>
        public virtual IEnumerator AttachBlockJointAnimated(BlockJoint otherJoint, float animationTime)
        {
            if (this.ConnectedBlock)
            {
                throw new FailedToConnectBlockException(
                    "Failed to connect block: There is already a block connected to this joint. You need to disconnect the connected block first, before connecting a new one. The currently connected block is: "
                    + this.ConnectedBlock + " The joint that you tried to connect is: " + otherJoint);
            }

            this.ConnectedJoint = otherJoint;
            otherJoint.ConnectedJoint = this;

            var deltaToOwningBlock = otherJoint.LocalEntityPosition;

            yield return
                this.StartCoroutine(
                    otherJoint.RotateJointAnimated(this.JointConnectionConstraintRotation, animationTime / 2.0f));

            HOTween.To(
                otherJoint.OwningBlock,
                animationTime / 2.0f,
                new TweenParms().Prop(
                    "EntityPosition",
                    this.EntityPosition - otherJoint.transform.forward * deltaToOwningBlock.magnitude)
                                .Ease(EaseType.EaseOutCubic));
        }

        public void DetachBlockJoint(BlockJoint otherJoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Rotates the joint to the specified rotation. The owning block of this joint will be rotated relative to the joint.
        /// </summary>
        /// <param name="rotation"> The rotation  </param>
        public void RotateJoint(Quaternion rotation)
        {
            // Get the delta between the specified and our current rotation. Transform.RotateAround works with relative rotations only, so we can't just feed it an absolute value.
            var deltaRotation = Quaternion.FromToRotation(
                this.EntityRotation * Vector3.forward,
                rotation * Vector3.forward);

            // Extract the angle and axis of the delta rotation.
            float angle;
            Vector3 axis;
            deltaRotation.ToAngleAxis(out angle, out axis);

            // Rotate the Block around the joint to match the specified rotation.
            this.OwningBlock.transform.RotateAround(this.EntityPosition, axis, angle);
        }

        /// <summary>
        ///     Rotates the joint to the specified rotation. The owning block of this joint will be rotated relative to the joint.
        /// </summary>
        /// <param name="rotation"> The rotation  </param>
        public IEnumerator RotateJointAnimated(Quaternion rotation, float animationTime)
        {
            // Get the delta between the specified and our current rotation. Transform.RotateAround works with relative rotations only, so we can't just feed it an absolute value.
            var deltaRotation = Quaternion.FromToRotation(
                this.EntityRotation * Vector3.forward,
                rotation * Vector3.forward);

            // Extract the angle and axis of the delta rotation.
            float angle;
            Vector3 axis;
            deltaRotation.ToAngleAxis(out angle, out axis);

            float currentRotation = 0;
            float rotationPerSecond = angle / animationTime;

            while (currentRotation < Mathf.Abs(angle))
            {
                var rotationThisFrame = rotationPerSecond * Time.deltaTime;

                // Make sure the rotation does not overshoot.
                if (currentRotation + rotationThisFrame > Mathf.Abs(angle))
                {
                    rotationThisFrame = Mathf.Abs(angle) - currentRotation;
                }

                // Rotate the Block around the joint to match the specified rotation.
                this.OwningBlock.transform.RotateAround(this.EntityPosition, axis, rotationThisFrame);
                currentRotation += rotationThisFrame;
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            base.Initialize();
        }

        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }

        #endregion

        /// <summary>
        ///     Thrown if the attachement of a block to another one fails.
        /// </summary>
        public class FailedToConnectBlockException : Exception
        {
            #region Constructors and Destructors

            public FailedToConnectBlockException()
            {
            }

            public FailedToConnectBlockException(string message)
                : base(message)
            {
            }

            public FailedToConnectBlockException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            #endregion
        }
    }
}
