using UnityEngine;
using UnityEditor;
using System.Linq;
using Global;

public class ConversionTools : EditorWindow
{
    [MenuItem("Tools/RR Conversion Tools")]
    public static void ShowWindow()
    {
        var window = GetWindow<ConversionTools>();
        window.titleContent = new GUIContent("RR Conversion Tools");
    }

    private void OnGUI()
    {
        GUILayout.Label("Light Controllers", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Convert topOrBottom to Drawer Enum"))
        {
            if (EditorUtility.DisplayDialog("Confirm Conversion", 
                    "This will convert all LightController topOrBottom values to the Drawer enum, and empty out the value of topOrBottom. Proceed?", 
                    "Yes", "No"))
            {
                ConvertTopOrBottom();
            }
        }
    }

    private void ConvertTopOrBottom()
    {
        var lightControllers = FindObjectsOfType<LightController>();
        int convertedCount = 0;

        foreach (var controller in lightControllers)
        {
            if (controller.topOrBottom == 'T')
            {
                controller.drawer = Drawer.Top;
                convertedCount++;
            }
            else if (controller.topOrBottom == 'B')
            {
                controller.drawer = Drawer.Bottom;
                convertedCount++;
            }

            controller.topOrBottom = '\0';
        }
        
        Debug.Log($"Conversion completed. {convertedCount} LightControllers updated.");
    }
}