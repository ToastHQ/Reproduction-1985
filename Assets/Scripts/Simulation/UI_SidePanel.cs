using System.Collections;
using SFB;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_SidePanel : MonoBehaviour
{
    //Windows
    public GameObject WindowMenu;
    public GameObject WindowSound;
    public GameObject WindowShow;
    public GameObject WindowCamera;
    public GameObject WindowFlows;
    public GameObject WindowBitVis;

    //Text
    public Text psiText;
    public Text volumeText;
    public Text spacialText;
    public Text curtainText;
    public Text upperLightText;
    public Text camSmoothText;
    public Text soundvolumeText;
    public Text stagevolumeText;
    public Text signalSwapText;

    public string FileExtention;

    //Other
    public GameObject areaLights;
    public UI_PlayRecord showPanelUI;
    public Static staticUI;

    public float[] copyPasteValues = new float[8];
    private int flowNumber;

    //Flow Controls
    private int flowProfile;

    private bool hidepanels;

    private void Awake()
    {
        showPanelUI.thePlayer = GameObject.Find("Player");
        AudioSource gg = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        soundvolumeText.GetComponent<Text>().text = Mathf.Ceil(gg.volume * 100).ToString();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (hidepanels)
            {
                hidepanels = !hidepanels;
                transform.parent.localPosition -= Vector3.one * 100;
                showPanelUI.transform.parent.localPosition -= Vector3.one * 100;
            }
            else
            {
                hidepanels = !hidepanels;
                transform.parent.localPosition += Vector3.one * 100;
                showPanelUI.transform.parent.localPosition += Vector3.one * 100;
            }
        }

        if (Input.GetKeyDown(KeyCode.BackQuote)) showPanelUI.pauseSong();
    }

    public void Upperlights(int input)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        Resources.Load("ting");
        sc.clip = (AudioClip)Resources.Load("Tech Lights");
        sc.pitch = Random.Range(0.95f, 1.05f);
        sc.Play();
        if (areaLights.activeSelf)
        {
            upperLightText.GetComponent<Text>().text = "Off";
            areaLights.SetActive(false);
        }
        else
        {
            upperLightText.GetComponent<Text>().text = "On";
            areaLights.SetActive(true);
        }
    }

    public void SwapWindow(int input)
    {
        staticUI.flash = true;
        WindowCamera.SetActive(false);
        WindowMenu.SetActive(false);
        WindowShow.SetActive(false);
        WindowSound.SetActive(false);
        WindowFlows.SetActive(false);
        WindowBitVis.SetActive(false);
        switch (input)
        {
            case 0:
                WindowMenu.SetActive(true);
                break;
            case 1:
                WindowSound.SetActive(true);
                break;
            case 2:
                WindowShow.SetActive(true);
                break;
            case 3:
                WindowCamera.SetActive(true);
                break;
            case 5:
                WindowFlows.SetActive(true);
                break;
            case 6:
                WindowBitVis.SetActive(true);
                break;
        }
    }

    public void StageVolume(int input)
    {
        var yea = FindObjectsOfType(typeof(InstrumentSound)) as InstrumentSound[];
        for (int i = 0; i < yea.Length; i++)
        {
            yea[i].volume = Mathf.Min(Mathf.Max(yea[i].volume + input * .05f, 0), 1);
            stagevolumeText.text = Mathf.Ceil(yea[i].volume * 100).ToString();
        }
    }

    public void SignalSwap(int input)
    {
        if (showPanelUI.signalChange == UI_PlayRecord.SignalChange.normal)
        {
            signalSwapText.text = "On";
            switch (input)
            {
                case 0:
                    showPanelUI.signalChange = UI_PlayRecord.SignalChange.PreCU;
                    break;
                case 1:
                    showPanelUI.signalChange = UI_PlayRecord.SignalChange.PrePTT;
                    break;
            }
        }
        else
        {
            signalSwapText.text = "Off";
            showPanelUI.signalChange = UI_PlayRecord.SignalChange.normal;
        }
    }

    public void SetSmoothCam(int input)
    {
        if (showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth)
        {
            camSmoothText.text = "Off";
            showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth = false;
        }
        else
        {
            camSmoothText.text = "On";
            showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth = true;
        }
    }

    public void AutoCurtains(int input)
    {
        for (int i = 0; i < showPanelUI.stages.Length; i++)
            if (showPanelUI.stages[i].curtainValves != null)
            {
                if (showPanelUI.stages[i].curtainValves.curtainOverride)
                {
                    curtainText.text = "Off";
                    showPanelUI.stages[i].curtainValves.curtainOverride = false;
                }
                else
                {
                    curtainText.text = "On";
                    showPanelUI.stages[i].curtainValves.curtainOverride = true;
                }
            }
    }

    public void PSIChange(int input)
    {
        showPanelUI.mackValves.GetComponent<Mack_Valves>().PSI =
            Mathf.Max(5, showPanelUI.mackValves.GetComponent<Mack_Valves>().PSI + input);
        psiText.text = showPanelUI.mackValves.GetComponent<Mack_Valves>().PSI + " PSI";
    }

    public void MusicVolumeChange(int input)
    {
        for (int i = 0; i < showPanelUI.speakerR.Length; i++)
            showPanelUI.speakerR[i].GetComponent<AudioSource>().volume += input * .05f;
        for (int i = 0; i < showPanelUI.speakerL.Length; i++)
            showPanelUI.speakerL[i].GetComponent<AudioSource>().volume += input * .05f;
        volumeText.GetComponent<Text>().text =
            Mathf.Ceil(showPanelUI.speakerR[0].GetComponent<AudioSource>().volume * 100).ToString();
    }


    public void SoundVolumeChange(int input)
    {
        AudioSource gg = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        AudioSource ga = GameObject.Find("GlobalAmbience").GetComponent<AudioSource>();
        gg.volume += input * .05f;
        ga.volume += input * .05f;
        soundvolumeText.GetComponent<Text>().text = Mathf.Ceil(gg.volume * 100).ToString();
    }

    public void SpacialToggle(int input)
    {
        if (showPanelUI.speakerR[0].GetComponent<AudioSource>().spatialBlend == 0)
        {
            for (int i = 0; i < showPanelUI.speakerR.Length; i++)
                showPanelUI.speakerR[i].GetComponent<AudioSource>().spatialBlend = 1;
            for (int i = 0; i < showPanelUI.speakerL.Length; i++)
                showPanelUI.speakerL[i].GetComponent<AudioSource>().spatialBlend = 1;
            spacialText.GetComponent<Text>().text = "On";
        }
        else
        {
            for (int i = 0; i < showPanelUI.speakerR.Length; i++)
                showPanelUI.speakerR[i].GetComponent<AudioSource>().spatialBlend = 0;
            for (int i = 0; i < showPanelUI.speakerL.Length; i++)
                showPanelUI.speakerL[i].GetComponent<AudioSource>().spatialBlend = 0;
            spacialText.GetComponent<Text>().text = "Off";
        }
    }

    /*
    public void FlowProfileUpDown(int input)
    {
        flowNumber = 0;
        flowProfile += input;
        if (flowProfile < 0) flowProfile = 0;
        if (flowProfile > showPanelUI.characters.Length - 1) flowProfile = showPanelUI.characters.Length - 1;
        FlowUpdate();
    }

    public void FlowNumberUpDown(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        flowNumber += input;
        if (flowNumber < 0) flowNumber = 0;
        if (flowNumber > theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().cylBit.Count - 1)
            flowNumber = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().cylBit.Count - 1;
        FlowUpdate();
    }
    
    

    public void FlowCopy()
    {
        copyPasteValues[0] = float.Parse(flowSpeedInText.text);
        copyPasteValues[1] = float.Parse(flowSpeedOutText.text);
        copyPasteValues[2] = float.Parse(flowWeightInText.text);
        copyPasteValues[3] = float.Parse(flowWeightOutText.text);
        copyPasteValues[4] = float.Parse(flowSlamInText.text);
        copyPasteValues[5] = float.Parse(flowSlamOutText.text);
        copyPasteValues[6] = float.Parse(flowSlamSpeedInText.text);
        copyPasteValues[7] = float.Parse(flowSlamSpeedOutText.text);
    }

    public void FlowPaste()
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlIn[flowNumber] =
            copyPasteValues[0];
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlOut[flowNumber] =
            copyPasteValues[1];
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScale[flowNumber] =
            copyPasteValues[2];
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScaleOut[flowNumber] =
            copyPasteValues[3];
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlIn[flowNumber] =
            copyPasteValues[4];
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlOut[flowNumber] =
            copyPasteValues[5];
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedIn[flowNumber] =
            copyPasteValues[6];
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedOut[flowNumber] =
            copyPasteValues[7];
        FlowUpdate();
    }

    public void FlowUpdate()
    {
        GameObject theCharacter = null;
        
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        flowProfileText.text = showPanelUI.characters[flowProfile].characterName;
        flowNumText.text =
            SearchBitChartName(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().cylBit[flowNumber],
                theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().cylDrawer[flowNumber]);
        flowSpeedInText.text = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>()
            .flowControlIn[flowNumber].ToString();
        flowSpeedOutText.text = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>()
            .flowControlOut[flowNumber].ToString();
        flowWeightInText.text = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>()
            .gravityScale[flowNumber].ToString();
        flowWeightOutText.text = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>()
            .gravityScaleOut[flowNumber].ToString();
        flowSlamInText.text = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>()
            .smashControlIn[flowNumber].ToString();
        flowSlamOutText.text = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>()
            .smashControlOut[flowNumber].ToString();
        flowSlamSpeedInText.text = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>()
            .smashSpeedIn[flowNumber].ToString();
        flowSlamSpeedOutText.text = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>()
            .smashSpeedOut[flowNumber].ToString();
    }
    
    */
    
    public void FlowUpdater(GameObject thetest)
    {
        string[] paths = new string[1];

        paths[0] = Application.dataPath + "/StreamingAssets/Flows/Default." + FileExtention;

        if (paths.Length > 0)
            if (paths[0] != "")
            {
                flowFormat thefile = flowFormat.ReadFromFile(paths[0]);
                for (int i = 0; i < thefile.characters.Length; i++)
                {
                    GameObject theCharacter = null;
                    if (thefile.characters[i] != null)
                    {
                        switch (thefile.characters[i].name)
                        {
                            case "Billy Bob":
                                thefile.characters[i].name = "Unknown Mech";
                                break;
                            case "Looney Bird":
                                thefile.characters[i].name = "Pizza Cam";
                                break;
                            case "Rolfe & Earl":
                                thefile.characters[i].name = "Chuck E. Cheese";
                                break;
                            case "Mitzi":
                                thefile.characters[i].name = "Helen Henny";
                                break;
                            case "Sun":
                                thefile.characters[i].name = "Building";
                                break;
                            case "Klunk":
                                thefile.characters[i].name = "Uncle Pappy";
                                break;
                            case "Beach Bear":
                                thefile.characters[i].name = "Jasper T. Jowls";
                                break;
                            case "Fatz":
                                thefile.characters[i].name = "Mr. Munch";
                                break;
                            case "Dook":
                                thefile.characters[i].name = "Pasqually";
                                break;
                        }
                        string finalname = thetest.transform.parent.name;
                        finalname = finalname.Replace("(Clone)", "").Trim();
                        
                        // Prefab names are now used for identifying character names, flows still use the old names.
                        switch (finalname) {
                               case "Building":
                                   finalname = "3-Stage Building";
                                   break;
                               case "Chuck E. Cheese":
                                   finalname = "3-Stage Chuck";
                                   break;
                               case "Helen Henny":
                                   finalname = "3-Stage Helen";
                                   break;
                               case "Jasper T. Jowls":
                                   finalname = "3-Stage Jasper";
                                   break;
                               case "Mr. Munch":
                                   finalname = "3-Stage Munch";
                                   break;
                               case "Pasqually":
                                   finalname = "3-Stage Pasqually";
                                   break;
                               case "Pizza Cam":
                                   finalname = "3-Stage Pizzacam";
                                   break;
                               case "Unused Mech":
                                   finalname = "3-Stage Unused Mech";
                                   break;
                               case "Wink":
                                   finalname = "3-Stage Wink";
                                   break;
                               case "Chuck E Cheese":
                                   finalname = "Cyberamic Chuck";
                                   break;
                               case "Flags":
                                   finalname = "Cyberamic Flag Wavers";
                                   break;
                               case "Guest":
                                   finalname = "Cyberamic Guest";
                                   break;
                               case "Jasper T Jowls":
                                   finalname = "Cyberamic Jasper";
                                   break;
                               case "Munch":
                                   finalname = "Cyberamic Munch";
                                   break;
                               case "Warblettes":
                                   finalname = "Cyberamic Warblettes";
                                   break;
                               case "Uncle Pappy":
                                   finalname = "Uncle Pappy";
                                   break;
                               case "Kooser Chuck":
                                   finalname = "Kooser Chuck";
                                   break;
                           }
                           

                        if (finalname == thefile.characters[i].name)
                        {
                            theCharacter = thetest;
                            Debug.Log("Flows: Character found: " + thefile.characters[i].name);
                        }
                        else
                        {
                            Debug.Log("Flows: Error: " + finalname + "!=" + thefile.characters[i].name);
                        }
                    }

                    if (theCharacter != null)
                    {
                        int extra = thefile.characters[i].flowsIn.Length / 2;
                        Character_Valves cv = theCharacter.transform.GetComponent<Character_Valves>();
                        if (extra < cv.flowControlIn.Length)
                            //Old File Format
                            for (int e = 0; e < cv.flowControlIn.Length; e++)
                            {
                                cv.flowControlIn[e] = thefile.characters[i].flowsIn[e] / 1000f;
                                cv.flowControlOut[e] = thefile.characters[i].flowsOut[e] / 1000f;
                                cv.gravityScale[e] = thefile.characters[i].weightIn[e] / 1000f;
                                cv.gravityScaleOut[e] = thefile.characters[i].weightOut[e] / 1000f;
                            }
                        else
                            //New File Format
                            for (int e = 0; e < extra; e++)
                            {
                                cv.flowControlIn[e] = thefile.characters[i].flowsIn[e] / 1000f;
                                cv.flowControlOut[e] = thefile.characters[i].flowsOut[e] / 1000f;
                                cv.gravityScale[e] = thefile.characters[i].weightIn[e] / 1000f;
                                cv.gravityScaleOut[e] = thefile.characters[i].weightOut[e] / 1000f;
                                cv.smashControlIn[e] = thefile.characters[i].flowsIn[e + extra] / 1000f;
                                cv.smashControlOut[e] = thefile.characters[i].flowsOut[e + extra] / 1000f;
                                cv.smashSpeedIn[e] = thefile.characters[i].weightIn[e + extra] / 1000f;
                                cv.smashSpeedOut[e] = thefile.characters[i].weightOut[e + extra] / 1000f;
                            }
                    }
                }
            }
    }

    public string SearchBitChartName(int bit, bool drawer)
    {
        if (drawer) bit += 150;
        UI_WindowMaker windowMaker = showPanelUI.GetComponent<UI_WindowMaker>();
        for (int i = 0; i < windowMaker.recordingGroups.Length; i++)
        for (int e = 0; e < windowMaker.recordingGroups[i].inputNames.Length; e++)
        {
            int finalBitNum = 0;
            if (windowMaker.recordingGroups[i].inputNames[e].drawer) finalBitNum += 150;
            if (windowMaker.recordingGroups[i].inputNames[e].index[0] + finalBitNum == bit)
                return windowMaker.recordingGroups[i].inputNames[e].name;
        }

        return "Nothing";
    }

    /// The old flow control system is deprecated and should not be used. Working on a new system in the future hopefully.
    /// 
    /*
    public void FlowInSpeedUpDown(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlIn[flowNumber] = Mathf.Max(0,
            (Mathf.Round(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlIn[flowNumber] *
                         100) + input) / 100.00f);
        FlowUpdate();
    }

    public void FlowOutSpeedUpDown(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlOut[flowNumber] = Mathf.Max(0,
            (Mathf.Round(
                 theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().flowControlOut[flowNumber] * 100) +
             input) / 100.00f);
        FlowUpdate();
    }

    public void FlowInWeightUpDown(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScale[flowNumber] = Mathf.Max(0,
            (Mathf.Round(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScale[flowNumber] *
                         100) + input) / 100.00f);
        FlowUpdate();
    }

    public void FlowOutWeightUpDown(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScaleOut[flowNumber] = Mathf.Max(0,
            (Mathf.Round(
                 theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().gravityScaleOut[flowNumber] *
                 100) +
             input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void SmashControlIn(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlIn[flowNumber] = Mathf.Max(0,
            (Mathf.Round(
                 theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlIn[flowNumber] * 100) +
             input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void SmashControlOut(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlOut[flowNumber] = Mathf.Max(0,
            (Mathf.Round(
                 theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashControlOut[flowNumber] *
                 100) +
             input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void SmashSpeedIn(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedIn[flowNumber] = Mathf.Max(0,
            (Mathf.Round(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedIn[flowNumber] *
                         100) + input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void SmashSpeedOut(int input)
    {
        GameObject theCharacter = null;
        foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
            if (child.name == showPanelUI.characters[flowProfile].characterName)
                theCharacter = child.gameObject;

        if (theCharacter == null) return;
        theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedOut[flowNumber] = Mathf.Max(0,
            (Mathf.Round(theCharacter.transform.GetChild(0).GetComponent<Character_Valves>().smashSpeedOut[flowNumber] *
                         100) + input) / 100.00f);
        ;
        FlowUpdate();
    }

    public void FlowSave(int input)
    {
        //Save to file
        Cursor.lockState = CursorLockMode.None;
        string path = StandaloneFileBrowser.SaveFilePanel("Save Flows", "", "MyFlows", FileExtention);
        Cursor.lockState = CursorLockMode.Locked;
        if (!string.IsNullOrEmpty(path))
        {
            //Gather Data
            var characterFlows = new flowControls[showPanelUI.characters.Length];
            for (int i = 0; i < characterFlows.Length; i++)
            {
                GameObject theCharacter = null;
                foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
                    if (child.name == showPanelUI.characters[i].characterName)
                        theCharacter = child.gameObject;

                if (theCharacter == null)
                {
                    Debug.Log("Error Character");
                }
                else
                {
                    characterFlows[i] = new flowControls();
                    characterFlows[i].name = showPanelUI.characters[i].characterName;
                    Character_Valves cv = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>();
                    characterFlows[i].flowsIn = new int[cv.flowControlIn.Length * 2];
                    characterFlows[i].flowsOut = new int[cv.gravityScaleOut.Length * 2];
                    characterFlows[i].weightIn = new int[cv.gravityScale.Length * 2];
                    characterFlows[i].weightOut = new int[cv.gravityScaleOut.Length * 2];
                    int extra = characterFlows[i].flowsIn.Length / 2;
                    for (int e = 0; e < extra; e++)
                    {
                        characterFlows[i].flowsIn[e] = Mathf.RoundToInt(cv.flowControlIn[e] * 1000);
                        characterFlows[i].flowsOut[e] = Mathf.RoundToInt(cv.flowControlOut[e] * 1000);
                        characterFlows[i].weightIn[e] = Mathf.RoundToInt(cv.gravityScale[e] * 1000);
                        characterFlows[i].weightOut[e] = Mathf.RoundToInt(cv.gravityScaleOut[e] * 1000);
                        characterFlows[i].flowsIn[e + extra] = Mathf.RoundToInt(cv.smashControlIn[e] * 1000);
                        characterFlows[i].flowsOut[e + extra] = Mathf.RoundToInt(cv.smashControlOut[e] * 1000);
                        characterFlows[i].weightIn[e + extra] = Mathf.RoundToInt(cv.smashSpeedIn[e] * 1000);
                        characterFlows[i].weightOut[e + extra] = Mathf.RoundToInt(cv.smashSpeedOut[e] * 1000);
                    }
                }
            }

            //Save
            flowFormat shw = new() { characters = characterFlows };
            shw.Save(path);
        }
        else
        {
            Debug.Log("No Save Path");
        }
    }
    
    */

    public void FlowLoad(int input)
    {
        StartCoroutine(FlowLoadRoutine(input));
    }

    private IEnumerator FlowLoadRoutine(int input)
    {
        string[] paths = new string[1];
        if (input == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            paths = StandaloneFileBrowser.OpenFilePanel("Browse Flow Controls", "", FileExtention, false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            paths[0] = Application.dataPath + "/StreamingAssets/Flows/Default." + FileExtention;
        }

        if (paths.Length > 0)
            if (paths[0] != "")
            {
                flowFormat thefile = flowFormat.ReadFromFile(paths[0]);
                for (int i = 0; i < thefile.characters.Length; i++)
                {
                    GameObject theCharacter = null;
                    foreach (Transform child in showPanelUI.stages[showPanelUI.currentStage].animatronics.transform)
                        if (thefile.characters[i] != null)
                        {
                            // I don't know what the fuck this does. You're on your own.
                            // If it ain't broken don't fix it.
                            switch (thefile.characters[i].name) {
                                    case "Billy Bob":
                                        thefile.characters[i].name = "3-Stage Unused Mech";
                                        break;
                                    case "Looney Bird":
                                        thefile.characters[i].name = "3-Stage Pizzacam";
                                        break;
                                    case "Rolfe & Earl":
                                    case "Chuck E. Cheese":
                                        thefile.characters[i].name = "3-Stage Chuck";
                                        break;
                                    case "Mitzi":
                                    case "Helen Henny":
                                        thefile.characters[i].name = "3-Stage Helen";
                                        break;
                                    case "Sun":
                                    case "Building":
                                        thefile.characters[i].name = "3-Stage Building";
                                        break;
                                    case "Klunk":
                                    case "Uncle Pappy":
                                        thefile.characters[i].name = "Uncle Pappy";
                                        break;
                                    case "Beach Bear":
                                    case "Jasper T. Jowls":
                                        thefile.characters[i].name = "3-Stage Jasper";
                                        break;
                                    case "Fatz":
                                    case "Mr. Munch":
                                        thefile.characters[i].name = "3-Stage Munch";
                                        break;
                                    case "Dook":
                                    case "Pasqually":
                                        thefile.characters[i].name = "3-Stage Pasqually";
                                        break;
                                    case "Pizza Cam":
                                        thefile.characters[i].name = "3-Stage Pizzacam";
                                        break;
                                    case "Unused Mech":
                                        thefile.characters[i].name = "3-Stage Unused Mech";
                                        break;
                                    case "Wink":
                                        thefile.characters[i].name = "3-Stage Wink";
                                        break;
                                    case "CU Flower":
                                        thefile.characters[i].name = "3-Stage Flower";
                                        break;
                            }

                            if (child.name == thefile.characters[i].name)
                            {
                                theCharacter = child.gameObject;
                                Debug.Log("Flows: Character found: " + thefile.characters[i].name);
                            }
                        }

                    if (theCharacter == null)
                    {
                        if (thefile.characters[i] != null)
                            Debug.Log("Flows: No Character named " + thefile.characters[i].name);
                        else
                            Debug.Log("Flows: Null Character. Just ignore probably.");
                    }
                    else
                    {
                        int extra = thefile.characters[i].flowsIn.Length / 2;
                        Character_Valves cv = theCharacter.transform.GetChild(0).GetComponent<Character_Valves>();
                        if (extra < cv.flowControlIn.Length)
                            //Old File Format
                            for (int e = 0; e < cv.flowControlIn.Length; e++)
                            {
                                cv.flowControlIn[e] = thefile.characters[i].flowsIn[e] / 1000f;
                                cv.flowControlOut[e] = thefile.characters[i].flowsOut[e] / 1000f;
                                cv.gravityScale[e] = thefile.characters[i].weightIn[e] / 1000f;
                                cv.gravityScaleOut[e] = thefile.characters[i].weightOut[e] / 1000f;
                            }
                        else
                            //New File Format
                            for (int e = 0; e < extra; e++)
                            {
                                cv.flowControlIn[e] = thefile.characters[i].flowsIn[e] / 1000f;
                                cv.flowControlOut[e] = thefile.characters[i].flowsOut[e] / 1000f;
                                cv.gravityScale[e] = thefile.characters[i].weightIn[e] / 1000f;
                                cv.gravityScaleOut[e] = thefile.characters[i].weightOut[e] / 1000f;
                                cv.smashControlIn[e] = thefile.characters[i].flowsIn[e + extra] / 1000f;
                                cv.smashControlOut[e] = thefile.characters[i].flowsOut[e + extra] / 1000f;
                                cv.smashSpeedIn[e] = thefile.characters[i].weightIn[e + extra] / 1000f;
                                cv.smashSpeedOut[e] = thefile.characters[i].weightOut[e + extra] / 1000f;
                            }
                    }
                }
            }
        yield return null;
    }
}