using System.Collections;
using UnityEngine;

public class APMR_SceneController : MonoBehaviour
{


    [Header("Scene References")]
    
    [SerializeField]
    private APMR_InstructionsDisplay instructionsDisplay = null;

    private AudioSource audioSource = null;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PrepareSceneCondition(APMR_ExperimentRunner.Condition condition)
    {
        Debug.Log("Prepare scene for trial with condition " + condition);

        SetAvatarsVisible(true);

    }

    public void ClearScene()
    {

        SetAvatarsVisible(false);
        MuteAudioSources();

    }

    public void DisplayInstructions(string instructionString)
    {
        Debug.Log("Instructions shown: " + instructionString);
        instructionsDisplay.ShowInstructions(instructionString);
    }
    public void HideInstructions()
    {
        instructionsDisplay.HideInstructions();
    }
    public void DisplayInstructionsForDuration(string instructionString, float duration)
    {
        instructionsDisplay.ShowInstructions(instructionString);
        StartCoroutine(HideInstructionsAfterSeconds(duration));
    }
    public IEnumerator HideInstructionsAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        instructionsDisplay.HideInstructions();
    }


    void SetAvatarsVisible(bool shouldBeVisible)
    {
        int visibleLayer = 0;
        int invisibleLayer = 3;
        int layer = shouldBeVisible ? visibleLayer : invisibleLayer;


 
        // TODO how to locate avatar root GOs?
        /*
        foreach (Transform child in trackingAreaTransform)
        {
            if (child.name.Contains("[Remote]"))
            {
                UtilityFunctions.SetLayerRecursively(child.gameObject, layer);
            }
        }
        */
    }


    public void MuteAudioSources()
    {
        var audioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (var audioSource in audioSources)
        {
            Debug.Log("muting audio source " + audioSource.gameObject.name);
            audioSource.gameObject.GetComponent<AudioSource>().volume = 0.0f;
        }

    }

    public void PlayWarningSound()
    {
        string resourcePath = "Sounds/timeout_sig";

        // Load audio clip from resources folder
        AudioClip audioClip = Resources.Load<AudioClip>(resourcePath);

        // play loaded clip
        audioSource.clip = audioClip;
        audioSource.Play();

    }
}
