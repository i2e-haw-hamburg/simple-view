using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectTracking.Interface
{
    using UnityEngine;

    using Object = System.Object;

    /// <summary>
    /// A physical object that is tracked by the sensors of the system.
    /// </summary>
    public class TrackedObject : IEquatable<TrackedObject>
    {
        private static Int32 mm_to_m = 1000;

        /// <summary>
        /// The unique ID of the tracked object.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// The category of the physical object. Different categories could have different behaviours in the virtual representation. The semantics of the different values are defined in the visualization. This component obviously doesn't care about the semantics of this property. 
        /// </summary>
        public int TrackedObjectType { get; private set; }
        
        /// <summary>
        /// The X-coordinate of the position of the object in world-space (in meters).
        /// </summary>
        public float PositionX { get; private set; }

        /// <summary>
        /// The Y-coordinate of the position of the object in world-space (in meters).
        /// </summary>
        public float PositionY { get; private set; }

        /// <summary>
        /// The Z-coordinate of the position of the object in world-space (in meters).
        /// </summary>
        public float PositionZ { get; private set; }

        /// <summary>
        /// The rotation of the object on the X-axis in world-space in euler-angles (http://en.wikipedia.org/wiki/Euler_angles).
        /// </summary>
        public float RotationX { get; private set; }

        /// <summary>
        /// The rotation of the object on the Y-axis in world-space in euler-angles (http://en.wikipedia.org/wiki/Euler_angles).
        /// </summary>
        public float RotationY { get; private set; }

        /// <summary>
        /// The rotation of the object on the Z-axis in world-space in euler-angles (http://en.wikipedia.org/wiki/Euler_angles).
        /// </summary>
        public float RotationZ { get; private set; }

        public TrackedObject(long id, int trackedObjectType, float positionX, float positionY, float positionZ, float rotationX = 0, float rotationY = 0, float rotationZ = 0)
        {
            this.Id = id;
            this.TrackedObjectType = trackedObjectType;
            this.PositionX = positionX / mm_to_m;
            this.PositionY = positionY / mm_to_m;
            this.PositionZ = positionZ / mm_to_m;
            this.RotationX = rotationX;
            this.RotationY = rotationY;
            this.RotationZ = rotationZ;
        }

        public bool Equals(TrackedObject other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the properties are equal.
            return this.Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            return hash;
        }
    }

}
