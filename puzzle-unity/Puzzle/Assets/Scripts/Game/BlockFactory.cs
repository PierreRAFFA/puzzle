using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public BlockFactory() 
    {
    }

    public static GameObject CreateBlock1x1(Transform container, float positionX, float positionY, float columnWidth)
    {
        float factor = 1f; // 1.26f

        GameObject block = Instantiate(Resources.Load<GameObject>("Prefabs/Block1x1"), new Vector3(1, 1, 0), Quaternion.identity) as GameObject;
        block.name = "BlockDraggable";
        block.transform.SetParent(container, false);

        //scale instead of sizeDelta because we want the boxCollider to follow
        float originalWidth = block.GetComponent<RectTransform>().rect.width;
        block.GetComponent<RectTransform>().localScale = new Vector3(columnWidth / originalWidth * factor, columnWidth / originalWidth * factor, 1);

        //block.GetComponent<RectTransform>().sizeDelta = new Vector3(columnWidth * factor, columnWidth * factor);
        block.GetComponent<Block>().color = GetRandomColor();
        block.GetComponent<RectTransform>().anchoredPosition = new Vector3(positionX, positionY);

        return block;
    }

    public static GameObject CreateBlock2x2(Transform container, float positionX, float positionY, float width, float height, Vector2 sizeUnits, bool draggable, BlockColor? color)
    {
        float factor = 1f; // 1.26f

        GameObject block = Instantiate(Resources.Load<GameObject>("Prefabs/Block2x2"), new Vector3(1, 1, 0), Quaternion.identity) as GameObject;
        block.name = draggable ? "BlockDraggable" : "BlockNotDraggable";
        block.transform.SetParent(container, false);

        //apply position and size
        block.GetComponent<RectTransform>().sizeDelta = new Vector3(width * factor, height * factor);
        block.GetComponent<RectTransform>().anchoredPosition = new Vector3(positionX, positionY);
        block.GetComponent<Block>().sizeUnits = sizeUnits;

        //apply color
        block.GetComponent<Block>().color = color ?? GetRandomColor();

        //adjust boxCollider
        int paddingX = 30;
        block.GetComponent<BoxCollider2D>().offset = new Vector2(-(width * factor) / 2, (height * factor) / 2);
        block.GetComponent<BoxCollider2D>().size = new Vector2(width * factor - paddingX, height * factor);


        return block;
    }


    static BlockColor GetRandomColor()
    {
        //return BlockColor.Blue;

        int rand = Random.Range(0, 2);
        switch (rand)
        {
            default:
            case 0: return BlockColor.Blue;
            case 1: return BlockColor.Green;
            case 2: return BlockColor.Orange;
            case 3: return BlockColor.Red;
        }
    }
}

