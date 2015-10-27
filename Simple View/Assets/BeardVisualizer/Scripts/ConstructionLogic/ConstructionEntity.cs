#region usages

using UnityEngine;

#endregion

namespace Assets.Scripts.ConstructionLogic
{
    #region usages

    using System;

    #endregion

    /// <summary>
    ///     Base class for all entities that are used for construction purposes.
    /// </summary>
    public class ConstructionEntity : Entity
    {
        #region Fields

        private long id;

        private bool idInitialized = false;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The position of the entity in world space.
        /// </summary>
        public Vector3 EntityPosition
        {
            get
            {
                return this.transform.position;
            }
            set
            {
                this.transform.position = value;
            }
        }

        /// <summary>
        ///     The rotation of the entity in world space.
        /// </summary>
        public Quaternion EntityRotation
        {
            get
            {
                return this.transform.rotation;
            }
            set
            {
                this.transform.rotation = value;
            }
        }

        /// <summary>
        ///     The unique identifier of this entity.
        /// </summary>
        public long ID
        {
            get
            {
                return this.id;
            }

            set
            {
                if (!ConstructionEntityManager.Instance.IsIdFree(value))
                {
                    throw new ArgumentException("Failed to set ID: The id " + value + " is already in use.");
                }

                if (this.idInitialized)
                {
                    ConstructionEntityManager.Instance.UnregisterConstructionInstance(this);
                }

                this.id = value;
                this.idInitialized = true;

                ConstructionEntityManager.Instance.RegisterConstructionInstance(this);
            }
        }

        /// <summary>
        ///     The position of the entity in local space.
        /// </summary>
        public Vector3 LocalEntityPosition
        {
            get
            {
                return this.transform.localPosition;
            }

            set
            {
                this.transform.localPosition = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Destroys this entity.
        /// </summary>
        public void Destroy()
        {
            Destroy(this.gameObject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((ConstructionEntity)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ this.ID.GetHashCode();
            }
        }

        #endregion

        #region Methods

        protected override void AfterInitialize()
        {
            base.AfterInitialize();

            if (!this.idInitialized)
            {
                this.ID = ConstructionEntityManager.Instance.GetFreeID();
            }
        }

        protected override void Dispose()
        {
            base.Dispose();

            try
            {
                ConstructionEntityManager.Instance.UnregisterConstructionInstance(this);
            }
            catch (MissingReferenceException)
            {
                // If the application is shutting down, the ConstructionEntityManager might have already been destroyed by now...
                // In this case we don't need to unregister this instance.
            }
        }

        protected bool Equals(ConstructionEntity other)
        {
            return base.Equals(other) && this.ID == other.ID;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        #endregion
    }
}
