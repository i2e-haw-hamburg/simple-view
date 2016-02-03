#region usages

using UnityEngine;
using System.Collections;

#endregion

public class MouseOver : MonoBehaviour
{
    #region Public Properties

    public bool IsMouseOver { get; private set; }

    #endregion

    #region Methods

    private void OnMouseExit()
    {
        this.IsMouseOver = false;
    }

    private void OnMouseOver()
    {
        this.IsMouseOver = true;
    }

    #endregion
}