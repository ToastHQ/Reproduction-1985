using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaybackUI : MonoBehaviour
{

    public Texture2D PlayIcon, PauseIcon;
    
    private VisualElement _root;

    private DF_ShowManager _showManager;
    private DF_ShowtapeManager _showtapeManager;
    private DF_ShowtapeCreator _showtapeCreator;

    private VisualElement _container;
    private Button _playbackButton, _reverseButton, _stopButton, _forwardButton, _curtainButton;
    private Slider _dataSlider, _volumeSlider;

    private void Awake()
    {
        GameObject ui = GameObject.Find("UI");
        _showManager = ui.GetComponent<DF_ShowManager>();
        _showtapeManager = ui.GetComponent<DF_ShowtapeManager>();
        _showtapeCreator = ui.GetComponent<DF_ShowtapeCreator>();
    }

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _container = _root.Q<VisualElement>("Playback");
        _container.RegisterCallback<PointerEnterEvent>(evt => ToggleUI(true));
        _container.RegisterCallback<PointerLeaveEvent>(evt => ToggleUI(false));

        _playbackButton = _container.Q<Button>("TogglePlayback");
        _reverseButton = _root.Q<Button>("ReverseShowtape");
        _stopButton = _root.Q<Button>("StopShowtape");
        _forwardButton = _root.Q<Button>("ForwardShowtape");
        _curtainButton = _root.Q<Button>("CurtainToggle");
        
        _dataSlider = _root.Q<Slider>("DataSlider");
        _volumeSlider = _root.Q<Slider>("VolumeSlider");


        _playbackButton.clicked += () => _showManager.TogglePlayback();
        _curtainButton.clicked += () => ToggleCurtains();
        _reverseButton.clicked += () => _showManager.FFSong(-1);
        _stopButton.clicked += () => _showManager.Stop();
        _forwardButton.clicked += () => _showManager.FFSong(1);

        UpdatePlaybackActivity();

        _showtapeManager.audioVideoPlay.AddListener(UpdatePlaybackActivity);
        _showtapeManager.audioVideoPause.AddListener(UpdatePlaybackActivity);
        
        _dataSlider.RegisterValueChangedCallback(evt => UpdateDataTime(evt.newValue));
        _volumeSlider.RegisterValueChangedCallback(evt => UpdateVolume(evt.newValue));


        _container.style.bottom = -93;

    }
    
    private void Update()
    {
        if (_showtapeManager.rshwData != null)
        {
            if (_showtapeManager.speakerClip.length > 0)
            {
                _dataSlider.value = (_showtapeManager.referenceSpeaker.time / _showtapeManager.speakerClip.length) * 100;
                _volumeSlider.value = _showManager.speakerL[0].volume * 100;
            }
            if (_showtapeManager.referenceSpeaker.isPlaying)
                _playbackButton.iconImage = PauseIcon;
            else
                _playbackButton.iconImage = PlayIcon;
        }
        else
        {
            _playbackButton.iconImage = PlayIcon;
        }
    }
    
    private void OnDisable()
    {
        _showtapeManager.audioVideoPlay.RemoveListener(UpdatePlaybackActivity);
        _showtapeManager.audioVideoPause.RemoveListener(UpdatePlaybackActivity);
    }

    private void ToggleUI(bool show)
    {
        float start = _container.style.bottom.value.value;
        float end = show ? 0 : -93;

        LeanTween.value(start, end, 0.2f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnUpdate(val => _container.style.bottom = val);
    }
    
    /// <summary>
    /// Updates the playback position of the showtape via the data slider
    /// </summary>
    /// <param name="value"></param>
    private void UpdateDataTime(float value)
    {
        if (_showtapeManager.speakerClip.length > 0)
        {
            _showtapeManager.referenceSpeaker.time = (value / 100) * _showtapeManager.speakerClip.length;
            for (int i = 0; i < _showManager.speakerL.Length; i++) _showManager.speakerL[i].time = (value / 100) * _showtapeManager.speakerClip.length;
            for (int i = 0; i < _showManager.speakerR.Length; i++) _showManager.speakerR[i].time = (value / 100) * _showtapeManager.speakerClip.length;
            
            if (_showtapeManager.videoPath != null)
                _showManager.videoplayer.time = (value / 100) * _showtapeManager.speakerClip.length;
        }
    }

    /// <summary>
    /// Updates the volume of the show speaker via the volume slider
    /// </summary>
    /// <param name="value"></param>
    private void UpdateVolume(float value)
    {
        if (_showtapeManager.speakerClip.length > 0)
        {
            for (int i = 0; i < _showManager.speakerL.Length; i++) _showManager.speakerL[i].volume = (value / 100);
            for (int i = 0; i < _showManager.speakerR.Length; i++) _showManager.speakerR[i].volume = (value / 100);
        }
    }

    /// <summary>
    /// Updates the Enabled properties of most interactive playback elements depending on if the show is playing or not
    /// </summary>
    private void UpdatePlaybackActivity()
    {
        bool isPlaying = _showtapeManager.playMovements;
        _reverseButton.SetEnabled(isPlaying);
        _stopButton.SetEnabled(isPlaying);
        _forwardButton.SetEnabled(isPlaying);
        _dataSlider.SetEnabled(isPlaying);
        _volumeSlider.SetEnabled(isPlaying);
    }
    
    
    private void ToggleCurtains()
    {
        foreach (StageSelector t in _showManager.stages)
            if (t.curtainValves != null)
            {
                if (t.curtainValves.curtainOverride)
                {
                    t.curtainValves.curtainOverride = false;
                    _curtainButton.text = "Open Curtains";
                }
                else
                {
                    t.curtainValves.curtainOverride = true;
                    _curtainButton.text = "Close Curtains";
                }
            }
    }
    
}
