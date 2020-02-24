using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataHandler : MonoBehaviour
{
    public GameObject gridSceneObject;
    private GridManagerAPI gridAPI;

    private TimeKeeper stopwatch;
    private bool newPf = false;
    private int TValueToChoose = 0;

    private Tile curPTile;
    private float lerpValue = 1.0f;

    private int numTilesChecked = 0;
    private int totalPathNumMoves = 0;
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
                        gridAPI.SetNumTilesChecked(numTilesChecked);
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
                gridAPI.SetNumTilesChecked(numTilesChecked);
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
            stopwatch.Start();
            totalPathNumMoves = 0;
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
                gridAPI.SetNumTilesChecked(numTilesChecked);
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
                gridAPI.SetNumTilesChecked(numTilesChecked);
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

        //PART 7 Of The Project
        if (gridAPI.boardGenerated && gridAPI.doGeneticMating && gridAPI.GridSize() <= 16)
        {
            gridAPI.doGeneticMating = false;
            stopwatch.Start();
            Genetics();
            gridAPI.SetTimerText(stopwatch.Stop().ToString());
            gridAPI.ResetDepthValues();
            BFS();
        }

        //Path tile animation
        if (newPf && Input.GetMouseButton(0))
        {
            if(curPTile.pfParent != null && curPTile.pfParent != gridAPI.GetStartTile())
            {
                totalPathNumMoves += curPTile.pfParent.GetNumMoves();
                //Debug.Log(curPTile.pfParent.GetNumMoves());
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
            else
            {
                totalPathNumMoves += gridAPI.GetStartTile().GetNumMoves();
                //Debug.Log(totalPathNumMoves);
                totalPathNumMoves = 0;
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

    private int RandReductionTileMove(int x, int y, int randomSide)
    {
        //Random.InitState(oldSeed + 100);
        //oldSeed += 100;
        //Debug.Log("element: " + (i * gridSize + j));

        int gridSize = gridAPI.GridSize();
        
        int maxX = (x <= gridSize / 2) ? gridSize - x : gridSize - x / 2;
        int minX = 1;
        int maxY = (y <= gridSize / 2) ? gridSize - y : gridSize - y / 2;
        int minY = 1;

        int dividePoint = 3;


        if((x != 0 && y != 0) && (x != gridSize - 1 && y != 1))
        {
            if (randomSide == 0)
            {
                if (y >= gridSize / dividePoint)
                {
                    maxY = Mathf.Min(5, gridSize / dividePoint);
                    minY = 1;
                }
                else
                {
                    maxY = (y <= gridSize / 2) ? gridSize - y : gridSize - y / 2;
                    minY = 1;
                }
                maxX = (x <= gridSize / 2) ? gridSize - x : gridSize - x / 2;
                minX = 1;
            }
            else
            {
                if (x <= (dividePoint - 1) * gridSize / dividePoint)
                {
                    maxX = Mathf.Min(5, gridSize / dividePoint);
                    minX = 1;
                }
                else
                {
                    maxX = (x <= gridSize / 2) ? gridSize - x : gridSize - x / 2;
                    minX = 1;
                }

                maxY = (y <= gridSize / 2) ? gridSize - y : gridSize - y / 2;
                minY = 1;
            }
        }
        else
        {
            if(x == 0 && y == 0)
            {
                minX = 1;
                maxX = gridSize - 1;

                minY = 1;
                maxY = gridSize - 1;
            }
            else
            {
                minX = 2;
                maxX = gridSize;

                minY = 2;
                maxY = gridSize;
            }
        }
        
        //Random function's second parameter is exclusive
        int randomX = Random.Range(minX, maxX);
        int randomY = Random.Range(minY, maxY);

        int random = Random.Range(0, 2);
        if (random == 0)
        {
            return randomX;
        }
        else
        {
            return randomY;
        }
    }

    private bool CalculateDepthForEachTile()
    {
        gridAPI.ResetDepthValues();
        Queue<Tile> nextLookAtTile = new Queue<Tile>();

        gridAPI.GetStartTile().SetTileDepth(0);
        nextLookAtTile.Enqueue(gridAPI.GetStartTile());
        numTilesChecked = 1;
        
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
                        numTilesChecked++;
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
            gridAPI.GetGoalTile().SetTileDepth(-(gridAPI.CurNumTiles() - numTilesChecked));
        }

        return true;
    }

    private bool BFS()
    {
        Queue<Tile> nextLookAtTile = new Queue<Tile>();

        gridAPI.GetStartTile().SetTileDepth(0);
        nextLookAtTile.Enqueue(gridAPI.GetStartTile());
        numTilesChecked = 1;

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
                        numTilesChecked++;
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
            gridAPI.GetGoalTile().SetTileDepth(-(gridAPI.CurNumTiles() - numTilesChecked));
        }
        //Debug.Log(numTilesVisited);
        return true;
    }

    private bool HillClimb(int numItterations)
    {
        int oldMoves;
        int oldGoalDepth;

        int smallSide = Random.Range(0, 2);
        int avg = gridAPI.GridSize() / 2;
        //Debug.Log(smallSide);
        for (int i = 0; i < numItterations; i++)
        {
            int randX = Random.Range(0, gridAPI.GridSize());
            int randY = (randX == 0) ? Random.Range(0, gridAPI.GridSize() - 1) : Random.Range(0, gridAPI.GridSize());

            oldMoves = gridAPI.GetTileByCoord(randX, randY).GetNumMoves();
            oldGoalDepth = gridAPI.GetGoalTile().GetTileDepth();

            gridAPI.GetTileByCoord(randX, randY).SetNumMoves(RandReductionTileMove(randX, randY, 0));
            if (CalculateDepthForEachTile())
            {
                if (gridAPI.GetGoalTile().GetTileDepth() < oldGoalDepth || EvaluateBySumAverage(smallSide, 3) >= avg + 1)
                {
                    gridAPI.GetTileByCoord(randX, randY).SetNumMoves(oldMoves);
                    numTilesChecked = 0;
                    CalculateDepthForEachTile();
                    gridAPI.GetGoalTile().SetTileDepth(oldGoalDepth);
                }
            }
        }

        return true;
    }

    private bool ShortestPathFirst()
    {
        List<Tile> unVisitedKnownTiles = new List<Tile>();
        unVisitedKnownTiles.Add(gridAPI.GetStartTile());

        gridAPI.GetStartTile().SetSpfValue(0);
        numTilesChecked = 1;

        Tile curTile;
        bool foundGoal = false;

        while(unVisitedKnownTiles.Count > 0 && !foundGoal)
        {
            int curTileIndex = GetShortestTileIndex(unVisitedKnownTiles);
            curTile = unVisitedKnownTiles[curTileIndex];
            curTile.visited = 1;
            //Debug.Log(curTile.y + ", " + curTile.x);
            if (curTile != gridAPI.GetGoalTile())
            {
                List<Tile> tilesInRange = GetInRangeTiles(curTile, 1);

                for (int i = 0; i < tilesInRange.Count; i++)
                {
                    if (tilesInRange[i].GetSpfValue() == -1 || tilesInRange[i].GetSpfValue() > curTile.GetSpfValue() + 1)
                    {
                        numTilesChecked++;
                        if (tilesInRange[i].inAStarList == 0)
                        {
                            unVisitedKnownTiles.Add(tilesInRange[i]);
                            tilesInRange[i].inAStarList = 1;
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
            gridAPI.GetGoalTile().SetTileDepth(-(gridAPI.CurNumTiles() - numTilesChecked));
        }

        return true;
    }

    private bool AStar()
    {
        List<Tile> unVisitedKnownTiles = new List<Tile>();
        unVisitedKnownTiles.Add(gridAPI.GetStartTile());
        numTilesChecked = 1;

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
                    //curTile.pfParent = prevBTile;

                    List<Tile> tilesInRange = GetInRangeTiles(curTile, 2);

                    //Debug.Log(curTile.GetNumMoves() + ", " + tilesInRange.Count + "\t" + curTile.y + ", " + curTile.x);

                    for (int i = 0; i < tilesInRange.Count; i++)
                    {
                        if (tilesInRange[i].visited == 0 && (tilesInRange[i].GetAStarFV() == -1 || tilesInRange[i].GetAStarFV() > curTile.GetAStarFV() + 1))
                        {
                            numTilesChecked++;
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

        if (gridAPI.GetGoalTile().GetAStarFV() == -1)
        {
            gridAPI.GetGoalTile().SetAStarValue(-(gridAPI.CurNumTiles() - numTilesChecked));
        }
        //Debug.Log(numTilesVisited);
        return true;
    }

    private void Genetics()
    {
        int gridSize = gridAPI.GridSize();

        int numCopies = 3;
        GenTile[,] genTiles = GeneticCopy(numCopies);

        float avg = gridAPI.GridSize() / 2;
        int divide = 3;
        int divideSide = 0;

        int[] oldGoalDepths = new int[numCopies];
        int[] oldMoves = new int[numCopies];

        int[] fitness = new int[numCopies];

        int numIterationsH = 100;
        int numIterationsM = 10;

        int randX;
        int randY;

        int gensFittest = -1;

        for (int mateIter = 0; mateIter < numIterationsM; mateIter++)
        {
            for (int iter = 0; iter < numIterationsH; iter++)
            {
                randX = Random.Range(0, gridAPI.GridSize());
                randY = (randX == 0) ? Random.Range(0, gridAPI.GridSize() - 1) : Random.Range(0, gridAPI.GridSize());

                int pos = randX * gridSize + randY;
                for (int i = 0; i < numCopies; i++)
                {
                    oldMoves[i] = genTiles[pos, i].numMoves;
                    //Debug.Log(i + ": " + oldMoves[i]);
                    //Store depth before changing
                    oldGoalDepths[i] = genTiles[gridSize - 1, i].tileDepth;//GOAL TILE DEPTH

                    //Change and calculate depth
                    genTiles[pos, i].numMoves = RandReductionTileMove(randX, randY, divideSide);
                    GenResetDepth(ref genTiles, i);
                    GenBFS(ref genTiles, i);

                    float sumEval = GenEvaluateBySumAverage(ref genTiles, i, divideSide, divide);

                    if (genTiles[gridSize - 1, i].tileDepth < oldGoalDepths[i] || sumEval >= avg + 1)
                    {
                        genTiles[pos, i].numMoves = oldMoves[i];
                        GenResetDepth(ref genTiles, i);
                        GenBFS(ref genTiles, i);
                        //Debug.Log("Failed");
                    }
                    else
                    {
                        //Debug.Log("Worked " + i + ": " + genTiles[gridSize - 1, i].tileDepth + ", " + oldGoalDepths[i]);
                        fitness[i] = genTiles[gridSize - 1, i].tileDepth + gridSize - (int)sumEval;
                    }
                }
            }

            gensFittest = MaxFitnessIndex(fitness);

            if(gensFittest == 0)
            {
                if (fitness[1] > fitness[2])
                {
                    MateGenTiles(ref genTiles, 0, 1, divideSide, divide, 1);
                    MateGenTiles(ref genTiles, 0, 2, divideSide, divide, 0);
                }
                else
                {
                    MateGenTiles(ref genTiles, 0, 2, divideSide, divide, 1);
                    MateGenTiles(ref genTiles, 0, 1, divideSide, divide, 0);
                }
            }
            else if(gensFittest == 1)
            {
                if (fitness[2] > fitness[0])
                {
                    MateGenTiles(ref genTiles, 1, 2, divideSide, divide, 1);
                    MateGenTiles(ref genTiles, 1, 0, divideSide, divide, 0);
                }
                else
                {
                    MateGenTiles(ref genTiles, 1, 0, divideSide, divide, 1);
                    MateGenTiles(ref genTiles, 1, 2, divideSide, divide, 0);
                }

            }
            else
            {
                if (fitness[0] > fitness[1])
                {
                    MateGenTiles(ref genTiles, 2, 0, divideSide, divide, 1);
                    MateGenTiles(ref genTiles, 2, 1, divideSide, divide, 0);
                }
                else
                {
                    MateGenTiles(ref genTiles, 2, 1, divideSide, divide, 1);
                    MateGenTiles(ref genTiles, 2, 0, divideSide, divide, 0);
                }
            }

            for (int reCals = 0; reCals < numCopies; reCals++)
            {
                GenResetDepth(ref genTiles, reCals);
                GenBFS(ref genTiles, reCals);
            }
        }


        int mostFit = MaxFitnessIndex(fitness);

        int cPos = -1;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                cPos = i * gridSize + j;
                gridAPI.GetTileByCoord(i, j).SetNumMoves(genTiles[cPos, mostFit].numMoves);
                gridAPI.GetTileByCoord(i, j).SetTileDepth(genTiles[cPos, mostFit].tileDepth);
            }
        }

        //for (int i = 0; i < numCopies; i++)
        //{
        //    Debug.Log(fitness[i]);
        //    Debug.Log("GOAL: " + oldGoalDepths[i]);
        //}

        //Debug.Log(MaxFitnessIndex(fitness));

    }

    private int MaxFitnessIndex(int[] fitness)
    {
        int a = Mathf.Max(fitness[0], fitness[1], fitness[2]);

        if(a == fitness[0])
        {
            return 0;
        }else if(a == fitness[1])
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    private GenTile[,] GeneticCopy(int numCopies)
    {
        CalculateDepthForEachTile();
        int totalNumEle = (int)Mathf.Pow(gridAPI.GridSize(), 2);
        GenTile[,] strands = new GenTile[totalNumEle,numCopies];

        Tile tempTile;
        for (int i = 0; i < totalNumEle; i++)
        {
            tempTile = gridAPI.ListEleByIndex(i);

            for (int j = 0; j < numCopies; j++)
            {
                strands[i, j] = new GenTile();
                strands[i, j].numMoves = tempTile.GetNumMoves();
                strands[i, j].tileDepth = tempTile.GetTileDepth();
                strands[i, j].x = tempTile.x;
                strands[i, j].y = tempTile.y;
                strands[i, j].index = i;
            }
        }

        return strands;
    }

    private void GenBFS(ref GenTile[,] genTiles, int cloneNum)
    {
        Queue<GenTile> nextLookAtTile = new Queue<GenTile>();

        int gridSize = gridAPI.GridSize();

        genTiles[gridSize * (gridSize - 1), cloneNum].tileDepth = 0;
        nextLookAtTile.Enqueue(genTiles[gridSize * (gridSize - 1), cloneNum]);
        numTilesChecked = 1;

        GenTile dequeTile;

        while (nextLookAtTile.Count > 0)
        {
            dequeTile = nextLookAtTile.Dequeue();

            if (dequeTile != gridAPI.GetGoalTile())
            {
                List<GenTile> inRangeTiles = GetInRangeTiles(ref genTiles, dequeTile.index, cloneNum);
                //Debug.Log(inRangeTiles.Count);
                for (int i = 0; i < inRangeTiles.Count; i++)
                {
                    if (inRangeTiles[i].tileDepth == -1)
                    {
                        inRangeTiles[i].tileDepth = dequeTile.tileDepth + 1;
                        numTilesChecked++;
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

        if (genTiles[gridSize - 1, cloneNum].tileDepth == -1)
        {
            genTiles[gridSize - 1, cloneNum].tileDepth = -(gridAPI.CurNumTiles() - numTilesChecked);
        }
        //Debug.Log(numTilesVisited);
    }

    private void GenResetDepth(ref GenTile[,] genTiles, int cloneNum)
    {
        int numTiles = gridAPI.CurNumTiles();
        for (int i = 0; i < numTiles; i++)
        {
            genTiles[i, cloneNum].tileDepth = -1;
        }
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

        if (listToFindFrom != null)
        {
            int curLowestGValue = -1;
            int curLowestFValue = -1;

            for (int i = 0; i < listToFindFrom.Count; i++)
            {
                if (listToFindFrom[i].visited == 0 && (curLowestGValue == -1 || listToFindFrom[i].GetAStarGV() < curLowestGValue))
                {
                    if (listToFindFrom[i].GetAStarGV() == curLowestGValue)
                    {
                        if (listToFindFrom[i].GetAStarFV() < curLowestFValue)
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
                }
            }
            if (found)
            {
                //Debug.Log("FoundA*LOWESET: " + curLowestGValue);
                return retIndex;
            }
            else
            {
                return -2;
            }
        }
        else
        {
            return -1;
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

    private List<GenTile> GetInRangeTiles(ref GenTile[,] tile, int index, int cloneNum)
    {
        List<GenTile> returnTiles = new List<GenTile>();
        int numMoves = tile[index, cloneNum].numMoves;

        int gridSize = gridAPI.GridSize();

        GenTile tileToAdd;
        if (numMoves != 0)
        {
            if (tile[index, cloneNum].y - numMoves >= 0)
            {
                tileToAdd = tile[(tile[index, cloneNum].y - numMoves) * gridSize + tile[index, cloneNum].x, cloneNum];
                returnTiles.Add(tileToAdd);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Down: " + tileToAdd.GetTileDepth());
            }
            if (tile[index, cloneNum].y + numMoves < gridSize)
            {
                tileToAdd = tile[(tile[index, cloneNum].y + numMoves) * gridSize + tile[index, cloneNum].x, cloneNum];
                returnTiles.Add(tileToAdd);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Up: " + tileToAdd.GetTileDepth());
            }

            if (tile[index, cloneNum].x - numMoves >= 0)
            {
                tileToAdd = tile[tile[index, cloneNum].y * gridSize + tile[index, cloneNum].x - numMoves, cloneNum];
                returnTiles.Add(tileToAdd);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth left: " + tileToAdd.GetTileDepth());
            }
            if (tile[index, cloneNum].x + numMoves < gridSize)
            {
                tileToAdd = tile[tile[index, cloneNum].y * gridSize + tile[index, cloneNum].x + numMoves, cloneNum];
                returnTiles.Add(tileToAdd);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Right: " + tileToAdd.GetTileDepth());
            }
        }

        return returnTiles;
    }

    private float EvaluateBySumAverage(int divideSide, int divide)
    {
        int gridSize = gridAPI.GridSize();
        int i = (divideSide == 0) ? 0 : gridSize / divide;
        int j = (divideSide == 1) ? 0 : gridSize / divide;

        float sum = 0;
        for (; i < gridSize; i++)
        {
            for (; j < gridSize; j++)
            {
                sum += gridAPI.GetTileByCoord(i, j).GetNumMoves();
            }
        }

        sum /= (gridAPI.GridSize() - 1);

        return sum;
    }

    private float GenEvaluateBySumAverage(ref GenTile[,] genTiles, int cloneNum, int divideSide, int divide)
    {
        int gridSize = gridAPI.GridSize();
        int i = (divideSide == 0) ? gridSize / divide : 0;
        int j = 0;

        int iMax = gridSize;
        int jMax = (divideSide == 1) ? (divide - 1) * gridSize / divide : gridSize;

        float sum = 0;
        for (; i < iMax; i++)
        {
            for (; j < jMax; j++)
            {
                sum += genTiles[i * gridSize + j, cloneNum].numMoves;
            }
        }

        sum /= (gridAPI.GridSize() - 1);

        return sum;
    }

    private void MateGenTiles(ref GenTile[,] genTiles, int cloneNumA, int cloneNumB, int divideSide, int divide, int greaterSide)
    {
        int gridSize = gridAPI.GridSize();

        if(greaterSide == 1)
        {
            int i = (divideSide == 0) ? gridSize / divide : 0;
            int j = 0;

            int iMax = gridSize;
            int jMax = (divideSide == 1) ? (divide - 1) * gridSize / divide : gridSize;

            int pos = 0;

            for (; i < iMax; i++)
            {
                for (; j < jMax; j++)
                {
                    pos = i * gridSize + j;
                    genTiles[pos, cloneNumB].numMoves = genTiles[pos, cloneNumA].numMoves;
                }
            }
        }
        else
        {
            int i = 0;
            int j = (divideSide == 0) ? 0 : (divide - 1) * gridSize / divide;

            int iMax = (divideSide == 0) ? gridSize / divide : gridSize;
            int jMax = gridSize;

            int pos = 0;

            for (; i < iMax; i++)
            {
                for (; j < jMax; j++)
                {
                    pos = i * gridSize + j;
                    genTiles[pos, cloneNumB].numMoves = genTiles[pos, cloneNumA].numMoves;
                }
            }
        }
    }
}
