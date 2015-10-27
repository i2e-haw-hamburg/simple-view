#region usages

using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.ActionLogic;
using Assets.Scripts.Adapters.ObjectTracking;
using Assets.Scripts.ConstructionLogic;
using Assets.Scripts.UserActions;
using Assets.Scripts.Utilities;

using BeardLogger.Interface;

using ObjectTracking.Interface;

using UnityEngine;

using System.Collections;

using BeardUnityUtilities.Threading;

#endregion

public class ObjectTrackingAdapter : UnitySingleton<ObjectTrackingAdapter>
{
    #region Fields

    [SerializeField]
    private bool cfgDebugMessages = false;

    [SerializeField]
    private float cfgPositionScale = 10.0f;

    [SerializeField]
    private TrackedObjectType[] cfgTrackedObjectTypeIDs;

    [SerializeField]
    private string cfgObjectTrackingServiceIp;

    [SerializeField]
    private int cfgObjectTrackingServicePort;

    private NetworkedObjectTrackingAdapter objectTracking;

    #endregion

    #region Public Properties

    /// <summary>
    ///     The Construction entity that will be used as the target for executing user actions.
    ///     TODO: Improve the detection algorithm for the currently selected object. The detection of the selected entity is
    ///     currently very primitive. The last object that is rotated or moved is considered to be the selected one. This could
    ///     lead to non intuitive behaviour of the application.
    /// </summary>
    public ConstructionEntity CurrentlySelectedEntity { get; private set; }

    private bool objectTrackingActive = false;

    private ILogger logger;

    #endregion

    #region Methods

    protected override void AfterInitialize()
    {
        base.AfterInitialize();

        logger = BeardLogger.Interface.LoggerFactory.GetDefaultFileLogger();
        var temp = UnityThreadHelper.Dispatcher;
    }

    private void OnGUI()
    {
        if (!objectTrackingActive)
        {
            if (GUILayout.Button("ConnectToObjectTracking"))
            {
                try
                {
                    this.objectTracking = new NetworkedObjectTrackingAdapter(this.cfgObjectTrackingServiceIp, this.cfgObjectTrackingServicePort);

                    this.objectTracking.NewTrackedObjectDetected += this.ObjectTrackingOnNewTrackedObjectDetected;
                    this.objectTracking.TrackedObjectLost += this.ObjectTrackingOnTrackedObjectLost;

                    this.objectTracking.TrackedObjectPositionChange += this.ObjectTrackingOnTrackedObjectPositionChange;
                    this.objectTracking.TrackedObjectRotationChange += this.ObjectTrackingOnTrackedObjectRotationChange;

                    this.objectTracking.StartTracking();
                    this.objectTrackingActive = true;
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while initializing object tracking:\n" + e);
                }
            }
        }
    }

    protected override void Dispose()
    {
        base.Dispose();

        if (this.objectTracking != null)
        {
            this.objectTracking.StopTracking();
        }
    }

    private Vector3 ConvertPosition(Vector3 pos)
    {
        return pos;
        //return new Vector3(pos.x, pos.z, -pos.y);
    }

    private void ObjectTrackingOnNewTrackedObjectDetected(TrackedObject trackedObject)
    {
        if (this.cfgDebugMessages)
        {
            logger.Debug("Message received: New tracked object " + trackedObject.Id);
        }

        try
        {
            var objectId = this.cfgTrackedObjectTypeIDs.Single(x => x.typeId == trackedObject.TrackedObjectType).plan;

            ActionRequester.Instance.RequestAction(
                new CreateNewBuildingPlanAtPosition(
                    trackedObject.Id,
                    new Vector3(trackedObject.PositionX, trackedObject.PositionY, trackedObject.PositionZ),
                    Quaternion.Euler(trackedObject.RotationX, trackedObject.RotationY, trackedObject.RotationZ),
                    objectId));
        }
        catch (InvalidOperationException)
        {
            throw new Exception(
                "Failed to create new tracked object: A tracked object of type: " + trackedObject.TrackedObjectType
                + " is unknown.");
        }
    }

    private void ObjectTrackingOnTrackedObjectLost(long trackedObjectId)
    {
        if (this.cfgDebugMessages)
        {
            this.logger.Debug("Message received: Tracked object lost " + trackedObjectId);
        }

        ActionRequester.Instance.RequestAction(new RemoveBuildingPlan(new Quaternion(), Vector3.zero, trackedObjectId));
    }

    private void ObjectTrackingOnTrackedObjectPositionChange(long id, float x, float y, float z)
    {
        if (this.cfgDebugMessages)
        {
            this.logger.Debug("Message received: Tracked object position change id: " + id);
            this.logger.Debug(x + ", " + y + ", " + z);
        }

        var movedEntity = ConstructionEntityManager.Instance.GetEntityWithID(id);

        if (movedEntity != null)
        {
            this.CurrentlySelectedEntity = movedEntity;
        }

        var convertedPosition = this.ConvertPosition(new Vector3(x, y, z));
        var scaledPosition = convertedPosition * this.cfgPositionScale;

        ActionRequester.Instance.RequestAction(new BuildingPlanMoved(id, scaledPosition));
    }

    private void ObjectTrackingOnTrackedObjectRotationChange(long id, float x, float y, float z)
    {
        if (this.cfgDebugMessages)
        {
            this.logger.Debug("Message received: Tracked object rotation change id: " + id);
        }

        var movedEntity = ConstructionEntityManager.Instance.GetEntityWithID(id);

        if (movedEntity != null)
        {
            this.CurrentlySelectedEntity = movedEntity;
        }

        ActionRequester.Instance.RequestAction(new BuildingPlanRotated(id, Quaternion.Euler(x, y, z)));
    }

    #endregion

    [Serializable]
    public class TrackedObjectType
    {
        #region Fields

        public BuildingPlan plan;

        public int typeId;

        #endregion
    }
}
