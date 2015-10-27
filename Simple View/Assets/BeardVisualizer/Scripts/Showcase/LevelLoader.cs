using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

    public KeyCode RELEASETHEBALLS;

    public GameObject BALLS;

    public GameObject[] Showcases;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < Showcases.Length; i++)
            {
                Showcases[i].SetActiveRecursively(false);
            }
            Showcases[0].SetActiveRecursively(true);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int i = 0; i < Showcases.Length; i++)
            {
                Showcases[i].SetActiveRecursively(false);
            }
            Showcases[1].SetActiveRecursively(true);
        }

        if (Input.GetKeyDown(RELEASETHEBALLS))
        {
            BALLS.SetActiveRecursively(!BALLS.activeInHierarchy);
        }
	}
}
