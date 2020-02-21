using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataHandler : MonoBehaviour
{
    public GameObject gridSceneObject;
    private GridManagerAPI gridAPI;

    public Color pathColor;

    private TimeKeeper stopwatch;
    private bool newPf = false;
    private int TValueToChoose = 0;

    private Tile curPTile;
    private float lerpValue = 1.0f;

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
            if (gridAPI.ResetDepthValues())
            {
                //PART 2 OF PROJECT
                stopwatch.Start();
                if (SetLegalMovesForAllTiles())
                {
                    //PART 3 OF PROJECT
                    if (CalculateDepthForEachTile())
                    {
                        //Debug.Log("Created Board With Initial Values");
                        gridAPI.SetTimerText(stopwatch.Stop());
                        newPf = true;
                        TValueToChoose = 0;
                        curPTile = gridAPI.GetGoalTile();
                        lerpValue = 1.0f;
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

        //Just separated BFS to see how it performs
        if (gridAPI.boardGenerated && gridAPI.doBFS)
        {
            gridAPI.ResetDepthValues();
            stopwatch.Start();
            if (BFS())
            {
                gridAPI.doBFS = false;
                gridAPI.SetTimerText(stopwatch.Stop());
                newPf = true;
                gridAPI.ResetTileColours();
                TValueToChoose = 0;
                curPTile = gridAPI.GetGoalTile();
                lerpValue = 1.0f;
            }
            else
            {
                stopwatch.Stop();
                gridAPI.SetTimerText("Failed");
            }
        }

        //PART 4 Of The Project
        if (gridAPI.boardGenerated && gridAPI.doHillClimb)
        {
            gridAPI.ResetDepthValues();
            stopwatch.Start();
            if (HillClimb(gridAPI.hilClimbNumIter))
            {
                gridAPI.doHillClimb = false;
                gridAPI.SetTimerText(stopwatch.Stop());
                gridAPI.ResetTileColours();
                newPf = true;
                TValueToChoose = 0;
                curPTile = gridAPI.GetGoalTile();
                lerpValue = 1.0f;
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
            gridAPI.ResetSpfRValues();
            stopwatch.Start();
            if(ShortestPathFirst())
            {
                gridAPI.doSPF = false;
                gridAPI.SetTimerText(stopwatch.Stop());
                newPf = true;
                gridAPI.ResetTileColours();
                TValueToChoose = 1;
                curPTile = gridAPI.GetGoalTile();
                lerpValue = 1.0f;
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
            gridAPI.ResetAStarValues();
            stopwatch.Start();
            if (AStar())
            {
                gridAPI.doAStar = false;
                gridAPI.SetTimerText(stopwatch.Stop());
                newPf = true;
                gridAPI.ResetTileColours();
                TValueToChoose = 2;
                curPTile = gridAPI.GetGoalTile();
                lerpValue = 1.0f;
            }
            else
            {
                stopwatch.Stop();
                gridAPI.SetTimerText("Failed");
            }
        }

        
        if (newPf && Input.GetMouseButton(0))
        {
            if(curPTile.pfParent != null && curPTile.pfParent != gridAPI.GetStartTile())
            {
                if(TValueToChoose == 0)
                {
                    lerpValue -= 1.0f / gridAPI.GetGoalTile().GetTileDepth();
                    curPTile.pfParent.SetSpriteColor(Color.Lerp(gridAPI.gridStartElementColour,
                        gridAPI.gridGoalElementColour,
                        lerpValue));
                }
                else if (TValueToChoose == 1)
                {
                    lerpValue -= 1.0f / gridAPI.GetGoalTile().GetSpfValue();

                    curPTile.pfParent.SetSpriteColor(Color.Lerp(gridAPI.gridStartElementColour,
                        gridAPI.gridGoalElementColour,
                        lerpValue));
                }
                else if (TValueToChoose == 2)
                {
                    lerpValue -= 1.0f / gridAPI.GetGoalTile().GetAStarFV();

                    curPTile.pfParent.SetSpriteColor(Color.Lerp(gridAPI.gridStartElementColour,
                        gridAPI.gridGoalElementColour,
                        lerpValue));
                }
                curPTile = curPTile.pfParent;
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
                        inRangeTiles[i].pfParent = dequeTile;
                        nextLookAtTile.Enqueue(inRangeTiles[i]);
                        if(inRangeTiles[i] == gridAPI.GetGoalTile())
                        {
                            break;
                        }
                        //Debug.Log(inRangeTiles[i].x + ", " + inRangeTiles[i].y);
                    }
                }
            }
            else
            {
                break;
            }
        }

        if(gridAPI.GetGoalTile().GetTileDepth() == -1)
        {
            gridAPI.GetGoalTile().SetTileDepth(-(gridAPI.CurNumTiles() - numTilesVisited));
        }

        return true;
    }

    private bool BFS()
    {
        Queue<Tile> nextLookAtTile = new Queue<Tile>();

        gridAPI.GetStartTile().SetTileDepth(0);
        nextLookAtTile.Enqueue(gridAPI.GetStartTile());
        int numTilesVisited = 1;

        Tile dequeTile;

        while (nextLookAtTile.Count > 0)
        {
            dequeTile = nextLookAtTile.Dequeue();

            if (dequeTile != gridAPI.GetGoalTile())
            {
                List<Tile> inRangeTiles = GetInRangeTiles(dequeTile, 0);
                //Debug.Log(inRangeTiles.Count);
                for (int i = 0; i < inRangeTiles.Count; i++)
                {
                    if (inRangeTiles[i].GetTileDepth() == -1)
                    {
                        inRangeTiles[i].SetTileDepth(dequeTile.GetTileDepth() + 1);
                        numTilesVisited++;
                        inRangeTiles[i].pfParent = dequeTile;
                        nextLookAtTile.Enqueue(inRangeTiles[i]);
                        if (inRangeTiles[i] == gridAPI.GetGoalTile())
                        {
                            break;
                        }
                        //Debug.Log(inRangeTiles[i].x + ", " + inRangeTiles[i].y);
                    }
                }
            }
            else
            {
                break;
            }
        }

        if (gridAPI.GetGoalTile().GetTileDepth() == -1)
        {
            gridAPI.GetGoalTile().SetTileDepth(-(gridAPI.CurNumTiles() - numTilesVisited));
        }
        //Debug.Log(numTilesVisited);
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
        bool foundGoal = false;

        while(unVisitedKnownTiles.Count > 0 && !foundGoal)
        {
            int curTileIndex = GetShortestTileIndex(unVisitedKnownTiles);
            curTile = unVisitedKnownTiles[curTileIndex];
            curTile.visited = 1;
            numTilesVisited++;
            //Debug.Log(curTile.y + ", " + curTile.x);
            if (curTile != gridAPI.GetGoalTile())
            {
                List<Tile> tilesInRange = GetInRangeTiles(curTile, 1);

                for (int i = 0; i < tilesInRange.Count; i++)
                {
                    if (tilesInRange[i].GetSpfValue() == -1 || tilesInRange[i].GetSpfValue() > curTile.GetSpfValue() + 1)
                    {
                        if (!unVisitedKnownTiles.Contains(tilesInRange[i]))
                        {
                            unVisitedKnownTiles.Add(tilesInRange[i]);
                        }
                        tilesInRange[i].SetSpfValue(curTile.GetSpfValue() + 1);
                        tilesInRange[i].pfParent = curTile;
                        if(tilesInRange[i] == gridAPI.GetGoalTile())
                        {
                            foundGoal = true;
                            break;
                        }
                    }
                }
                if (!foundGoal)
                {
                    unVisitedKnownTiles.RemoveAt(curTileIndex);
                }
            }
            else
            {
                break;
            }
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
        int numTilesVisited = 1;

        gridAPI.GetStartTile().SetAStarValue(0, GetAStarManHeuWMoves(gridAPI.GetStartTile()));

        int lastIndex = 0;

        Tile curTile;
        bool goalFound = false;

        while(unVisitedKnownTiles.Count > 0 && !goalFound)
        {
            int curIndex = GetBestAStarManTile(unVisitedKnownTiles);
            if(curIndex >= 0)
            {
                curTile = unVisitedKnownTiles[curIndex];
                if (curTile != gridAPI.GetGoalTile())
                {
                    curTile.visited = 1;
                    numTilesVisited++;
                    //curTile.pfParent = prevBTile;

                    List<Tile> tilesInRange = GetInRangeTiles(curTile, 2);

                    //Debug.Log(curTile.GetNumMoves() + ", " + tilesInRange.Count + "\t" + curTile.y + ", " + curTile.x);

                    for (int i = 0; i < tilesInRange.Count; i++)
                    {
                        if (tilesInRange[i].visited == 0 && (tilesInRange[i].GetAStarFV() == -1 || tilesInRange[i].GetAStarFV() > curTile.GetAStarFV() + 1))
                        {

                            if (tilesInRange[i].GetAStarHV() == -1)
                            {
                                tilesInRange[i].SetAStarValue(curTile.GetAStarFV() + 1, GetAStarManHeuWMoves(tilesInRange[i]));
                            }
                            else
                            {
                                //Debug.Log("Changed depth");
                                tilesInRange[i].SetAStarValue(curTile.GetAStarFV() + 1);
                            }

                            tilesInRange[i].pfParent = curTile;

                            if (tilesInRange[i] == gridAPI.GetGoalTile())
                            {
                                goalFound = true;
                                break;
                            }

                            if (tilesInRange[i].inAStarList == 0)
                            {
                                unVisitedKnownTiles.Add(tilesInRange[i]);
                                tilesInRange[i].inAStarList = 1;
                            }
                        }
                    }

                    //Debug.Log("X0X0X0X0X0X0X0X0X0X0X0X0X0X0X0X0X0X0X0X0");

                    if (!goalFound)
                    {
                        unVisitedKnownTiles.RemoveAt(curIndex);
                    }
                }
                else
                {
                    break;
                }
            }
            else if(curIndex == -2)
            {
                break;
            }
            lastIndex++;
        }
        //Debug.Log(numTilesVisited);
        return true;
    }

    private int GetAStarManHeuWMoves(Tile tileToSet)
    {
        int tX = tileToSet.x;
        int tY = tileToSet.y;

        int hX = gridAPI.GetGoalTile().x - tX;
        int hY = tY - gridAPI.GetGoalTile().y;

        int delX = hX - tileToSet.GetNumMoves();
        int delY = hY - tileToSet.GetNumMoves();

        //int aDelX = Mathf.Abs(delX);
        //int aDelY = Mathf.Abs(delY);

        if(delX < 0 && delY >= 0)
        {
            return delY;
        }
        else if(delY < 0 && delX >= 0)
        {
            return delX;
        }
        else
        {
           return (delX < delY) ? delX : delY;
        }
    }

    private int GetBestAStarManTile(List<Tile> listToFindFrom)
    {
        int retIndex = -1;
        bool found = false;

        if(listToFindFrom != null)
        {
            int curLowestGValue = -1;
            int curLowestFValue = -1;

            for (int i = 0; i < listToFindFrom.Count; i++)
            {
                if(listToFindFrom[i].visited == 0 && (curLowestGValue == -1 || listToFindFrom[i].GetAStarGV() < curLowestGValue))
                {
                    if(listToFindFrom[i].GetAStarGV() == curLowestGValue)
                    {
                        if(listToFindFrom[i].GetAStarFV() < curLowestFValue)
                        {
                            curLowestGValue = listToFindFrom[i].GetAStarGV();
                            curLowestFValue = listToFindFrom[i].GetAStarFV();
                            retIndex = i;
                            found = true;
                        }
                    }
                    else
                    {
                        curLowestGValue = listToFindFrom[i].GetAStarGV();
                        curLowestFValue = listToFindFrom[i].GetAStarFV();
                        retIndex = i;
                        found = true;
                    }
                    //Debug.Log("FoundA*LOWESET: " + curLowestGValue);
                }
            }
        }

        if (found)
        {
            return retIndex;
        }
        else
        {
            return -2;
        }

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

                if (listToSortFrom[i] == gridAPI.GetGoalTile())
                {
                    return i;
                }
                else if (listToSortFrom[i].visited == 0 && curTileValue < curLowestValue)
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
                else if (mode == 2 && tileToAdd.visited == 0)
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
                else if (mode == 2 && tileToAdd.visited == 0)
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
                else if (mode == 2 && tileToAdd.visited == 0)
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
                else if (mode == 2 && tileToAdd.visited == 0)
                {
                    returnTiles.Add(tileToAdd);
                }
            }
        }

        return returnTiles;
    }
}
