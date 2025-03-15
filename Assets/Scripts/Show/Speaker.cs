using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
    AudioSource audioSource;
    ShowController _showController;
    private void Start()
    {
        _showController = GameObject.FindGameObjectWithTag("Show Controller").GetComponent<ShowController>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (!_showController || !audioSource) return;
        
        if (_showController.playing)
        {
            audioSource.clip = _showController.referenceAudio.clip;
            audioSource.volume = _showController.referenceAudio.volume;
            audioSource.pitch = _showController.referenceAudio.pitch;
            audioSource.time = _showController.referenceAudio.time;
        }
    }

    void LateUpdate()
    {
        audioSource.enabled = _showController.referenceAudio.enabled;
    }
}
