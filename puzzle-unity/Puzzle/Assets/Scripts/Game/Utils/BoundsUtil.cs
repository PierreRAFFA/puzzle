using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class BoundsUtil
{
    public BoundsUtil()
    {
    }

    /// <summary>
    /// Returns the bounds from all blocks geometry
    /// </summary>
    /// <param name="blocks"></param>
    /// <returns></returns>
    public static List<Vector2> GetBoundsFromGeometry(List<GameObject> blocks)
    {
        List<Vector2> bounds = new List<Vector2>();

        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float maxY = -Mathf.Infinity;

        for (var iB = 0; iB < blocks.Count; iB++)
        {
            float blockMinX = blocks[iB].GetComponent<RectTransform>().localPosition.x - blocks[iB].GetComponent<Block>().GetRealWidth();
            float blockMaxY = blocks[iB].GetComponent<RectTransform>().localPosition.y + blocks[iB].GetComponent<Block>().GetRealHeight();

            minX = Mathf.Min(minX, Mathf.Round(blockMinX));
            minY = Mathf.Min(minY, Mathf.Round(blocks[iB].GetComponent<RectTransform>().localPosition.y));

            maxX = Mathf.Max(maxX, Mathf.Round(blocks[iB].GetComponent<RectTransform>().localPosition.x));
            maxY = Mathf.Max(maxY, Mathf.Round(blockMaxY));
        }
        Debug.Log(minX + " " + minY + " " + maxX + " " + maxY);

        bounds.Add(new Vector2(minX, minY));
        bounds.Add(new Vector2(maxX, maxY));

        return bounds;

    }

    /// <summary>
    /// Returns the bounds based on block1x1 geometry
    /// </summary>
    /// <param name="blocks"></param>
    /// <returns></returns>
    public static List<Vector2> GetBoundsFromMatrix(List<GameObject> blocks)
    {
        List<Vector2> bounds = new List<Vector2>();

        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float maxY = -Mathf.Infinity;

        for (var iB = 0; iB < blocks.Count; iB++)
        {
            float blockMinX = blocks[iB].GetComponent<RectTransform>().localPosition.x - Block.BlockSize;
            float blockMaxY = blocks[iB].GetComponent<RectTransform>().localPosition.y + Block.BlockSize;

            minX = Mathf.Min(minX, blockMinX);
            minY = Mathf.Min(minY, blocks[iB].GetComponent<RectTransform>().localPosition.y);

            maxX = Mathf.Max(maxX, blocks[iB].GetComponent<RectTransform>().localPosition.x);
            maxY = Mathf.Max(maxY, blockMaxY);
        }
        Debug.Log(minX + " " + minY + " " + maxX + " " + maxY);

        bounds.Add(new Vector2(minX, minY));
        bounds.Add(new Vector2(maxX, maxY));

        return bounds;
    }

    /// <summary>
    /// Return true if widths and heights are equal ( +tolerance )
    /// </summary>
    /// <param name="bounds1"></param>
    /// <param name="bounds2"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public static bool AreBoundsSizeEqual(List<Vector2> bounds1, List<Vector2> bounds2, int tolerance)
    {
        float width1 = bounds1[1].x - bounds1[0].x;
        float height1 = bounds1[1].y - bounds1[0].y;

        float width2 = bounds2[1].x - bounds2[0].x;
        float height2 = bounds2[1].y - bounds2[0].y;

        //Debug.Log("AreBoundsEqual width1:" + width1);
        //Debug.Log("AreBoundsEqual height1:" + height1);
        //Debug.Log("AreBoundsEqual width2:" + width2);
        //Debug.Log("AreBoundsEqual height2:" + height2);

        return (width1 - width2 <= tolerance && width1 - width2 <= tolerance);

        //return ((Mathf.Abs(bounds1[0].x - bounds2[0].x) <= tolerance)
        //    && (Mathf.Abs(bounds1[0].y - bounds2[0].y) <= tolerance)
        //    && (Mathf.Abs(bounds1[1].x - bounds2[1].x) <= tolerance)
        //    && (Mathf.Abs(bounds1[1].y - bounds2[1].y) <= tolerance));
    }

}