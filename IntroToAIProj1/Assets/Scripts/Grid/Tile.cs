using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int y = 0;
    public int x = 0;

    private int numMoves = 1;
    private int depth = -1;
    private UITextHandler tileMovesUI;
    private UITextHandler tileDepthUI;

    public Tile()
    {
        x = 0;
        y = 0;
        numMoves = 1;
        depth = -1;
        tileMovesUI = null;
        tileDepthUI = null;
    }

    public Tile(int posX, int posY, UITextHandler tileMovesTextComp, UITextHandler tileDepthTextComp)
    {
        InitializeTile(posX, posY, tileMovesTextComp, tileDepthTextComp);
    }

    public Tile(Tile tileToCopy)
    {
        InitializeTile(tileToCopy.x, tileToCopy.y, tileToCopy.GetTileMovesUI(), tileToCopy.GetTileDepthUI());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(/*"x: " + x + "y: " + y + */"Moves count: " + tileUI.Text());
    }

    public void InitializeTile(int posX, int posY, UITextHandler tileMovesTextComp, UITextHandler tileDepthTextComp)
    {
        y = posX;
        x = posY;
        tileMovesUI = tileMovesTextComp;
        if (tileMovesUI)
        {
            tileMovesUI.SetTextOfTile(numMoves.ToString());
        }
        else
        {
            Debug.LogError("TileMovesUI Not SET");
        }

        tileDepthUI = tileDepthTextComp;
        if (tileDepthUI)
        {
            tileMovesUI.SetTextOfTile(depth.ToString());
        }
        else
        {
            Debug.LogError("TileDepthUI Not SET");
        }
    }

    public int SetNumMoves(int _numMoves)
    {
        numMoves = _numMoves;
        if(numMoves != 0)
        {
            tileMovesUI.SetTextOfTile(numMoves.ToString());
        }
        else
        {
            tileMovesUI.SetTextOfTile("G");
        }
        return numMoves;
    }

    public int SetTileDepth(int _tileDepth)
    {
        depth = _tileDepth;
        tileDepthUI.SetTextOfTile(depth.ToString());
        //Debug.Log("Set depth: "+ depth);
        return depth;
    }

    public int GetNumMoves()
    {
        return numMoves;
    }
    public int GetTileDepth()
    {
        return depth;
    }
    public UITextHandler GetTileMovesUI()
    {
        return tileMovesUI;
    }
    public UITextHandler GetTileDepthUI()
    {
        return tileDepthUI;
    }
}
