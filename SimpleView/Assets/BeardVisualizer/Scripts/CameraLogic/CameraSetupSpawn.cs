// ----------------------------------------------------------------
//  <author> Malte Eckhoff </author>
//  <datecreated> 16.02.2015 </datecreated>
//  <date> 14.06.2015 </date>
// 
//  <copyright file="UnityVS.BeardVisualizer/UnityVS.BeardVisualizer.CSharp/CameraSetupSpawn.cs" owner="Malte Eckhoff" year=2015> 
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
using NetworkMessages;
using UnityEngine;
using Object = UnityEngine.Object;

public class CameraSetupSpawn : MonoBehaviour
{
    /// <summary>
    ///     The prefab that will be used to spawn new camera setups.
    /// </summary>
    [SerializeField] private CameraSetup cfgCameraSetupPrefab;

    /// <summary>
    ///     The TCP port, the adapter will listen to incoming connections from mobile devices.
    /// </summary>
    [SerializeField] private int cfgMobileDeviceCommunicationPort = 30000;

    /// <summary>
    ///     Holds all mobile devices that have been registered. These devices will receive the camera output.
    /// </summary>
    private IList<CameraSetup> connectedMobileDevices;

    private void Start()
    {
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
            .StartListeningForMessagesOnTCPPort(this.cfgMobileDeviceCommunicationPort);
        this.connectedMobileDevices = new List<CameraSetup>();

        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
            .SubscribeToMessagesOfType<RegisterMobileDeviceMessage>(this.OnMobileDeviceRegistered);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
            .SubscribeToMessagesOfType<UnregisterMobileDeviceMessage>(this.OnMobileDeviceUnregistered);
    }

    private void OnMobileDeviceUnregistered(
        UnregisterMobileDeviceMessage message,
        IPEndPoint remoteEndPoint,
        IPEndPoint localEndPoint,
        Guid transactionId)
    {
        try
        {
            Destroy(this.connectedMobileDevices.FirstOrDefault(x => x.DeviceId == message.id));
            this.connectedMobileDevices.Remove(this.connectedMobileDevices.First(x => x.DeviceId == message.id));
        }
        catch (Exception e)
        {
            DefaultLogger.Instance.Error(
                string.Format("Error while unregistering mobile device with id: {0}\n{1}", message.id, e));
        }

        DefaultLogger.Instance.Debug(
            string.Format(
                "Mobile device unregistered: DeviceId: {0} IPEndpoint: {1}, LocalEndpoint: {2}",
                message.id,
                remoteEndPoint,
                localEndPoint));
    }

    private void OnMobileDeviceRegistered(
        RegisterMobileDeviceMessage message,
        IPEndPoint remoteEndPoint,
        IPEndPoint localEndPoint,
        Guid transactionId)
    {
        CameraSetup newSetup = null;

        try
        {
            newSetup = Instantiate(this.cfgCameraSetupPrefab) as CameraSetup;
            newSetup.DeviceId = message.id;
            newSetup.DeviceAddress = IPAddress.Parse(message.address);
            newSetup.DevicePort = remoteEndPoint.Port;
            this.connectedMobileDevices.Add(newSetup);
        }
        catch (Exception e)
        {
            // Cleanup if an error occurs.
            if (newSetup)
            {
                Destroy(newSetup);
            }
            var setupToRemove = this.connectedMobileDevices.FirstOrDefault(x => x.DeviceId == message.id);
            if (setupToRemove)
            {
                this.connectedMobileDevices.Remove(setupToRemove);
            }

            DefaultLogger.Instance.Error(
                string.Format("Error while registering mobile device with id: {0}\n{1}", message.id, e));
        }

        DefaultLogger.Instance.Debug(
            string.Format(
                "Mobile device registered: DeviceId: {0} DeviceAddress: {3}, DevicePort: {4}, IPEndpoint: {1}, LocalEndpoint: {2}",
                message.id,
                remoteEndPoint,
                localEndPoint,
                message.address,
                message.port));
    }
}