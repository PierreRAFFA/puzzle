using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField]
    public BlockColor color;


    public delegate void BlockDragging();
    public event BlockDragging OnBlockDragging;


    private bool IsDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/block1x1");
        this.GetComponent<Image>().overrideSprite = sprites[(int) this.color];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDragging(bool value)
    {
        this.IsDragging = value;
    }

    public float GetRealHeight()
    {
        return this.GetComponent<RectTransform>().rect.height * this.GetComponent<RectTransform>().localScale.y;
    }

    public float GetRealWidth()
    {
        return this.GetComponent<RectTransform>().rect.width * this.GetComponent<RectTransform>().localScale.x;
    }
    ////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    public int GetColumnIndex()
    {
        float width = this.GetComponent<RectTransform>().rect.width * this.GetComponent<RectTransform>().localScale.x;
        return Mathf.RoundToInt(this.GetComponent<RectTransform>().localPosition.x / width) - 1;
    }

    public void SetColumIndex(int index)
    {
        print("SetColumIndex");
        print(index);
        float width = this.GetComponent<RectTransform>().rect.width * this.GetComponent<RectTransform>().localScale.x;

        Hashtable ht = new Hashtable();
        ht.Add("x", (index + 1) * width);
        ht.Add("time", 0.2f);
        ht.Add("delay", 0.0f);
        ht.Add("isLocal", true);
        ht.Add("easetype", iTween.EaseType.easeInQuint);
        iTween.MoveTo(this.gameObject, ht);
    }
    ////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    public int GetRowIndex()
    {
        float width = this.GetComponent<RectTransform>().rect.width * this.GetComponent<RectTransform>().localScale.x;
        return Mathf.RoundToInt(this.GetComponent<RectTransform>().localPosition.y / width) - 1;
    }

    public void SetRowIndex(int index)
    {
        print("SetRowIndex");
        print(index);
        float width = this.GetComponent<RectTransform>().rect.width * this.GetComponent<RectTransform>().localScale.x;

        Hashtable ht = new Hashtable();
        ht.Add("y", (index + 1) * width);
        ht.Add("time", 0.2f);
        ht.Add("delay", 0.0f);
        ht.Add("isLocal", true);
        ht.Add("easetype", iTween.EaseType.easeInQuint);
        iTween.MoveTo(this.gameObject, ht);
    }
}

public enum BlockColor
{
    Blue = 0,
    Green = 1,
    Orange = 2,
    Red = 3
}