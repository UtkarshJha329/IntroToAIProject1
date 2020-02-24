using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenetic : MonoBehaviour
{
    private struct GeneticTile
    {
        public int x;
        public int y;
        public int numMoves;
        public int oldNumMoves;
        public int depthBFS;
        public int debugVal;
        public int flagged;
    }

    [SerializeField] private ComputeShader c_Shader;

    private GridManagerAPI gridAPI;

    private TimeKeeper stopWatch = new TimeKeeper();

    private string cs_RandomIntKernal = "RandomLegalMoves";
    private string cs_SwapBackToOld = "SwapBackToOld";
    private string cs_ReWriteOldNumMoves = "ReWriteOldNumMoves";
    private int cs_RandIntKernalId = 0;
    private int cs_SwapBackToOldId = 0;
    private int cs_ReWriteOldNumMovesId = 0;

    private ComputeBuffer vals0Buf;
    private ComputeBuffer vals1Buf;
    private ComputeBuffer vals2Buf;
    private const int gridSize = 16;
    private GeneticTile[] vals0 = new GeneticTile[gridSize * gridSize];
    private GeneticTile[] vals1 = new GeneticTile[gridSize * gridSize];
    private GeneticTile[] vals2 = new GeneticTile[gridSize * gridSize];

    private float divide = 3.0f;

    private int oldDepth = 0;
    private int numTilesChecked = 0;

    private bool boardCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        //gridAPI = GetComponent<GridManagerAPI>();

        //int sizeOfTileData = (sizeof(int) * 7);

        //if (c_Shader != null)
        //{
        //    vals0Buf = new ComputeBuffer(vals0.Length, sizeOfTileData);
        //    vals0Buf.SetData(vals0);

        //    vals1Buf = new ComputeBuffer(vals1.Length, sizeOfTileData);
        //    vals1Buf.SetData(vals1);

        //    vals2Buf = new ComputeBuffer(vals2.Length, sizeOfTileData);
        //    vals2Buf.SetData(vals2);

        //    cs_RandIntKernalId = c_Shader.FindKernel(cs_RandomIntKernal);
        //    cs_SwapBackToOldId = c_Shader.FindKernel(cs_SwapBackToOld);
        //    cs_ReWriteOldNumMovesId = c_Shader.FindKernel(cs_ReWriteOldNumMoves);

        //    c_Shader.SetBuffer(cs_RandIntKernalId, "testIntsA", vals0Buf);
        //    c_Shader.SetBuffer(cs_RandIntKernalId, "testIntsB", vals1Buf);
        //    c_Shader.SetBuffer(cs_RandIntKernalId, "testIntsC", vals2Buf);
            
        //    c_Shader.SetBuffer(cs_SwapBackToOldId, "testIntsA", vals0Buf);
        //    c_Shader.SetBuffer(cs_SwapBackToOldId, "testIntsB", vals1Buf);
        //    c_Shader.SetBuffer(cs_SwapBackToOldId, "testIntsC", vals2Buf);

        //    c_Shader.SetBuffer(cs_ReWriteOldNumMovesId, "testIntsA", vals0Buf);
        //    c_Shader.SetBuffer(cs_ReWriteOldNumMovesId, "testIntsB", vals1Buf);
        //    c_Shader.SetBuffer(cs_ReWriteOldNumMovesId, "testIntsC", vals2Buf);

        //}
    }

    // Update is called once per frame
    void Update()
    {
        //bool mRightclick = Input.GetMouseButtonDown(1);
        //bool mMiddielClick = Input.GetMouseButtonDown(2);
        //if (mRightclick || mMiddielClick)
        //{
        //    Shader.SetGlobalInt("gridSize", gridSize);
        //    Shader.SetGlobalFloat("seedTime", Time.time);

        //    Shader.SetGlobalFloat("divide", 3);
        //    Shader.SetGlobalFloat("divideSide", 0);
        //    Shader.SetGlobalFloat("greaterDivide", (divide - 1) * gridSize / divide);

        //    //Debug.Log((divide - 1) * gridSize / divide);

        //    stopWatch.Start();
        //    if (mRightclick)
        //    {
        //        c_Shader.Dispatch(cs_RandIntKernalId, 1, 1, 1);
        //        boardCreated = true;
        //        {
        //            //for (int i = 0; i < vals0.Length; i++)
        //            //{
        //            //    //if(vals[i].seed > 1.0f)
        //            //    //{
        //            //    //    Debug.Log(vals[i].seed);
        //            //    //}
        //            //    //Debug.Log(vals0[i].numMoves + "\t" + vals1[i].numMoves + "\t" + vals2[i].numMoves/* + ": " + vals[i].x + vals[i].y + ": " + vals2[i].x + vals2[i].y*/);
        //            //    //Debug.Log(vals0[i].oldNumMoves + "\t" + vals1[i].oldNumMoves + "\t" + vals2[i].oldNumMoves/* + ": " + vals[i].x + vals[i].y + ": " + vals2[i].x + vals2[i].y*/);
        //            //    //Debug.Log(vals0[i].debugVal + "\t" + vals1[i].debugVal + "\t" + vals2[i].debugVal/* + ": " + vals[i].x + vals[i].y + ": " + vals2[i].x + vals2[i].y*/);
        //            //    //Debug.Log(vals0[i].seed + "\t" + vals1[i].seed + "\t" + vals2[i].seed);
        //            //    //Debug.Log("( " + vals0[i].x + ", " + vals0[i].y + ")\t" + "( " + vals1[i].x + ", " + vals1[i].y + ")");
        //            //}
        //            //Debug.Log(stopWatch.GetLastElapsedTime());
        //        }
        //        FetchBufferValues();

        //        SetTileValuesBFS();
        //        oldDepth = gridAPI.GetGoalTile().GetTileDepth();
        //        stopWatch.Stop();
        //    }
        //    else if(mMiddielClick && boardCreated)
        //    {
        //        HillClimb(1000);
        //        Debug.Log("Done");
        //        SetTileValuesBFS();
        //        oldDepth = gridAPI.GetGoalTile().GetTileDepth();
        //    }
        //}
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

            if (RandHillClimbTileMove(randX, randY, 0))
            {
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

        }

        return true;
    }

    private void RandomTileSRandomNum(int num)
    {
        int randX;
        int randY;

        int lRandX = 0;
        int lRandY = 0;

        int maxX = gridSize;
        int minX = 1;
        int maxY = gridSize;
        int minY = 1;

        int randomSide = 0;

        int randomY = 1;
        int randomX = 1;

        for (int i = 0; i < num; i++)
        {
            randX = ExRandomRange(0, lRandX, gridSize);
            randY = (randX == 0) ? ExRandomRange(0, lRandY, gridSize - 1) : ExRandomRange(0, lRandX, gridSize);

            vals0[randX * gridSize + randY].flagged = 1;

            lRandX = randX;
            lRandY = randY;

            maxX = (randX <= gridSize / 2) ? gridSize - randX : gridSize - randX / 2;
            minX = 1;
            maxY = (randY <= gridSize / 2) ? gridSize - randY : gridSize - randY / 2;
            minY = 1;

            gridAPI.GetTileByCoord(randX, randY).oldNumMovesG = gridAPI.GetTileByCoord(randX, randY).GetNumMoves();

            if ((randX != 0 && randY != 0) && (randX != gridSize - 1 && randY != 1))
            {
                if (randomSide == 0)
                {
                    if (randY >= gridSize / divide)
                    {
                        maxY = Mathf.Min(4, gridSize / (int)divide);
                        minY = 1;
                    }
                    else
                    {
                        maxY = (randY <= gridSize / 2) ? gridSize - randY : gridSize - randY / 2;
                        minY = 1;
                    }

                    randomY = Random.Range(minY, maxY);
                    gridAPI.GetTileByCoord(randX, randY).SetNumMoves(randomY);
                }
                else
                {
                    if (randX <= (divide - 1) * gridSize / divide)
                    {
                        maxX = Mathf.Min(4, gridSize / (int)divide);
                        minX = 1;
                    }
                    else
                    {
                        maxX = (randX <= gridSize / 2) ? gridSize - randX : gridSize - randX / 2;
                        minX = 1;
                    }

                    randomX = Random.Range(minX, maxX);
                    gridAPI.GetTileByCoord(randX, randY).SetNumMoves(randomX);
                }
            }
            else
            {
                if (randX == 0 && randY == 0)
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
                
                
                int random = Random.Range(0, 2);

                randomY = Random.Range(minY, maxY);
                randomX = Random.Range(minX, maxX);

                if (random == 0)
                {
                    gridAPI.GetTileByCoord(randX, randY).SetNumMoves(randomX);
                }
                else
                {
                    gridAPI.GetTileByCoord(randX, randY).SetNumMoves(randomY);
                }
            }
        }
    }

    private void SwapToOld()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                gridAPI.GetTileByCoord(i, j).SetToOldNumMoves();
            }
        }
    }

    private void SetOldValues()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                gridAPI.GetTileByCoord(i, j).oldNumMovesG = gridAPI.GetTileByCoord(i, j).GetNumMoves();
            }
        }
    }

    private bool SetTileValuesBFS()
    {
        if (gridAPI.boardGenerated && gridAPI.GridSize() == gridSize/* && false*/)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    gridAPI.GetTileByCoord(i, j).SetNumMoves(vals0[i * gridSize + j].numMoves);
                }
            }
        }

        gridAPI.GetGoalTile().SetNumMoves(0);


        return CalculateDepthForEachTile();
    }

    private void FetchBufferValues()
    {
        vals0Buf.GetData(vals0);
        vals1Buf.GetData(vals1);
        vals2Buf.GetData(vals2);
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

        return true;
    }

    private List<Tile> GetInRangeTiles(Tile tile, int mode)
    {
        List<Tile> returnTiles = new List<Tile>();
        int numMoves = tile.GetNumMoves();

        Tile tileToAdd;
        if (numMoves != 0)
        {
            if (tile.y - numMoves >= 0)
            {
                tileToAdd = gridAPI.GetTileByCoord(tile.y - numMoves, tile.x);
                //Debug.Log("TileValue: " + tileToAdd.GetNumMoves() + "TileDepth Down: " + tileToAdd.GetTileDepth());
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

    private int ExRandomRange(int min, int b, int max)
    {
        int mi = -1;
        int ma = -1;

        if(min != b)
        {
            mi = Random.Range(min, b);
        }
        else if(max != b)
        {
            ma = Random.Range(b + 1, max);
        }

        if(Random.Range(0, 2) == 0)
        {
            if(mi != -1)
            {
                return mi;
            }
            else
            {
                return ma;
            }
        }
        else
        {
            if (ma != -1)
            {
                return ma;
            }
            else
            {
                return mi;
            }
        }
    }

    private bool RandHillClimbTileMove(int x, int y, int randomSide)
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


        if ((x != 0 && y != 0) && (x != gridSize - 1 && y != 1))
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
            if (x == 0 && y == 0)
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
            gridAPI.SetNumMovesForTile(x, y, randomX);
        }
        else
        {
            gridAPI.SetNumMovesForTile(x, y, randomY);
        }

        return true;
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
}
