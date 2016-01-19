using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MenuActions : MonoBehaviour
{
    public void RemoveModel()
    {
        var children = (from Transform child in transform select child.gameObject).ToList();
        children.ForEach(Destroy);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}