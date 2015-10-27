// ----------------------------------------------------------------
//  <author> Malte Eckhoff </author>
//  <datecreated> 16.02.2015 </datecreated>
//  <date> 20.07.2015 </date>
// 
//  <copyright file="UnityVS.BeardVisualizer/UnityVS.BeardVisualizer.CSharp/CameraSetup.cs" owner="Malte Eckhoff" year=2015> 
//   All rights are reserved. Reproduction or transmission in whole or in part, in
//   any form or by any means, electronic, mechanical or otherwise, is prohibited
//   without the prior written consent of the copyright owner.
//  </copyright>
// ----------------------------------------------------------------

#region usages

using System;
using System.Collections;
using System.Net;
using System.Threading;

using Assets.Scripts.Utilities;

using BeardUtilities.Tools;

using BeardWire.Interface;
using BeardWire.Interface.MessageDeliveryOptions;

using NetworkMessages;

using UnityEngine;

#endregion

/// <summary>
///     Handles the communication with mobile devices. The camera output is sent to the mobile devices, and commands will
///     be handled from them.
/// </summary>
public class CameraSetup : MonoBehaviour
{
    public enum ImageEncoding
    {
        NONE,

        JPEG,

        PNG
    }

    [SerializeField]
    private bool cfgCompressImages = true;

    /// <summary>
    ///     The amount of compression that is applied to images sent to mobile devices where 1 is highest, 100 is lowest.
    /// </summary>
    [SerializeField]
    private int cfgImageCompression = 1;

    [SerializeField]
    private ImageEncoding cfgImageEncoding = ImageEncoding.JPEG;

    /// <summary>
    ///     The amount of images that are sent to a mobile device per second.
    /// </summary>
    [SerializeField]
    private float cfgImagesSentPerSecond = 30;

    /// <summary>
    ///     The camera that is used to render the scene localy.
    /// </summary>
    [SerializeField]
    private Camera cfgLocalCamera;

    /// <summary>
    ///     The camera that is used to render the scene for the mobile devices.
    /// </summary>
    [SerializeField]
    private Camera cfgMobileRenderCamera;

    [SerializeField]
    private int cfgRenderTextureDepth = 24;

    [SerializeField]
    private int cfgRenderTextureHeight = 540;

    [SerializeField]
    private int cfgRenderTextureWidth = 960;

    [SerializeField]
    private Texture2D cfgStaticImage;

    [SerializeField]
    private bool cfgUseStaticImage = false;

    private Color32[] imageColors;

    private byte[] imageData;

    /// <summary>
    ///     The texture that will be sent to the registered mobile devices each frame.
    /// </summary>
    private RenderTexture renderTexture;

    private Texture2D workingTexture;

    /// <summary>
    ///     The ID of the mobile device.
    /// </summary>
    public long DeviceId { get; set; }

    /// <summary>
    ///     The IP-address of the mobile device.
    /// </summary>
    public IPAddress DeviceAddress { get; set; }

    /// <summary>
    ///     The port of the mobile device.
    /// </summary>
    public int DevicePort { get; set; }

    // Use this for initialization
    private void Start()
    {
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .SubscribeToMessagesOfType<SetCameraFieldOfViewMessage>(this.OnSetCameraFieldOfView);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .SubscribeToMessagesOfType<SetCameraResolutionMessage>(this.OnSetCameraResolution);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .SubscribeToMessagesOfType(
                                 new MessageDeliveryOptionOnlyLatestInTimeInterval<SetCameraTransformMessage>(
                                     this.OnSetCameraTransform,
                                     message => { return "TransformMessages"; },
                                     30));

        this.workingTexture = new Texture2D(this.cfgRenderTextureWidth, this.cfgRenderTextureHeight);
        this.InitializeRenderTexture(
            this.cfgRenderTextureWidth,
            this.cfgRenderTextureHeight,
            this.cfgRenderTextureDepth);

        this.StartCoroutine(this.CameraUpdate());
    }

    private void OnDestroy()
    {
        DefaultLogger.Instance.Debug(string.Format("Camera setup {0} is destroyed.", this.ToString()));

        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .UnsubscribeFromMessagesOfType<SetCameraFieldOfViewMessage>(this.OnSetCameraFieldOfView);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .UnsubscribeFromMessagesOfType<SetCameraResolutionMessage>(this.OnSetCameraResolution);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                             .UnsubscribeFromMessagesOfType<SetCameraTransformMessage>(this.OnSetCameraTransform);
    }

    private void InitializeRenderTexture(int width, int height, int depth)
    {
        if (this.renderTexture)
        {
            this.renderTexture.Release();
        }

        this.imageData = new byte[width * height * 4];
        this.renderTexture = new RenderTexture(width, height, depth);
        this.cfgMobileRenderCamera.targetTexture = this.renderTexture;
    }

    private void OnSetCameraTransform(
        SetCameraTransformMessage message,
        IPEndPoint remoteEndPoint,
        IPEndPoint localEndPoint,
        Guid transactionId)
    {
        try
        {
            // Ignore all messages that don't come from the mobile device of this camera setup.
            if (message.id != this.DeviceId)
            {
                DefaultLogger.Instance.Debug(
                    string.Format(
                        "Ignoring transform message for id: {0} on camera setup {1}",
                        message.id,
                        this.ToString()));
                return;
            }

            DefaultLogger.Instance.Debug(
                string.Format(
                    "Setting camera transform on camera setup {0} to position: x: {1} y: {2} z: {3}\nrotation: x:{4} y:{5} z: {6}",
                    this.ToString(),
                    message.cameraPositionX,
                    message.cameraPositionY,
                    message.cameraPositionZ,
                    message.cameraRotationX,
                    message.cameraRotationY,
                    message.cameraRotationZ));

            this.transform.position = new Vector3(
                message.cameraPositionX,
                message.cameraPositionY,
                message.cameraPositionZ);
            this.transform.rotation = Quaternion.Euler(
                message.cameraRotationX,
                message.cameraRotationY,
                message.cameraRotationZ);
        }
        catch (Exception e)
        {
            DefaultLogger.Instance.Warn(string.Format("Failed to process SetCameraTransformMessage:\n{0}", e));
        }
    }

    private void OnSetCameraResolution(
        SetCameraResolutionMessage message,
        IPEndPoint remoteEndPoint,
        IPEndPoint localEndPoint,
        Guid transactionId)
    {
        // Ignore all messages that don't come from the mobile device of this camera setup.
        if (message.id != this.DeviceId)
        {
            DefaultLogger.Instance.Debug(
                string.Format(
                    "Ignoring resolution message for id: {0} on camera setup {1}",
                    message.id,
                    this.ToString()));
            return;
        }

        DefaultLogger.Instance.Debug(
            string.Format(
                "Setting camera resolution on camera setup {0} to width: {1}, height: {2}",
                this.ToString(),
                message.width,
                message.height));

        this.workingTexture.Resize(message.width, message.height);
        //this.workingTexture.height = message.height;
        //this.workingTexture.width = message.width;

        this.InitializeRenderTexture(message.width, message.height, this.cfgRenderTextureDepth);
    }

    private void OnSetCameraFieldOfView(
        SetCameraFieldOfViewMessage message,
        IPEndPoint remoteEndPoint,
        IPEndPoint localEndPoint,
        Guid transactionId)
    {
        // Ignore all messages that don't come from the mobile device of this camera setup.
        if (message.id != this.DeviceId)
        {
            DefaultLogger.Instance.Debug(
                string.Format(
                    "Ignoring field of view message for id: {0} on camera setup {1}",
                    message.id,
                    this.ToString()));
            return;
        }

        DefaultLogger.Instance.Debug(
            string.Format(
                "Setting camera field of view on camera setup {0} to {1}",
                this.ToString(),
                message.fieldOfView));

        this.SetCameraFov(message.fieldOfView);
    }

    private void SetCameraFov(float fov)
    {
        this.cfgLocalCamera.fieldOfView = fov;
        this.cfgMobileRenderCamera.fieldOfView = fov;
    }

    private Texture2D RTImage(Camera cam)
    {
        var currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        this.workingTexture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        this.workingTexture.Apply(false);
        RenderTexture.active = currentRT;
        return this.workingTexture;
    }

    public override string ToString()
    {
        return
            string.Format(
                "{0}, DeviceId: {1}, DeviceAddress: {2}, DevicePort: {3}, CfgCompressImages: {4}, CfgImageCompression: {5}, CfgImageEncoding: {6}, CfgImagesSentPerSecond: {7}, CfgRenderTextureDepth: {8}, CfgRenderTextureHeight: {9}, CfgRenderTextureWidth: {10}",
                base.ToString(),
                this.DeviceId,
                this.DeviceAddress,
                this.DevicePort,
                this.cfgCompressImages,
                this.cfgImageCompression,
                this.cfgImageEncoding,
                this.cfgImagesSentPerSecond,
                this.cfgRenderTextureDepth,
                this.cfgRenderTextureHeight,
                this.cfgRenderTextureWidth);
    }

    /// <summary>
    ///     Send the camera output to all connected mobile devices each frame.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CameraUpdate()
    {
        RenderTexture.active = this.renderTexture;

        while (true)
        {
            yield return new WaitForSeconds(1.0f / this.cfgImagesSentPerSecond);
            yield return new WaitForEndOfFrame();

            DefaultLogger.Instance.Debug(string.Format("Sending camera update to mobile device: {0}", this.ToString()));

            try
            {
                Texture2D image;

                if (!this.cfgUseStaticImage)
                {
                    // Render the output of the camera into a texture.
                    image = this.RTImage(this.cfgMobileRenderCamera);
                }
                else
                {
                    // Use the given static image for testing purposes.
                    image = this.cfgStaticImage;
                }

                // Encode the image to a byte array
                switch (this.cfgImageEncoding)
                {
                    case ImageEncoding.JPEG:
                        this.imageData = image.EncodeToJPG();
                        break;
                    case ImageEncoding.PNG:
                        this.imageData = image.EncodeToPNG();
                        break;
                    case ImageEncoding.NONE:
                        this.imageColors = image.GetPixels32();
                        var i = 0;

                        for (var j = 0; j < this.imageColors.Length; j++)
                        {
                            this.imageData[i] = this.imageColors[j].r;
                            i++;
                            this.imageData[i] = this.imageColors[j].g;
                            i++;
                            this.imageData[i] = this.imageColors[j].b;
                            i++;
                            this.imageData[i] = this.imageColors[j].a;
                            i++;
                        }
                        break;
                }

                // Put the rendered output into a message.
                var screenMessage = new CameraScreenMessage();

                // Compress the encoded byte array image if configured
                if (this.cfgCompressImages)
                {
                    screenMessage.cameraFrame = ByteArrayCompressionTool.Compress(this.imageData);
                }
                else
                {
                    screenMessage.cameraFrame = this.imageData;
                }

                var loggerForError = DefaultLogger.Instance;
                // Make sure Instance gets not called from a thread, this could upset Unity.
                ThreadPool.QueueUserWorkItem(
                    state =>
                        {
                            try
                            {
                                NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                                                     .SendMessageOverTCP(
                                                         screenMessage,
                                                         this.DeviceAddress,
                                                         this.DevicePort);
                            }
                            catch (Exception e)
                            {
                                loggerForError.Error(
                                    string.Format(
                                        "Error while sending message to mobile device:{0}\n{1}",
                                        " Device ID: " + this.DeviceId + ", Device Address: " + this.DeviceAddress
                                        + ", Device Port:" + this.DevicePort,
                                        e));
                            }
                        });
            }
            catch (Exception e)
            {
                DefaultLogger.Instance.Error(
                    string.Format("Error while sending message to mobile device:{0}\n{1}", this.ToString(), e));
            }
        }
    }
}