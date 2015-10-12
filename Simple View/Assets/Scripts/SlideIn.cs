using UnityEngine;
using System.Collections;

public class SlideIn : MonoBehaviour
{

    private bool _show = false;
    private RectTransform _panel;


    public void Start()
    {
        _panel = GetComponent<RectTransform>();
    }
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyUp(KeyCode.Space))
	    {
	        _show = !_show;
	    }

	    if (_show)
	    {
	        _panel.localPosition = new Vector3(Mathf.Lerp(-_panel.rect.width, 0, (float) (0.4* Time.time)), _panel.localPosition.y, _panel.localPosition.z);
	    }
        
	}
}
