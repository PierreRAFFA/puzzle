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
            print("SET enablePhysics" + this.GetInstanceID());
            print("destroyed: " + this.gameObject.IsDestroyed());
            this._enablePhysics = value;
            if (value)
            {
                this.positionOnCheckMotion = Vector2.zero;
                InvokeRepeating("CheckMotion", 0, 0.5f);
            }
            else
            {
                this.positionOnCheckMotion = Vector2.zero;
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
        print("CheckMotion " + this.GetInstanceID());
        print("CheckMotion " + this.GetInstanceID());

        if (this.positionOnCheckMotion == Vector2.zero)
        {
            print("CheckMotion NEW " + this.GetInstanceID());
            print("CheckMotion NEW " + this.GetInstanceID());
            this.positionOnCheckMotion = this.GetComponent<RectTransform>().localPosition;
        }
        else
        {
            print(this.positionOnCheckMotion);
            print(this.GetComponent<RectTransform>().localPosition);
            //if (this.positionOnCheckMotion.Equals(this.GetComponent<RectTransform>().localPosition))
            if (Vector3Util.AreVectorEqual(this.positionOnCheckMotion, this.GetComponent<RectTransform>().localPosition, 1))
            {
                print("CheckMotion Cancel " + this.GetInstanceID());
                CancelInvoke();
                this.enablePhysics = false;
            }
            else
            {
                this.positionOnCheckMotion = this.GetComponent<RectTransform>().localPosition;
                print("CheckMotion Continue " + this.GetInstanceID());
            }
        }
    }

    private void OnDestroy()
    {
        print("OnDestroy " + this.GetInstanceID());
        print("this._enablePhysics: " + this._enablePhysics + " " + this.GetInstanceID());
        if (this._enablePhysics)
        {
            print("a " + this.GetInstanceID());
            if (this.OnPhysicsComplete != null)
            {
                print("b " + this.GetInstanceID());
                this.OnPhysicsComplete(this.gameObject);
            }
        }
    }

    //void Start()
    //{
    //    contactFilter.useTriggers = false;
    //}

}