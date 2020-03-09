using System;
using System.Collections;
using UnityEngine;

public class BlockDoubleTapManager: MonoBehaviour
{
    public BlockManager blockManager;
    public BlockUnionManager blockUnionManager;

    private void Start()
    {
        
    }

    public void NotifyBlockDoubleTapped(GameObject block)
    {
        print("NotifyBlockDoubleTapped");
        this.blockManager.RemoveBlock(block);

        //Force to InvalidateRows because a block could be moving
        StartCoroutine(WaitAndInvalidateRows());
    }

    IEnumerator WaitAndInvalidateRows()
    {
        yield return new WaitForSeconds(0.5f);

        this.blockManager.InvalidateRows();

        this.blockUnionManager.FindBlocksForUnion();

        
    }
}

