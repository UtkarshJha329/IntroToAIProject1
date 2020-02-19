using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataHandler : MonoBehaviour
{
    public GameObject gridSceneObject;
    private GridManagerAPI gridAPI;

    private int oldSeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        oldSeed = Random.seed;

        if (gridSceneObject != null)
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
        if(gridAPI.generateBoard && gridAPI.boardGenerated)
        {
            if (gridAPI.ResetDepthValues())
            {
                    //PART 2 OF PROJECT
                if (SetLegalMovesForAllTiles())
                {
                    //PART 3 OF PROJECT
                    if (CalculateDepthForEachTile())
                    {
                        //Debug.Log("Created Board With Initial Values");
                    }
                }
            }
            gridAPI.generateBoard = false;
        }

        //PART 4 Of The Project
        if (gridAPI.boardGenerated && gridAPI.doHillClimb)
        {
            HillClimb(gridAPI.hilClimbNumIter);
            gridAPI.doHillClimb = false;
        }
    }

    private bool SetLegalMovesForAllTiles()
    {
        for (int x = 0; x < gridAPI.GridSize(); x++)
        {
            for (int y = 0; y < gridAPI.GridSize(); y++)
            {
                SetTilRandomLegalMove(x, y);
            }
        }
        gridAPI.GetGoalTile().SetNumMoves(0);

        return true;
    }

    private bool SetTilRandomLegalMove(int x, int y)
    {
        //Random.InitState(oldSeed + 100);
        //oldSeed += 100;
        //Debug.Log("element: " + (i * gridSize + j));
        int maxX = (x <= gridAPI.GridSize() / 2) ? gridAPI.GridSize() - x : gridAPI.GridSize() - x / 2;
        int maxY = (y <= gridAPI.GridSize() / 2) ? gridAPI.GridSize() - y : gridAPI.GridSize() - y / 2;

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

        return true;
    }

    private bool CalculateDepthForEachTile()
    {
        gridAPI.ResetDepthValues();
        Queue<Tile> nextLookAtTile = new Queue<Tile>();

        gridAPI.GetStartTile().SetTileDepth(0);
        nextLookAtTile.Enqueue(gridAPI.GetStartTile());
        int numTilesVisited = 1;
        
        Tile dequeTile;

        while (nextLookAtTile.Count > 0)
        {
            dequeTile = nextLookAtTile.Dequeue();

            if(dequeTile != gridAPI.GetGoalTile())
            {
                List<Tile> inRangeTiles = GetInRangeTiles(dequeTile);
                //Debug.Log(inRangeTiles.Count);
                for (int i = 0; i < inRangeTiles.Count; i++)
                {
                    if (inRangeTiles[i].GetTileDepth() == -1)
                    {
                        inRangeTiles[i].SetTileDepth(dequeTile.GetTileDepth() + 1);
                        numTilesVisited++;
                        nextLookAtTile.Enqueue(inRangeTiles[i]);
                        //Debug.Log(inRangeTiles[i].x + ", " + inRangeTiles[i].y);
                    }
                }
            }
        }

        if(gridAPI.GetGoalTile().GetTileDepth() == -1)
        {
            gridAPI.GetGoalTile().SetTileDepth(-(gridAPI.CurNumTiles() - numTilesVisited));
        }

        return true;
    }

    private void HillClimb(int numItterations)
    {
        //Debug.Log("Doing Hill Climb");
        int oldMoves;
        int oldGoalDepth;

        for (int i = 0; i < numItterations; i++)
        {
            int randX = Random.Range(0, gridAPI.GridSize());
            int randY = (randX == 0) ? Random.Range(0, gridAPI.GridSize() - 1) : Random.Range(0, gridAPI.GridSize());
            //Debug.Log(randX + ", " + randY);

            oldMoves = gridAPI.GetTileByCoord(randX, randY).GetNumMoves();
            oldGoalDepth = gridAPI.GetGoalTile().GetTileDepth();
            //Debug.Log("BEFORE: (" + randX + ", " + randY + ")" + "NumMoves: " + gridAPI.GetTileByCoord(randX, randY).GetNumMoves());
            // yield return new WaitForSeconds(0.5f);

            if (SetTilRandomLegalMove(randX, randY))
            {
                //gridAPI.ResetDepthValues();
                //Debug.Log("AFTER: (" + randX + ", " + randY + ")" + "NumMoves: " + gridAPI.GetTileByCoord(randX, randY).GetNumMoves());
                if (CalculateDepthForEachTile())
                {
                    if (gridAPI.GetGoalTile().GetTileDepth() < oldGoalDepth)
                    {
                        gridAPI.GetTileByCoord(randX, randY).SetNumMoves(oldMoves);
                        CalculateDepthForEachTile();
                    }
                    else
                    {
                        //Debug.Log(gridAPI.GetGoalTile().GetTileDepth() + ", " + oldGoalDepth);
                    }
                }
            }

            //yield return new WaitForSeconds(0.5f);
        }

        //yield return null;

    }

    private List<Tile> GetInRangeTiles(Tile tile)
    {
        List<Tile> returnTiles = new List<Tile>();
        int numMoves = tile.GetNumMoves();

        Tile tileToAdd;
        if(numMoves != 0)
        {
            if (tile.y - numMoves >= 0)
            {
                tileToAdd = gridAPI.GetTileByCoord(tile.y - numMoves, tile.x);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Down: " + tileToAdd.GetTileDepth());
                if (tileToAdd.GetTileDepth() < 0)
                {
                    returnTiles.Add(tileToAdd);
                }
            }
            if (tile.y + numMoves < gridAPI.GridSize())
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
        }

        return returnTiles;
    }
}
