using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataHandler : MonoBehaviour
{
    public GameObject gridSceneObject;
    private GridManagerAPI gridAPI;

    [SerializeField] private int gridSize = 5;

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
        else
        {
            //Creates the visible parts of the grid
            gridAPI.CreateGrid_WithUI_InScene(gridSize);

            //Part 1
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    //Debug.Log("element: " + (i * gridSize + j));
                    SetRandomLegalTileMove(i, j); 
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetRandomLegalTileMove(int x, int y)
    {
        int maxX = gridSize - x;
        int maxY = gridSize - y;

        //Random function's second parameter is exclusive
        int randomX = Random.Range(1, maxX);
        int randomY = Random.Range(1, maxY);

        int random = Random.Range(0, 2);
        if(random == 0)
        {
            gridAPI.SetNumMovesForTile(x, y, randomX);
        }
        else
        {
            gridAPI.SetNumMovesForTile(x, y, randomY);
        }
    }
}
