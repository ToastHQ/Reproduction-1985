using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WindowMaker : MonoBehaviour
{
    public GameObject Viewport;
    
    [HideInInspector]
    public GameObject Loading;
    private GameObject ErrorWindow;
    private GameObject ThreeWindow;
    private GameObject TwoWindow;
    private GameObject PlayWindow;
    private GameObject NewRecordWindow;
    private GameObject RecordIconsWindow;
    private GameObject MoveTestWindow;
    private GameObject CharacterCustomizeWindow;
    private GameObject StageCustomizeWindow;
    private GameObject DeleteOne;

    public int deletePage;
    
    public MovementRecordings[] recordingGroups;
    public ShowtapeYear[] allShowtapes;

    [Header("Topbar")] 
    public TMP_Text Clock;
    
    [Header("Playback")]
    public PlayMenuManager playMenuManager;

    private void Awake()
    {
        Loading = Viewport.transform.Find("Loading Window").gameObject;
        ErrorWindow = Viewport.transform.Find("Error Window").gameObject;
        ThreeWindow = Viewport.transform.Find("3 Window").gameObject;
        TwoWindow = Viewport.transform.Find("2 Window").gameObject;
        PlayWindow = Viewport.transform.Find("Play Window").gameObject;
        NewRecordWindow = Viewport.transform.Find("New Record Window").gameObject;
        RecordIconsWindow = Viewport.transform.Find("Record Icons Window").gameObject;
        MoveTestWindow = Viewport.transform.Find("Move Test Window").gameObject;
        CharacterCustomizeWindow = Viewport.transform.Find("Character Customise Window").gameObject;
        StageCustomizeWindow = Viewport.transform.Find("Stage Customise Window").gameObject;
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
        RecordIconsWindow.SetActive(true);

        for (int i = 0; i < 24; i++)
            if (i < recordingGroups.Length)
            {
                GameObject button = RecordIconsWindow.transform.Find("Button (" + i + ")").gameObject;
                button.SetActive(true);
                button.GetComponent<Image>().sprite = recordingGroups[i].groupIcon;
                button.transform.GetChild(0).GetComponent<Button3D>().funcName = "RecordingGroupMenu";
                button.transform.GetChild(0).GetComponent<Button3D>().funcWindow = i;
                button.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text =
                    recordingGroups[i].groupName;
            }
            else
            {
                RecordIconsWindow.transform.Find("Button (" + i + ")").gameObject.SetActive(false);
            }

        RecordIconsWindow.transform.Find("Back Button").GetComponent<Button3D>().funcWindow = 4;
    }

    public void MakeCharacterCustomizeIconsWindow(CharacterSelector[] characters)
    {
        DisableWindows();
        RecordIconsWindow.SetActive(true);

        int currentButton = 0;
        for (int i = 0; i < 24; i++)
            if (i < characters.Length && characters[i].allCostumes.Length > 1)
            {
                GameObject button = RecordIconsWindow.transform.Find("Button (" + currentButton + ")").gameObject;
                button.SetActive(true);
                button.transform.GetChild(0).GetComponent<Button3D>().funcName = "CharacterCustomMenu";
                button.transform.GetChild(0).GetComponent<Button3D>().funcWindow = i;
                button.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text =
                    characters[i].characterName;
                currentButton++;
            }

        for (int i = currentButton; i < 24; i++)
            RecordIconsWindow.transform.Find("Button (" + i + ")").gameObject.SetActive(false);
        RecordIconsWindow.transform.Find("Back Button").GetComponent<Button3D>().funcWindow = 8;
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

    public void MakeCharacterCustomizeWindow(CharacterSelector[] characters, int current)
    {
        DisableWindows();
        CharacterCustomizeWindow.SetActive(true);
        if (characters[current].currentCostume == characters[current].allCostumes.Length - 1)
            CharacterCustomizeWindow.transform.Find("Down").gameObject.SetActive(false);
        else
            CharacterCustomizeWindow.transform.Find("Down").gameObject.SetActive(true);
        if (characters[current].currentCostume == -1)
            CharacterCustomizeWindow.transform.Find("Up").gameObject.SetActive(false);
        else
            CharacterCustomizeWindow.transform.Find("Up").gameObject.SetActive(true);
        if (characters[current].currentCostume != -1)
        {
            CharacterCustomizeWindow.transform.Find("Full Costume").gameObject.GetComponent<Text>().text =
                characters[current].allCostumes.Length.ToString();
            CharacterCustomizeWindow.transform.Find("Current Costume").gameObject.GetComponent<Text>().text =
                (1 + characters[current].currentCostume).ToString();
            CharacterCustomizeWindow.transform.Find("Name").gameObject.GetComponent<Text>().text =
                characters[current].allCostumes[characters[current].currentCostume].costumeName;
            CharacterCustomizeWindow.transform.Find("Type").gameObject.GetComponent<Text>().text = characters[current]
                .allCostumes[characters[current].currentCostume].costumeType.ToString();
            CharacterCustomizeWindow.transform.Find("Description").gameObject.GetComponent<Text>().text =
                characters[current].allCostumes[characters[current].currentCostume].costumeDesc;
            CharacterCustomizeWindow.transform.Find("Year").gameObject.GetComponent<Text>().text =
                characters[current].allCostumes[characters[current].currentCostume].yearOfCostume;
            CharacterCustomizeWindow.transform.Find("Down").gameObject.GetComponent<Button3D>().funcWindow = current;
            CharacterCustomizeWindow.transform.Find("Up").gameObject.GetComponent<Button3D>().funcWindow = current;
            CharacterCustomizeWindow.transform.Find("Icon").gameObject.GetComponent<RawImage>().texture =
                characters[current].allCostumes[characters[current].currentCostume].costumeIcon.texture;
        }
        else
        {
            CharacterCustomizeWindow.transform.Find("Full Costume").gameObject.GetComponent<Text>().text =
                characters[current].allCostumes.Length.ToString();
            CharacterCustomizeWindow.transform.Find("Current Costume").gameObject.GetComponent<Text>().text =
                (1 + characters[current].currentCostume).ToString();
            CharacterCustomizeWindow.transform.Find("Name").gameObject.GetComponent<Text>().text = "None";
            CharacterCustomizeWindow.transform.Find("Type").gameObject.GetComponent<Text>().text = "";
            CharacterCustomizeWindow.transform.Find("Description").gameObject.GetComponent<Text>().text =
                "No character is present.";
            CharacterCustomizeWindow.transform.Find("Year").gameObject.GetComponent<Text>().text = "";
            CharacterCustomizeWindow.transform.Find("Down").gameObject.GetComponent<Button3D>().funcWindow = current;
            CharacterCustomizeWindow.transform.Find("Up").gameObject.GetComponent<Button3D>().funcWindow = current;
            CharacterCustomizeWindow.transform.Find("Icon").gameObject.GetComponent<RawImage>().texture =
                characters[current].allCostumes[characters[current].currentCostume + 1].costumeIcon.texture;
        }
    }

    public void MakeStageCustomizeWindow(StageSelector[] stages, int current)
    {
        DisableWindows();
        StageCustomizeWindow.SetActive(true);
        StageCustomizeWindow.transform.Find("Full Costume").gameObject.GetComponent<Text>().text =
            stages.Length.ToString();
        StageCustomizeWindow.transform.Find("Current Costume").gameObject.GetComponent<Text>().text =
            (1 + current).ToString();
        StageCustomizeWindow.transform.Find("Name").gameObject.GetComponent<Text>().text = stages[current].stageName;
        StageCustomizeWindow.transform.Find("Type").gameObject.GetComponent<Text>().text =
            stages[current].stageType.ToString();
        StageCustomizeWindow.transform.Find("Description").gameObject.GetComponent<Text>().text =
            stages[current].stageDesc;
        StageCustomizeWindow.transform.Find("Year").gameObject.GetComponent<Text>().text = stages[current].stageDate;
        StageCustomizeWindow.transform.Find("Type").gameObject.GetComponent<Text>().text =
            stages[current].stageType.ToString();
        StageCustomizeWindow.transform.Find("Down").gameObject.GetComponent<Button3D>().funcWindow = current;
        StageCustomizeWindow.transform.Find("Up").gameObject.GetComponent<Button3D>().funcWindow = current;
        StageCustomizeWindow.transform.Find("Icon").gameObject.GetComponent<RawImage>().texture =
            stages[current].stageIcon.texture;
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

[Serializable]
public class ShowtapeYear
{
    public ShowTapeSelector[] groups;
}

[Serializable]
public class ShowTapeSelector
{
    public string showtapeName;
    public string showtapeDate;
    public string showtapeLength;
    public string ytLink;
}