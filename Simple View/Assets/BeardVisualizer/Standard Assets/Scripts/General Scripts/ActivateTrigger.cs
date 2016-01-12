#region usages

using UnityEngine;

#endregion

public class ActivateTrigger : MonoBehaviour
{
    #region Fields

    /// The action to accomplish
    public Mode action = Mode.Activate;

    ///
    public bool repeatTrigger = false;

    public GameObject source;

    /// The game object to affect. If none, the trigger work on this game object
    public Object target;

    public int triggerCount = 1;

    #endregion

    #region Enums

    public enum Mode
    {
        Trigger = 0, // Just broadcast the action on to the target

        Replace = 1, // replace target with source

        Activate = 2, // Activate the target GameObject

        Enable = 3, // Enable a component

        Animate = 4, // Start animation on target

        Deactivate = 5 // Decativate target GameObject
    }

    #endregion

    #region Methods

    private void DoActivateTrigger()
    {
        this.triggerCount--;

        if (this.triggerCount == 0 || this.repeatTrigger)
        {
            Object currentTarget = this.target != null ? this.target : this.gameObject;
            Behaviour targetBehaviour = currentTarget as Behaviour;
            GameObject targetGameObject = currentTarget as GameObject;
            if (targetBehaviour != null)
            {
                targetGameObject = targetBehaviour.gameObject;
            }

            switch (this.action)
            {
                case Mode.Trigger:
                    targetGameObject.BroadcastMessage("DoActivateTrigger");
                    break;
                case Mode.Replace:
                    if (this.source != null)
                    {
                        Instantiate(
                            this.source,
                            targetGameObject.transform.position,
                            targetGameObject.transform.rotation);
                        DestroyObject(targetGameObject);
                    }
                    break;
                case Mode.Activate:
                    targetGameObject.SetActive(true);
                    break;
                case Mode.Enable:
                    if (targetBehaviour != null)
                    {
                        targetBehaviour.enabled = true;
                    }
                    break;
                case Mode.Animate:
                    targetGameObject.GetComponent<Animation>().Play();
                    break;
                case Mode.Deactivate:
                    targetGameObject.SetActive(false);
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        this.DoActivateTrigger();
    }

    #endregion
}