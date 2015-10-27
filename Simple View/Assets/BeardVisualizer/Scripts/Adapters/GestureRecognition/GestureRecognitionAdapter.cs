// ----------------------------------------------------------------
//  <author> Malte Eckhoff </author>
//  <date> 13.04.2015 </date>
// 
//  <copyright file="UnityVS.BeardVisualizer/UnityVS.BeardVisualizer.CSharp/GestureRecognitionAdapter.cs" owner="Malte Eckhoff" year=2015> 
//   All rights are reserved. Reproduction or transmission in whole or in part, in
//   any form or by any means, electronic, mechanical or otherwise, is prohibited
//   without the prior written consent of the copyright owner.
//  </copyright>
// ----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Assets.Scripts.Utilities;

using BeardWire.Interface;
using BeardWire.Interface.MessageDeliveryOptions;
using NetworkMessages;

using UnityEngine;

public class GestureRecognitionAdapter : UnitySingleton<GestureRecognitionAdapter>
{
    [SerializeField]
    private bool cfgDebugChecks = true;

    [SerializeField]
    private GestureRecognitionUserBodyPart cfgBodyPartPrefab;

    [SerializeField]
    private string cfgGestureRecognitionServiceAddress = "127.0.0.1";

    [SerializeField]
    private string cfgGestureRecognitionServicePort = "1234";

    private bool connectedToGestureRecognitionService;

    private IPAddress gestureRecognitionIP;

    private int gestureRecognitionPort;

    private Dictionary<long, GestureInterfaceUser> users;

    private void Start()
    {
        this.users = new Dictionary<long, GestureInterfaceUser>();

        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .SubscribeToMessagesOfType<UserInitMessage>(this.OnUserInitMessage);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .SubscribeToMessagesOfType<UserRemoveMessage>(this.OnUserRemoveMessage);
        
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .SubscribeToMessagesOfType<UserUpdateMessage>(new MessageDeliveryOptionOnlyLatestInTimeInterval<UserUpdateMessage>(this.OnUserUpdateMessage, message => "UserUpdates" + message.userId, 30));
        
        NetworkAdapterInit.Instance.OnShutdown += this.DisconnectFromGestureRecognitionService;
    }

    private void DisconnectFromGestureRecognitionService()
    {
        DefaultLogger.Instance.Debug("Disconnecting from gesture recognition service.");

        try
        {
            if (this.connectedToGestureRecognitionService)
            {
                try
                {
                    var userKeys = this.users.Keys.ToArray();

                    for (int i = userKeys.Length - 1; i >= 0; i--)
                    {
                        this.RemoveUserWithID(userKeys[i]);
                    }

                    this.users.Clear();
                }
                catch (Exception e)
                {
                    DefaultLogger.Instance.Error("Error while disconnecting from gesture recognition service service: " + e);
                }
                
                NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                     .SendMessageOverTCP(
                                         new UnregisterFromGestureInterfaceMessage(),
                                         this.gestureRecognitionIP,
                                         this.gestureRecognitionPort);
                NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                         .DisconnectFromTCPRemote(this.gestureRecognitionIP, this.gestureRecognitionPort);

                this.connectedToGestureRecognitionService = false;
            }
        }
        catch (Exception e)
        {
            DefaultLogger.Instance.Error(
                "Failed to unregister from gesture recognition adapter. See the inner exception for more details:\n\t"
                + e);
        }
    }
    
    private void OnGUI()
    {
        GUILayout.Space(100);

        var currentInputValid = false;

        if (!this.connectedToGestureRecognitionService)
        {
            GUILayout.Label("Gesture recognition service IP Address:");
            this.cfgGestureRecognitionServiceAddress = GUILayout.TextField(this.cfgGestureRecognitionServiceAddress);

            currentInputValid = IPAddress.TryParse(
                this.cfgGestureRecognitionServiceAddress,
                out this.gestureRecognitionIP);

            if (!currentInputValid)
            {
                GUILayout.Label("Invalid IP address");
            }

            GUILayout.Label("Gesture recognition service Port:");
            this.cfgGestureRecognitionServicePort = GUILayout.TextField(this.cfgGestureRecognitionServicePort);

            currentInputValid = currentInputValid
                                && int.TryParse(this.cfgGestureRecognitionServicePort, out this.gestureRecognitionPort);

            if (!currentInputValid)
            {
                GUILayout.Label("Invalid port");
            }

            GUI.enabled = currentInputValid;

            if (GUILayout.Button("Connect to gesture recognition service"))
            {
                try
                {
                    NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                         .ConnectToTCPRemote(this.gestureRecognitionIP, this.gestureRecognitionPort);

                    NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                         .SendMessageOverTCP(
                                             new RegisterForGestureInterfaceMessage(),
                                             this.gestureRecognitionIP,
                                             this.gestureRecognitionPort);

                    this.connectedToGestureRecognitionService = true;
                }
                catch (Exception e)
                {
                    DefaultLogger.Instance.Error(
                        "Failed to connect to gesture recognition service. See the inner exception for more details:\n\t"
                        + e);
                }
            }

            GUI.enabled = true;
        }
        else
        {
            if (GUILayout.Button("Disconnect from gesture recognition service."))
            {
                this.DisconnectFromGestureRecognitionService();
            }
        }
    }

    private void OnUserUpdateMessage(
        UserUpdateMessage message,
        IPEndPoint remoteEndPoint,
        IPEndPoint localEndPoint,
        Guid transactionId)
    {
        if (!this.users.ContainsKey(message.userId))
        {
            DefaultLogger.Instance.Error(
                "Failed to update user with ID " + message.userId + ": There is no user registered with this ID.");
            return;
        }
        
        DefaultLogger.Instance.Debug("Got user update message with following contents:"
            + "\nuser ID: " + message.userId
            + "\nbody part positions (X,Y,Z):\n" 
                + message.bodyPartPositionX.Select(x => x + "").DefaultIfEmpty().Aggregate<string>((a, b) => a + ", " + b) + ";\n"
                + message.bodyPartPositionY.Select(x => x + "").DefaultIfEmpty().Aggregate<string>((a, b) => a + ", " + b) + ";\n"
                + message.bodyPartPositionZ.Select(x => x + "").DefaultIfEmpty().Aggregate<string>((a, b) => a + ", " + b)
            + "\nbody part rotations (X,Y,Z,W):\n"
                + message.bodyPartRotationX.Select(x => x + "").DefaultIfEmpty().Aggregate<string>((a, b) => a + ", " + b) + ";\n"
                + message.bodyPartRotationY.Select(x => x + "").DefaultIfEmpty().Aggregate<string>((a, b) => a + ", " + b) + ";\n"
                + message.bodyPartRotationZ.Select(x => x + "").DefaultIfEmpty().Aggregate<string>((a, b) => a + ", " + b) + ";\n"
                + message.bodyPartRotationW.Select(x => x + "").DefaultIfEmpty().Aggregate<string>((a, b) => a + ", " + b));

        this.users[message.userId].UpdateUserPositionAndRotation(
            message.bodyPartId,
            message.bodyPartPositionX,
            message.bodyPartPositionY,
            message.bodyPartPositionZ,
            message.bodyPartRotationX,
            message.bodyPartRotationY,
            message.bodyPartRotationZ,
            message.bodyPartRotationW);

        this.users[message.userId].UpdateUserVelocityAndAcceleration(
            message.bodyPartId,
            message.bodyPartVelocityX,
            message.bodyPartVelocityY,
            message.bodyPartVelocityZ,
            message.bodyPartAccelerationX,
            message.bodyPartAccelerationY,
            message.bodyPartAccelerationZ);

        this.users[message.userId].UpdateUserAngularVelocityAndAcceleration(
            message.bodyPartId,
            message.bodyPartAngularVelocityX,
            message.bodyPartAngularVelocityY,
            message.bodyPartAngularVelocityZ,
            message.bodyPartAngularAccelerationX,
            message.bodyPartAngularAccelerationY,
            message.bodyPartAngularAccelerationZ);
    }

    private void OnUserRemoveMessage(
        UserRemoveMessage message,
        IPEndPoint remoteEndPoint,
        IPEndPoint localEndPoint,
        Guid transactionId)
    {
        if (!this.users.ContainsKey(message.userId))
        {
            DefaultLogger.Instance.Error(
                "Failed to remove user with ID " + message.userId + ": There is no user registered with this ID.");
            return;
        }

        this.RemoveUserWithID(message.userId);
    }

    private void RemoveUserWithID(long id)
    {
        try
        {
            this.users[id].DestroyUser();
            this.users.Remove(id);
        }
        catch (Exception e)
        {
            DefaultLogger.Instance.Error(
                "Failed to remove user with ID " + id + ": See the inner exception for more details:\n\t" + e);
        }
    }

    private void OnUserInitMessage(
        UserInitMessage message,
        IPEndPoint remoteEndPoint,
        IPEndPoint localEndPoint,
        Guid transactionId)
    {
        GameObject userGameobject;

        try
        {
            userGameobject = new GameObject("User" + message.userId);

            var userBodyParts = new Dictionary<int, GestureRecognitionUserBodyPart>();

            for (var i = 0; i < message.bodyPartId.Count; i++)
            {
                var bodyPartInitialPosition =
                    ConversionUtilities.NetworkWorldCoordinatesToUnityCoordinates(
                        new Vector3(
                            message.bodyPartPositionX[i],
                            message.bodyPartPositionY[i],
                            message.bodyPartPositionZ[i]));

                var bodyPartInitialRotation = new Quaternion(
                    message.bodyPartRotationX[i],
                    message.bodyPartRotationY[i],
                    message.bodyPartRotationZ[i],
                    1);

                var bodyPartLength = message.bodyPartLength[i];

                var bodyPart = Instantiate(this.cfgBodyPartPrefab);
                bodyPart.BodyPartId = message.bodyPartId[i];
                bodyPart.transform.position = bodyPartInitialPosition;
                bodyPart.transform.rotation = bodyPartInitialRotation;
                bodyPart.Length = bodyPartLength;

                var scale = bodyPart.transform.localScale;
                scale.x = scale.x * 0.1f;
                scale.z = scale.z * 0.1f;
                bodyPart.transform.localScale = scale;

                bodyPart.transform.parent = userGameobject.transform;
                userBodyParts.Add(message.bodyPartId[i], bodyPart);

                DefaultLogger.Instance.Debug(
                    "Registered new body part for user with ID " + message.userId + " body part is " + bodyPart);
            }

            var bodyParts = userBodyParts.Values;

            // Ignore collisions with other body parts of the same user.
            foreach (var bodyPart in bodyParts)
            {
                foreach (var otherBodyPart in bodyParts)
                {
                    if (otherBodyPart != bodyPart)
                    {
                        Physics.IgnoreCollision(bodyPart.Collider, otherBodyPart.Collider);
                    }
                }
            }

            var newUser = new GestureInterfaceUser(message.userId, userBodyParts, userGameobject);
            this.users.Add(message.userId, newUser);

            DefaultLogger.Instance.Debug("Registered new user " + newUser);
        }
        catch (Exception e)
        {
            DefaultLogger.Instance.Error(
                "Failed to initialize Gesture interface user. See the inner exception for more details:\n\t" + e);
        }
    }

    /// <summary>
    ///     Represents a user of the gesture recognition system.
    /// </summary>
    private class GestureInterfaceUser
    {
        public GestureInterfaceUser(long userId, Dictionary<int, GestureRecognitionUserBodyPart> bodyParts, GameObject userRootGameObject)
        {
            this.UserId = userId;
            this.BodyParts = bodyParts;
            this.UserRootGameObject = userRootGameObject;
        }

        public GameObject UserRootGameObject { get; private set; }

        /// <summary>
        ///     The id of the user.
        /// </summary>
        public long UserId { get; private set; }

        /// <summary>
        ///     The body parts of this user.
        /// </summary>
        public Dictionary<int, GestureRecognitionUserBodyPart> BodyParts { get; private set; }

        public override string ToString()
        {
            return string.Format("UserId: {0}, BodyParts: {1}", this.UserId, this.BodyParts);
        }

        public void DestroyUser()
        {
            foreach (var bodyPart in this.BodyParts)
            {
                Destroy(bodyPart.Value);
            }

            Destroy(this.UserRootGameObject);
            this.BodyParts.Clear();
        }

        public void UpdateUserPositionAndRotation(
            List<int> bodyPartIds,
            List<float> bodyPartPositionsX,
            List<float> bodyPartPositionsY,
            List<float> bodyPartPositionsZ,
            List<float> bodyPartRotationsX,
            List<float> bodyPartRotationsY,
            List<float> bodyPartRotationsZ,
            List<float> bodyPartRotationsW)
        {
            if (Instance.cfgDebugChecks)
            {
                // Check the number of positions
                if (bodyPartIds.Count != bodyPartPositionsX.Count)
                {
                    DefaultLogger.Instance.Error(
                        "bodyPartPositionsX count is not equal to the number of IDs: BodyPartsIds: " + bodyPartIds.Count
                        + ", bodyPartPositions: " + bodyPartPositionsX.Count);
                    return;
                }
                if (bodyPartIds.Count != bodyPartPositionsY.Count)
                {
                    DefaultLogger.Instance.Error(
                        "bodyPartPositionsY count is not equal to the number of IDs: BodyPartsIds: " + bodyPartIds.Count
                        + ", bodyPartPositions: " + bodyPartPositionsY.Count);
                    return;
                }
                if (bodyPartIds.Count != bodyPartPositionsZ.Count)
                {
                    DefaultLogger.Instance.Error(
                        "bodyPartPositionsZ count is not equal to the number of IDs: BodyPartsIds: " + bodyPartIds.Count
                        + ", bodyPartPositions: " + bodyPartPositionsZ.Count);
                    return;
                }

                // Check the number of rotations
                if (bodyPartIds.Count != bodyPartRotationsX.Count)
                {
                    DefaultLogger.Instance.Error(
                        "bodyPartRotationsX count is not equal to the number of IDs: BodyPartsIds: " + bodyPartIds.Count
                        + ", bodyPartPositions: " + bodyPartRotationsX.Count);
                    return;
                }
                if (bodyPartIds.Count != bodyPartRotationsY.Count)
                {
                    DefaultLogger.Instance.Error(
                        "bodyPartRotationsY count is not equal to the number of IDs: BodyPartsIds: " + bodyPartIds.Count
                        + ", bodyPartPositions: " + bodyPartRotationsY.Count);
                    return;
                }
                if (bodyPartIds.Count != bodyPartRotationsZ.Count)
                {
                    DefaultLogger.Instance.Error(
                        "bodyPartRotationsZ count is not equal to the number of IDs: BodyPartsIds: " + bodyPartIds.Count
                        + ", bodyPartPositions: " + bodyPartRotationsZ.Count);
                    return;
                }
                if (bodyPartIds.Count != bodyPartRotationsW.Count)
                {
                    DefaultLogger.Instance.Error(
                        "bodyPartRotationsW count is not equal to the number of IDs: BodyPartsIds: " + bodyPartIds.Count
                        + ", bodyPartPositions: " + bodyPartRotationsW.Count);
                    return;
                }
            }

            for (var i = 0; i < bodyPartIds.Count; i++)
            {
                var bodyPartId = bodyPartIds[i];
                try
                {
                    DefaultLogger.Instance.Debug("Body part with the following ID will be updated: " + bodyPartId);

                    this.BodyParts[bodyPartId].Position =
                        ConversionUtilities.NetworkWorldCoordinatesToUnityCoordinates(
                            new Vector3(bodyPartPositionsX[i], bodyPartPositionsY[i], bodyPartPositionsZ[i]));
                    this.BodyParts[bodyPartId].Rotation = new Quaternion(
                        bodyPartRotationsX[i],
                        bodyPartRotationsY[i],
                        bodyPartRotationsZ[i],
                        bodyPartRotationsW[i]);
                }
                catch (IndexOutOfRangeException)
                {
                    DefaultLogger.Instance.Error(
                        "Failed to update position and rotation of body part with ID " + bodyPartId
                        + ": There was no position or rotation specified for this body part.");
                }
                catch (KeyNotFoundException)
                {
                    DefaultLogger.Instance.Error(
                        "Failed to update position and rotation of body part with ID " + bodyPartId
                        + ": There is no body part with this ID.");
                }
                catch (Exception e)
                {
                    DefaultLogger.Instance.Error(
                        "Unexpected error while updating body part. See the inner exception for more details:\n\t" + e);
                }
            }
        }

        public void UpdateUserVelocityAndAcceleration(
            List<int> bodyPartIds,
            List<float> bodyPartVelocitiesX,
            List<float> bodyPartVelocitiesY,
            List<float> bodyPartVelocitiesZ,
            List<float> bodyPartAccelerationsX,
            List<float> bodyPartAccelerationsY,
            List<float> bodyPartAccelerationsZ)
        {
            for (var i = 0; i < bodyPartIds.Count; i++)
            {
                var bodyPartId = bodyPartIds[i];
                try
                {
                    this.BodyParts[bodyPartId].Velocity =
                        ConversionUtilities.NetworkWorldCoordinatesToUnityCoordinates(
                            new Vector3(bodyPartVelocitiesX[i], bodyPartVelocitiesY[i], bodyPartVelocitiesZ[i]));
                    this.BodyParts[bodyPartId].Acceleration =
                        ConversionUtilities.NetworkWorldCoordinatesToUnityCoordinates(
                            new Vector3(bodyPartAccelerationsX[i], bodyPartAccelerationsY[i], bodyPartAccelerationsZ[i]));
                }
                catch (IndexOutOfRangeException)
                {
                    DefaultLogger.Instance.Error(
                        "Failed to update velocity and acceleration of body part with ID " + bodyPartId
                        + ": There was no velocity or acceleration specified for this body part.");
                }
                catch (KeyNotFoundException)
                {
                    DefaultLogger.Instance.Error(
                        "Failed to update velocity and acceleration of body part with ID " + bodyPartId
                        + ": There is no body part with this ID.");
                }
                catch (Exception e)
                {
                    DefaultLogger.Instance.Error(
                        "Unexpected error while updating body part. See the inner exception for more details:\n\t" + e);
                }
            }
        }

        public void UpdateUserAngularVelocityAndAcceleration(
            List<int> bodyPartIds,
            List<float> bodyPartAngularVelocitiesX,
            List<float> bodyPartAngularVelocitiesY,
            List<float> bodyPartAngularVelocitiesZ,
            List<float> bodyPartAngularAccelerationsX,
            List<float> bodyPartAngularAccelerationsY,
            List<float> bodyPartAngularAccelerationsZ)
        {
            for (var i = 0; i < bodyPartIds.Count; i++)
            {
                var bodyPartId = bodyPartIds[i];
                try
                {
                    this.BodyParts[bodyPartId].AngularVelocity =
                        ConversionUtilities.NetworkWorldCoordinatesToUnityCoordinates(
                            new Vector3(
                                bodyPartAngularVelocitiesX[i],
                                bodyPartAngularVelocitiesY[i],
                                bodyPartAngularVelocitiesZ[i]));
                    this.BodyParts[bodyPartId].AngularAcceleration =
                        ConversionUtilities.NetworkWorldCoordinatesToUnityCoordinates(
                            new Vector3(
                                bodyPartAngularAccelerationsX[i],
                                bodyPartAngularAccelerationsY[i],
                                bodyPartAngularAccelerationsZ[i]));
                }
                catch (IndexOutOfRangeException)
                {
                    DefaultLogger.Instance.Error(
                        "Failed to update angular velocity and angular acceleration of body part with ID " + bodyPartId
                        + ": There was no angular velocity and angular acceleration specified for this body part.");
                }
                catch (KeyNotFoundException)
                {
                    DefaultLogger.Instance.Error(
                        "Failed to update angular velocity and angular acceleration of body part with ID " + bodyPartId
                        + ": There is no body part with this ID.");
                }
                catch (Exception e)
                {
                    DefaultLogger.Instance.Error(
                        "Unexpected error while updating body part. See the inner exception for more details:\n\t" + e);
                }
            }
        }
    }
}