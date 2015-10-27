// ----------------------------------------------------------------
//  <author> Malte Eckhoff </author>
//  <datecreated> 28.11.2014 </datecreated>
//  <date> 11.05.2015 </date>
// 
//  <copyright file="UnityVS.BeardVisualizer/UnityVS.BeardVisualizer.CSharp/NetworkedObjectTrackingAdapter.cs" owner="Malte Eckhoff" year=2015> 
//   All rights are reserved. Reproduction or transmission in whole or in part, in
//   any form or by any means, electronic, mechanical or otherwise, is prohibited
//   without the prior written consent of the copyright owner.
//  </copyright>
// ----------------------------------------------------------------

#region usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using BeardWire.Interface.MessageDeliveryOptions;

#endregion

namespace Assets.Scripts.Adapters.ObjectTracking
{
    #region usages

    using System.Net;

    using BeardWire.Interface;
    using BeardWire.Interface.Exceptions;

    using global::ObjectTracking.Interface;

    using NetworkMessages.ObjectTracking;

    #endregion

    internal class NetworkedObjectTrackingAdapter
    {
        private string objectTrackingServiceIp;

        private int objectTrackingServicePort;

        private IList<TrackedObject> trackedObjects = new List<TrackedObject>();

        public NetworkedObjectTrackingAdapter(string objectTrackingServiceIp, int objectTrackingServicePort)
        {
            this.objectTrackingServiceIp = objectTrackingServiceIp;
            this.objectTrackingServicePort = objectTrackingServicePort;
        }

        public event Action<TrackedObject> NewTrackedObjectDetected;

        public event Action<long> TrackedObjectLost;

        public event Action<long, float, float, float> TrackedObjectPositionChange;

        public event Action<long, float, float, float> TrackedObjectRotationChange;

        public List<TrackedObject> GetAllObjects()
        {
            return this.trackedObjects.ToList();
        }

        public void StartTracking()
        {
            var remoteAddress = IPAddress.Parse(this.objectTrackingServiceIp);

            NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                 .ConnectToTCPRemote(remoteAddress, this.objectTrackingServicePort);

            NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                 .SubscribeToMessagesOfType<ObjectNew>(
                                     (message, address, port, id) =>
                                         {
                                             var trackedObject = new TrackedObject(
                                                 message.id,
                                                 message.object_type,
                                                 message.pos_x,
                                                 message.pos_y,
                                                 message.pos_z);

                                             this.trackedObjects.Add(trackedObject);
                                             this.NewTrackedObjectDetected(trackedObject);
                                         });
            NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                 .SubscribeToMessagesOfType<ObjectLost>(
                                     (message, remoteEndPoint, localEndPoint, transactionId) =>
                                         {
                                             this.trackedObjects.Remove(
                                                 this.trackedObjects.First(x => x.Id == message.id));
                                             this.TrackedObjectLost(message.id);
                                         });
            NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                 .SubscribeToMessagesOfType<PositionChanged>(
                                     new MessageDeliveryOptionOnlyLatestInTimeInterval<PositionChanged>(
                                         (message, remoteEndPoint, localEndPoint, transactionId) =>
                                         this.TrackedObjectPositionChange(message.id, message.x, message.y, message.z),
                                         message => "" + message.id,
                                         30));
            NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                 .SubscribeToMessagesOfType<RotationChanged>(
                                     new MessageDeliveryOptionOnlyLatestInTimeInterval<RotationChanged>(
                                         (message, remoteEndPoint, localEndPoint, transactionId) =>
                                         this.TrackedObjectRotationChange(message.id, message.x, message.y, message.z),
                                         message => "" + message.id,
                                         30));

            var registerMessage = new RegisterForObjectTrackingMessages();
            registerMessage.listenerip = NetworkAdapterFactory.GetUnityNetworkAdapterInstance().LocalAddress.ToString();
            registerMessage.listenerport =
                NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                     .EstablishedTcpConnections.Single(
                                         x =>
                                         x.RemoteEndPoint.Address.Equals(remoteAddress)
                                         && x.RemoteEndPoint.Port == this.objectTrackingServicePort)
                                     .LocalEndPoint.Port;

            NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                 .SendMessageOverTCP(
                                     registerMessage,
                                     IPAddress.Parse(this.objectTrackingServiceIp),
                                     this.objectTrackingServicePort);
        }

        public void StopTracking()
        {
            try
            {
                NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                     .DisconnectFromTCPRemote(
                                         IPAddress.Parse(this.objectTrackingServiceIp),
                                         this.objectTrackingServicePort);
            }
            catch (NotConnectedException)
            {
                // We are already disconnected. This happens if the application is shutting down. In this case we can safely ignore this exception.
            }
        }
    }
}
