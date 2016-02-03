#region usages

using UnityEngine;
using System.Collections;

#endregion

public class ResetMarble : MonoBehaviour
{
    #region Fields

    [SerializeField] private KeyCode cfgKeyResetMarble = KeyCode.Space;

    private Vector3 startingPos;

    #endregion

    #region Methods

    //private void OnGUI()
    //{
    //    if (GUILayout.Button(this.cfgButtonResetText))
    //    {
    //        this.Reset();
    //    }
    //}

    private void Reset()
    {
        this.transform.position = this.startingPos;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(cfgKeyResetMarble))
        {
            this.Reset();
        }
    }

    private void Start()
    {
        this.startingPos = this.transform.position;
    }

    #endregion
}