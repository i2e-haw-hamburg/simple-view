#region usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Assets.Scripts.Adapters.ObjectTracking
{
    #region usages

    using System.Net.Security;

    using Assets.Scripts.ActionLogic;
    using Assets.Scripts.ConstructionLogic;
    using Assets.Scripts.UserActions;
    using Assets.Scripts.Utilities;

    using global::ObjectTracking.Interface;

    using UnityEngine;

    #endregion

    public class MockObjectTracking : UnitySingleton<MockObjectTracking>
    {
        #region Fields

        [SerializeField]
        private GameObject[] cfgMockTrackedObjects;

        private Dictionary<GameObject, TrackedObject> trackedObjects;

        #endregion

        #region Public Events

        public event Action<TrackedObject> NewTrackedObjectDetected;

        public event Action<long> TrackedObjectLost;

        public event Action<long, float, float, float> TrackedObjectPositionChange;

        public event Action<long, float, float, float> TrackedObjectRotationChange;

        #endregion

        #region Public Methods and Operators

        public List<TrackedObject> GetAllObjects()
        {
            return this.trackedObjects.Values.ToList();
        }

        public void StartTracking()
        {
        }

        public void StopTracking()
        {
        }

        #endregion

        #region Methods

        protected override void AfterInitialize()
        {
            base.AfterInitialize();

            foreach (var tracked in this.cfgMockTrackedObjects)
            {
                var pos = tracked.transform.position;
                var newTrackedObj = new TrackedObject(
                    ConstructionEntityManager.Instance.GetFreeID(),
                    0,
                    pos.x,
                    pos.y,
                    pos.z);

                this.trackedObjects.Add(tracked, newTrackedObj);

                if (this.NewTrackedObjectDetected != null)
                {
                    this.NewTrackedObjectDetected(newTrackedObj);
                }
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.trackedObjects = new Dictionary<GameObject, TrackedObject>();
        }

        private void Update()
        {
            foreach (var mock in this.cfgMockTrackedObjects)
            {
                var trackedObj = this.trackedObjects[mock];
                var oldPos = new Vector3(trackedObj.PositionX, trackedObj.PositionY, trackedObj.PositionZ);
                var oldRot = Quaternion.Euler(trackedObj.RotationX, trackedObj.RotationY, trackedObj.RotationZ);

                var newPos = mock.transform.position;
                var newRot = mock.transform.rotation;

                if (Vector3.Distance(oldPos, newPos) > 0.01f)
                {
                    var newTrackedObj = new TrackedObject(
                        trackedObj.Id,
                        trackedObj.TrackedObjectType,
                        newPos.x,
                        newPos.y,
                        newPos.z);
                    this.trackedObjects[mock] = newTrackedObj;

                    if (this.TrackedObjectPositionChange != null)
                    {
                        this.TrackedObjectPositionChange(
                            newTrackedObj.Id,
                            newTrackedObj.PositionX,
                            newTrackedObj.PositionY,
                            newTrackedObj.PositionZ);
                    }
                }

                trackedObj = this.trackedObjects[mock];

                if (Quaternion.Angle(oldRot, newRot) > 0.01f)
                {
                    var newRotEuler = newRot.eulerAngles;
                    var newTrackedObj = new TrackedObject(
                        trackedObj.Id,
                        trackedObj.TrackedObjectType,
                        trackedObj.PositionX,
                        trackedObj.PositionY,
                        trackedObj.PositionZ,
                        newRotEuler.x,
                        newRotEuler.y,
                        newRotEuler.z);
                    this.trackedObjects[mock] = newTrackedObj;

                    if (this.TrackedObjectPositionChange != null)
                    {
                        this.TrackedObjectRotationChange(
                            newTrackedObj.Id,
                            newTrackedObj.RotationX,
                            newTrackedObj.RotationY,
                            newTrackedObj.RotationZ);
                    }
                }

                if (mock.GetComponent<MouseOver>().IsMouseOver && Input.GetMouseButtonDown(1))
                {
                    var id = this.trackedObjects[mock].Id;
                    ActionRequester.Instance.RequestAction(
                        new AttachBuildingBlockFromBuildingPlanToConstruct(id, Vector3.zero, new Quaternion()));
                }
            }
        }

        #endregion
    }
}
