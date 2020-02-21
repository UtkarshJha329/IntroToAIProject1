using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int y = 0;
    public int x = 0;

    [HideInInspector] public int visited = 0;
    [HideInInspector] public Tile pfParent;

    private int numMoves = 1;
    private int depth = -1;
    private int spfV = -1;
    private int aStarFV = -1;
    private int aStarHV = -1;
    private int aStarGV = -1;
    [HideInInspector] public int inAStarList = 0;
    private UITextHandler tileMovesUI;
    private UITextHandler tileDepthUI;
    private UITextHandler tileSpfVUI;

    private SpriteRenderer tileSprite;

    public Tile()
    {
        x = 0;
        y = 0;
        numMoves = 1;
        depth = -1;
        spfV = -1;
        tileMovesUI = null;
        tileDepthUI = null;
    }

    public Tile(int posX, int posY, UITextHandler tileMovesTextComp, UITextHandler tileDepthTextComp, UITextHandler tileSpfVTextComp, SpriteRenderer tileSprite)
    {
        InitializeTile(posX, posY, tileMovesTextComp, tileDepthTextComp, tileSpfVTextComp, tileSprite);
    }

    public Tile(Tile tileToCopy)
    {
        InitializeTile(tileToCopy.x, tileToCopy.y, tileToCopy.GetTileMovesUI(), tileToCopy.GetTileDepthUI(), tileToCopy.GetTileSpfVUI(), tileToCopy.tileSprite);
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

    public void InitializeTile(int posX, int posY, UITextHandler tileMovesTextComp, UITextHandler tileDepthTextComp, UITextHandler tileSpfVTextComp, SpriteRenderer _tileSprite)
    {
        y = posX;
        x = posY;
        tileMovesUI = tileMovesTextComp;
        tileSprite = _tileSprite;
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

        tileSpfVUI = tileSpfVTextComp;
        if (tileSpfVUI)
        {
            tileSpfVUI.SetTextOfTile(spfV.ToString());
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

    public int SetSpfValue(int _spfV)
    {
        spfV = _spfV;
        tileSpfVUI.SetTextOfTile(spfV.ToString());
        //Debug.Log("Set depth: "+ depth);
        return spfV;
    }
    public void SetAStarValue(int _aStarF)
    {
        aStarFV = _aStarF;
        aStarGV = aStarFV + aStarHV;
        tileSpfVUI.SetTextOfTile(aStarFV.ToString());
        //Debug.Log("Set depth: "+ depth);
    }

    public void SetAStarValue(int _aStarF, int _aStarH)
    {
        aStarFV = _aStarF;
        aStarHV = _aStarH;
        aStarGV = aStarFV + aStarHV;
        tileSpfVUI.SetTextOfTile(aStarFV.ToString());
        //Debug.Log("Set depth: "+ depth);
    }

    public int GetAStarFV()
    {
        return aStarFV;
    }
    public int GetAStarHV()
    {
        return aStarHV;
    }
    public int GetAStarGV()
    {
        return aStarGV;
    }
    public int GetNumMoves()
    {
        return numMoves;
    }
    public int GetTileDepth()
    {
        return depth;
    }
    public int GetSpfValue()
    {
        return spfV;
    }

    public UITextHandler GetTileMovesUI()
    {
        return tileMovesUI;
    }
    public UITextHandler GetTileDepthUI()
    {
        return tileDepthUI;
    }
    public UITextHandler GetTileSpfVUI()
    {
        return tileSpfVUI;
    }

    public SpriteRenderer GetSprite()
    {
        return tileSprite;
    }

    public void SetSpriteColor(Color color)
    {
        tileSprite.color = color;
    }
}
