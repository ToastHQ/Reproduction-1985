using UnityEngine;
using UnityEngine.UI;

public class BitVisualization : MonoBehaviour
{
    public Texture2D texture;
    public MacValves mackvalves;
    public Color32[] colors;
    public Color offColor;
    public Color onColor;
    private RawImage image;

    private void Start()
    {
        colors = new Color32[20 * 60];
        image = GetComponent<RawImage>();
        texture = new Texture2D(20, 60, TextureFormat.ARGB32, false, false);
        texture.anisoLevel = 0;
        texture.filterMode = FilterMode.Point;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (mackvalves != null)
        {
            int e = 0;
            for (int i = 0; i < colors.Length; i++)
                if (i % 2 == 0 && i / 20 % 2 == 0)
                {
                    if (e < 150)
                        switch (mackvalves.topDrawer[e])
                        {
                            case true:
                                colors[i] = onColor;
                                break;
                            case false:
                                colors[i] = offColor;
                                break;
                        }
                    else if (e < 300)
                        switch (mackvalves.bottomDrawer[e - 150])
                        {
                            case true:
                                colors[i] = onColor;
                                break;
                            case false:
                                colors[i] = offColor;
                                break;
                        }

                    e++;
                }

            texture.SetPixels32(colors);
            texture.Apply();
            image.texture = texture;
        }
        else
        {
            for (int i = 0; i < colors.Length; i++)
                if (i % 2 == 0 && i / 20 % 2 == 0)
                    colors[i] = offColor;

            texture.SetPixels32(colors);
            texture.Apply();
            image.texture = texture;
        }
    }
}