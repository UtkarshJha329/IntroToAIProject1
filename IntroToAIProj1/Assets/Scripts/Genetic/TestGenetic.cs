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
        public float seed;
    }

    [SerializeField] private ComputeShader c_Shader;

    private GridManagerAPI gridAPI;

    private TimeKeeper stopWatch = new TimeKeeper();

    private string cs_RandomIntKernal = "RandomLegalMoves";
    private int cs_RandIntKernalId = 0;

    private ComputeBuffer valsBuf;
    private ComputeBuffer vals2Buf;
    private const int gridSize = 16;
    private GeneticTile[] vals = new GeneticTile[gridSize * gridSize];
    private GeneticTile[] vals2 = new GeneticTile[gridSize * gridSize];

    // Start is called before the first frame update
    void Start()
    {
        gridAPI = GetComponent<GridManagerAPI>();

        int sizeOfTileData = (sizeof(int) * 3) + sizeof(float);


        if (c_Shader != null)
        {
            valsBuf = new ComputeBuffer(vals.Length, sizeOfTileData);
            valsBuf.SetData(vals);

            vals2Buf = new ComputeBuffer(vals2.Length, sizeOfTileData);
            vals2Buf.SetData(vals2);

            cs_RandIntKernalId = c_Shader.FindKernel(cs_RandomIntKernal);

            c_Shader.SetBuffer(cs_RandIntKernalId, "testInts", valsBuf);
            c_Shader.SetBuffer(cs_RandIntKernalId, "testInts2", vals2Buf);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Shader.SetGlobalInt("gridSize", gridSize);
            Shader.SetGlobalFloat("seedTime", Time.time);

            stopWatch.Start();
            c_Shader.Dispatch(cs_RandIntKernalId, 4, 4, 1);
            

            //valsBuf.SetData(vals);

            valsBuf.GetData(vals);
            vals2Buf.GetData(vals2);
            stopWatch.Stop();
            for (int i = 0; i < vals.Length; i++)
            {
                //if(vals[i].seed > 1.0f)
                //{
                //    Debug.Log(vals[i].seed);
                //}
                //Debug.Log(vals[i].numMoves + "\t" + vals2[i].numMoves/* + ": " + vals[i].x + vals[i].y + ": " + vals2[i].x + vals2[i].y*/);
                //Debug.Log(vals[i].seed + "\t" + vals2[i].seed);
                //Debug.Log("( " + vals[i].x + ", " + vals[i].y + ")\t" + "( " + vals2[i].x + ", " + vals2[i].y + ")");
            }
            //Debug.Log(stopWatch.GetLastElapsedTime());


            if (gridAPI.boardGenerated && gridAPI.GridSize() == gridSize/* && false*/)
            {
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        gridAPI.SetNumMovesForTile(i, j, vals[i * gridSize + j].numMoves);
                    }
                }
            }

        }
    }
}
