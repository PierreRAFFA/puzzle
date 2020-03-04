using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockFusionManager : MonoBehaviour
{
    public Board board;

    public BlockManager blockManager;

    private const int numColumns = 6;

    private void Start()
    {

    }

    public List<List<GameObject>> FindBlocksForUnion()
    {
        print("FindBlocksForUnion");
        List <List<GameObject>> allBlocks = blockManager.GetRows();

        List<List<GameObject>> groups = new List<List<GameObject>>();
        List<GameObject> selectedBlocks = new List<GameObject>();
        var iR = 0;
        var iC = 0;

        while (iR < allBlocks.Count - 1 && iC < allBlocks[0].Count)
        {
            print(iR + " " + iC);
            if (selectedBlocks.IndexOf(allBlocks[iR][iC]) == -1)
            {
                // union to the right and bottom
                List<List<GameObject>> groupRight = new List<List<GameObject>>();
                groupRight = this.UnionRight(allBlocks, new List<List<GameObject>>(), iR, iC, 2, selectedBlocks);
                groupRight = this.UnionBottom(allBlocks, groupRight, iR + 2, iC, groupRight.Count, selectedBlocks);
                List<GameObject> flattenGroupRight = groupRight.SelectMany(i => i).ToList<GameObject>();

                // union to the bottom and right
                List<List<GameObject>> groupBottom = new List<List<GameObject>>();
                groupBottom = this.UnionBottom(allBlocks, new List<List<GameObject>>(), iR, iC, 2, selectedBlocks);
                groupBottom = this.UnionRight(allBlocks, groupBottom, iR, iC + 2, groupBottom.Count, selectedBlocks);
                List<GameObject> flattenGroupBottom = groupBottom.SelectMany(i => i).ToList<GameObject>();


                List<GameObject> selectedGroup;
                if (flattenGroupRight.Count >= 4 || flattenGroupBottom.Count >= 4)
                {
                    print(iR + " " + iC);
                    if (flattenGroupRight.Count >= flattenGroupBottom.Count)
                    {
                        selectedGroup = flattenGroupRight;
                        iC = iC + groupRight.Count;
                    }
                    else
                    {
                        selectedGroup = flattenGroupBottom;
                        iC = iC + flattenGroupBottom.Count / groupBottom.Count;
                    }

                    //for (var iB = 0; iB < flattenGroupBottom.Count; iB++)
                    //{
                    print(flattenGroupBottom[0].GetComponent<Block>().color);
                    //}

                    groups.Add(selectedGroup);

                    selectedBlocks.InsertRange(selectedBlocks.Count, selectedGroup);
                }
                else
                {
                    iC++;
                }
            }
            else
            {
                //print("already selected");
                iC++;
            }
        
            //boundaries
            if (iC > allBlocks[0].Count - 2)
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

    private List<List<GameObject>> UnionRight(List<List<GameObject>> allBlocks, List<List<GameObject>> group, int row, int column, int length, List<GameObject> selectedBlocks)
    {
        if (column < allBlocks[0].Count && length >= 2)
        {
            GameObject block = allBlocks[row][column];

            if (block != null)
            {
                BlockColor color = group.Count > 0 ? group[0][0].GetComponent<Block>().color: block.GetComponent<Block>().color;

                List<GameObject> blocks = new List<GameObject>();
                bool sameColor = true;
                bool isOneOfBlocksNull = false;
                bool isOneOfBlocksAlreadySelected = false;

                for (var iB = 0; iB < length; iB++)
                {
                    GameObject currentBlock = allBlocks[row + iB][column];
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
                    }
                    else
                    {
                        isOneOfBlocksNull = true;
                    }

                }

                // console.log(sameColor);
                // console.log(isOneOfBlocksAlreadySelected);
                if (sameColor && isOneOfBlocksAlreadySelected == false && isOneOfBlocksNull == false)
                {
                    group.Add(blocks);
                    group = this.UnionRight(allBlocks, group, row, column + 1, length, selectedBlocks);
                }

            }
        }
        return group;
    }

    private List<List<GameObject>> UnionBottom(List<List<GameObject>> allBlocks, List<List<GameObject>> group, int row, int column, int length, List<GameObject> selectedBlocks)
    {
        if (row < allBlocks.Count && length >= 2)
        {
            GameObject block = allBlocks[row][column];

            if (block != null)
            {
                BlockColor color = group.Count > 0 ? group[0][0].GetComponent<Block>().color : block.GetComponent<Block>().color;

                List<GameObject> blocks = new List<GameObject>();
                bool sameColor = true;
                bool isOneOfBlocksNull = false;
                bool isOneOfBlocksAlreadySelected = false;

                for (var iB = 0; iB < length; iB++)
                {
                    GameObject currentBlock = allBlocks[row][column + iB];
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
                    }
                    else
                    {
                        isOneOfBlocksNull = true;
                    }

                }

                // console.log(sameColor);
                // console.log(isOneOfBlocksAlreadySelected);
                if (sameColor && isOneOfBlocksAlreadySelected == false && isOneOfBlocksNull == false)
                {
                    group.Add(blocks);
                    group = this.UnionBottom(allBlocks, group, row + 1, column, length, selectedBlocks);
                }

            }
        }
        return group;
    }
}
