using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using BeardWire.Interface;
using DG.Tweening;
using NetworkMessages.GestureRecognition.UserCommands;

public class ModelActions : MonoBehaviour
{
    [SerializeField]
    private float cfgScaleMin = 0.1f;

    [SerializeField]
    private float cfgScaleMax = 3.0f;

    private float rotationSpeed = 60.0f;

    private float explosionTime = 1.0f;

    private float explosionOffsetFactor = 6.0f;

    private bool exploded = false;

    private Dictionary<Transform, Vector3> initialChildPositions = new Dictionary<Transform, Vector3>();

    [SerializeField]
    private KeyCode RotateLeftKey = KeyCode.LeftArrow;
    [SerializeField]
    private KeyCode RotateRightKey = KeyCode.RightArrow;

    [SerializeField]
    private KeyCode ExplodeToggleKey = KeyCode.E;
    [SerializeField]
    private KeyCode ImplodeKey = KeyCode.I;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            initialChildPositions.Add(child, child.transform.localPosition);
        }

        RegisterNetworkMessageListeners();
    }

    private void OnDestroy()
    {
        UnregisterNetworkMessageListeners();
    }

    private void RegisterNetworkMessageListeners()
    {
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().SubscribeToMessagesOfType<ScaleAndRotate>(OnScaleAndRotateMessage);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().SubscribeToMessagesOfType<Explode>(OnExplodeMessage);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().SubscribeToMessagesOfType<Implode>(OnImplodeMessage);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().SubscribeToMessagesOfType<Samurai>(OnSamuraiMessage);
    }

    private void UnregisterNetworkMessageListeners()
    {
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().UnsubscribeFromMessagesOfType<ScaleAndRotate>(OnScaleAndRotateMessage);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().UnsubscribeFromMessagesOfType<Explode>(OnExplodeMessage);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().UnsubscribeFromMessagesOfType<Implode>(OnImplodeMessage);
        NetworkAdapterFactory.GetUnityNetworkAdapterInstance().UnsubscribeFromMessagesOfType<Samurai>(OnSamuraiMessage);
    }

    private void OnSamuraiMessage(Samurai message, IPEndPoint remoteendpoint, IPEndPoint localendpoint, Guid transactionid)
    {
        Debug.Log("The Samurai isn't ready yet. But soon he will be...");
    }

    private void OnImplodeMessage(Implode message, IPEndPoint remoteendpoint, IPEndPoint localendpoint, Guid transactionid)
    {
        this.Implode();
    }

    private void OnExplodeMessage(Explode message, IPEndPoint remoteendpoint, IPEndPoint localendpoint, Guid transactionid)
    {
        this.Explode();
    }

    private void OnScaleAndRotateMessage(ScaleAndRotate message, IPEndPoint remoteEndPoint, IPEndPoint localEndPoint, Guid transactionId)
    {
        Debug.Log(string.Format("Got Scale and rotate message: ({0},{1},{2})", message.x_rotation, message.y_rotation, message.z_rotation));

        this.RotateBy(new Vector3(message.x_rotation, message.y_rotation, message.z_rotation));
        this.ScaleTo(new Vector3(message.x_scale, message.y_scale, message.z_scale));
    }

    private void Update()
    {
        if (Input.GetKey(RotateLeftKey))
        {
            this.RotateTo(this.transform.rotation.eulerAngles + new Vector3(0, -rotationSpeed, 0)*Time.deltaTime);
        }
        else if (Input.GetKey(RotateRightKey))
        {
            this.RotateTo(this.transform.rotation.eulerAngles + new Vector3(0, rotationSpeed, 0)*Time.deltaTime);
        }

        if (Input.GetKeyDown(ImplodeKey))
        {
            this.Implode();
        }
        else if (Input.GetKeyDown(ExplodeToggleKey))
        {
            if (!exploded)
            {
                this.Explode();
            }
            else
            {
                this.Implode();
            }
        }
    }

    public void RotateTo(Vector3 newRotation)
    {
        this.RotateTo(Quaternion.Euler(newRotation));
    }

    public void RotateTo(Quaternion newRotation)
    {
        this.transform.rotation = newRotation;
    }

    public void RotateBy(Vector3 deltaRotation)
    {
        this.transform.rotation = Quaternion.Euler(deltaRotation + this.transform.rotation.eulerAngles);
    }

    public void RotateBy(Quaternion deltaRotation)
    {
        this.transform.rotation = transform.rotation * deltaRotation;
    }

    public void ScaleTo(Vector3 newScale)
    {
        newScale.x = Mathf.Clamp(newScale.x, this.cfgScaleMin, this.cfgScaleMax);
        newScale.y = Mathf.Clamp(newScale.y, this.cfgScaleMin, this.cfgScaleMax);
        newScale.z = Mathf.Clamp(newScale.z, this.cfgScaleMin, this.cfgScaleMax);

        this.transform.localScale = newScale;
    }

    public void Implode()
    {
        if (!exploded)
        {
            return;
        }

        foreach (var childPos in initialChildPositions)
        {
            childPos.Key.DOLocalMove(childPos.Value, explosionTime);
        }

        exploded = false;
    }

    public void Explode()
    {
        if (exploded)
        {
            return;
        }

        foreach (var childPos in initialChildPositions)
        {
            childPos.Key.DOLocalMove(childPos.Value + childPos.Value * explosionOffsetFactor, explosionTime);
        }

        exploded = true;
    }
}