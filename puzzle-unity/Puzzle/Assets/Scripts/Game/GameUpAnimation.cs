using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUpAnimation : MonoBehaviour
{
    public BlockUnionManager blockUnionManager;
    public BlockManager blockManager;

    private float previousModulo = -1;
    // Start is called before the first frame update
    void Start()
    {
        BlockPlayable.Delta = 0;

        InvokeRepeating("CheckNewLineVisible", 0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        BlockPlayable.Delta += 0.05f;
    }

    void CheckNewLineVisible()
    {
        float currentModulo = BlockPlayable.Delta % Block.BlockSize;
        //print("MODULO current:" + currentModulo);
        //print("MODULO previous:" + previousModulo);

        if (previousModulo >= 0)
        {
            if (currentModulo < previousModulo)
            {
                //this is a new line visible
                //print("MODULO new line");

                int index = this.blockManager.GetFirstRowVisibleIndex();
                //print("MODULO new line " + index);
                blockUnionManager.FindBlocksForUnionFromRowRange(index, index + 1);
            }
        }

        previousModulo = currentModulo;
    }
}
