using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
public class Toggle : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    private bool _active = false;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private bool cfgDefaultActive = false;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private bool cfgShowOnButtonPress = false;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private GameObject cfgGameObject;

    private void Start()
    {
        _active = cfgDefaultActive;
        this.cfgGameObject.SetActive(_active);
    }

    private void Update()
    {
        // TODO: define in config
        if (cfgShowOnButtonPress && Input.GetButtonDown("ToggleSideMenu"))
        {
            ToggleElement();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideElement()
    {
        _active = false;
        this.cfgGameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowElement()
    {
        _active = true;
        this.cfgGameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ToggleElement()
    {
        _active = !_active;
        this.cfgGameObject.SetActive(_active);
    }
}