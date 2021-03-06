﻿using System;
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

    /// <summary>
    /// Creates a new line at the very bottom of the board and add manually the new row to the list
    /// </summary>
    public void CreateNewLine()
    {
        float lastRowPositionY = this.rows[0][0].transform.localPosition.y;
        print("lastRowPositionY:" + lastRowPositionY);
        print("Block.BlockSize:" + Block.BlockSize);
        print("BlockPlayable.Delta:" + BlockPlayable.Delta);
        float newRowPositionY = lastRowPositionY - Block.BlockSize;
        print("newRowPositionY:" + newRowPositionY);

        List<GameObject> row = new List<GameObject>();
        for (var iC = 0; iC < numColumns; iC++)
        {
            row.Add(this.CreateBlock(
                Mathf.RoundToInt((iC + 1) * Block.BlockSize),
                newRowPositionY,
                1, 1,
                true,
                null));
        }

        this.rows.Insert(0, row);
    }

    public void RemoveTopLine()
    {
        print("RemoveTopLine");
        List<GameObject> row = this.rows.Last();
        for (var iC = 0; iC < row.Count; iC++)
        {
            Destroy(row[iC]);
            this.blocks.Remove(row[iC]);
        }

        this.rows.Remove(row);
    }

    public void RemoveBlock(GameObject block)
    {
        this.blocks.Remove(block);
        Destroy(block);

        this.InvalidateRows();
    }

    /// <summary>
    /// Unions blocks from the block list representing blocks whoich can be united.
    /// </summary>
    /// <param name="blocks"></param>
    public void UnionBlocks(List<GameObject> blocks)
    {
        print("UnionBlocks");
        BlockColor color = blocks[0].GetComponent<Block>().color;

        //get Bounds
        List<Vector2> bounds = BoundsUtil.GetBoundsFromGeometry(blocks);

        //Destroy blocks
        for (var iB = 0; iB < blocks.Count; iB++)
        {
            this.blocks.Remove(blocks[iB]);
            Destroy(blocks[iB]);
        }

        //Create unioned Block
        int widthUnits = Mathf.RoundToInt((bounds[1].x - bounds[0].x) / this.blockSize);
        int heightUnits = Mathf.RoundToInt((bounds[1].y - bounds[0].y) / this.blockSize);
        GameObject block = this.CreateBlock(bounds[1].x, bounds[0].y, widthUnits, heightUnits, false, color);

        //Invalidate
        this.InvalidateRows();
    }


    private void RegisterBlock(GameObject block)
    {
        this.blocks.Add(block);
        this.InvalidateRows();
    }

    public void InvalidateRows()
    {
        print("=====InvalidateRows=====");
        print("this.blocks.Count:" + this.blocks.Count);

        int count = 0;
        this.rows = this.blocks
            // repeat block in different rows if spread in multiple yIndexes
            .SelectMany(b => b.GetComponent<Block>().GetYIndexes().Select((yIndex, i) => new KeyValuePair<int, GameObject>(yIndex, b)))
            .GroupBy(item => item.Key)
            .OrderBy(grp => grp.Key)
            .Select(grp => grp.Select(kv => kv.Value))
            .Select(grp =>
            {
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
                    if (count == 12)
                    {
                        print("count:" + result.Count + " " + grpList.Count);
                        if (index < grpList.Count && grpList[index] != null)
                        {
                            print(grpList[index].GetComponent<Block>().color + " " + String.Join(",", grpList[index].GetComponent<Block>().GetColumnIndexes().ToArray()));
                        }
                        
                    }
                    if (index < grpList.Count &&
                        (grpList[index].GetComponent<Block>().GetColumnIndexes().IndexOf(result.Count) >= 0 || grpList[index].GetComponent<Block>().GetColumnIndexes().IndexOf(result.Count - 1) >= 0))
                    {
                        if (count == 12)
                        {
                            print("=>add " + grpList[index].GetComponent<Block>().color + " " + String.Join(",", grpList[index].GetComponent<Block>().GetColumnIndexes().ToArray()));
                        }
                        result.Add(grpList[index]);
                        index++;
                    }
                    else
                    {
                        result.Add(null);

                        if (count == 12)
                        {
                            print("=>add null");
                        }
                    }

                    //if (index >= grpList.Count)
                    //{
                    //    //print("GetBlocksFromSameRowThan Add Null1");
                    //    result.Add(null);
                    //    index++;
                    //}
                    //else if (grpList[index].GetComponent<Block>().GetColumnIndexes().IndexOf(result.Count) >= 0) // <= to keep draggingBlock when dragging
                    //{
                    //    //print("GetBlocksFromSameRowThan Element " + grpList[index].GetComponent<Block>().color);
                    //    result.Add(grpList[index]);
                    //    index++;
                    //}
                    //else
                    //{
                    //    //print("GetBlocksFromSameRowThan Add Null2");
                    //    result.Add(null);
                    //}
                }
                count++;
                return result;
            })
            .Select(grp => grp.ToList())
            .ToList();

        print("*********");
        //BlocksUtil.LogRows(this.blocks
        //    // repeat block in different rows if spread in multiple yIndexes
        //    .SelectMany(b => b.GetComponent<Block>().GetYIndexes().Select((yIndex, i) => new KeyValuePair<int, GameObject>(yIndex, b)))
        //    .GroupBy(item => item.Key)
        //    .OrderBy(grp => grp.Key)
        //    .Select(grp => grp.Select(kv => kv.Value))
        //    .Select(grp =>
        //    {
        //        return grp
        //            // repeat block in different columns if spread in multiple columnIndexes
        //            .SelectMany(b => b.GetComponent<Block>().GetColumnIndexes().Select((columnIndex, i) => new KeyValuePair<int, GameObject>(columnIndex, b)))
        //            .OrderBy(item => item.Key)
        //            .Select(kv => kv.Value);
        //    })
        //    .Select(grp => grp.ToList())
        //    .ToList());
        BlocksUtil.LogRows(this.rows);
        print("*********");
    }

    public List<List<GameObject>> GetRows()
    {
        return this.rows;
    }
    public List<GameObject> GetBlocks()
    {
        return this.blocks;
    }

    public int GetFirstRowVisibleIndex()
    {
        return this.rows.FindIndex(row => row.Count > 0 && row[0].GetComponent<BlockPlayable>().isVisible);
    }

    public IEnumerable<List<GameObject>> GetRowsFromBlock(GameObject block)
    {
        return this.rows.Where(row => row.IndexOf(block) >= 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
