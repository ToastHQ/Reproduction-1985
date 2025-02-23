using UnityEngine;

public class CameraItem : MonoBehaviour
{
    public Texture2D image;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("Item: Camera") == 1) Destroy(gameObject);
    }

    public void Pickup(int input)
    {
        Player player = GameObject.Find("Player").GetComponent<Player>();
        Player player2;
        if (input == 0)
        {
            player.playerState = Player.PlayerState.frozenAllUnlock;
        }
        else
        {
            player2 = GameObject.Find("Player 2").GetComponent<Player>();
            player2.playerState = Player.PlayerState.frozenAllUnlock;
        }

        PlayerPrefs.SetInt("Item: Camera", 1);
        Destroy(gameObject);
    }
}