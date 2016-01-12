using UnityEngine;
using System.Collections;

public class ResetCube : MonoBehaviour
{
    [SerializeField] private KeyCode cfgResetKey = KeyCode.Space;

    private bool initialized = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    // Use this for initialization
    private void Start()
    {
        this.startPosition = transform.position;
        this.startRotation = transform.rotation;

        this.initialized = true;
    }

    private void OnEnable()
    {
        if (initialized)
        {
            Reset();
        }
    }

    private void Reset()
    {
        this.transform.position = this.startPosition;
        this.transform.rotation = this.startRotation;

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(this.cfgResetKey))
        {
            Reset();
        }
    }
}