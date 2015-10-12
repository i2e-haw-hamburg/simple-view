using UnityEngine;
using System.Collections;

public class ToggleSidebar : MonoBehaviour
{
    private bool sidebarActive = false;

    [SerializeField]
    private GameObject cfgSidebarGameObject;

	// Use this for initialization
	void Start () {
	    this.cfgSidebarGameObject.SetActive(sidebarActive);
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButtonDown("ToggleSideMenu"))
	    {
	        sidebarActive = !sidebarActive;
            this.cfgSidebarGameObject.SetActive(sidebarActive);
	    }
	}

    public void HideSidebar()
    {
        sidebarActive = false;
        this.cfgSidebarGameObject.SetActive(false);
    }
}
