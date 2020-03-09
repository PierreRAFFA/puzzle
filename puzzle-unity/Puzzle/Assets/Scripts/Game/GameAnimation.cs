using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAnimation : MonoBehaviour
{
    public BlockUnionManager blockUnionManager;
    public BlockManager blockManager;

    private float previousModulo = -1;
    private float total = 0;
    // Start is called before the first frame update
    void Start()
    {
        BlockPlayable.Delta = 0;

        InvokeRepeating("CheckNewLineVisible", 0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        BlockPlayable.Delta = Time.deltaTime * 5;
        this.total += BlockPlayable.Delta;
    }

    void CheckNewLineVisible()
    {
        float currentModulo = this.total % Block.BlockSize;
        //print("MODULO current:" + currentModulo);
        //print("MODULO previous:" + previousModulo);

        if (previousModulo >= 0)
        {
            if (currentModulo < previousModulo)
            {
                //this is a new line visible
                //print("MODULO new line");

                //check for unions
                int index = this.blockManager.GetFirstRowVisibleIndex();
                print(this.blockManager.GetRows()[index - 1][0].transform.position.y);
                blockUnionManager.FindBlocksForUnionFromRowRange(index, index);


                //create a new line
                this.blockManager.CreateNewLine();

                ////////////////////////////////////////////////////////////
                //////////////////////////////////////////// TEMPORARY
                //check to remove top line
                if (this.blockManager.GetRows().Count > 24)
                {
                    this.blockManager.RemoveTopLine();
                }
                ////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////

            }
        }

        previousModulo = currentModulo;
    }
}
