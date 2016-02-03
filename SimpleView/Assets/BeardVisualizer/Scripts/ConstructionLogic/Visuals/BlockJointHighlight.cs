#region usages

using Assets.Scripts.ConstructionLogic;
using Holoville.HOTween;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#endregion

/// <summary>
///     Implements functionality for visibly highlighting joints.
/// </summary>
[RequireComponent(typeof (BlockJoint))]
public class BlockJointHighlight : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The time the highlighting animation will take.
    /// </summary>
    [SerializeField] private float cfgHighlightingAnimationTime = 3.0f;

    /// <summary>
    ///     The color in which a highlighted joint will be rendered.
    /// </summary>
    [SerializeField] private Color cfgJointHighlightColor = Color.red;

    /// <summary>
    ///     The scale factor that will be applied to the initial scale of the joint. The joint will be scaled to this product if highlighted.
    /// </summary>
    [SerializeField] private float cfgHighlightScale = 1.5f;

    private Color initialJointColor;

    private bool isJointHighlited;

    private BlockJoint joint;

    private Renderer jointRenderer;

    private Vector3 initialScale;

    #endregion

    #region Public Methods and Operators

    public void Highlight()
    {
        transform.localScale = initialScale*this.cfgHighlightScale;
        this.jointRenderer.material.color = this.cfgJointHighlightColor;
        this.isJointHighlited = true;
    }

    public void Unhighlight()
    {
        transform.localScale = initialScale;
        this.jointRenderer.material.color = this.initialJointColor;
        this.isJointHighlited = false;
    }

    private IList<Tweener> currentActiveTweeners = new List<Tweener>();

    public void HighlightAnimated()
    {
        if (!this.isJointHighlited)
        {
            foreach (var currentActiveTweener in this.currentActiveTweeners)
            {
                currentActiveTweener.Kill();
            }

            currentActiveTweeners.Clear();

            var highLightTween = HOTween.To(this.jointRenderer.material, this.cfgHighlightingAnimationTime, "color",
                this.cfgJointHighlightColor);
            var scaleTween = HOTween.To(this.joint.transform, this.cfgHighlightingAnimationTime, "localScale",
                this.initialScale*this.cfgHighlightScale);
            currentActiveTweeners.Add(highLightTween);
            currentActiveTweeners.Add(scaleTween);
            this.isJointHighlited = true;
        }
    }

    public void UnHighlightAnimated()
    {
        if (this.isJointHighlited)
        {
            foreach (var currentActiveTweener in this.currentActiveTweeners)
            {
                currentActiveTweener.Kill();
            }

            currentActiveTweeners.Clear();

            var highLightTween = HOTween.To(this.jointRenderer.material, this.cfgHighlightingAnimationTime, "color",
                this.initialJointColor);
            var scaleTween = HOTween.To(this.joint.transform, this.cfgHighlightingAnimationTime, "localScale",
                this.initialScale);
            currentActiveTweeners.Add(highLightTween);
            currentActiveTweeners.Add(scaleTween);
            this.isJointHighlited = false;
        }
    }

    /// <summary>
    ///     Switches the highlighting of the joint on or off without an animation.
    /// </summary>
    public void ToggleHighlighting()
    {
        if (this.isJointHighlited)
        {
            this.Unhighlight();
        }
        else
        {
            this.Highlight();
        }
    }

    /// <summary>
    ///     Switches the highlighting of the joint on or off. The highlighting change will be animated over the specified
    ///     timeframe.
    /// </summary>
    /// <param name="animationTime"> The time the highlighting animation will take in seconds. </param>
    public void ToggleHighlightingAnimated()
    {
        if (this.isJointHighlited)
        {
            this.UnHighlightAnimated();
        }
        else
        {
            this.HighlightAnimated();
        }
    }

    #endregion

    #region Methods

    private void Start()
    {
        this.joint = this.GetComponent<BlockJoint>();

        if (!this.joint)
        {
            throw new MissingComponentException(
                "There is no BlockJoint script attached to this GameObject: " + this.gameObject.name);
        }

        this.initialScale = this.joint.transform.localScale;
        this.jointRenderer = this.joint.GetComponent<Renderer>();

        if (!this.jointRenderer)
        {
            throw new MissingComponentException(
                "There is no renderer attached to the joint: " + this.joint.gameObject.name);
        }

        this.isJointHighlited = false;
        this.initialJointColor = this.GetComponent<Renderer>().material.color;
    }

    #endregion
}