using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public static float BlockSize;

    [SerializeField]
    public BlockColor color;

    public Vector2 sizeUnits;


    protected string spritePath = "";


    // Start is called before the first frame update
    protected void Start()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(spritePath);
        this.GetComponent<Image>().overrideSprite = sprites[(int) this.color];

    }

    // Update is called once per frame
    public void RefreshPositionX()
    {
        //last columIndex to get the index at bottomLeft of the block
        int blockColumnIndex = this.GetColumnIndexes().Last();
        float gap = Mathf.Abs(this.GetComponent<RectTransform>().localPosition.x - (blockColumnIndex + 1) * BlockSize);
        print("gap:" + gap);
        if (gap > 5)
        {
            SetColumIndex(blockColumnIndex);
        }
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
    public List<int> GetColumnIndexes()
    {
        int baseIndex = Mathf.RoundToInt(this.GetComponent<RectTransform>().localPosition.x / BlockSize) - 1;
        List<int> result = new List<int>();
        for (var i = 0; i < this.sizeUnits.x; i++)
        {
            result.Insert(0, baseIndex - i);
        }
        return result;
    }

    public void SetColumIndex(int index)
    {
        print("SetColumIndex: " + index + " " + this.color);

        Hashtable ht = new Hashtable();
        ht.Add("x", (index + 1) * BlockSize);
        ht.Add("time", 0.2f);
        ht.Add("delay", 0.0f);
        ht.Add("isLocal", true);
        ht.Add("easetype", iTween.EaseType.easeInQuint);
        iTween.MoveTo(this.gameObject, ht);
    }
    ////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////
    public List<int> GetYIndexes()
    {
        int baseIndex = Mathf.RoundToInt(this.GetComponent<RectTransform>().localPosition.y / BlockSize) - 1;
        List<int> result = new List<int>();
        for (var i = 0; i < this.sizeUnits.y; i++)
        {
            result.Add(baseIndex + i);
        }
        return result;
    }


    public bool Is1x1()
    {
        return this.GetColumnIndexes().Count == 1 && this.GetYIndexes().Count == 1;
    }
    //public void SetRowIndex(int index)
    //{
    //    print("SetRowIndex");
    //    print(index);
    //    float width = this.GetComponent<RectTransform>().rect.width * this.GetComponent<RectTransform>().localScale.x;

    //    Hashtable ht = new Hashtable();
    //    ht.Add("y", (index + 1) * width);
    //    ht.Add("time", 0.2f);
    //    ht.Add("delay", 0.0f);
    //    ht.Add("isLocal", true);
    //    ht.Add("easetype", iTween.EaseType.easeInQuint);
    //    iTween.MoveTo(this.gameObject, ht);
    //}
}

public enum BlockColor
{
    Blue = 0,
    Green = 1,
    Orange = 2,
    Red = 3
}