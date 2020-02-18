using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataHandler : MonoBehaviour
{
    public GameObject gridSceneObject;
    private GridManagerAPI gridAPI;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(gridAPI.boardGenerated == true)
        {
            //PART 2 OF PROJECT
            SetRandomLegalTileMove();
            gridAPI.boardGenerated = false;
        }
    }

    private void SetRandomLegalTileMove()
    {
        for (int x = 0; x < gridAPI.GridSize(); x++)
        {
            for (int y = 0; y < gridAPI.GridSize(); y++)
            {
                //Debug.Log("element: " + (i * gridSize + j));
                int maxX = (x <= gridAPI.GridSize() / 2) ? gridAPI.GridSize() - x : gridAPI.GridSize() - x/2;
                int maxY = (y <= gridAPI.GridSize() / 2) ? gridAPI.GridSize() - y : gridAPI.GridSize() - y/2;

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
            }
        }
    }

}
