using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public TextMeshProUGUI notifText;

    void Start()
    {
    
    }

    public IEnumerator SendNotification(string text, int time)
    {

        notifText.text = text;
        yield return new WaitForSeconds(time);
        notifText.text = "";

    }

    public void CallSend(string text, int time)
    {
        StartCoroutine(SendNotification(text, time));
    }
}
