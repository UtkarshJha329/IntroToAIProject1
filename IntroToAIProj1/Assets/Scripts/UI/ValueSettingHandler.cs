using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValueSettingHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NValueText;
    [SerializeField] private Slider NValueSLider;

    private int nValue = 5;
    [HideInInspector] public bool generateGrid = false;

    // Start is called before the first frame update
    void Start()
    {
        if(NValueSLider == null && NValueText == null)
        {
            Debug.LogError("N_VALUE_SLIDER Or N_VALUE_TEXT reference needs to be added to the script");
        }

        NValue();
    }

    // Update is called once per frame
    void Update()
    {
        NValue();
    }

    public int NValue()
    {
        int sliderValue = (int)NValueSLider.value;
        switch (sliderValue)
        {
            case 1:
                NValueText.SetText("N value: 5");
                nValue = 5;
                break;
            case 2:
                NValueText.SetText("N value: 7");
                nValue = 7;
                break;
            case 3:
                NValueText.SetText("N value: 9");
                nValue = 9;
                break;
            case 4:
                NValueText.SetText("N value: 11");
                nValue = 11;
                break;

        }
        return nValue;
    }

    public void Generate()
    {
        generateGrid = true;
    }
}
