using UnityEngine;
using UnityEngine.UIElements;

public class ControlUI : MonoBehaviour
{
    private VisualElement _root;
    
    private VisualElement _popup;
    private VisualElement _warningPopup;
    private Button _warningProceed, _warningCancel;

    private DF_ShowController _showManager;
    private DF_ShowtapeManager _showtapeManager;

    private bool _warningResult; // Track the result

    private void Awake()
    {
        GameObject ui = GameObject.Find("Scripts");
        _showManager = GameObject.FindGameObjectWithTag("Show Controller").GetComponent<DF_ShowController>();
        _showtapeManager = GameObject.FindGameObjectWithTag("Showtape Manager").GetComponent<DF_ShowtapeManager>();
    }

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        
        _popup = _root.Q<VisualElement>("Popup");
        _popup.visible = false;
        _warningPopup = _popup.Q<VisualElement>("Warning");
        _warningPopup.visible = false;
        _warningProceed = _warningPopup.Q<Button>("WarningProceed");
        _warningCancel = _warningPopup.Q<Button>("WarningCancel");

        _warningProceed.clicked += OnWarningProceedClicked;
        _warningCancel.clicked += OnWarningCancelClicked;
    }

    private void OnWarningProceedClicked()
    {
        _warningResult = true;
        _popup.visible = false;
        _warningPopup.visible = false;
    }

    private void OnWarningCancelClicked()
    {
        _warningResult = false;
        _popup.visible = false;
        _warningPopup.visible = false;
    }

    public bool DisplayWarning(string content)
    {
        _warningPopup.Q<Label>("Content").text = content;
        _popup.visible = true;
        _warningPopup.visible = true;
        return _warningResult;
    }
}