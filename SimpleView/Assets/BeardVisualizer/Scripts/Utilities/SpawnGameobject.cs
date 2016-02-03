using UnityEngine;
using System.Collections;

public class SpawnGameobject : MonoBehaviour
{
    [SerializeField] private GameObject cfgSpawnedObject;

    [SerializeField] private float cfgSpawnIntervall;

    // Use this for initialization
    private void Start()
    {
        StartCoroutine(SpawnGameObjectCoroutine());
    }

    // Update is called once per frame
    private IEnumerator SpawnGameObjectCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(cfgSpawnIntervall);
            var spawned = Instantiate(cfgSpawnedObject, transform.position, transform.rotation) as GameObject;
            spawned.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere*10, ForceMode.VelocityChange);
        }
    }
}