#region usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Assets.Scripts
{
    #region usages

    using UnityEngine;

    #endregion

    public class Entity : MonoBehaviour
    {
        #region Static Fields

        protected static bool isApplicationQuitting = false;

        #endregion

        #region Methods

        protected virtual void AfterInitialize()
        {
        }

        protected virtual void ApplicationQuit()
        {
        }

        protected virtual void Dispose()
        {
        }

        protected virtual void Initialize()
        {
        }

        private void Awake()
        {
            this.Initialize();
        }

        private void OnApplicationQuit()
        {
            isApplicationQuitting = true;
            this.ApplicationQuit();
        }

        private void OnDestroy()
        {
            this.Dispose();
        }

        private void Start()
        {
            this.AfterInitialize();
        }

        #endregion
    }
}
