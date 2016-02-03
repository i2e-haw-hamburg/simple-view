#region usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Assets.Scripts.ConstructionLogic
{
    #region usages

    using Assets.Scripts.Utilities;

    #endregion

    /// <summary>
    ///     Keeps track of all active instances of <see cref="ConstructionEntity" />.
    /// </summary>
    public class ConstructionEntityManager : UnitySingleton<ConstructionEntityManager>
    {
        #region Fields

        private HashSet<ConstructionEntity> knownEnties;

        private Random random = new Random();

        #endregion

        #region Public Properties

        /// <summary>
        ///     All currently active entities.
        /// </summary>
        public IList<ConstructionEntity> RegisteredConstructionEntities
        {
            get { return this.knownEnties.ToList(); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the entity with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ConstructionEntity GetEntityWithID(long id)
        {
            return this.knownEnties.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        ///     Gets an ID that is currently not assigned to any other entity.
        /// </summary>
        /// <returns></returns>
        public long GetFreeID()
        {
            long id = 0;
            bool freeIdFound = false;

            while (!freeIdFound)
            {
                id = this.random.NextInt64();

                if (this.IsIdFree(id))
                {
                    freeIdFound = true;
                }
            }

            return id;
        }

        /// <summary>
        ///     True if there is no other entity with the specified ID, otherwise false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsIdFree(long id)
        {
            return this.GetEntityWithID(id) == null;
        }

        /// <summary>
        ///     Registers a new entity. This will be called automatically by <see cref="ConstructionEntity" /> on creation. In
        ///     theory it should never be necessary to call it manually.
        /// </summary>
        /// <param name="entity"></param>
        public void RegisterConstructionInstance(ConstructionEntity entity)
        {
            if (this.knownEnties.Any(x => x.ID == entity.ID))
            {
                throw new ArgumentException(
                    "Failed to register new entity: There is already an entity with the ID " + entity.ID
                    + " known. Adding the new entity would overwrite the old one. The new entity is: " + entity
                    + " The old existing entity is " + this.knownEnties.Single(x => x.ID == entity.ID));
            }

            this.knownEnties.Add(entity);
        }

        /// <summary>
        ///     Unregisters an existing entity. This will be called automatically by <see cref="ConstructionEntity" /> on
        ///     destruction. In theory it should never be necessary to call it manually.
        /// </summary>
        /// <param name="entity"></param>
        public void UnregisterConstructionInstance(ConstructionEntity entity)
        {
            if (!this.knownEnties.Any(x => x.Equals(entity)))
            {
                throw new ArgumentException(
                    "Failed to unregister entity: The entity " + entity
                    + " is not known. You need to register entities first, before unregistering them.");
            }

            this.knownEnties.Remove(entity);
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            base.Initialize();
            this.knownEnties = new HashSet<ConstructionEntity>();
        }

        #endregion
    }
}