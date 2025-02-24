using System.Collections.Generic;
using UnityEngine;

public class Character_Valves : MonoBehaviour
{
    public enum DualPressureState
    {
        off,
        CRAE
    }

    public enum SpecialState
    {
        off,
        Cyber,
        CyberSwinger
    }

    [HideInInspector] public GameObject mackValves;

    public float[] invertChart;

    [HideInInspector] public List<int> cylParamHash = new();

    [HideInInspector] public List<bool> cylDrawer = new();

    [HideInInspector] public List<int> cylBit = new();

    [HideInInspector] public List<int> cylLayerId = new();

    [Header("Attributes")] public DualPressureState dualPressureState = DualPressureState.off;

    [Header("Special State")] public SpecialState specialState = SpecialState.off;

    public int headOutBit;
    public int headInBit;

    [Header("Flows")] public float PSIScale = 1;

    public float[] gravityScale;
    public float[] gravityScaleOut;
    public float[] flowControlOut;
    public float[] flowControlIn;
    public float[] smashControlOut;
    public float[] smashControlIn;
    public float[] smashSpeedOut;
    public float[] smashSpeedIn;
    private Mack_Valves bitChart;

    private Animator characterValves;

    private List<int> cylParamId = new();

    private float headSwingAccel;
    private float[] heavyChartIn;
    private float[] heavyChartOut;
    private GameObject mv;

    private bool numeratorLoop;
    private int[] smashIteration;
    private bool[] smashState;
    private UI_PlayRecord ui;

    private void Awake()
    {
        //Create Charts
        heavyChartOut = new float[flowControlIn.Length];
        heavyChartIn = new float[flowControlIn.Length];
        invertChart = new float[flowControlIn.Length];
        smashIteration = new int[flowControlIn.Length];
        smashState = new bool[flowControlIn.Length];
        smashControlIn = new float[flowControlIn.Length];
        smashControlOut = new float[flowControlIn.Length];
        smashSpeedIn = new float[flowControlIn.Length];
        smashSpeedOut = new float[flowControlIn.Length];
        for (int i = 0; i < invertChart.Length; i++)
        {
            invertChart[i] = 1;
            smashIteration[i] = 1;
        }
    }

    private void OnDisable()
    {
        if (ui != null)
        {
            ui.characterEvent.RemoveListener(CreateMovements);
        }
    }

    public void StartUp()
    {
        transform.parent.name = transform.parent.name.Replace("(Clone)", "").Trim();
        cylParamHash = new List<int>();
        cylDrawer = new List<bool>();
        cylBit = new List<int>();
        cylLayerId = new List<int>();
        cylParamId = new List<int>();
        ////////////////////////////THIS NEEDS TO BE IN HERE NOTHING SHOULD BE IENUMERATOR
        Debug.Log("Startup Performed on " + name);
        characterValves = GetComponent<Animator>();

        ui = GameObject.Find("UI").GetComponent<UI_PlayRecord>();
        ui.characterEvent.AddListener(CreateMovements);
        bitChart = GameObject.Find("Mack Valves").GetComponent<Mack_Valves>();
        GameObject.Find("UI Side Panel").GetComponent<UI_SidePanel>().FlowUpdater(gameObject);

        for (int i = 0; i < characterValves.layerCount; i++)
        {
            string layerName = characterValves.GetLayerName(i);
            bool goodToGo = true;
            int bitParse = int.Parse(layerName.Substring(0, layerName.Length - 1));
            for (int v = 0; v < cylBit.Count; v++)
                if (bitParse == cylBit[v])
                    goodToGo = false;

            if (goodToGo)
            {
                cylLayerId.Add(i);
                cylBit.Add(bitParse);
                cylDrawer.Add(layerName[layerName.Length - 1] == 'B');
                cylParamHash.Add(Animator.StringToHash(layerName));
                for (int j = 0; j < characterValves.parameters.Length; j++)
                    if (characterValves.parameters[j].name == layerName)
                    {
                        cylParamId.Add(j);
                        break;
                    }
            }
        }
    }

    /// <summary>
    ///     Simulates the exact position for each animation layer of the character.
    ///     Thanks to Himitsu for completely overhauling the entire sim to improve
    ///     its effeciency.
    /// </summary>
    /// <param name="timeDeltaTime"></param>
    public void CreateMovements(float timeDeltaTime)
    {
        if (bitChart != null)
            for (int layerid = 0; layerid < characterValves.layerCount; layerid++)
            {
                if (specialState == SpecialState.Cyber || specialState == SpecialState.CyberSwinger)
                    while (layerid <= 1)
                    {
                        CreateCyberMovements(layerid, timeDeltaTime);
                        layerid++;
                    }

                int bitnum = cylBit[layerid] - 1;
                bool drawer = cylDrawer[layerid];
                int hash = cylParamHash[layerid];
                int i = cylParamId[layerid];
                bool state;
                bool dpr = false;
                if (drawer)
                {
                    state = bitChart.bottomDrawer[bitnum];
                    switch (dualPressureState)
                    {
                        case DualPressureState.CRAE:
                            dpr = bitChart.bottomDrawer[60];
                            break;
                    }
                }
                else
                {
                    state = bitChart.topDrawer[bitnum];
                    switch (dualPressureState)
                    {
                        case DualPressureState.CRAE:
                            dpr = bitChart.topDrawer[40];
                            break;
                    }
                }

                //Assign PSI and Valve Position
                float currentValvePos = characterValves.GetFloat(hash);
                float valvePSI = bitChart.PSI / 1050f * PSIScale * timeDeltaTime;

                characterValves.SetLayerWeight(layerid, 1f);
                //If dual pressure regulation is on
                if (dpr)
                {
                    if (dualPressureState == DualPressureState.CRAE)
                        valvePSI *= 0.75f;
                    else
                        valvePSI *= 0.65f;
                }

                float smash;
                float smashSpeed;
                if (state)
                {
                    //Outwards Movement Calculation
                    heavyChartOut[i] = Mathf.Min(heavyChartOut[i] + flowControlOut[i] * flowControlOut[i] / 2f,
                        1f + (1f - flowControlOut[i]) * 0.3f);
                    heavyChartIn[i] = 0f;
                    currentValvePos += valvePSI * flowControlOut[i] * heavyChartOut[i] * gravityScaleOut[i] *
                                       invertChart[i];
                    smash = smashControlOut[i];
                    smashSpeed = smashSpeedOut[i];
                }
                else
                {
                    //Inwards Movement Calculation
                    heavyChartIn[i] = Mathf.Min(heavyChartIn[i] + flowControlIn[i] * flowControlIn[i] / 2f,
                        1f + (1f - flowControlIn[i]) * 0.3f);
                    heavyChartOut[i] = 0f;
                    currentValvePos -= valvePSI * flowControlIn[i] * heavyChartIn[i] * gravityScale[i] * invertChart[i];
                    smash = smashControlIn[i];
                    smashSpeed = smashSpeedIn[i];
                }

                if (smashState[i] != state)
                {
                    smashState[i] = state;
                    smashIteration[i] = 1;
                    invertChart[i] = 1;
                }

                //Smash Calculation
                if (invertChart[i] < 1) invertChart[i] = Mathf.Min(invertChart[i] + timeDeltaTime * smashSpeed, 1f);
                if (currentValvePos < 0 && smash != 0 && smashIteration[i] < 4)
                {
                    invertChart[i] = -smash * (Mathf.Abs(currentValvePos + 1) / smashIteration[i] / timeDeltaTime);
                    smashState[i] = state;
                    smashIteration[i]++;
                }

                if (currentValvePos > 1 && smash != 0 && smashIteration[i] < 4)
                {
                    invertChart[i] = -smash * (currentValvePos / smashIteration[i] / timeDeltaTime);
                    smashState[i] = state;
                    smashIteration[i]++;
                }

                //Final Value
                currentValvePos = Mathf.Min(Mathf.Max(currentValvePos, 0f), 1f);
                characterValves.SetFloat(hash, currentValvePos);
            }
    }

    /// <summary>
    ///     A special simulation for Cyberamic characters, as their heads and necks
    ///     are the only outlier to the normal pushing in and out of most
    ///     animatronic movements.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="timeDeltatime"></param>
    public void CreateCyberMovements(int e, float timeDeltatime)
    {
        int layerid = 0;
        int hash = cylParamHash[layerid];
        int i = cylParamId[layerid];
        bool state;
        bool nothing = true;
        bool dpr = false;
        float currentAnimState = characterValves.GetFloat(hash);
        float finalPSIScale = bitChart.PSI / 1050f * PSIScale;

        if (headSwingAccel > 0)
            headSwingAccel = Mathf.Clamp(headSwingAccel - Time.deltaTime * 0.055f, 0, 0.1f);
        else
            headSwingAccel = Mathf.Clamp(headSwingAccel + Time.deltaTime * 0.055f, -0.1f, 0);


        //Check which bit
        if (e == 0)
        {
            state = bitChart.topDrawer[headOutBit - 1];
            if (state) nothing = false;
            //Recheck
            if (currentAnimState == 0f && state)
            {
                layerid = 1;
                hash = cylParamHash[layerid];
                i = cylParamId[layerid];
                currentAnimState = characterValves.GetFloat(hash);
                state = false;
            }
            else
            {
                state = false;
            }
        }
        else
        {
            state = bitChart.topDrawer[headInBit - 1];
            if (state) nothing = false;
            if (!state && specialState == SpecialState.CyberSwinger)
            {
                layerid = 1;
                hash = cylParamHash[layerid];
                i = cylParamId[layerid];
                currentAnimState = characterValves.GetFloat(hash);
                heavyChartIn[i] = Mathf.Min(heavyChartIn[i] + flowControlIn[i] * flowControlIn[i] / 2f,
                    1f + (1f - flowControlIn[i]) * 0.3f * timeDeltatime);
                heavyChartOut[i] = 0f;
                currentAnimState -=
                    finalPSIScale * flowControlIn[i] * heavyChartIn[i] * timeDeltatime * gravityScale[i];
            }

            //Recheck
            if (currentAnimState == 1f && state)
            {
                layerid = 1;
                hash = cylParamHash[layerid];
                i = cylParamId[layerid];
                currentAnimState = characterValves.GetFloat(hash);
            }
        }

        //Apply Anims
        if (currentAnimState == 0f && !state)
        {
            characterValves.SetLayerWeight(layerid, 0f);
        }
        else if (currentAnimState != 1f || !state)
        {
            characterValves.SetLayerWeight(layerid, 1f);
            if (dpr) finalPSIScale *= 0.75f;
            if (state && !nothing)
            {
                heavyChartOut[i] = Mathf.Min(heavyChartOut[i] + flowControlOut[i] * flowControlOut[i] / 2f,
                    1f + (1f - flowControlOut[i]) * 0.3f * timeDeltatime);
                heavyChartIn[i] = 0f;
                currentAnimState += finalPSIScale * flowControlOut[i] * heavyChartOut[i] * timeDeltatime *
                                    gravityScaleOut[i];
                headSwingAccel += finalPSIScale * flowControlOut[i] * heavyChartOut[i] * timeDeltatime *
                                  gravityScaleOut[i];
            }
            else if (!state && !nothing)
            {
                heavyChartIn[i] = Mathf.Min(heavyChartIn[i] + flowControlIn[i] * flowControlIn[i] / 2f,
                    1f + (1f - flowControlIn[i]) * 0.3f * timeDeltatime);
                heavyChartOut[i] = 0f;
                currentAnimState -=
                    finalPSIScale * flowControlIn[i] * heavyChartIn[i] * timeDeltatime * gravityScale[i];
                headSwingAccel -= finalPSIScale * flowControlIn[i] * heavyChartIn[i] * timeDeltatime * gravityScale[i];
            }
        }

        currentAnimState += headSwingAccel * 0.06f;
        currentAnimState = Mathf.Min(Mathf.Max(currentAnimState, 0f), 1f);
        characterValves.SetFloat(hash, currentAnimState);
    }
}