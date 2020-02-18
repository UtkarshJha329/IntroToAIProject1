using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x = 0;
    public int y = 0;

    private int numMoves = 1;
    private int depth = 0;
    private UITextHandled tileUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(/*"x: " + x + "y: " + y + */"Moves count: " + tileUI.Text());
    }

    public void InitializeTile(int posX, int posY, UITextHandled tileTextComponent)
    {
        x = posX;
        y = posY;
        tileUI = tileTextComponent;
        tileUI.SetTextOfTile(numMoves.ToString());
    }

    public int SetNumMoves(int _numMoves)
    {
        numMoves = _numMoves;
        tileUI.SetTextOfTile(numMoves.ToString());
        return numMoves;
    }
}
