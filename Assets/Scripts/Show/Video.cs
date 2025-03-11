using UnityEngine;

namespace Show
{
    public class Video : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        DF_ShowController showController;
        MeshRenderer renderer;

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            showController = GameObject.FindGameObjectWithTag("Show Controller").GetComponent<DF_ShowController>();
            renderer = gameObject.GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (showController.active)
                renderer.material.SetColor(BaseColor, Color.black);
            else
                renderer.material.SetColor(BaseColor, Color.white);
        }
    }
}



