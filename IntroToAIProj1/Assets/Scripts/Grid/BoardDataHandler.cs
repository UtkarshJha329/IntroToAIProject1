using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataHandler : MonoBehaviour
{
    public GameObject gridSceneObject;
    private GridManagerAPI gridAPI;

    // Start is called before the first frame update
    void Start()
    {
        if(gridSceneObject != null)
        {
            gridAPI = gridSceneObject.GetComponent<GridManagerAPI>();
        }

        if(gridAPI == null)
        {
            Debug.LogError("GRID API IS NOT SET, ATTACH GridManagerAPI script to grid parent object.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gridAPI.boardGenerated == true)
        {
            gridAPI.ResetValues();

            //PART 2 OF PROJECT
            SetRandomLegalTileMove();

            //PART 3 OF PROJECT
            CalculateDepthForEachTile();
            
            gridAPI.boardGenerated = false;
        }
    }

    private void SetRandomLegalTileMove()
    {
        for (int x = 0; x < gridAPI.GridSize(); x++)
        {
            for (int y = 0; y < gridAPI.GridSize(); y++)
            {
                //Debug.Log("element: " + (i * gridSize + j));
                int maxX = (x <= gridAPI.GridSize() / 2) ? gridAPI.GridSize() - x : gridAPI.GridSize() - x/2;
                int maxY = (y <= gridAPI.GridSize() / 2) ? gridAPI.GridSize() - y : gridAPI.GridSize() - y/2;

                //Random function's second parameter is exclusive
                int randomX = Random.Range(1, maxX);
                int randomY = Random.Range(1, maxY);

                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    gridAPI.SetNumMovesForTile(x, y, randomX);
                }
                else
                {
                    gridAPI.SetNumMovesForTile(x, y, randomY);
                }
            }
        }
        gridAPI.GetGoalTile().SetNumMoves(0);
    }

    private void CalculateDepthForEachTile()
    {
        Queue<Tile> nextLookAtTile = new Queue<Tile>();

        gridAPI.GetStartTile().SetTileDepth(0);
        nextLookAtTile.Enqueue(gridAPI.GetStartTile());
        
        Tile dequeTile;

        while (nextLookAtTile.Count > 0)
        {
            dequeTile = nextLookAtTile.Dequeue();

            List<Tile> inRangeTiles = GetInRangeTiles(dequeTile);
            //Debug.Log(inRangeTiles.Count);
            
            for (int i = 0; i < inRangeTiles.Count; i++)
            {
                if(inRangeTiles[i].GetTileDepth() == -1)
                {
                    inRangeTiles[i].SetTileDepth(dequeTile.GetTileDepth() + 1);
                    nextLookAtTile.Enqueue(inRangeTiles[i]);
                    //Debug.Log(inRangeTiles[i].x + ", " + inRangeTiles[i].y);
                }
            }
        }
    }

    private List<Tile> GetInRangeTiles(Tile tile)
    {
        List<Tile> returnTiles = new List<Tile>();
        int numMoves = tile.GetNumMoves();

        Tile tileToAdd;

        if(tile.y - numMoves >= 0)
        {
            tileToAdd = gridAPI.GetTileByCoord(tile.y - numMoves, tile.x);
            //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Down: " + tileToAdd.GetTileDepth());
            if(tileToAdd.GetTileDepth() < 0)
            {
                returnTiles.Add(tileToAdd);
            }
        }
        if(tile.y + numMoves < gridAPI.GridSize())
        {
            tileToAdd = gridAPI.GetTileByCoord(tile.y + numMoves, tile.x);
            //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Up: " + tileToAdd.GetTileDepth());
            if (tileToAdd.GetTileDepth() < 0)
            {
                returnTiles.Add(tileToAdd);
            }
        }

        if (tile.x - numMoves >= 0)
        {
            tileToAdd = gridAPI.GetTileByCoord(tile.y, tile.x - numMoves);
            //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth left: " + tileToAdd.GetTileDepth());
            if (tileToAdd.GetTileDepth() < 0)
            {
                returnTiles.Add(tileToAdd);
            }
        }
        if (tile.x + numMoves < gridAPI.GridSize())
        {
            tileToAdd = gridAPI.GetTileByCoord(tile.y, tile.x + numMoves);
            //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Right: " + tileToAdd.GetTileDepth());
            if (tileToAdd.GetTileDepth() < 0)
            {
                returnTiles.Add(tileToAdd);
            }
        }

        return returnTiles;
    }
}
