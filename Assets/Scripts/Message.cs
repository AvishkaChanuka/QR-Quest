using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI messageBody;

    public void ShowMessage(string message)
    {
        messageBody.text = message;
        gameObject.SetActive(true);
    }
    public void HideMessage()
    {
        gameObject.SetActive(false);
    }
}
