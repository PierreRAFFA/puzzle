using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public Transform container;

    private List<GameObject> blocks = new List<GameObject>();
    private List<List<GameObject>> rows = new List<List<GameObject>>();

    private float blockSize;
    private const int numColumns = 6;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        this.blockSize = this.container.GetComponent<RectTransform>().rect.width / numColumns;
        print("this.blockSize " + this.blockSize);
    }

    

    public GameObject CreateBlock(float positionX, float positionY, float widthUnits, float heightUnits, bool draggable, BlockColor? color)
    {
        GameObject block = BlockFactory.CreateBlock2x2(this.container, positionX, positionY, widthUnits * this.blockSize, heightUnits * this.blockSize, new Vector2(widthUnits, heightUnits), draggable, color);
        this.RegisterBlock(block);
        return block;
    }

    public void UnionBlocks(List<GameObject> blocks)
    {
        print("UnionBlocks");
        BlockColor color = blocks[0].GetComponent<Block>().color;

        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float maxY = -Mathf.Infinity;
        for (var iB = 0; iB < blocks.Count; iB++)
        {
            print("block found");
            minX = Mathf.Min(minX, blocks[iB].GetComponent<RectTransform>().localPosition.x);
            minY = Mathf.Min(minY, blocks[iB].GetComponent<RectTransform>().localPosition.y);

            maxX = Mathf.Max(maxX, blocks[iB].GetComponent<RectTransform>().localPosition.x);
            maxY = Mathf.Max(maxY, blocks[iB].GetComponent<RectTransform>().localPosition.y);


            this.blocks.Remove(blocks[iB]);
            Destroy(blocks[iB]);
        }
        print(minX + " " + minY + " " + maxX + " " + maxY);

        int widthUnits = Mathf.RoundToInt((maxX - minX + this.blockSize) / this.blockSize);
        int heightUnits = Mathf.RoundToInt((maxY - minY + this.blockSize) / this.blockSize);
        GameObject block = this.CreateBlock(maxX, minY, widthUnits, heightUnits, false, color);

        this.blocks.Add(block);


    }

    public void CreateLine()
    {

    }


    private void RegisterBlock(GameObject block)
    {
        this.blocks.Add(block);
        this.InvalidateRows();
    }

    private void InvalidateRows()
    {
        this.rows = this.blocks
            // repeat block in different rows if spread in multiple yIndexes
            .SelectMany(b => b.GetComponent<Block>().GetYIndexes().Select((yIndex, i) => new KeyValuePair<int, GameObject>(yIndex, b)))
            .GroupBy(item => item.Key)
            .OrderBy(grp => grp.Key)
            .Select(grp => grp.Select(kv => kv.Value))
            .Select(grp => {
                return grp
                    // repeat block in different columns if spread in multiple columnIndexes
                    .SelectMany(b => b.GetComponent<Block>().GetColumnIndexes().Select((columnIndex, i) => new KeyValuePair<int, GameObject>(columnIndex, b)))
                    .OrderBy(item => item.Key)
                    .Select(kv => kv.Value);
            })
            .Select(grp =>
            {
                List<GameObject> grpList = grp.ToList();
                List<GameObject> result = new List<GameObject>();
                int index = 0;
                while (result.Count < numColumns)
                {
                    if (index >= grpList.Count)
                    {
                        //print("GetBlocksFromSameRowThan Add Null1");
                        result.Add(null);
                        index++;
                    }
                    else if (grpList[index].GetComponent<Block>().GetColumnIndexes().IndexOf(result.Count) >= 0) // <= to keep draggingBlock when dragging
                    {
                        //print("GetBlocksFromSameRowThan Element " + grpList[index].GetComponent<Block>().color);
                        result.Add(grpList[index]);
                        index++;
                    }
                    else
                    {
                        //print("GetBlocksFromSameRowThan Add Null2");
                        result.Add(null);
                    }
                }
                return result;
            })
            .Select(grp => grp.ToList())
            .ToList();


            //.SelectMany(b => b.GetComponent<Block>().GetYIndexes().Select((yIndex, i) => new KeyValuePair<int, GameObject>(yIndex, b)))
            //.GroupBy(item => item.Key)
            //.OrderBy(item => item.transform.position.x)
            //.GroupBy(item => item.GetComponent<Block>().GetYIndex())
            //.Select(grp =>
            //{
            //    List<GameObject> grpList = grp.ToList();
            //    List<GameObject> result = new List<GameObject>();
            //    int index = 0;
            //    while (result.Count < numColumns)
            //    {
            //        if (index >= grpList.Count)
            //        {
            //            //print("GetBlocksFromSameRowThan Add Null1");
            //            result.Add(null);
            //            index++;
            //        }
            //        else if (grpList[index].GetComponent<Block>().GetColumnIndex() == result.Count) // <= to keep draggingBlock when dragging
            //        {
            //            //print("GetBlocksFromSameRowThan Element " + grpList[index].GetComponent<Block>().color);
            //            result.Add(grpList[index]);
            //            index++;
            //        }
            //        else
            //        {
            //            //print("GetBlocksFromSameRowThan Add Null2");
            //            result.Add(null);
            //        }
            //    }
            //    return result;
            //})
            ////.Select(grp => grp.ToList())
            //.ToList();

        this.LogRows(this.rows);
    }

    public List<List<GameObject>> GetRows()
    {
        return this.rows;
    }
    public List<GameObject> GetBlocks()
    {
        return this.blocks;
    }

    public void LogRows(List<List<GameObject>> rows)
    {
        string s = "";
        for(var iR = rows.Count - 1; iR >= 0; iR--)
        {
            for (var iC = 0; iC < rows[iR].Count; iC++)
            {
                GameObject block = rows[iR][iC];
                if (block != null)
                {
                    s += "" + block.GetComponent<Block>().GetColumnIndexes()[0] + "" + block.GetComponent<Block>().color.ToString().ToCharArray()[0];
                }
                else
                {
                    s += "nn";
                }
                s += " ";
            }

            s += "\n";
        }
        print(s);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
