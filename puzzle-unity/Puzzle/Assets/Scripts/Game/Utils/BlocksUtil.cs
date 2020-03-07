using System;
using System.Collections.Generic;
using UnityEngine;


public class BlocksUtil
{
    public BlocksUtil()
    {

    }

    public static void LogRows(List<List<GameObject>> rows)
    {
        string s = "";
        for (var iR = rows.Count - 1; iR >= 0; iR--)
        {
            for (var iC = 0; iC < rows[iR].Count; iC++)
            {
                GameObject block = rows[iR][iC];
                if (block != null)
                {

                    s += "(" + String.Join(",", block.GetComponent<Block>().GetColumnIndexes().ToArray()) + " " + block.GetComponent<Block>().color.ToString().ToCharArray()[0] + ")";
                }
                else
                {
                    s += "____";
                }
                s += " ";
            }

            s += "\n";
        }
        Debug.Log(s);
    }

    public static string LogList(List<GameObject> blocks)
    {
        string s = "";
        for (var iC = 0; iC < blocks.Count; iC++)
        {
            GameObject block = blocks[iC];
            if (block != null)
            {

                s += "(" + String.Join(",", block.GetComponent<Block>().GetColumnIndexes().ToArray()) + " " + block.GetComponent<Block>().color.ToString().ToCharArray()[0] + ")";
            }
            else
            {
                s += "____";
            }
            s += " ";
        }
        return s;
    }
}

