using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #region CAMERA ZOOM
        //ZOOM OUT if Key 'O' is pressed
        if (Input.GetKey(KeyCode.O))
        {
            Camera.main.orthographicSize++;
        }

        //ZOOM IN if Key 'I' is pressed
        else if (Input.GetKey(KeyCode.I))
        {
            if (Camera.main.orthographicSize >= 6)
            {
                Camera.main.orthographicSize-= 0.5f;
            }
        }
        #endregion
    }
}
