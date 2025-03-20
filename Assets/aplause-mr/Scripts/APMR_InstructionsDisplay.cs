using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;


public class APMR_InstructionsDisplay : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> TextBoxes;
   

    public void ShowInstructions(string instructions)
    {
        foreach (var t in TextBoxes)
        {
            t.text = instructions;
        }
        gameObject.SetActive(true);
    }

    public void HideInstructions()
    {
        foreach (var t in TextBoxes)
        {
            t.text = string.Empty;
        }
        gameObject.SetActive(false);
    }


}
