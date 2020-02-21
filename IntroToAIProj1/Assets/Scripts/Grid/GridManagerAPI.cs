using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManagerAPI : MonoBehaviour
{
    //The variable that will store the main camera that is currently being used in the scene.
    private Camera mainCamera;

    //GridSize (As the grid is always a square this will define how big one side is)
    private int gridSize = 5;
    //Returns GridSize
    public int GridSize()
    {
        return gridSize;
    }

    //Adds padding in between each tile
    //The SerilizeField tag shows the variable beside it in the inspector
    [SerializeField] private float gridElePaddingX = 0.2f;
    [SerializeField] private float gridElePaddingY = 0.2f;

    //The Instantiated Elements of a Grid (Instantiate creates the element in the scene.)
    [SerializeField] private GameObject gridElement;
    [SerializeField] private Transform gridParent;

    [SerializeField] private RectTransform canvasTransform;
    private ValueSettingHandler valueSettingHandler;

    //Lists will hold references to each individual tile
    private static List<GameObject> tiles = new List<GameObject>();
    //private static List<GameObject> tileMovesUIList = new List<GameObject>();
    //private static List<GameObject> tileDepthUIList = new List<GameObject>();
    public static List<Tile> tileDataList = new List<Tile>();

    private Vector3 instantiatePosition = Vector3.zero;

    [HideInInspector] public bool generateBoard = false;
    [HideInInspector] public bool boardGenerated = false;
    [HideInInspector] public bool doBFS = false;
    [HideInInspector] public bool doHillClimb = false;
    [HideInInspector] public bool doSPF = false;
    [HideInInspector] public bool doAStar = false;
    [HideInInspector] public int hilClimbNumIter = 100;

    public Color gridElementColour;
    public Color gridStartElementColour;
    public Color gridGoalElementColour;

    private Vector2Int startTileCoord = new Vector2Int(0, 0);
    private Vector2Int goalTileCoords = new Vector2Int(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        valueSettingHandler = canvasTransform.GetComponent<ValueSettingHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (valueSettingHandler.generateGrid)
        {
            valueSettingHandler.generateGrid = false;
            CreateGrid_WithUI_InScene();
        }

        if (valueSettingHandler.doBFS)
        {
            doBFS = true;
            valueSettingHandler.doBFS = false;
        }

        if (valueSettingHandler.doHillClimb)
        {
            doHillClimb = true;
            valueSettingHandler.doHillClimb = false;
        }
        hilClimbNumIter = valueSettingHandler.HillClmbItterVal();

        if (valueSettingHandler.doSPF)
        {
            doSPF = true;
            valueSettingHandler.doSPF = false;
        }

        if (valueSettingHandler.doAStar)
        {
            doAStar = true;
            valueSettingHandler.doAStar = false;
        }

    }

    public void CreateGrid_WithUI_InScene()
    {
        DeactivateAllObjects();

        //Get a reference to the Main Camera in the scene
        mainCamera = Camera.main;

        //Set gridSize
        gridSize = valueSettingHandler.NValue();
        //Instantiate Start Position
        instantiatePosition = new Vector3(-gridSize / 2, -gridSize / 2, 5);
        Vector3 middlePos = Vector3.zero;

        if (gridElement != null && gridParent != null)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    int pos = x * gridSize + y;
                    
                    if (x * gridSize + y >= tileDataList.Count)
                    {
                        //Create the game objects in the world (Instantiate is like the new Keyword except it creates the gameobject in the world)
                        GameObject ele = Instantiate(gridElement, instantiatePosition, Quaternion.identity, gridParent);
                        GameObject tileMovesUI = ele.transform.GetChild(0).gameObject;
                        GameObject tileDepthUI = ele.transform.GetChild(1).gameObject;
                        GameObject tileSpfVUI = ele.transform.GetChild(2).gameObject;

                        //Initialise the Tile by setting its data
                        tileDataList.Add(ele.GetComponent<Tile>());
                        tileDataList[tileDataList.Count - 1].InitializeTile(x, y, tileMovesUI.GetComponent<UITextHandler>(), tileDepthUI.GetComponent<UITextHandler>(), tileSpfVUI.GetComponent<UITextHandler>(), ele.GetComponent<SpriteRenderer>());

                        //Add objects to list for future references.
                        tiles.Add(ele);
                        //tileMovesUIList.Add(tileMovesUI);
                    }
                    else
                    {
                        tiles[pos].transform.position = instantiatePosition;
                        tiles[pos].SetActive(true);

                        //tileMovesUIList[pos].SetActive(true);
                        
                        tileDataList[pos].y = x;
                        tileDataList[pos].x = y;
                        tileDataList[pos].SetNumMoves(0);
                        tileDataList[pos].SetTileDepth(-1);
                        tileDataList[pos].SetSpfValue(-1);
                    }

                    SetTileColour(pos, x, y);

                    //Move the next instantiated tile a little bit to the left.
                    instantiatePosition.x += 1 + gridElePaddingX;

                    if (x == gridSize / 2 && x == y + 1)
                    {
                        middlePos = instantiatePosition;
                    }
                }
                //Move the next Instantiated tile back to the left most position (Start of Row).
                instantiatePosition.x = -gridSize / 2;
                //Move the next Instantiated tile a little bit down.
                instantiatePosition.y += (1 + gridElePaddingY);
            }

            //Adjust main camera position according to the center of the instantiated tiles.
            mainCamera.transform.position = new Vector3(middlePos.x, middlePos.y, mainCamera.transform.position.z);
            boardGenerated = true;
            generateBoard = true;
        }
    }

    public Tile GetTileByCoord(int x, int y)
    {
        return tileDataList[x * gridSize + y];
    }

    public Tile GetStartTile()
    {
        return GetTileByCoord(startTileCoord.x, startTileCoord.y);
    }
    public Tile GetGoalTile()
    {
        return GetTileByCoord(goalTileCoords.x, goalTileCoords.y);
    }

    public Tile GetCopyOfTile(Tile tileToCopy)
    {
        Tile copiedTile = new Tile(tileToCopy);

        return copiedTile;
    }

    public void SetNumMovesForTile(int x, int y, int numMoves)
    {
        tileDataList[x * gridSize + y].SetNumMoves(numMoves);
    }

    public int CurNumTiles()
    {
        return gridSize * gridSize;
    }

    public bool ResetDepthValues()
    {
        for (int i = 0; i < tileDataList.Count; i++)
        {
            tileDataList[i].pfParent = null;
            tileDataList[i].SetTileDepth(-1);
        }
        return true;
    }

    public bool ResetSpfRValues()
    {
        for (int i = 0; i < tileDataList.Count; i++)
        {
            tileDataList[i].SetSpfValue(-1);
            tileDataList[i].visited = 0;
        }
        return true;
    }

    public bool ResetAStarValues()
    {
        for (int i = 0; i < tileDataList.Count; i++)
        {
            tileDataList[i].pfParent = null;
            tileDataList[i].SetAStarValue(-1, 0);
            tileDataList[i].visited = 0;
            tileDataList[i].inAStarList = 0;
        }
        return true;
    }

    public void SetTimerText(string timerText)
    {
        valueSettingHandler.SetTimerText(timerText);
    }

    public void ResetTileColours()
    {
        int x = 0;
        int y = 0;
        int pos = y * gridSize + x;
        for (x = 0; x < gridSize; x++)
        {
            for (y = 0; y < gridSize; y++)
            {
                pos = x * gridSize + y;
                if (pos == gridSize - 1)
                {
                    tileDataList[pos].SetSpriteColor(gridGoalElementColour);
                }
                else if (pos == gridSize * (gridSize - 1))
                {
                    tileDataList[pos].SetSpriteColor(gridStartElementColour);
                }
                else
                {
                    tileDataList[pos].SetSpriteColor(gridElementColour);
                }
            }
        }
        
    }

    private void SetTileColour(int pos, int x, int y)
    {
        if (pos == gridSize - 1)
        {
            tileDataList[pos].SetSpriteColor(gridGoalElementColour);
            goalTileCoords.x = x;
            goalTileCoords.y = y;
        }
        else if (pos == gridSize * (gridSize - 1))
        {
            tileDataList[pos].SetSpriteColor(gridStartElementColour);
            startTileCoord.x = x;
            startTileCoord.y = y;
        }
        else
        {
            tileDataList[pos].SetSpriteColor(gridElementColour);
        }
    }

    private void DeactivateAllObjects()
    {
        for (int i = 0; i < tileDataList.Count; i++)
        {
            tiles[i].SetActive(false);
            tileDataList[i].SetTileDepth(-1);
            //tileMovesUIList[i].SetActive(false);
        }
    }
}
