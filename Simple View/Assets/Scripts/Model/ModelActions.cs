using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ModelActions : MonoBehaviour
{
    private float rotationSpeed = 60.0f;

    private float explosionTime = 1.0f;

    private float explosionOffsetFactor = 3.0f;

    private bool exploded = false;

    private Dictionary<Transform, Vector3> initialChildPositions = new Dictionary<Transform, Vector3>();

    private KeyCode RotateLeftKey = KeyCode.LeftArrow;
    private KeyCode RotateRightKey = KeyCode.RightArrow;
    
    private KeyCode ExplodeToggleKey = KeyCode.E;
    private KeyCode ImplodeKey = KeyCode.I;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            initialChildPositions.Add(child, child.transform.localPosition);
        }


        //while (true)
        //{
        //    yield return new WaitForSeconds(1.5f);
        //    this.Explode();
        //    yield return new WaitForSeconds(1.5f);
        //    this.Implode();
        //}
    }

    private void Update()
    {
        if (Input.GetKey(RotateLeftKey))
        {
            this.RotateTo(this.transform.rotation.eulerAngles + new Vector3(0, -rotationSpeed, 0)*Time.deltaTime);
        }
        else if (Input.GetKey(RotateRightKey))
        {
            this.RotateTo(this.transform.rotation.eulerAngles + new Vector3(0, rotationSpeed, 0)*Time.deltaTime);
        }

        if (Input.GetKeyDown(ImplodeKey))
        {
            this.Implode();
        }
        else if (Input.GetKeyDown(ExplodeToggleKey))
        {
            if (!exploded)
            {
                this.Explode();
            }
            else
            {
                this.Implode();
            }
        }
    }

    public void RotateTo(Vector3 newRotation)
    {
        this.RotateTo(Quaternion.Euler(newRotation));
    }

    public void RotateTo(Quaternion newRotation)
    {
        this.transform.rotation = newRotation;
    }

    public void Implode()
    {
        if (!exploded)
        {
            return;
        }

        foreach (var childPos in initialChildPositions)
        {
            //childPos.Key.DOMove(transform.TransformPoint(childPos.Value), explosionTime);
            childPos.Key.DOLocalMove(childPos.Value, explosionTime);
        }

        exploded = false;
    }

    public void Explode()
    {
        if (exploded)
        {
            return;
        }

        foreach (var childPos in initialChildPositions)
        {
            //childPos.Key.DOMove(childPos.Value + transform.TransformPoint(childPos.Value * explosionOffsetFactor), explosionTime);
            childPos.Key.DOLocalMove(childPos.Value + childPos.Value * explosionOffsetFactor, explosionTime);
        }

        exploded = true;
    }
}