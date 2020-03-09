using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockUnionManager : MonoBehaviour
{
    public Board board;

    public BlockManager blockManager;

    private const int numColumns = 6;

    private void Start()
    {

    }

    public List<List<GameObject>> FindBlocksForUnion()
    {
        return this.DoFindBlocksForUnion(0, blockManager.GetRows().Count - 2);
    }

    public List<List<GameObject>> FindBlocksForUnionFromRowRange(int rowStart, int rowEnd)
    {
        return this.DoFindBlocksForUnion(rowStart, rowEnd);
    }

    private List<List<GameObject>> DoFindBlocksForUnion(int rowStart, int rowEnd)
    {
        print("=============================FindBlocksForUnion");
        List <List<GameObject>> rows = blockManager.GetRows();

        List<List<GameObject>> groups = new List<List<GameObject>>();
        List<GameObject> selectedBlocks = new List<GameObject>();
        var iR = rowStart;
        var iC = 0;

        while (iR <= rowEnd && iC < rows[0].Count)
        {
            print("=============iR iC:" + iR + " " + iC);

            GameObject currentBlock = rows[iR][iC];
            if (currentBlock != null)
            {
                print(currentBlock.GetComponent<Block>().color);

                bool isBlockAlreadySelected = selectedBlocks.IndexOf(currentBlock) > 0;
                bool isBlockVisible = currentBlock.GetComponent<BlockPlayable>().isVisible;

                if (isBlockAlreadySelected == false && isBlockVisible)
                {
                    // union to the right and top
                    print("iR iC ok:" + iR + "," + iC);
                    List<List<GameObject>> groupRight = new List<List<GameObject>>();
                    groupRight = this.UnionWithRight(rows, new List<List<GameObject>>(), iR, iC, 2, selectedBlocks);
                    print("groupRight1");
                    BlocksUtil.LogRows(groupRight);
                    groupRight = this.UnionWithTop(rows, groupRight, iR + 2, iC, groupRight.Count > 0 ? groupRight[0].Count : 0, selectedBlocks);
                    print("groupRight2");
                    BlocksUtil.LogRows(groupRight);
                    List<GameObject> flattenGroupRight = groupRight.SelectMany(i => i).ToList<GameObject>();

                    // union to the top and right
                    List<List<GameObject>> groupTop = new List<List<GameObject>>();
                    groupTop = this.UnionWithTop(rows, new List<List<GameObject>>(), iR, iC, 2, selectedBlocks);
                    print("groupTop1");
                    BlocksUtil.LogRows(groupTop);
                    groupTop = this.UnionWithRight(rows, groupTop, iR, iC + 2, groupTop.Count, selectedBlocks);
                    print("groupTop2");
                    BlocksUtil.LogRows(groupTop);
                    List<GameObject> flattenGroupTop = groupTop.SelectMany(i => i).ToList<GameObject>();

                    List<GameObject> selectedGroup;
                    if (flattenGroupRight.Count >= 4 || flattenGroupTop.Count >= 4)
                    {
                        List<Vector2> groupRightBounds = this.GetBoundsFromGroup(groupRight, iR, iC);
                        List<Vector2> groupTopBounds = this.GetBoundsFromGroup(groupTop, iR, iC);

                        bool isGroupRightValid = this.IsGroupValid(flattenGroupRight, groupRightBounds);
                        bool isGroupTopValid = this.IsGroupValid(flattenGroupTop, groupTopBounds);

                        print("VALID right: " + isGroupRightValid);
                        print("VALID top: " + isGroupTopValid);

                        //Is any of the groups valid ?
                        if (isGroupRightValid || isGroupTopValid)
                        {
                            //if right not valid, just reset
                            if (isGroupRightValid == false)
                            {
                                flattenGroupRight = new List<GameObject>();
                                groupRight = new List<List<GameObject>>();
                            }

                            //if top not valid, just reset
                            if (isGroupTopValid == false)
                            {
                                flattenGroupTop = new List<GameObject>();
                                groupTop = new List<List<GameObject>>();
                            }

                            print(iR + " " + iC + " found");

                            if (flattenGroupRight.Count >= flattenGroupTop.Count)
                            {
                                selectedGroup = flattenGroupRight;
                                iC += groupRight[0].Count;
                            }
                            else
                            {
                                selectedGroup = flattenGroupTop;
                                iC += groupTop[0].Count;
                            }

                           
                            print(iR + " " + iC + " " + selectedGroup[0].GetComponent<Block>().color);
                            groups.Add(selectedGroup);

                            selectedBlocks.InsertRange(selectedBlocks.Count, selectedGroup);

                        }
                        else
                        {
                            //otherwise go to the new column
                            iC++;
                        }
                    }
                    else
                    {
                        iC++;
                    }
                }
                else
                {
                    iC++;
                }
            }
            else
            {
                iC++;
            }
            
            //boundaries
            if (iC > rows[0].Count - 2)
            {
                iR++;
                iC = 0;
            }
        }


        for (var iG = 0; iG < groups.Count; iG++)
        {
            this.blockManager.UnionBlocks(groups[iG]);
        }

        print("groups.Count " + groups.Count);
        return groups;

    }

    private List<List<GameObject>> UnionWithRight(List<List<GameObject>> rows, List<List<GameObject>> group, int row, int column, int length, List<GameObject> selectedBlocks)
    {
        if (column < rows[0].Count && length >= 2)
        {
            GameObject block = rows[row][column];

            if (block != null)
            {
                BlockColor color = group.Count > 0 ? group[0][0].GetComponent<Block>().color: block.GetComponent<Block>().color;

                List<GameObject> blocks = new List<GameObject>();
                bool sameColor = true;
                bool isOneOfBlocksNull = false;
                bool isOneOfBlocksAlreadySelected = false;
                bool isBlockWithDifferentSize = false;

                for (var iB = 0; iB < length; iB++)
                {
                    GameObject currentBlock = rows[row + iB][column];
                    if (currentBlock != null)
                    {
                        blocks.Add(currentBlock);
                        // console.log(currentBlock.color);
                        if (currentBlock.GetComponent<Block>().color != color)
                        {
                            sameColor = false;
                        }

                        if (selectedBlocks.IndexOf(currentBlock) >= 0)
                        {
                            isOneOfBlocksAlreadySelected = true;
                        }

                        //// To manage this case
                        //// Here, the length is 2 but the next block to check is a block2x3 which can not be added to the group
                        ////
                        ////  ->o o O O
                        ////    o o O O
                        ////        O O
                        ////
                        //if (currentBlock.GetComponent<Block>().GetYIndexes().Count != length)
                        //{
                        //    isBlockWithDifferentSize = true;
                        //    print("isBlockWithDifferentSize Right:" + isBlockWithDifferentSize);
                        //}
                    }
                    else
                    {
                        isOneOfBlocksNull = true;
                    }

                }

                print("sameColor:" + sameColor);
                print("isOneOfBlocksAlreadySelected:" + isOneOfBlocksAlreadySelected);
                print("isOneOfBlocksNull:" + isOneOfBlocksNull);
                if (sameColor && isOneOfBlocksAlreadySelected == false && isOneOfBlocksNull == false)//&& isBlockWithDifferentSize == false)
                {
                    //add each block to the correct row
                    for (var iB = 0; iB < blocks.Count; iB++)
                    {
                        if (group.Count > iB)
                        {
                            group[iB].Add(blocks[iB]);
                        }
                        else
                        {
                            group.Add(new List<GameObject>() { blocks[iB] });
                        }
                    }
                    BlocksUtil.LogRows(group);
                    
                    group = this.UnionWithRight(rows, group, row, column + 1, length, selectedBlocks);
                }

            }
        }
        return group;
    }

    private List<List<GameObject>> UnionWithTop(List<List<GameObject>> rows, List<List<GameObject>> group, int row, int column, int length, List<GameObject> selectedBlocks)
    {
        if (row < rows.Count && length >= 2)
        {
            GameObject block = rows[row][column];

            if (block != null)
            {
                BlockColor color = group.Count > 0 ? group[0][0].GetComponent<Block>().color : block.GetComponent<Block>().color;

                List<GameObject> blocks = new List<GameObject>();
                bool sameColor = true;
                bool isOneOfBlocksNull = false;
                bool isOneOfBlocksAlreadySelected = false;
                bool isBlockWithDifferentSize = false;

                for (var iB = 0; iB < length; iB++)
                {
                    GameObject currentBlock = rows[row][column + iB];
                    if (currentBlock != null)
                    {
                        blocks.Add(currentBlock);
                        // console.log(currentBlock.color);
                        if (currentBlock.GetComponent<Block>().color != color)
                        {
                            sameColor = false;
                        }

                        if (selectedBlocks.IndexOf(currentBlock) >= 0)
                        {
                            isOneOfBlocksAlreadySelected = true;
                        }

                        //// To manage this case
                        //// Here, the length is 2 but the next block to check is a block3x2 which can not be added to the group
                        ////
                        ////    O O O
                        ////    O O O
                        ////    o o
                        ////  ->o o 
                        //if(currentBlock.GetComponent<Block>().GetColumnIndexes().Count != length)
                        //{
                        //    isBlockWithDifferentSize = true;
                        //    print("isBlockWithDifferentSize Top:" + isBlockWithDifferentSize);
                        //}
                    }
                    else
                    {
                        isOneOfBlocksNull = true;
                    }

                }

                // console.log(sameColor);
                // console.log(isOneOfBlocksAlreadySelected);
                if (sameColor && isOneOfBlocksAlreadySelected == false && isOneOfBlocksNull == false)//&& isBlockWithDifferentSize == false)
                {
                    group.Add(blocks);
                    group = this.UnionWithTop(rows, group, row + 1, column, length, selectedBlocks);
                }

            }
        }
        return group;
    }

    private bool IsGroupAlreadyExist(List<GameObject> group)
    {
        GameObject groupBlock = null;
        bool isAlreadyGroup = true;
        for (var iB = 0; iB < group.Count; iB++)
        {
            if (groupBlock != null)
            {
                if (groupBlock != group[iB])
                {
                    isAlreadyGroup = false;
                }
            }
            else
            {
                groupBlock = group[iB];
            }
        }
        return isAlreadyGroup;
    }

    private List<Vector2> GetBoundsFromGroup(List<List<GameObject>> blocks, int rowIndex, int columnIndex)
    {
        List<Vector2> bounds = new List<Vector2>();

        if (blocks.Count >= 2)
        {
            int numColumns = blocks[0].Count;
            int numRows = blocks.Count;

            print("numColumns:" + numColumns);
            print("numRows:" + numRows);
            BlocksUtil.LogRows(blocks);

            bounds.Add(new Vector2(columnIndex * Block.BlockSize, rowIndex * Block.BlockSize));
            bounds.Add(new Vector2((columnIndex + numColumns) * Block.BlockSize, (rowIndex + numRows) * Block.BlockSize));

            print(columnIndex * Block.BlockSize + " " + rowIndex * Block.BlockSize + " " + (columnIndex + numColumns) * Block.BlockSize + " " + (rowIndex + numRows) * Block.BlockSize);
        }
        else
        {
            bounds.Add(Vector2.zero);
            bounds.Add(Vector2.zero);
        }
        return bounds;
    }

    private bool IsGroupValid(List<GameObject> group, List<Vector2> blockBounds)
    {
        //check if existing group
        bool isAlreadyGroup = this.IsGroupAlreadyExist(group);

        //check if the geometry is correct
        List<Vector2> boundsFromGeometry = BoundsUtil.GetBoundsFromGeometry(group);
        bool areBoundsEqual = BoundsUtil.AreBoundsSizeEqual(boundsFromGeometry, blockBounds, Block.BlockSize / 4f);

        return isAlreadyGroup == false && areBoundsEqual;
    }
}
