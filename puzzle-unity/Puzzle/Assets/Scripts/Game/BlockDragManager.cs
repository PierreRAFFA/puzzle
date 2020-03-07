using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockDragManager: MonoBehaviour
{
    public BlockManager blockManager;
    public BlockUnionManager blockUnionManager;

    private List<GameObject> sameRowBlocks;
    private GameObject draggingBlock;
    private float[] draggingBlockBoundaries;

    void Start()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public float[] NotifyDraggingBlockStart(GameObject b)
    {
        print("SetColumIndex NotifyDraggingBlockStart");

        //store the draggingBlock
        this.draggingBlock = b;

        // As a security
        this.blockManager.InvalidateRows();

        //get All blocks from the same row for manipulation
        this.sameRowBlocks = this.GetBlocksFromSameRow(b);
        print(this.sameRowBlocks.Count);

        this.draggingBlockBoundaries = this.CalculateBlockBoundaries(b);
        return this.draggingBlockBoundaries;
    }

    /// <summary>
    /// 
    /// </summary>
    public void NotifyDraggingBlockEnd(float delta)
    {
        print("delta:" + delta);

        this.RefreshBlockColumnIndexBasedOnPositionX(this.blockManager.GetRows());

        if (delta > 0)
        {
            Hashtable ht = new Hashtable();
            ht.Add("name", "dragging");
            ht.Add("x", this.draggingBlock.transform.position.x + (4 * delta));
            ht.Add("time", Mathf.Abs(delta) / 40);
            ht.Add("isLocal", false);
            ht.Add("easetype", iTween.EaseType.easeOutQuint);

            Hashtable updateParams = new Hashtable();
            updateParams.Add("block", this.draggingBlock);
            updateParams.Add("blockBoundaries", this.draggingBlockBoundaries);
            ht.Add("onupdate", "OnInertiaUpdate");
            ht.Add("onupdatetarget", this.gameObject);
            ht.Add("onupdateparams", updateParams);

            ht.Add("oncomplete", "OnInertiaDone");
            ht.Add("oncompletetarget", this.gameObject);
            ht.Add("oncompleteparams", this.draggingBlock);

            iTween.MoveTo(this.draggingBlock.gameObject, ht);
        }
        else
        {
            OnInertiaDone(this.draggingBlock);
        }
    }

    void OnInertiaUpdate(Hashtable updateParams)
    {
        GameObject block = (GameObject)updateParams["block"];
        float[] blockBoundaries = (float[])updateParams["blockBoundaries"];
        if (block.transform.localPosition.x < blockBoundaries[0] || block.transform.localPosition.x > blockBoundaries[1])
        {
            float newX = Mathf.Min(blockBoundaries[1], block.transform.localPosition.x);
            newX = Mathf.Max(blockBoundaries[0], newX);

            block.transform.localPosition = new Vector2(
                newX,
                block.transform.localPosition.y
            );
            iTween.StopByName("dragging");
            OnInertiaDone(block);
        }
    }

    void OnInertiaDone(GameObject block)
    {
        //Force the dragging block to be column-perfect
        int index = this.sameRowBlocks.IndexOf(block); // TODO can be critical as sameRowBlocks might have changed here
        print("SetColumIndex OnInertiaDone: " + index);
        block.GetComponent<Block>().SetColumIndex(index);

        block.GetComponent<PhysicsBlock>().enablePhysics = true;
        block.GetComponent<PhysicsBlock>().OnPhysicsComplete += onDraggingBlockPhysicsComplete;
    }

    private void onDraggingBlockPhysicsComplete(GameObject block)
    {
        block.GetComponent<PhysicsBlock>().OnPhysicsComplete -= onDraggingBlockPhysicsComplete;

        //Force to InvalidateRows because a block could have been moved
        this.blockManager.InvalidateRows();

        //FindBlocksForUnion will InvalidateRows if some groups have been found
        List<List<GameObject>> groups = this.blockUnionManager.FindBlocksForUnion();

        //unregister the draggingBlock only if it was not reaffected by the player
        if (this.draggingBlock == block)
        {
            print("UNREGISTER draggingBlock true");
            this.draggingBlock = null;
        }
        else
        {
            print("UNREGISTER draggingBlock false");
        }
    }

    /// <summary>
    /// Refreshes columnIndex for each block based on positionX
    /// </summary>
    /// <param name="row"></param>
    private void RefreshBlockColumnIndexBasedOnPositionX(List<List<GameObject>> rows)
    {
        for (int iR = 0; iR < rows.Count; iR++)
        {
            for (int iB = 0; iB < rows[iR].Count; iB++)
            {
                GameObject block = rows[iR][iB];
                if (block != null && block != this.draggingBlock)
                {
                    if (block.GetComponent<Block>().Is1x1())
                    {
                        block.GetComponent<Block>().SetColumIndex(iB);
                    }
                    else
                    {
                        block.GetComponent<Block>().SetColumIndex(block.GetComponent<Block>().GetColumnIndexes().Last());
                    }
                   
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float[] CalculateBlockBoundaries(GameObject b)
    {
        int left = 0;
        int right = 5;

        List<GameObject> underRowBlocks = this.GetBlocksFromRowBelow(b);
        List<GameObject> sameRowBlocks = this.GetBlocksFromSameRow(b);

        if (underRowBlocks.Count > 0)
        {
            // take the first element as only 1x1 block are draggable
            // then GetColumnIndexes returns only one index
            int columnIndex = b.GetComponent<Block>().GetColumnIndexes()[0];

            //left
            for (int iB = columnIndex; iB >= 0; iB--)
            {
                if (sameRowBlocks[iB]?.GetComponent<Block>().Is1x1() == false)
                {
                    left = iB + 1;
                    break;
                }

                if (underRowBlocks[iB] == null)
                {
                    left = iB;
                    break;
                }
            }

            //right
            for (int iB = columnIndex; iB < underRowBlocks.Count; iB++)
            {
                if (sameRowBlocks[iB]?.GetComponent<Block>().Is1x1() == false)
                {
                    right = iB - 1;
                    break;
                }

                if (underRowBlocks[iB] == null)
                {
                    right = iB;
                    break;
                }
            }
        }

        print("leftRightMax:" + left + " " + right);
        return new float[] {
            (left + 1) * Block.BlockSize,
            (right + 1) * Block.BlockSize,
        };
    }

    /// <summary>
    /// Returns all blocks from same row
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    private List<GameObject> GetBlocksFromSameRow(GameObject b)
    {
        int index = this.blockManager.GetRows().FindIndex(row => row.IndexOf(b) >= 0);

        return this.blockManager.GetRows()[index];
    }

    /// <summary>
    /// Returns all blocks from same row below
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    private List<GameObject> GetBlocksFromRowBelow(GameObject b)
    {
        int index = this.blockManager.GetRows().FindIndex(row => row.IndexOf(b) >= 0);
        print("BOUNDARIES: rowIndex" + index);
        return this.blockManager.GetRows()[index - 1]; //Todo Careful here
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    /// <param name="index"></param>
    private void MoveBlockAtIndex(GameObject b, int index)
    {
        print("SetColumIndex MoveBlockAtIndex: " + b.GetComponent<Block>().color + " " + index);

        int currentBlockIndex = this.sameRowBlocks.IndexOf(b);
        //print(currentBlockIndex);
        this.sameRowBlocks.RemoveAt(currentBlockIndex);
        this.sameRowBlocks.Insert(index, draggingBlock);

        print("SetColumIndex MoveBlockAtIndex after: " + BlocksUtil.LogList(this.sameRowBlocks));

        int startIndex = Mathf.Min(index, currentBlockIndex);
        int endIndex = Mathf.Max(index, currentBlockIndex);
        for (int iB = startIndex; iB <= endIndex; iB++)
        {
            Block block = this.sameRowBlocks[iB]?.GetComponent<Block>();

            //avoid dragging block
            if (this.sameRowBlocks[iB] != b)
            {
                if (block != null)
                {
                    block.SetColumIndex(iB);
                }
                else
                {
                    GameObject blockAtTheBottom = this.EnablePhysicsForBlocksAtColumnIndex(iB, b.GetComponent<RectTransform>().position.y);

                    if (blockAtTheBottom)
                    {
                        print("blockAtTheBottom: " + blockAtTheBottom.GetComponent<Block>().GetColumnIndexes()[0] + " " + blockAtTheBottom.GetComponent<Block>().color);

                        //Insert the falling block to the row (iB should be equal to blockAtTheBottom.GetComponent<Block>().GetColumnIndex()
                        this.sameRowBlocks[blockAtTheBottom.GetComponent<Block>().GetColumnIndexes()[0]] = blockAtTheBottom;

                        string s = "";
                        for (int iB2 = 0; iB2 < this.sameRowBlocks.Count; iB2++)
                        {
                            s += (this.sameRowBlocks[iB2] != null ? this.sameRowBlocks[iB2].GetComponent<Block>().color.ToString() : "___") + " ";
                        }
                        print("Big Total Row is: " + s);
                    }
                    
                }
            }
        }


    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnIndex"></param>
    /// <param name="positionY"></param>
    /// <returns>Returns the block at the bottom</returns>
    private GameObject EnablePhysicsForBlocksAtColumnIndex(int columnIndex, float positionY)
    {
        List<GameObject> blocks = this.GetBlocksOnTop(columnIndex, positionY);
        print("EnablePhysicsForBlocksAtColumnIndex:" + BlocksUtil.LogList(blocks));

        //get toppest block from the column and wait for the block to finish to fall down, then InvalidateBlock
        //as some blocks might finish the motion after the draggingBlock finish its own one.
        if (blocks.Count > 0)
        {
            blocks.Last().GetComponent<PhysicsBlock>().OnPhysicsComplete += OnTopBlockPhysicsComplete;
        }

        for (int iB = 0; iB < blocks.Count; iB++)
        {
            blocks[iB].GetComponent<PhysicsBlock>().enablePhysics = true;
        }

        // can be null when moving block on the top row
        return blocks.Count > 0 ? blocks[0] : null;
    }

    void OnTopBlockPhysicsComplete(GameObject block)
    {
        block.GetComponent<PhysicsBlock>().OnPhysicsComplete -= OnTopBlockPhysicsComplete;

        //Force to InvalidateRows because a block could have been moved
        this.blockManager.InvalidateRows();

        //FindBlocksForUnion will InvalidateRows if some groups have been found
        this.blockUnionManager.FindBlocksForUnion();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnIndex"></param>
    /// <param name="positionY"></param>
    /// <returns></returns>
    private List<GameObject> GetBlocksOnTop(int columnIndex, float positionY)
    {
        return this.blockManager.GetBlocks()
            .Where(item => item.GetComponent<Block>().GetColumnIndexes()[0] == columnIndex && item.GetComponent<RectTransform>().position.y > positionY)
            .OrderBy(item => item.GetComponent<RectTransform>().position.y)
            .ToList<GameObject>();
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (this.draggingBlock != null)
        {
            int draggingBlockColumnIndex = this.draggingBlock.GetComponent<Block>().GetColumnIndexes()[0];

            //print("SetColumIndex sameRowBlocks:" + BlocksUtil.LogList(this.sameRowBlocks));
            if (this.sameRowBlocks[draggingBlockColumnIndex] != this.draggingBlock)
            {
                //print("something to do");
                this.MoveBlockAtIndex(this.draggingBlock, draggingBlockColumnIndex);
            }
        }
    }


}

