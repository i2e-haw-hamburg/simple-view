using UnityEngine;
using System.Collections;
using System.Net;
using BeardWire.Interface;
using NetworkMessages;

public class SendCameraFrame : MonoBehaviour
{
    private Texture2D workingTexture;

    private bool connectedToMobileDevice = false;

    [SerializeField]
    private RenderTexture cfgRenderTexture;

    [SerializeField]
    private string cfgMobileDevicePort = "12000";

    [SerializeField]
    private string cfgMobileDeviceAddress = "127.0.0.1";

    void Start()
    {
        workingTexture = new Texture2D(cfgRenderTexture.width, cfgRenderTexture.height);
        this.StartCoroutine(CameraUpdate());
    }
    
    // Update is called once per frame
	IEnumerator CameraUpdate ()
	{
	    while (true)
	    {
            yield return new WaitForEndOfFrame();

	        if (connectedToMobileDevice)
	        {
	            RenderTexture.active = cfgRenderTexture;
	            workingTexture.ReadPixels(new Rect(0, 0, cfgRenderTexture.width, cfgRenderTexture.height), 0, 0);
	            workingTexture.Apply();

                var screenMessage = new CameraScreenMessage();
                screenMessage.cameraFrame = this.workingTexture.EncodeToJPG();

                NetworkAdapterFactory.GetUnityNetworkAdapterInstance()
                    .SendMessageOverTCP(screenMessage, IPAddress.Parse(cfgMobileDeviceAddress), int.Parse(this.cfgMobileDevicePort));
	        }
	    }
	}
}
