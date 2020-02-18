using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextHandled : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float cameraOrthoSize = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        cameraOrthoSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.orthographicSize != cameraOrthoSize)
        {
            cameraOrthoSize = Camera.main.orthographicSize;
            if (Input.GetKeyDown(KeyCode.I))
            {
                text.fontSize += 2;
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                text.fontSize -= 2;
            }
        }
    }

    public string Text()
    {
        return text.text;
    }

    public void SetTextOfTile(string str)
    {
        if(text != null)
        {
            text.SetText(str);
        }
        else
        {
            text = gameObject.GetComponent<TextMeshProUGUI>();
            text.SetText(str);
            //Debug.LogError("Tile Text UI has not been assigned.");
        }
    }
}
