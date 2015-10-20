using UnityEngine;
using System.Collections;

public class ToggleDebug : MonoBehaviour
{
    private bool _debugActive = false;

    [SerializeField]
    private GameObject cfgDebugPanelGameObject;

	// Use this for initialization
	void Start () {
	    this.cfgDebugPanelGameObject.SetActive(_debugActive);
	}
	
    public void Toggle()
    {
        _debugActive = !_debugActive;
        this.cfgDebugPanelGameObject.SetActive(_debugActive);
    }
}
