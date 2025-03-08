using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DF_WindowManager : MonoBehaviour
{
    DF_ShowManager _uiPlayRecord;
    
    public GameObject Viewport;
    
    [HideInInspector]
    public GameObject Loading;
    private GameObject ErrorWindow;
    private GameObject ThreeWindow;
    private GameObject TwoWindow;
    private GameObject PlayWindow;
    private GameObject NewRecordWindow;
    private GameObject GridWindow;
    private GameObject MoveTestWindow;
    private GameObject CustomiseWindow;
    private GameObject DeleteOne;

    public int deletePage;
    
    public MovementRecordings[] recordingGroups;

    [Header("Topbar")] 
    public TMP_Text Clock;
    
    [Header("Playback")]
    public PlayMenuManager playMenuManager;

    private void Awake()
    {
        _uiPlayRecord = GetComponent<DF_ShowManager>();
        
        Loading = Viewport.transform.Find("Loading Window").gameObject;
        ErrorWindow = Viewport.transform.Find("Error Window").gameObject;
        ThreeWindow = Viewport.transform.Find("3 Window").gameObject;
        TwoWindow = Viewport.transform.Find("2 Window").gameObject;
        PlayWindow = Viewport.transform.Find("Play Window").gameObject;
        NewRecordWindow = Viewport.transform.Find("New Record Window").gameObject;
        GridWindow = Viewport.transform.Find("Grid Window").gameObject;
        MoveTestWindow = Viewport.transform.Find("Move Test Window").gameObject;
        CustomiseWindow = Viewport.transform.Find("Customise Window").gameObject;
        DeleteOne = Viewport.transform.Find("Delete One Window").gameObject;
    }

    private void Update()
    {
        
        
        
        // Update Clock
        Clock.text = DateTime.Now.ToString("h:mm tt").ToUpper();
    }

    public void MakeErrorWindow(Exception exception)
    {
        ErrorWindow.SetActive(true);
        TMP_Text content = ErrorWindow.transform.Find("Content").GetComponent<TMP_Text>();
        content.text = exception.Message + "\n \n" + exception.StackTrace;
    }

    public void MakeThreeWindow(Sprite one, Sprite two, Sprite three, int switchBack, int switchOne, int switchTwo,
        int switchThree, string butOne, string butTwo, string butThree)
    {
        DisableWindows();
        ThreeWindow.SetActive(true);
        ThreeWindow.transform.Find("Container/Button1/Icon").GetComponent<Image>().sprite = one;
        ThreeWindow.transform.Find("Container/Button2/Icon").GetComponent<Image>().sprite = two;
        ThreeWindow.transform.Find("Container/Button3/Icon").GetComponent<Image>().sprite = three;
        ThreeWindow.transform.Find("Container/Button1").GetComponent<Button3D>().funcWindow = switchOne;
        ThreeWindow.transform.Find("Container/Button2").GetComponent<Button3D>().funcWindow = switchTwo;
        ThreeWindow.transform.Find("Container/Button3").GetComponent<Button3D>().funcWindow = switchThree;
        ThreeWindow.transform.Find("Container/Button1").GetChild(0).GetComponent<TMP_Text>().text = butOne;
        ThreeWindow.transform.Find("Container/Button2").GetChild(0).GetComponent<TMP_Text>().text = butTwo;
        ThreeWindow.transform.Find("Container/Button3").GetChild(0).GetComponent<TMP_Text>().text = butThree;

        if (switchBack == 0)
        {
            ThreeWindow.transform.Find("Back").gameObject.SetActive(false);
        }
        else
        {
            ThreeWindow.transform.Find("Back").gameObject.SetActive(true);
            ThreeWindow.transform.Find("Back").GetComponent<Button3D>().funcWindow = switchBack;
        }
    }

    public void MakeTwoWindow(Sprite one, Sprite two, int switchBack, int switchOne, int switchTwo, string butOne,
        string butTwo)
    {
        DisableWindows();
        TwoWindow.SetActive(true);
        TwoWindow.transform.Find("Container/Button1/Icon").GetComponent<Image>().sprite = one;
        TwoWindow.transform.Find("Container/Button2/Icon").GetComponent<Image>().sprite = two;
        TwoWindow.transform.Find("Container/Button1").GetComponent<Button3D>().funcWindow = switchOne;
        TwoWindow.transform.Find("Container/Button2").GetComponent<Button3D>().funcWindow = switchTwo;
        TwoWindow.transform.Find("Back").GetComponent<Button3D>().funcWindow = switchBack;
        TwoWindow.transform.Find("Container/Button1").GetChild(0).GetComponent<TMP_Text>().text = butOne;
        TwoWindow.transform.Find("Container/Button2").GetChild(0).GetComponent<TMP_Text>().text = butTwo;
    }
    public void MakePlayWindow(bool recording)
    {
        DisableWindows();
        PlayWindow.SetActive(true);
    }

    public void MakeNewRecordWindow()
    {
        DisableWindows();
        NewRecordWindow.SetActive(true);
    }

    public void MakeRecordIconsWindow()
    {
        DisableWindows();
        GridWindow.SetActive(true);
        GameObject templateButton = GridWindow.transform.Find("Container/Template").gameObject;

        foreach (Transform child in templateButton.transform.parent.transform) // Clear all buttons except the template
        {
            if (child != templateButton.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        for (int i = 0; i < recordingGroups.Length; i++)
        {
            GameObject button = Instantiate(templateButton, templateButton.transform.parent);
            button.SetActive(true);
            button.name = recordingGroups[i].groupName;
            button.transform.Find("Icon").gameObject.SetActive(true);
            button.transform.Find("Icon").GetComponent<Image>().sprite = recordingGroups[i].groupIcon;
            button.GetComponent<Button3D>().funcName = "RecordingGroupMenu";
            button.GetComponent<Button3D>().funcWindow = i;
            button.transform.Find("Text").GetComponent<TMP_Text>().text = recordingGroups[i].groupName;
            
        }
        
        templateButton.SetActive(false);
        GridWindow.transform.Find("Back").GetComponent<Button3D>().funcWindow = 4;
    }

    public void MakeCharacterCustomizeIconsWindow()
    {
        DisableWindows();
        GridWindow.SetActive(true);
        GameObject templateButton = GridWindow.transform.Find("Container/Template").gameObject;

        foreach (Transform child in templateButton.transform.parent.transform) // Clear all buttons except the template
        {
            if (child != templateButton.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        for (int i = 0; i < _uiPlayRecord.stages[_uiPlayRecord.currentStage].animatronics.Length; i++)
        {
            GameObject button = Instantiate(templateButton, templateButton.transform.parent);
            button.SetActive(true);
            button.name = _uiPlayRecord.stages[_uiPlayRecord.currentStage].animatronics[i].name;
            button.transform.Find("Icon").gameObject.SetActive(false);
            button.GetComponent<Button3D>().funcName = "CharacterCustomMenu";
            button.GetComponent<Button3D>().funcWindow = i;
            button.transform.Find("Text").GetComponent<TMP_Text>().text = _uiPlayRecord.stages[_uiPlayRecord.currentStage].animatronics[i].name;
            
        }
        
        templateButton.SetActive(false);
        GridWindow.transform.Find("Back").GetComponent<Button3D>().funcWindow = 4;
        
    }
    
    public void MakePropCustomizeIconsWindow()
    {
        DisableWindows();
        GridWindow.SetActive(true);
        
    }

    public void MakeMoveTestWindow(int currentGroup)
    {
        DisableWindows();
        MoveTestWindow.transform.Find("Ready").GetComponent<InputSetter>().mapping = currentGroup + 1;
        MoveTestWindow.SetActive(true);
        for (int i = 0; i < 33; i++)
            if (i < recordingGroups[currentGroup].inputNames.Length)
            {
                GameObject button = MoveTestWindow.transform.Find("Button " + i).gameObject;

                button.SetActive(true);
                button.transform.transform.Find("Text").GetComponent<Text>().text =
                    recordingGroups[currentGroup].inputNames[i].name;
            }
            else
            {
                MoveTestWindow.transform.Find("Button " + i).gameObject.SetActive(false);
            }

        MoveTestWindow.transform.Find("Back Button").GetComponent<Button3D>().funcWindow = 21;
    }

    public void MakeCharacterCustomizeWindow(int current)
    {
        
        
        DisableWindows();
        CustomiseWindow.SetActive(true);
        
        AnimatronicData currentAnimatronic = _uiPlayRecord.stages[_uiPlayRecord.currentStage].animatronics[current];
        
        if (currentAnimatronic.currentCostume == currentAnimatronic.costumes.Length - 1)
            CustomiseWindow.transform.Find("Down").gameObject.SetActive(false);
        else
            CustomiseWindow.transform.Find("Down").gameObject.SetActive(true);
        if (currentAnimatronic.currentCostume == -1)
            CustomiseWindow.transform.Find("Up").gameObject.SetActive(false);
        else
            CustomiseWindow.transform.Find("Up").gameObject.SetActive(true);
        if (currentAnimatronic.currentCostume != -1)
        {
            CustomiseWindow.transform.Find("Total").gameObject.GetComponent<TMP_Text>().text = ((1 + currentAnimatronic.currentCostume) + "/" + currentAnimatronic.costumes.Length);
            CustomiseWindow.transform.Find("Name").gameObject.GetComponent<TMP_Text>().text =
                currentAnimatronic.costumes[currentAnimatronic.currentCostume].name;
        }
        else
        {
            CustomiseWindow.transform.Find("Total").gameObject.GetComponent<TMP_Text>().text = "";
            CustomiseWindow.transform.Find("Name").gameObject.GetComponent<TMP_Text>().text = "Hidden";
            CustomiseWindow.transform.Find("Description").gameObject.GetComponent<TMP_Text>().text =
                "No character is present.";
            CustomiseWindow.transform.Find("Down").gameObject.GetComponent<Button3D>().funcWindow = current;
            CustomiseWindow.transform.Find("Up").gameObject.GetComponent<Button3D>().funcWindow = current;
        }
        
    }

    public void MakeStageCustomizeWindow(StageSelector[] stages, int current)
    {
        DisableWindows();
        CustomiseWindow.SetActive(true);
        
        CustomiseWindow.transform.Find("Total").gameObject.GetComponent<TMP_Text>().text =
            ((1 + current) + "/" + stages.Length);
        CustomiseWindow.transform.Find("Name").gameObject.GetComponent<TMP_Text>().text = stages[current].stageName;
        CustomiseWindow.transform.Find("Down").gameObject.GetComponent<Button3D>().funcWindow = current;
        CustomiseWindow.transform.Find("Up").gameObject.GetComponent<Button3D>().funcWindow = current;
    }

    public void MakeDeleteMoveMenu(int page)
    {
        DisableWindows();
        DeleteOne.SetActive(true);
        deletePage += page;
        if (deletePage < 0) deletePage = 0;
        if (deletePage > 12) deletePage = 12;
        DeleteOne.transform.Find("pagenum").gameObject.GetComponent<Text>().text = deletePage + " / 12";
        for (int i = 0; i < 24; i++)
            DeleteOne.transform.Find("DLMV (" + i + ")").gameObject.transform.GetChild(0).GetComponent<Text>().text =
                SearchBitChartName(i + 1 + 24 * deletePage);
    }

    public string SearchBitChartName(int bit)
    {
        for (int i = 0; i < recordingGroups.Length; i++)
        for (int e = 0; e < recordingGroups[i].inputNames.Length; e++)
        {
            int finalBitNum = 0;
            if (recordingGroups[i].inputNames[e].drawer) finalBitNum += 150;
            if (recordingGroups[i].inputNames[e].index[0] + finalBitNum == bit)
                return bit + "- " + recordingGroups[i].groupName + " - " + recordingGroups[i].inputNames[e].name;
        }

        return bit + "- Nothing";
    }

    public int SearchBitChartGroupID(int bit)
    {
        for (int i = 0; i < recordingGroups.Length; i++)
        for (int e = 0; e < recordingGroups[i].inputNames.Length; e++)
        {
            int finalBitNum = 0;
            if (recordingGroups[i].inputNames[e].drawer) finalBitNum += 150;
            if (recordingGroups[i].inputNames[e].index[0] + finalBitNum == bit) return i;
        }

        return 0;
    }

    private void DisableWindows()
    {
        for (int i = 0; i < Viewport.transform.childCount; i++)
            if (Viewport.transform.GetChild(i).gameObject.activeSelf)
            {
                Viewport.transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
    }

    [Serializable]
    public class MovementRecordings
    {
        public string groupName;
        public Sprite groupIcon;
        public inputNames[] inputNames;
    }

    [Serializable]
    public class inputNames
    {
        public string name;
        public bool drawer;
        public int[] index;
    }
}