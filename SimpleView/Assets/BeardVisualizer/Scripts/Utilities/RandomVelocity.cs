using UnityEngine;
using System.Collections;

public class RandomVelocity : MonoBehaviour
{
    [SerializeField] private float cfgMaxVelocity = 10f;

    // Use this for initialization
    private void Start()
    {
        this.GetComponent<Rigidbody>()
            .AddForce(Random.onUnitSphere*Random.Range(0.0f, cfgMaxVelocity), ForceMode.VelocityChange);
    }
}