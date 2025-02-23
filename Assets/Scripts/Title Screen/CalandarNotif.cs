using System;
using UnityEngine;
using UnityEngine.UI;

public class CalandarNotif : MonoBehaviour
{
    private void Awake()
    {
        if (GameVersion.gameName == "Faz-Anim")
        {
            if (DateTime.Now.Month == 8
                && DateTime.Now.Day == 8)
                GetComponent<Text>().text = "Happy Birthday FNaF!";
            else if (DateTime.Now.Month == 11
                     && DateTime.Now.Day == 11)
                GetComponent<Text>().text = "Happy Birthday FNaF 2!";
            else if (DateTime.Now.Month == 3
                     && DateTime.Now.Day == 2)
                GetComponent<Text>().text = "Happy Birthday FNaF 3!";
            else if (DateTime.Now.Month == 7
                     && DateTime.Now.Day == 23)
                GetComponent<Text>().text = "Happy Birthday FNaF 4!";
            else if (DateTime.Now.Month == 10
                     && DateTime.Now.Day == 7)
                GetComponent<Text>().text = "Happy Birthday Sister Location!";
            else if (DateTime.Now.Month == 1
                     && DateTime.Now.Day == 21)
                GetComponent<Text>().text = "Happy Birthday FNaF World!";
            else if (DateTime.Now.Month == 10
                     && DateTime.Now.Day == 30)
                GetComponent<Text>().text = "Happy Birthday FNaF 4 Halloween Edition!";
            else if (DateTime.Now.Month == 12
                     && DateTime.Now.Day == 4)
                GetComponent<Text>().text = "Happy Birthday FFPS!";
            else if (DateTime.Now.Month == 5
                     && DateTime.Now.Day == 28)
                GetComponent<Text>().text = "Happy Birthday FNaF VR: Help Wanted!";
            else if (DateTime.Now.Month == 8
                     && DateTime.Now.Day == 24)
                GetComponent<Text>().text = "Happy Birthday RR!";
            else if (DateTime.Now.Month == 1
                     && DateTime.Now.Day == 22)
                GetComponent<Text>().text = "Happy Birthday Faz-Anim!";
            else
                Destroy(gameObject.transform.parent.gameObject);
        }

        if (DateTime.Now.Month == 8
            && DateTime.Now.Day == 24)
            GetComponent<Text>().text = "Happy Birthday RR!";
        else if (DateTime.Now.Month == 1
                 && DateTime.Now.Day == 22)
            GetComponent<Text>().text = "Happy Birthday Faz-Anim!";
        else
            Destroy(gameObject.transform.parent.gameObject);
    }
}