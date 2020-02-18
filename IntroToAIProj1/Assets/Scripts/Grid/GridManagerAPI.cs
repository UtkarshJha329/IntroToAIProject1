using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManagerAPI : MonoBehaviour
{
    //The variable that will store the main camera that is currently being used in the scene.
    private Camera mainCamera;

    //GridSize (As the grid is always a square this will define how big one side is)
    [SerializeField] private int gridSize = 5;

    //Adds padding in between each tile
    //The SerilizeField tag shows the variable beside it in the inspector
    [SerializeField] private float gridElePaddingX = 0.2f;
    [SerializeField] private float gridElePaddingY = 0.2f;

    //The Instantiated Elements of a Grid (Instantiate creates the element in the scene.)
    [SerializeField] private GameObject gridElement;
    [SerializeField] private Transform gridParent;

    [SerializeField] private GameObject tileUIPrefab;
    [SerializeField] private RectTransform canvasTransform;

    //Lists will hold references to each individual tile
    private static List<GameObject> tiles = new List<GameObject>();
    private static List<GameObject> movesUIList = new List<GameObject>();
    public static List<Tile> tileDataList = new List<Tile>();

    private Vector3 instantiatePosition = Vector3.zero;

    private bool once = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!once)
        {
            //Zoom out once so that all UI updates.
            mainCamera.orthographicSize++;
            once = true;
        }        
    }

    public void CreateGrid_WithUI_InScene(int _gridSize)
    {
        //Get a reference to the Main Camera in the scene
        mainCamera = Camera.main;

        //Set gridSize
        gridSize = _gridSize;
        //Instantiate Start Position
        instantiatePosition = new Vector3(-gridSize / 2, -gridSize / 2, 5);
        Vector3 middlePos = Vector3.zero;

        if (gridElement != null && gridParent != null && tileUIPrefab != null)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int k = 0; k < gridSize; k++)
                {
                    //Create the game objects in the world (Instantiate is like the new Keyword except it creates the gameobject in the world)
                    GameObject ele = Instantiate(gridElement, instantiatePosition, Quaternion.identity, gridParent);
                    GameObject tileUI = Instantiate(tileUIPrefab);


                    //Initialise the UI
                    tileUI.transform.SetParent(canvasTransform, false);
                    tileUI.GetComponent<UIPositionHandler>().SetData(ele.transform, canvasTransform);
                    //legalMoveUI.GetComponent<UIPositionHandler>().SetUIPosition();

                    //Initialise the Tile by setting its data
                    tileDataList.Add(ele.GetComponent<Tile>());
                    tileDataList[tileDataList.Count - 1].InitializeTile(i, k, tileUI.GetComponent<UITextHandled>());

                    //Add objects to list for future references.
                    //tiles.Add(ele);
                    //movesUIList.Add(tileUI);

                    //Move the next instantiated tile a little bit to the left.
                    instantiatePosition.x += 1 + gridElePaddingX;

                    if (i == gridSize / 2 && i == k + 1)
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
            once = false;
        }
    }

    public void SetNumMovesForTile(int x, int y, int numMoves)
    {
        tileDataList[x * gridSize + y].SetNumMoves(numMoves);
    }
}
