using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueUIManager : MonoBehaviour
{
    [Header("Dialogue Canvas")]
    public GameObject Canvas;

    [Header("Dialogue Text")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI ContentText;

    [Header("Dialogue Choice")]
    public GameObject ButtonColum;
    public GameObject ChoiceButtonPrefab;


    public void EnableUI()
    {
        Canvas.SetActive(true);
    }
    public void DisableUI()
    {
        Canvas.SetActive(false);
    }
    public void MakeButton(string buttonText,UnityAction action)
    {
        GameObject buttonGO=Instantiate(ChoiceButtonPrefab,ButtonColum.transform);
        buttonGO.SetActive(true);
        buttonGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonText;
        Button button =buttonGO.GetComponent<Button>();


        button.onClick.AddListener(action);
    }
    public void ClearAllButton()
    {
        for (int i = ButtonColum.transform.childCount-1; i >=1; i--)
        {
            Destroy(ButtonColum.transform.GetChild(i).gameObject);
        }
    }
}
