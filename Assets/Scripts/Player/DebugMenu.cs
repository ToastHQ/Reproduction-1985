using System;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    private bool _showHardwareDebugger;
    private bool _showFPS;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _showFPS = !_showFPS;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            _showHardwareDebugger = !_showHardwareDebugger;
        }
    }
    
    private Rect _windowRect = new Rect (20, 20, 300, Screen.height - 20);

    void OnGUI()
    {
        if (_showHardwareDebugger)
        {
            GUILayout.Label("\nHardware Debugger [F2]");
            GUILayout.Space(2);
            GUILayout.Label("\n========= general =========");
            GUILayout.Label($"unity version: {Application.unityVersion}");
            GUILayout.Label($"platform: {Application.platform}");
        
            // graphics info
            GUILayout.Label("========= graphics =========");
            GUILayout.Label($"gpu: {SystemInfo.graphicsDeviceName}");
            GUILayout.Label($"gpu vendor: {SystemInfo.graphicsDeviceVendor}");
            GUILayout.Label($"gpu version: {SystemInfo.graphicsDeviceVersion}");
            GUILayout.Label($"gpu memory: {SystemInfo.graphicsMemorySize} mb");
            GUILayout.Label($"shader level: {SystemInfo.graphicsShaderLevel}");
            GUILayout.Label($"multi-threaded rendering: {SystemInfo.graphicsMultiThreaded}");
            GUILayout.Label($"rendering api: {SystemInfo.graphicsDeviceType}");

            // cpu info
            GUILayout.Label("\n========= cpu =========");
            GUILayout.Label($"processor type: {SystemInfo.processorType}");
            GUILayout.Label($"processor count: {SystemInfo.processorCount}");
            GUILayout.Label($"system memory: {SystemInfo.systemMemorySize} mb");
            GUILayout.Label($"operating system: {SystemInfo.operatingSystem}");
            GUILayout.Label($"os family: {SystemInfo.operatingSystemFamily}");

            // audio info
            GUILayout.Label("\n========= audio =========");
            GUILayout.Label($"dsp buffer size: {AudioSettings.GetConfiguration().dspBufferSize}");
            GUILayout.Label($"sample rate: {AudioSettings.outputSampleRate}");
            GUILayout.Label($"speaker mode: {AudioSettings.speakerMode}");

            // screen info
            GUILayout.Label("\n========= screen =========");
            GUILayout.Label($"resolution: {Screen.currentResolution.width} x {Screen.currentResolution.height} @ {Screen.currentResolution.refreshRateRatio} hz");
            GUILayout.Label($"fullscreen: {Screen.fullScreen}");
            GUILayout.Label($"dpi: {Screen.dpi}");
            GUILayout.Label($"screen orientation: {Screen.orientation}");
        }
        if (_showFPS)
        {
            GUILayout.Label($"{Mathf.Round(1.0f / Time.deltaTime)}FPS [F1]");
        }
    }
}
