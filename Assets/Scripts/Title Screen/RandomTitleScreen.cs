using UnityEngine;
using UnityEngine.UI;

public class RandomTitleScreen : MonoBehaviour
{
    public Texture2D[] characters;

    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<RawImage>().texture = characters[Random.Range(0, characters.Length)];
        Destroy(GetComponent<RandomTitleScreen>());
    }
}