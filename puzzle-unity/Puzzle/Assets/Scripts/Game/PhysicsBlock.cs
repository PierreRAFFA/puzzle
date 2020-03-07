using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PhysicsBlock : MonoBehaviour
{
    public delegate void PhysicsComplete(GameObject g);
    public event PhysicsComplete OnPhysicsComplete;

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


    //void Start()
    //{
    //    contactFilter.useTriggers = false;
    //}

}