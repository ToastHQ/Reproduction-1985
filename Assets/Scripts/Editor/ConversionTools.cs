using UnityEngine;
using UnityEditor;
using Global;
using UnityEngine.SceneManagement;

public class ConversionTools : EditorWindow
{
    private LightController[] lightControllers;

    [MenuItem("Duofur/RR Conversion Tools")]
    public static void ShowWindow() => GetWindow<ConversionTools>().titleContent = new GUIContent("RR Conversion Tools");

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("It's best you read the documentation before using this tool. Making a backup of the active scene is recommended.", MessageType.Warning, true);
        GUILayout.Space(15);
        

        GUILayout.Label($"Scene ({SceneManager.GetActiveScene().name})", EditorStyles.boldLabel);
        if (GUILayout.Button("Convert old GameObject names (e.g Mack Valves \u2192 Mac Valves)") && ConfirmConversion("old","GameObject names"))
            ConvertGameObjectNames();
        
        GUILayout.Space(15);
        
        if (lightControllers == null)
            lightControllers = FindObjectsOfType<LightController>();
        
        GUILayout.Label($"Light Controllers ({lightControllers.Length})", EditorStyles.boldLabel);

        if (GUILayout.Button("Convert FadeSpeed \u2192 FadeTime") && ConfirmConversion("LightController","fadeSpeed"))
            ConvertFadeSpeed();

        if (GUILayout.Button("Convert TopOrBottom \u2192 Drawer Enum") && ConfirmConversion("LightController","topOrBottom"))
            ConvertTopOrBottom();

        if (GUILayout.Button("Merge IntensityMultiplier \u2192 Intensity") && ConfirmConversion("LightController","intensityMultiplier"))
            ConvertIntensityMultiplier();
        
        if (GUILayout.Button("Convert Flash & Strobe \u2192 LightMode") && ConfirmConversion("LightController","flash & strobe"))
            ConvertStrobeFlash();
        

    }

    private bool ConfirmConversion(string className, string action) =>
        EditorUtility.DisplayDialog("Confirm Conversion",
            $"This will convert all {className} {action} value/s permanently. Proceed?", "Yes", "No");

    
    // Scene Conversions ################################################################
    private void ConvertGameObjectNames()
    {
        GameObject mackValves = GameObject.Find("Mack Valves");
        if (mackValves != null)
        {
            mackValves.name = "Mac Valves";
        }
    }
    
    // Light Controller Conversions ################################################################
    private void ConvertFadeSpeed()
    {
        int convertedCount = 0;
        foreach (var controller in lightControllers)
        {
            if (controller.fadeSpeed != 0)
            {
                controller.fadeTime = 0.3f - controller.fadeSpeed;
                controller.fadeSpeed = 0;
                convertedCount++;
            }
        }
        Debug.Log($"fadeSpeed Conversion completed. {convertedCount} LightControllers updated.");
    }
    private void ConvertTopOrBottom()
    {
        int convertedCount = 0;
        foreach (var controller in lightControllers)
        {
            if (controller.topOrBottom == 'T') controller.drawer = Drawer.Top;
            else if (controller.topOrBottom == 'B') controller.drawer = Drawer.Bottom;

            controller.topOrBottom = '\0';
            convertedCount++;
        }
        Debug.Log($"topOrBottom Conversion completed. {convertedCount} LightControllers updated.");
    }

    private void ConvertIntensityMultiplier()
    {
        int convertedCount = 0;
        foreach (var controller in lightControllers)
        {
            if (controller.intensityMultiplier != 1)
            {
                controller.intensity *= controller.intensityMultiplier;
                controller.intensityMultiplier = 1;
                convertedCount++;
            }
        }
        Debug.Log($"intensityMultiplier Conversion completed. {convertedCount} LightControllers updated.");
    }
    
    private void ConvertStrobeFlash()
    {
        int convertedCount = 0;
        foreach (var controller in lightControllers)
        {
            if (controller.strobe)
            {
                controller.lightMode = LightController.LightMode.Strobe;
                controller.strobe = false;
                convertedCount++;
            }

            if (controller.flash)
            {
                controller.lightMode = LightController.LightMode.Flash;
                controller.flash = false;
                convertedCount++;
            }
        }
        Debug.Log($"Flash & Strobe Conversion completed. {convertedCount} LightControllers updated.");
    }
}