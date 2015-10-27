using UnityEngine;
using System.Collections;

public class ResetSphere : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;

    [SerializeField]
    private float cfgMaxVelocity = 10f;

	// Use this for initialization
	void Start ()
	{
	    this.startPosition = transform.position;
	    this.startRotation = transform.rotation;
        this.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * Random.Range(0.0f, cfgMaxVelocity), ForceMode.VelocityChange);
	}

    private void Reset()
    {
        this.transform.position = this.startPosition;
        this.transform.rotation = this.startRotation;

        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        this.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * Random.Range(0.0f, cfgMaxVelocity), ForceMode.VelocityChange);
    }
    
	// Update is called once per frame
	void Update () {
	    if (transform.position.y < -10)
	    {
            Reset();
	    }
	}
}
