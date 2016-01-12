using UnityEngine;
using System.Collections;

public class ShowcaseSpawner : MonoBehaviour
{
    public float SpawnRadius = 3.0f;

    public float SpawnDelay = 0.5f;

    public int numberOfPrefabs = 10;

    public GameObject Prefab;

    // Use this for initialization
    private void OnEnable()
    {
        StartCoroutine(SpawnPrefabs());
    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator SpawnPrefabs()
    {
        for (int i = 0; i < numberOfPrefabs; i++)
        {
            var spawnedInstance =
                (GameObject)
                    Instantiate(Prefab, transform.position + Random.onUnitSphere*Random.Range(0.0f, SpawnRadius),
                        transform.rotation);

            spawnedInstance.transform.parent = this.transform;

            yield return new WaitForSeconds(SpawnDelay);
        }
    }
}