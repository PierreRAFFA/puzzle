using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PhysicsBlock : MonoBehaviour
{
    public delegate void PhysicsComplete(GameObject g);
    public event PhysicsComplete OnPhysicsComplete;

    public float minGroundNormalY = .65f;
    public float gravityModifier = 60f;
    public float bounce = 1.3f;

    public bool _enablePhysics = false;
    public bool enablePhysics
    {
        set
        {
            this._enablePhysics = value;
            if (value)
            {
                this.positionOnCheckMotion = Vector2.zero;
                InvokeRepeating("CheckMotion", 0, 0.5f);
            }
            else
            {
                if (this.OnPhysicsComplete != null)
                {
                    this.OnPhysicsComplete(this.gameObject);
                }
            }
        }
    }
    private Vector2 positionOnCheckMotion = Vector2.zero;

    private bool canBeDisabled = false;

    protected bool grounded;
    protected Vector2 groundNormal;

    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;

    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float shellRadius = 0.1f;
    protected const float minMoveDistance = 0.01f;


    protected float previousDistance;

    void OnEnable()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false;
    }

    /// <summary>
    /// Make Physics disabled waiting for equilibrium first
    /// Nothing will be applied if _enablePhysics was not enabled
    /// </summary>
    public void CanBeDisabled()
    {
        if (this._enablePhysics)
        {
            this.canBeDisabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (false && this._enablePhysics)
        {
            this.velocity += Physics2D.gravity * Time.deltaTime;

            Vector2 deltaPosition = velocity * Time.deltaTime;

            Vector2 move = Vector2.up * deltaPosition;
            move.y *= this.gravityModifier;
            //print("move: " + this.gravityModifier + " " + move);
            Movement(move, true);
        }
        
    }

    private void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;
        //print("distance1: " + distance);
        //print("minMoveDistance: " + minMoveDistance);
        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                if (hitBuffer[i].collider.GetComponent<RectTransform>().position.y < this.GetComponent<RectTransform>().position.y)
                {
                    hitBufferList.Add(hitBuffer[i]);
                }
                
            }

            //print(hitBufferList.Count + " COLLISION !!!");

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                //print(currentNormal.y);
                //print(minGroundNormalY);
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(this.velocity, currentNormal) * this.bounce;
                //print(this.velocity);
                //print(currentNormal);
                //print(projection);
                if (projection < 0)
                {
                    this.velocity = this.velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
                distance = Mathf.Max(0, distance);
            }

            //print("distance2: " + distance);
            //print("move.normalized: " + move.normalized);
            //print("previousDistance: " + previousDistance);

            //if (distance > previousDistance)
            //{
            //    this.canBeDisabled = true;
            //}

            //if (distance < 0.01)
            //{
            //    this._enablePhysics = false;
            //    this.canBeDisabled = false;
            //    rb2d.position = new Vector2(rb2d.position.x, Mathf.RoundToInt(rb2d.position.y));
            //    //print("_enablePhysics false");

            //    if (this.OnPhysicsComplete != null)
            //    {
            //        this.OnPhysicsComplete();
            //    }

            //}
            previousDistance = distance;
        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }

    void CheckMotion()
    {
        print("CheckMotion");
        if (this.positionOnCheckMotion == Vector2.zero)
        {
            print("CheckMotion NEW");
            this.positionOnCheckMotion = this.GetComponent<RectTransform>().localPosition;
        }
        else
        {
            print(this.positionOnCheckMotion);
            print(this.GetComponent<RectTransform>().localPosition);
            //if (this.positionOnCheckMotion.Equals(this.GetComponent<RectTransform>().localPosition))
            if (Vector3Util.AreVectorEqual(this.positionOnCheckMotion, this.GetComponent<RectTransform>().localPosition, 10))
            {
                print("CheckMotion Cancel");
                CancelInvoke();
                this.enablePhysics = false;
            }
            else
            {
                this.positionOnCheckMotion = this.GetComponent<RectTransform>().localPosition;
                print("CheckMotion Continue");
            }
        }

    }
}