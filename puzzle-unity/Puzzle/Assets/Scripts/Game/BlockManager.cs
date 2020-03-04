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
        GameObject block = BlockFactory.CreateBlock2x2(this.container, positionX, positionY, widthUnits * this.blockSize, heightUnits * this.blockSize, draggable, color);
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
        this.InvalidateRows();
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
            .OrderBy(item => item.transform.position.x)
            .GroupBy(item => item.GetComponent<Block>().GetRowIndex())
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
                    else if (grpList[index].GetComponent<Block>().GetColumnIndex() == result.Count) // <= to keep draggingBlock when dragging
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
            //.Select(grp => grp.ToList())
            .ToList();
    }

    public List<List<GameObject>> GetRows()
    {
        return this.rows;
    }
    public List<GameObject> GetBlocks()
    {
        return this.blocks;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
