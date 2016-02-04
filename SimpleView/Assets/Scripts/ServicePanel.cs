using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.UI;

public class ServicePanel : MonoBehaviour {

    [SerializeField]
    private string defaultPort = "12345";
    [SerializeField]
    private string defaultIP = "127.0.0.1";

    [SerializeField]
    private GameObject cfgPortField;

    [SerializeField]
    private GameObject cfgIPField;

    private InputField _ipTextField;
    private InputField _portTextField;

    // Use this for initialization
    void Start () {
        _ipTextField = cfgIPField.GetComponent<InputField>();
        _ipTextField.text = defaultIP;
        _portTextField = cfgPortField.GetComponent<InputField>();
        _portTextField.text = defaultPort;
    }

    public void ConnectToService()
    {
        Debug.Log("Connect to Service on " + _ipTextField.text + ":" + _portTextField.text);
        var ip = IPAddress.Parse(_ipTextField.text);
        int port = int.Parse(_portTextField.text);
        GestureRecognitionAdapter.Instance.ConnectToGestureRecognition(ip, port);
    }
}
