using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BlockPlayable : MonoBehaviour
{
    public static float Delta;
    public delegate void PhysicsComplete(GameObject g);
    public event PhysicsComplete OnPhysicsComplete;

    private Rigidbody2D rb2d;

    private float originalLocalY;

    private bool _isActive = false;
    public bool isActive
    {
        get
        {
            return this._isActive;
        }
    }
    

    void OnEnable()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Static;

        
    }

    private void Start()
    {
        this.originalLocalY = this.GetComponent<RectTransform>().localPosition.y;
        print("originalLocalY :" + originalLocalY);
    }

    private void Update()
    {
        if (this._isActive == false)
        {
            // global position
            float positionY = this.transform.position.y;

            if (positionY > 0)
            {
                this._isActive = true;
                rb2d.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                this.GetComponent<RectTransform>().localPosition = new Vector3(
                    this.GetComponent<RectTransform>().localPosition.x,
                    originalLocalY + Delta,
                    this.GetComponent<RectTransform>().localPosition.z
                );
            }
        }
    }
    
}