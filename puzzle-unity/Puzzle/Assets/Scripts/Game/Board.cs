using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public BlockManager blockManager;
    public BlockUnionManager blockUnionManager;

    // Start is called before the first frame update
    void Start()
    {
        this.DisplayBlock();
    }

    void DisplayBlock()
	{
        float columnWidth = this.GetComponent<RectTransform>().rect.width / 6f;
        print(columnWidth);
        Block.BlockSize = columnWidth;

        //float factor = 1f; // 1.26f
        for (int iC = 0; iC < 6; iC++)
        {
            //int numBlocks = Random.Range(5, 12);//Random.Range(13, 22);
            //for (int iB = 0; iB < numBlocks; iB++)
            //{
            //    this.blockManager.CreateBlock(
            //        Mathf.RoundToInt((iC + 1) * columnWidth),
            //        Mathf.RoundToInt((iB - 0) * columnWidth),
            //        1, 1,
            //        true,
            //        null
            //    );
            //}

            int numBlocks = Random.Range(13, 22);
            for (int iB = 0; iB < numBlocks; iB++)
            {
                this.blockManager.CreateBlock(
                    Mathf.RoundToInt((iC + 1) * columnWidth),
                    Mathf.RoundToInt((iB - 0) * columnWidth),
                    1, 1,
                    true,
                    null
                );
            }
        }

        StartCoroutine(WaitAndUnion());
    }

    IEnumerator WaitAndUnion()
    {
        yield return new WaitForSeconds(1f);
        this.blockUnionManager.FindBlocksForUnion();
    }
    
        
    // Update is called once per frame
    void Update()
    {
        
    }
}
