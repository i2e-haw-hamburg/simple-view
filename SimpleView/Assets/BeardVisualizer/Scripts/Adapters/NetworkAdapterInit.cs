// ----------------------------------------------------------------
//  <author> Malte Eckhoff </author>
//  <datecreated> 28.11.2014 </datecreated>
//  <date> 11.05.2015 </date>
// 
//  <copyright file="UnityVS.BeardVisualizer/UnityVS.BeardVisualizer.CSharp/NetworkAdapterInit.cs" owner="Malte Eckhoff" year=2015> 
//   All rights are reserved. Reproduction or transmission in whole or in part, in
//   any form or by any means, electronic, mechanical or otherwise, is prohibited
//   without the prior written consent of the copyright owner.
//  </copyright>
// ----------------------------------------------------------------

using System;
using BeardWire.Interface;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Utilities;

public class NetworkAdapterInit : UnitySingleton<NetworkAdapterInit>
{
    private bool networkAdapterRunning;

    public event Action OnShutdown;

    [SerializeField] private TextAsset cfgNetworkMessageConfigFile;

    // Use this for initialization
    protected override void Initialize()
    {
        base.Initialize();

        networkAdapterRunning = true;
        NetworkAdapterFactory.GetNewUnityNetworkAdapter(DefaultLogger.Instance, this.cfgNetworkMessageConfigFile);
    }

    private void ShutdownNetworkAdapter()
    {
        if (!networkAdapterRunning)
        {
            return;
        }

        if (this.OnShutdown != null)
        {
            DefaultLogger.Instance.Debug("Informing listeners about network shutdown");
            this.OnShutdown();
        }

        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().Reset();
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().Shutdown();
        networkAdapterRunning = false;
    }

    private void OnDisable()
    {
        DefaultLogger.Instance.Debug("NetworkAdapterInit script is disabled: Disabling network adapter.");
        ShutdownNetworkAdapter();
    }

    private void OnApplicationQuit()
    {
        DefaultLogger.Instance.Debug("Application is quitting: Disabling network adapter.");
        ShutdownNetworkAdapter();
    }
}