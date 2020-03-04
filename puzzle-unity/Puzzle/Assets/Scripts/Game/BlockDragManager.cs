using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockDragManager: MonoBehaviour
{
    public BlockManager blockManager;
    public BlockFusionManager blockFusionManager;

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
        print("NotifyDraggingBlockStart");
        //store the draggingBlock
        this.draggingBlock = b;

        //temporarily deactivate the boxCollider
        //this.draggingBlock.GetComponent<BoxCollider2D>().enabled = false;

        //get All blocks from the same row for manipulation
        this.sameRowBlocks = this.GetBlocksFromSameRowThan(b.GetComponent<RectTransform>().position.y, b.GetComponent<RectTransform>().rect.height / 10);
        print(this.sameRowBlocks.Count);
        
        print("======");

        this.draggingBlockBoundaries = this.CalculateBlockBoundaries(b);
        return this.draggingBlockBoundaries;
    }

    /// <summary>
    /// 
    /// </summary>
    public void NotifyDraggingBlockEnd(float delta)
    {
        ////Physics can be disabled
        //for (int iB = 0; iB < this.blocks.Count; iB++)
        //{
        //    this.blocks[iB].GetComponent<PhysicsBlock>().CanBeDisabled();
        //}

        List<List<GameObject>> groups = this.blockFusionManager.FindBlocksForUnion();

        //activate back the boxCollider
        //this.draggingBlock.GetComponent<BoxCollider2D>().enabled = true;





        Hashtable ht = new Hashtable();
        ht.Add("name", "dragging");
        ht.Add("x", this.draggingBlock.transform.position.x + (4 * delta));
        ht.Add("time", Mathf.Abs(delta) / 40);
        ht.Add("isLocal", false);
        ht.Add("easetype", iTween.EaseType.easeOutQuint);
        ht.Add("onupdate", "OnInertiaUpdate");
        ht.Add("onupdatetarget", this.gameObject);
        ht.Add("oncomplete", "OnInertiaDone");
        ht.Add("oncompletetarget", this.gameObject);
        iTween.MoveTo(this.draggingBlock.gameObject, ht);

    }

    void OnInertiaUpdate()
    {
        if (this.draggingBlock.transform.localPosition.x < this.draggingBlockBoundaries[0] || this.draggingBlock.transform.localPosition.x > this.draggingBlockBoundaries[1])
        {
            float newX = Mathf.Min(this.draggingBlockBoundaries[1], this.draggingBlock.transform.localPosition.x);
            newX = Mathf.Max(this.draggingBlockBoundaries[0], newX);

            this.draggingBlock.transform.localPosition = new Vector2(
                newX,
                this.draggingBlock.transform.localPosition.y
            );
            iTween.StopByName("dragging");
            OnInertiaDone();
        }
    }

    void OnInertiaDone()
    {
        //Force the dragging block to be column-perfect
        int index = this.sameRowBlocks.IndexOf(this.draggingBlock);
        this.draggingBlock.GetComponent<Block>().SetColumIndex(index);

        this.draggingBlock.GetComponent<PhysicsBlock>().enablePhysics = true;
        StartCoroutine(WaitAndDraggingBlockPhysicsDisabled(this.draggingBlock));

        //unregister the draggingBlock
        this.draggingBlock = null;


    }

    private IEnumerator WaitAndDraggingBlockPhysicsDisabled(GameObject b)
    {
        yield return new WaitForSeconds(0.5f);

        b.GetComponent<PhysicsBlock>().CanBeDisabled();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float[] CalculateBlockBoundaries(GameObject b)
    {
        int left = 0;
        int right = 5;

        //check Null block under the current row
        float y = b.GetComponent<RectTransform>().position.y - b.GetComponent<Block>().GetRealHeight();
        float tolerance = b.GetComponent<Block>().GetRealHeight() / 10;
        List<GameObject> underRowBlocks = this.GetBlocksFromSameRowThan(y, tolerance);

        if (underRowBlocks.Count > 0)
        {
            int columnIndex = b.GetComponent<Block>().GetColumnIndex();

            //left

            for (int iB = columnIndex; iB >= 0; iB--)
            {
                if (underRowBlocks[iB] == null)
                {
                    left = iB;
                    break;
                }
            }

            //right

            for (int iB = columnIndex; iB < underRowBlocks.Count; iB++)
            {
                if (underRowBlocks[iB] == null)
                {
                    right = iB;
                    break;
                }
            }
        }
        

        float columWwidth = b.GetComponent<Block>().GetRealWidth();
        //print("LR: " + left + " " + right);
        return new float[] {
            (left + 1) * columWwidth,
            (right + 1) * columWwidth
        };
    }

    /// <summary>
    /// Returns all blocks from same row except the origin block `b`
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    private List<GameObject> GetBlocksFromSameRowThan(float positionY, float tolerance)
    {
        
        List<GameObject> found = this.blockManager.GetBlocks()
            .Where(item =>
                item.GetComponent<RectTransform>().position.y >= positionY - tolerance
                && item.GetComponent<RectTransform>().position.y <= positionY + tolerance )
            .OrderBy(item => item.GetComponent<RectTransform>().position.x)
            .ToList<GameObject>();

        List<GameObject> result = new List<GameObject>();
        int index = 0;
        while(result.Count < 6)
        {
            if (index >= found.Count)
            {
                //print("GetBlocksFromSameRowThan Add Null1");
                result.Add(null);
                index++;
            }else if (found[index].GetComponent<Block>().GetColumnIndex() == result.Count) // <= to keep draggingBlock when dragging
            {
                //print("GetBlocksFromSameRowThan Element " + found[index].GetComponent<Block>().color);
                result.Add(found[index]);
                index++;
            }
            else
            {
                //print("GetBlocksFromSameRowThan Add Null2");
                result.Add(null);
            }
        }

        string s = "";
        for (int iB = 0; iB < result.Count; iB++)
        {
            s += (result[iB] != null ? result[iB].GetComponent<Block>().color.ToString() : "___") + " ";
        }
        //print("Total Row is: " + s);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    /// <param name="index"></param>
    private void MoveBlockAtIndex(GameObject b, int index)
    {
        int currentBlockIndex = this.sameRowBlocks.IndexOf(b);
        //print(currentBlockIndex);
        this.sameRowBlocks.RemoveAt(currentBlockIndex);
        this.sameRowBlocks.Insert(index, draggingBlock);

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
                    GameObject blockAtTheBottom = this.MoveBlocksDown(iB, b.GetComponent<RectTransform>().position.y);

                    if (blockAtTheBottom)
                    {
                        print("blockAtTheBottom: " + blockAtTheBottom.GetComponent<Block>().GetColumnIndex() + " " + blockAtTheBottom.GetComponent<Block>().color);

                        //Insert the falling block to the row (iB should be equal to blockAtTheBottom.GetComponent<Block>().GetColumnIndex()
                        this.sameRowBlocks[blockAtTheBottom.GetComponent<Block>().GetColumnIndex()] = blockAtTheBottom;

                        string s = "";
                        for (int iB2 = 0; iB2 < this.sameRowBlocks.Count; iB2++)
                        {
                            s += (this.sameRowBlocks[iB2] != null ? this.sameRowBlocks[iB2].GetComponent<Block>().color.ToString() : "___") + " ";
                        }
                        print("Big Total Row is: " + s);

                        //Physics can be disabled
                        StartCoroutine(this.DisablePhysicsForAllBlocks());
                    }
                    
                }
            }
        }


    }

    private IEnumerator DisablePhysicsForAllBlocks()
    {
        yield return new WaitForSeconds(1f);
        List<GameObject> blocks = this.blockManager.GetBlocks();
        for (int iB = 0; iB < blocks.Count; iB++)
        {
            blocks[iB].GetComponent<PhysicsBlock>().CanBeDisabled();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnIndex"></param>
    /// <param name="positionY"></param>
    /// <returns>Returns the block at the bottom</returns>
    private GameObject MoveBlocksDown(int columnIndex, float positionY)
    {
        List<GameObject> blocks = this.GetBlocksOnTop(columnIndex, positionY);
        for (int iB = 0; iB < blocks.Count; iB++)
        {
            blocks[iB].GetComponent<PhysicsBlock>().enablePhysics = true;
        }

        // can be null when moving block on the top row
        return blocks.Count > 0 ? blocks[0] : null;
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
            .Where(item => item.GetComponent<Block>().GetColumnIndex() == columnIndex && item.GetComponent<RectTransform>().position.y > positionY)
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
            int draggingBlockColumnIndex = this.draggingBlock.GetComponent<Block>().GetColumnIndex();

            if (this.sameRowBlocks[draggingBlockColumnIndex] != this.draggingBlock)
            {
                //print("something to do");
                this.MoveBlockAtIndex(this.draggingBlock, draggingBlockColumnIndex);
            }
        }
    }


}

