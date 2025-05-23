using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ICSMR_InstructionsDisplay : MonoBehaviour
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
