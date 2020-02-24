using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValueSettingHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NValueText;
    [SerializeField] private Slider NValueSLider;
    [SerializeField] private TextMeshProUGUI HillClimbValueText;
    [SerializeField] private Slider HillClimbSlider;
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private TextMeshProUGUI NumTilesCheckedText;

    private int nValue = 5;
    [HideInInspector] public bool generateGrid = false;
    [HideInInspector] public bool doBFS = false;
    [HideInInspector] public bool doHillClimb = false;
    [HideInInspector] public bool doSPF = false;
    [HideInInspector] public bool doAStar = false;
    [HideInInspector] public bool doGeneticMating = false;

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
        HillClmbItterVal();
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
            case 5:
                NValueText.SetText("N value: 16");
                nValue = 16;
                break;
            case 6:
                NValueText.SetText("N value: 55");
                nValue = 55;
                break;
            case 7:
                NValueText.SetText("N value: 66");
                nValue = 66;
                break;
            case 8:
                NValueText.SetText("N value: 77");
                nValue = 77;
                break;
            case 9:
                NValueText.SetText("N value: 88");
                nValue = 88;
                break;
            case 10:
                NValueText.SetText("N value: 99");
                nValue = 99;
                break;

        }
        return nValue;
    }

    public int HillClmbItterVal()
    {
        int retVal = (int)HillClimbSlider.value * 100;
        HillClimbValueText.SetText("Num Itter: " + retVal);
        return retVal;
    }

    public void Generate()
    {
        if(nValue >= 55)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                generateGrid = true;
            }
            else
            {
                generateGrid = false;
            }
        }
        else
        {
            generateGrid = true;
        }
    }

    public void DoBFS()
    {
        doBFS = true;
    }

    public void DoHillClimb()
    {
        doHillClimb = true;
    }

    public void DoSPF()
    {
        doSPF = true;
    }

    public void DoAStar()
    {
        doAStar = true;
    }

    public void DoGeneticMating()
    {
        doGeneticMating = true;
    }

    public void SetTimerText(string _timerText)
    {
        TimerText.SetText(_timerText);
    }

    public void SetNumTilesText(int numTilesChecked)
    {
        NumTilesCheckedText.SetText(numTilesChecked.ToString());
    }
}
