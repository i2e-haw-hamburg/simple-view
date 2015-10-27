#region usages

using System.Linq;

using UnityEngine;

using System.Collections;

#endregion

public class MouseDrag : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private Vector3 cfgDragOffset = Vector3.up;

    private Vector3 screenPoint;

    #endregion

    #region Methods

    private void OnMouseDown()
    {
        this.screenPoint = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
    }

    private void OnMouseDrag()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);

        var nearestHit = hits.Where(x => x.collider != this.GetComponent<Collider>()).OrderBy(x => x.distance).FirstOrDefault();

        this.transform.position = nearestHit.point + this.cfgDragOffset;
    }

    #endregion
}
