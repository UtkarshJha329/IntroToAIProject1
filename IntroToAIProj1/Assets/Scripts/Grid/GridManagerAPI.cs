using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManagerAPI : MonoBehaviour
{
    //The variable that will store the main camera that is currently being used in the scene.
    private Camera mainCamera;

    //GridSize (As the grid is always a square this will define how big one side is)
    [SerializeField] private int gridSize = 5;
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
    private static List<GameObject> tileUIList = new List<GameObject>();
    public static List<Tile> tileDataList = new List<Tile>();

    private Vector3 instantiatePosition = Vector3.zero;

    public bool boardGenerated = false;
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
                    if (x * gridSize + y >= tileDataList.Count)
                    {
                        //Create the game objects in the world (Instantiate is like the new Keyword except it creates the gameobject in the world)
                        GameObject ele = Instantiate(gridElement, instantiatePosition, Quaternion.identity, gridParent);
                        GameObject tileUI = ele.transform.GetChild(0).gameObject;

                        //Initialise the Tile by setting its data
                        tileDataList.Add(ele.GetComponent<Tile>());
                        tileDataList[tileDataList.Count - 1].InitializeTile(x, y, tileUI.GetComponent<UITextHandled>());

                        //Add objects to list for future references.
                        tiles.Add(ele);
                        tileUIList.Add(tileUI);
                    }
                    else
                    {
                        int pos = x * gridSize + y;
                        tiles[pos].transform.position = instantiatePosition;
                        tiles[pos].SetActive(true);

                        tileUIList[pos].SetActive(true);
                        
                        tileDataList[pos].x = x;
                        tileDataList[pos].y = y;
                    }
                    

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
        }
    }

    public Tile GetTileByCoord(int x, int y)
    {
        return tileDataList[x * gridSize + y];
    }
    public void SetNumMovesForTile(int x, int y, int numMoves)
    {
        tileDataList[x * gridSize + y].SetNumMoves(numMoves);
    }

    private void DeactivateAllObjects()
    {
        for (int i = 0; i < tileDataList.Count; i++)
        {
            tiles[i].SetActive(false);
            tileUIList[i].SetActive(false);
        }
    }
}
