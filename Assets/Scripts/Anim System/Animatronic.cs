using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// This script controls movements & more for the animatronic the script is attached to. Formerly known as Character_Valves.cs 
/// </summary>
public class Animatronic : MonoBehaviour
{
    [Header("General")] 
    [Range(0, 1)]
    public float PSIScale = 1;
    public Movement[] movements; 
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
    

    [HideInInspector] public List<int> cylParamHash = new();

    [HideInInspector] public List<bool> cylDrawer = new();

    [HideInInspector] public List<int> cylBit = new();

    [HideInInspector] public List<int> cylLayerId = new();

    [Header("Attributes")] public DualPressureState dualPressureState = DualPressureState.off;

    [Header("Special State")] public SpecialState specialState = SpecialState.off;

    public int headOutBit;
    public int headInBit;
    
    private Mack_Valves bitChart;

    private Animator characterValves;

    private List<int> cylParamId = new();

    private float headSwingAccel;

    private bool numeratorLoop;
    private int[] smashIteration;
    private bool[] smashState;
    private DF_ShowManager ui;
    
    // Playables
    private PlayableGraph _graph;
    private AnimationMixerPlayable _mixer;
    private AnimationPlayableOutput _output;

    private void Awake()
    {
        if (movements.Length == 0)
        {
            Debug.LogWarning($"No movements assigned to {gameObject.name} so disabling. Please assign at least one in the inspector.");
            ui.characterEvent.RemoveListener(CreateMovements);

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

        ui = GameObject.Find("UI").GetComponent<DF_ShowManager>();
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
                    movements[i].weightOut = Mathf.Min(movements[i].weightOut + movements[i].flowControlOut * movements[i].flowControlOut / 2f,
                        1f + (1f - movements[i].flowControlOut) * 0.3f);
                    movements[i].weightIn = 0f;
                    currentValvePos += valvePSI * movements[i].flowControlOut * movements[i].weightOut * movements[i].gravityScaleOut *
                                       movements[i].invert;
                    smash = movements[i].smashOut;
                    smashSpeed = movements[i].smashSpeedOut;
                }
                else
                {
                    //Inwards Movement Calculation
                    movements[i].weightIn = Mathf.Min(movements[i].weightIn + movements[i].flowControlIn * movements[i].flowControlIn / 2f,
                        1f + (1f - movements[i].flowControlIn) * 0.3f);
                    movements[i].weightOut = 0f;
                    currentValvePos -= valvePSI * movements[i].flowControlIn * movements[i].weightIn * movements[i].gravityScale * movements[i].invert;
                    smash = movements[i].smashIn;
                    smashSpeed = movements[i].smashSpeedIn;
                }

                if (smashState[i] != state)
                {
                    smashState[i] = state;
                    smashIteration[i] = 1;
                    movements[i].invert = 1;
                }

                //Smash Calculation
                if (movements[i].invert == 1) movements[i].invert = Mathf.Min(movements[i].invert + timeDeltaTime * smashSpeed, 1f);
                if (currentValvePos < 0 && smash != 0 && smashIteration[i] < 4)
                {
                    movements[i].invert = -smash * (Mathf.Abs(currentValvePos + 1) / smashIteration[i] / timeDeltaTime);
                    smashState[i] = state;
                    smashIteration[i]++;
                }

                if (currentValvePos > 1 && smash != 0 && smashIteration[i] < 4)
                {
                    movements[i].invert = -smash * (currentValvePos / smashIteration[i] / timeDeltaTime);
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
                movements[i].weightIn = Mathf.Min(movements[i].weightIn + movements[i].flowControlIn * movements[i].flowControlIn / 2f,
                    1f + (1f - movements[i].flowControlIn) * 0.3f * timeDeltatime);
                movements[i].weightOut = 0f;
                currentAnimState -=
                    finalPSIScale * movements[i].flowControlIn * movements[i].weightIn * timeDeltatime * movements[i].gravityScale;
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
                movements[i].weightOut = Mathf.Min(movements[i].weightOut + movements[i].flowControlOut * movements[i].flowControlOut / 2f,
                    1f + (1f - movements[i].flowControlOut) * 0.3f * timeDeltatime);
                movements[i].weightIn = 0f;
                currentAnimState += finalPSIScale * movements[i].flowControlOut * movements[i].weightOut * timeDeltatime *
                                    movements[i].gravityScaleOut;
                headSwingAccel += finalPSIScale * movements[i].flowControlOut * movements[i].weightOut * timeDeltatime *
                                  movements[i].gravityScaleOut;
            }
            else if (!state && !nothing)
            {
                movements[i].weightIn = Mathf.Min(movements[i].weightIn + movements[i].flowControlIn * movements[i].flowControlIn / 2f,
                    1f + (1f - movements[i].flowControlIn) * 0.3f * timeDeltatime);
                movements[i].weightOut = 0f;
                currentAnimState -=
                    finalPSIScale * movements[i].flowControlIn * movements[i].weightIn * timeDeltatime * movements[i].gravityScale;
                headSwingAccel -= finalPSIScale * movements[i].flowControlIn * movements[i].weightIn * timeDeltatime * movements[i].gravityScale;
            }
        }

        currentAnimState += headSwingAccel * 0.06f;
        currentAnimState = Mathf.Min(Mathf.Max(currentAnimState, 0f), 1f);
        characterValves.SetFloat(hash, currentAnimState);
    }
    
    /// <summary>
    /// Sets up the graph, mixer, output and clips for the new Playables system
    /// Playables is a dynamic replacement for manually creating AnimationControllers for animatronics
    /// </summary>
    public void SetupPlayables()
    {
        _graph = PlayableGraph.Create("AdditiveAnimationGraph");
        _mixer = AnimationMixerPlayable.Create(_graph, movements.Length);
        _output = AnimationPlayableOutput.Create(_graph, "AnimationOutput", GetComponent<Animator>());
        _output.SetSourcePlayable(_mixer);

        for (int i = 0; i < movements.Length; i++)
        {
            var playable = AnimationClipPlayable.Create(_graph, movements[i].animation);
            playable.SetApplyFootIK(false);
            _graph.Connect(playable, i, _mixer, i);
            _mixer.SetInputWeight(i, 0);
        }
    }
}

/// <summary>
/// This contains all the data about the animatronic's movement. Replaces the deprecated arrays in the Animatronic.cs (formerly known as Character_Valves.cs) script
/// </summary>
[Serializable]
public class Movement
{
    [Header("General")] 
    public string name;

    public AnimationClip animation;
    public int bit = 1;
    public Drawer drawer;
    
    [Header("Sounds")] 
    public bool valves;
    public bool squeaks;
    public bool airLeaks;
    
    [Header("Flows")]
    [Range(0, 1)] public float flowControlOut = 1f;
    [Range(0, 1)] public float flowControlIn = 1f;
    [Space(10)]
    [Range(0, 1)] public float gravityScale = 1f;
    [Range(0, 1)] public float gravityScaleOut = 1f;
    [Space(10)]
    [Range(0, 1)] public float smashOut;
    [Range(0, 1)] public float smashIn;
    [Range(0, 1)] public float smashSpeedOut;
    [Range(0, 1)] public float smashSpeedIn;
    [Space(10)]
    [Range(0, 1)] public float weightOut;
    [Range(0, 1)] public float weightIn;
    [Space(10)]
    // Genuinely have no clue why this has to be a float instead of a bool.
    // The code is just too confusing, so I'm leaving it here as a float
    // unless someone wants to try turn it into a bool
    public float invert; 
}

public enum Drawer
{
    Top,
    Bottom,
}