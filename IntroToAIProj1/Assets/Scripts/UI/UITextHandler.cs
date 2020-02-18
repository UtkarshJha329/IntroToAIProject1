using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextHandler : MonoBehaviour
{
    private TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
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
            text = gameObject.GetComponent<TextMeshPro>();
            text.SetText(str);
            //Debug.LogError("Tile Text UI has not been assigned.");
        }
    }
}
