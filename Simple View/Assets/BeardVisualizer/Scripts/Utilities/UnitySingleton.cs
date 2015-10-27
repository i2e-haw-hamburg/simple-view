#region usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Assets.Scripts.Utilities
{
    #region usages

    using UnityEngine;

    #endregion

    public class UnitySingleton<T> : Entity
        where T : MonoBehaviour
    {
        #region Static Fields

        private static T instance;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the currently active instance of this singleton.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    if (!isApplicationQuitting)
                    {
                        instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                    else
                    {
                        throw new MissingReferenceException("Failed to retrieve instance: Application is quitting.");
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            base.Initialize();
            instance = this.GetComponent<T>();
        }

        #endregion
    }
}
