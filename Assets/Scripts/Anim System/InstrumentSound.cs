using System.Collections;
using UnityEngine;

public class InstrumentSound : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip[] sound;
    public AudioClip[] altSound;
    public AudioSource source;
    public float volume = 1;
    public bool soundSwap;
    public bool swapDrawer;
    public int swapBit;
    private bool readyToGo;
    private MacValves valves;

    private void Awake()
    {
        StartCoroutine(AwakeCoroutine());
        if (soundSwap) valves = GameObject.Find("Mac Valves").GetComponent<MacValves>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (readyToGo)
        {
            if (soundSwap)
            {
                if (!swapDrawer && valves.topDrawer[swapBit - 1])
                    source.clip = altSound[Random.Range(0, altSound.Length - 1)];
                else if (swapDrawer && valves.bottomDrawer[swapBit - 1])
                    source.clip = altSound[Random.Range(0, altSound.Length - 1)];
                else
                    source.clip = sound[Random.Range(0, sound.Length - 1)];
            }
            else
            {
                source.clip = sound[Random.Range(0, sound.Length - 1)];
            }

            source.volume = volume;
            source.PlayOneShot(source.clip);
        }
    }

    private IEnumerator AwakeCoroutine()
    {
        yield return new WaitForSeconds(5f);
        readyToGo = true;
    }
}