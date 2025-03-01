using System;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    private bool _showFPS;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _showFPS = !_showFPS;
        }
    }
    
    void OnGUI()
    {
        if (_showFPS)
        {
            GUILayout.Label($"{Mathf.Round(1.0f / Time.deltaTime)}FPS [F1]");
        }
    }
}
