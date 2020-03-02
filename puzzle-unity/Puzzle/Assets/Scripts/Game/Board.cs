using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public BlockDragManager blockDragManager;

    // Start is called before the first frame update
    void Start()
    {
        this.DisplayBlock();
    }

    void DisplayBlock()
	{
        float columnWidth = this.GetComponent<RectTransform>().rect.width / 6f;
        print(columnWidth);

        Transform container = this.transform.Find("BlockContainer");
        float factor = 1f; // 1.26f
        for (int iC = 0; iC < 6; iC++)
        {
            int numBlocks = Random.Range(13, 22);
            for (int iB = 0; iB < numBlocks; iB++)
            {
                GameObject block = Instantiate(Resources.Load<GameObject>("Prefabs/Block1x1"), new Vector3(1, 1, 0), Quaternion.identity) as GameObject;
                block.name = "BlockDraggable";
                block.transform.SetParent(container, false);

                //scale instead of sizeDelta because we want the boxCollider to follow
                float originalWidth = block.GetComponent<RectTransform>().rect.width;
                block.GetComponent<RectTransform>().localScale = new Vector3(columnWidth / originalWidth * factor, columnWidth / originalWidth * factor, 1);

                //block.GetComponent<RectTransform>().sizeDelta = new Vector3(columnWidth * factor, columnWidth * factor);
                block.GetComponent<Block>().color = GetRandomColor();
                block.GetComponent<RectTransform>().anchoredPosition = new Vector3(
                    Mathf.RoundToInt((iC + 1) * columnWidth),
                    Mathf.RoundToInt((iB - 12) * columnWidth)
                );

                this.blockDragManager.RegisterBlock(block);
            }
        }
    }

    BlockColor GetRandomColor()
    {
        int rand = Random.Range(0, 4);
        switch(rand)
        {
            default:
            case 0: return BlockColor.Blue;
            case 1: return BlockColor.Green;
            case 2: return BlockColor.Orange;
            case 3: return BlockColor.Red;
        }
    }
        
    // Update is called once per frame
    void Update()
    {
        
    }
}
