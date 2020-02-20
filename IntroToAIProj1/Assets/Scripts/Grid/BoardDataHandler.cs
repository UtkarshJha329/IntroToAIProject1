using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataHandler : MonoBehaviour
{
    public GameObject gridSceneObject;
    private GridManagerAPI gridAPI;

    private TimeKeeper stopwatch;

    // Start is called before the first frame update
    void Start()
    {
        stopwatch = new TimeKeeper();

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
            stopwatch.Start();
            if (gridAPI.ResetDepthValues())
            {
                    //PART 2 OF PROJECT
                if (SetLegalMovesForAllTiles())
                {
                    //PART 3 OF PROJECT
                    if (CalculateDepthForEachTile())
                    {
                        //Debug.Log("Created Board With Initial Values");
                        gridAPI.SetTimerText(stopwatch.Stop());
                    }
                    else
                    {
                        stopwatch.Stop();
                        gridAPI.SetTimerText("Failed");
                    }
                }
                else
                {
                    stopwatch.Stop();
                    gridAPI.SetTimerText("Failed");
                }

            }
            else
            {
                stopwatch.Stop();
                gridAPI.SetTimerText("Failed");
            }

            gridAPI.generateBoard = false;
        }

        //PART 4 Of The Project
        if (gridAPI.boardGenerated && gridAPI.doHillClimb)
        {
            stopwatch.Start();
            if (HillClimb(gridAPI.hilClimbNumIter))
            {
                gridAPI.doHillClimb = false;
                gridAPI.SetTimerText(stopwatch.Stop());
            }
            else
            {
                stopwatch.Stop();
                gridAPI.SetTimerText("Failed");
            }
        }


        //PART 5 Of The Project
        if (gridAPI.boardGenerated && gridAPI.doSPF)
        {
            stopwatch.Start();
            if(gridAPI.ResetSpfRValues() && ShortestPathFirst())
            {
                gridAPI.doSPF = false;
                gridAPI.SetTimerText(stopwatch.Stop());
            }
            else
            {
                stopwatch.Stop();
                gridAPI.SetTimerText("Failed");
            }
        }

        //PART 6 Of The Project
        if (gridAPI.boardGenerated && gridAPI.doAStar)
        {
            stopwatch.Start();
            if (gridAPI.ResetAStarValues() && AStar())
            {
                gridAPI.doAStar = false;
                gridAPI.SetTimerText(stopwatch.Stop());
            }
            else
            {
                stopwatch.Stop();
                gridAPI.SetTimerText("Failed");
            }
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
                List<Tile> inRangeTiles = GetInRangeTiles(dequeTile, 0);
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

    private bool HillClimb(int numItterations)
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
        return true;
    }

    private bool ShortestPathFirst()
    {
        List<Tile> unVisitedKnownTiles = new List<Tile>();
        unVisitedKnownTiles.Add(gridAPI.GetStartTile());

        gridAPI.GetStartTile().SetSpfValue(0);
        int numTilesVisited = 1;

        Tile curTile;

        while(unVisitedKnownTiles.Count > 0)
        {
            int curTileIndex = GetShortestTileIndex(unVisitedKnownTiles);
            curTile = unVisitedKnownTiles[curTileIndex];
            curTile.visited = 1;
            numTilesVisited++;
            List<Tile> tilesInRange = GetInRangeTiles(curTile, 1);

            for (int i = 0; i < tilesInRange.Count; i++)
            {
                if(tilesInRange[i].GetSpfValue() == -1 || tilesInRange[i].GetSpfValue() > curTile.GetSpfValue() + 1)
                {
                    if (!unVisitedKnownTiles.Contains(tilesInRange[i]))
                    {
                        unVisitedKnownTiles.Add(tilesInRange[i]);
                    }
                    tilesInRange[i].SetSpfValue(curTile.GetSpfValue() + 1);
                }
            }
            unVisitedKnownTiles.RemoveAt(curTileIndex);
        }

        if (gridAPI.GetGoalTile().GetSpfValue() == -1)
        {
            gridAPI.GetGoalTile().SetTileDepth(-(gridAPI.CurNumTiles() - numTilesVisited));
        }

        return true;
    }

    private bool AStar()
    {
        List<Tile> unVisitedKnownTiles = new List<Tile>();
        unVisitedKnownTiles.Add(gridAPI.GetStartTile());

        gridAPI.GetStartTile().SetAStarValue(0, GetAStarManHeuWMoves(gridAPI.GetStartTile()));

        Tile curTile;

        while(unVisitedKnownTiles.Count > 0)
        {
            int curTileIndex = GetBestAStarManTile(unVisitedKnownTiles);
            curTile = unVisitedKnownTiles[curTileIndex];
            curTile.visited = 1;
            List<Tile> tilesInRange = GetInRangeTiles(curTile, 1);

            for (int i = 0; i < tilesInRange.Count; i++)
            {
                if(tilesInRange[i].GetAStarFV() == -1 || tilesInRange[i].GetAStarFV() > curTile.GetAStarFV() + 1)
                {
                    if (!unVisitedKnownTiles.Contains(tilesInRange[i]))
                    {
                        unVisitedKnownTiles.Add(tilesInRange[i]);
                    }
                    tilesInRange[i].SetAStarValue(curTile.GetAStarFV() + 1, tilesInRange[i].GetAStarHV());
                }
            }
            unVisitedKnownTiles.RemoveAt(curTileIndex);
        }

        return true;
    }

    private int GetAStarManHeuWMoves(Tile tileToSet)
    {
        if(tileToSet == gridAPI.GetGoalTile())
        {
            return 0;
        }
        else
        {
            int hX = Mathf.Abs(tileToSet.x - gridAPI.GetGoalTile().x);
            int hY = Mathf.Abs(tileToSet.y - gridAPI.GetGoalTile().y);

            int delX = Mathf.Abs(hX - tileToSet.GetNumMoves());
            int delY = Mathf.Abs(hY - tileToSet.GetNumMoves());

            return delX + delY;
        }
    }

    private int GetBestAStarManTile(List<Tile> listToFindFrom)
    {
        int retIndex = -1;

        if(listToFindFrom != null)
        {
            int curLowestGValue = -1;
            int curLowestHValue = -1;

            for (int i = 0; i < listToFindFrom.Count; i++)
            {
                if(listToFindFrom[i].visited == 0 && (curLowestGValue == -1 || listToFindFrom[i].GetAStarGV() < curLowestGValue))
                {
                    if(listToFindFrom[0].GetAStarGV() == curLowestGValue)
                    {
                        if(listToFindFrom[i].GetAStarHV() < curLowestHValue)
                        {
                            curLowestGValue = listToFindFrom[i].GetAStarGV();
                            curLowestHValue = listToFindFrom[i].GetAStarHV();
                            retIndex = i;
                        }
                    }
                    else
                    {
                        curLowestGValue = listToFindFrom[i].GetAStarGV();
                        curLowestHValue = listToFindFrom[i].GetAStarHV();
                        retIndex = i;
                    }
                }
            }
        }

        return retIndex;
    }

    private int GetShortestTileIndex(List<Tile> listToSortFrom)
    {
        int retIndex = -1;

        if (listToSortFrom != null)
        {
            int curLowestValue = listToSortFrom[0].GetSpfValue();
            retIndex = 0;

            for (int i = 0; i < listToSortFrom.Count; i++)
            {
                int curTileValue = listToSortFrom[i].GetSpfValue();

                if (listToSortFrom[i].visited == 0 && curTileValue < curLowestValue)
                {
                    curLowestValue = curTileValue;
                    retIndex = i;
                }
            }
        }

        return retIndex;
    }

    private List<Tile> GetInRangeTiles(Tile tile, int mode)
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
                if (mode == 0 && tileToAdd.GetTileDepth() < 0)
                {
                    returnTiles.Add(tileToAdd);
                }
                else if(mode == 1)
                {
                    returnTiles.Add(tileToAdd);
                }
            }
            if (tile.y + numMoves < gridAPI.GridSize())
            {
                tileToAdd = gridAPI.GetTileByCoord(tile.y + numMoves, tile.x);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Up: " + tileToAdd.GetTileDepth());
                if (mode == 0 && tileToAdd.GetTileDepth() < 0)
                {
                    returnTiles.Add(tileToAdd);
                }
                else if (mode == 1)
                {
                    returnTiles.Add(tileToAdd);
                }
            }

            if (tile.x - numMoves >= 0)
            {
                tileToAdd = gridAPI.GetTileByCoord(tile.y, tile.x - numMoves);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth left: " + tileToAdd.GetTileDepth());
                if (mode == 0 && tileToAdd.GetTileDepth() < 0)
                {
                    returnTiles.Add(tileToAdd);
                }
                else if (mode == 1)
                {
                    returnTiles.Add(tileToAdd);
                }
            }
            if (tile.x + numMoves < gridAPI.GridSize())
            {
                tileToAdd = gridAPI.GetTileByCoord(tile.y, tile.x + numMoves);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Right: " + tileToAdd.GetTileDepth());
                if (mode == 0 && tileToAdd.GetTileDepth() < 0)
                {
                    returnTiles.Add(tileToAdd);
                }
                else if (mode == 1)
                {
                    returnTiles.Add(tileToAdd);
                }
            }
        }

        return returnTiles;
    }
}
