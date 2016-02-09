using UnityEngine;
using System.Collections;

public class Debug_LoadLevel : MonoBehaviour
{
    [SerializeField]
    private string cfgLoadDebugLevel;

	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForSeconds(2);

        Application.LoadLevel(this.cfgLoadDebugLevel);
	}
}
