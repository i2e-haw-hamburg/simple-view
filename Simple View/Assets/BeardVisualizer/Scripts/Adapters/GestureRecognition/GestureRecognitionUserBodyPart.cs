// ----------------------------------------------------------------
//  <author> Malte Eckhoff </author>
//  <date> 13.04.2015 </date>
// 
//  <copyright file="UnityVS.BeardVisualizer/UnityVS.BeardVisualizer.CSharp/GestureRecognitionUserBodyPart.cs" owner="Malte Eckhoff" year=2015> 
//   All rights are reserved. Reproduction or transmission in whole or in part, in
//   any form or by any means, electronic, mechanical or otherwise, is prohibited
//   without the prior written consent of the copyright owner.
//  </copyright>
// ----------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GestureRecognitionUserBodyPart : MonoBehaviour
{
    [SerializeField] private float cfgMaxForceDistance;

    /// <summary>
    ///     The amount of force, the body part will apply if colliding with something to reach its destination.
    /// </summary>
    [SerializeField]
    private float cfgPositionForceMultiplier = 100;

    /// <summary>
    ///     Keeps track of all gameobjects this body part currently collides with.
    /// </summary>
    private List<GameObject> CollidingGameObjects = new List<GameObject>();

    private bool isColliding = false;

    public GestureRecognitionUserBodyPart(Quaternion rotation, int bodyPartId, Vector3 position)
    {
        BodyPartId = bodyPartId;
        Rotation = rotation;
        Position = position;
    }

    public Vector3 Acceleration { get; set; }

    public Vector3 AngularAcceleration { get; set; }

    public Vector3 AngularVelocity
    {
        get { return GetComponent<Rigidbody>().angularVelocity; }
        set { GetComponent<Rigidbody>().angularVelocity = value; }
    }

    /// <summary>
    ///     The ID of this body part.
    /// </summary>
    public int BodyPartId { get; set; }

    public Collider Collider
    {
        get { return GetComponentInChildren<Collider>(); }
    }

    /// <summary>
    ///     The length in meters of this body part.
    /// </summary>
    public float Length
    {
        get { return transform.localScale.y; }
        set { transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z); }
    }

    /// <summary>
    ///     The current position of the body part.
    /// </summary>
    public Vector3 Position
    {
        get { return GetComponent<Rigidbody>().position; }
        set
        {
            if (!isColliding)
            {
                GetComponent<Rigidbody>().MovePosition(value);
            }
            else
            {
                var distance = Vector3.Distance(Position, value);

                if (distance < cfgMaxForceDistance)
                {
                    GetComponent<Rigidbody>()
                        .AddForce((value - Position).normalized*distance*cfgPositionForceMultiplier,
                            ForceMode.Acceleration);
                }
                else
                {
                    GetComponent<Rigidbody>().MovePosition(value);
                }
            }
        }
    }

    /// <summary>
    ///     The rotation of rotation of the body part.
    /// </summary>
    public Quaternion Rotation
    {
        get { return transform.rotation; }
        set { transform.rotation = value; }
    }

    public Vector3 Velocity
    {
        get { return GetComponent<Rigidbody>().velocity; }
        set { GetComponent<Rigidbody>().velocity = value; }
    }

    public override string ToString()
    {
        return
            string.Format(
                "{0}, Length: {1}, BodyPartId: {2}, Position: {3}, Rotation: {4}, Velocity: {5}, Acceleration: {6}, AngularVelocity: {7}, AngularAcceleration: {8}",
                base.ToString(),
                Length,
                BodyPartId,
                Position,
                Rotation,
                Velocity,
                Acceleration,
                AngularVelocity,
                AngularAcceleration);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (CollidingGameObjects.All(x => x.gameObject != collision.gameObject))
        {
            this.gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
            isColliding = true;
            CollidingGameObjects.Add(collision.gameObject);
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (CollidingGameObjects.Any(x => x.gameObject == collision.gameObject))
        {
            CollidingGameObjects.Remove(collision.gameObject);

            if (!CollidingGameObjects.Any())
            {
                this.gameObject.GetComponentInChildren<Renderer>().material.color = Color.blue;
                isColliding = false;
            }
        }
    }

    private void Awake()
    {
        this.CollidingGameObjects = new List<GameObject>();
    } 

    private void FixedUpdate()
    {
        if (Acceleration.magnitude > 0)
        {
            GetComponent<Rigidbody>().AddForce(Acceleration*Time.deltaTime, ForceMode.Acceleration);
        }

        if (AngularAcceleration.magnitude > 0)
        {
            GetComponent<Rigidbody>().AddTorque(AngularAcceleration*Time.deltaTime, ForceMode.Acceleration);
        }
    }
}