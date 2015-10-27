#region usages

using UnityEngine;

using System.Collections;

#endregion

public class MouseWheelRotate : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private float cfgRotationSpeed = 1.0f;

    private bool mouseOver = false;

    #endregion

    #region Methods

    private void OnMouseExit()
    {
        this.mouseOver = false;
    }

    private void OnMouseOver()
    {
        this.mouseOver = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.mouseOver)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
            {
                this.transform.Rotate(Vector3.up, -this.cfgRotationSpeed);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            {
                this.transform.Rotate(Vector3.up, this.cfgRotationSpeed);
            }
        }
    }

    #endregion
}
