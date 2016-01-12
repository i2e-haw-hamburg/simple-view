using System;
using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Random = UnityEngine.Random;

public class RandomRotate : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        this.StartCoroutine(this.Rotate());
    }

    private IEnumerator Rotate()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 3.0f));

        var easeTypes = Enum.GetValues(typeof (EaseType));
        var easeType = Random.Range(0, easeTypes.Length);
        var targetRotation = Random.rotation;
        HOTween.To(this.transform, Random.Range(0.1f, 10.0f),
            new TweenParms().NewProp("rotation", targetRotation)
                .Ease((EaseType) easeTypes.GetValue(easeType))
                .OnComplete(() => this.StartCoroutine(this.Rotate())));
    }
}