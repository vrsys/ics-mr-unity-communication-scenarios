using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class APMR_ExperimentRunner : NetworkBehaviour
{

    [Header("Experiment Run Data")]

    [SerializeField]
    [Tooltip("Choose whether this should be run in networked mode")]
    private bool runInNetworkedMode = false;


    [SerializeField]
    [Tooltip("The index of the interlocutor (participant) within their group")]
    private int interlocutorId = -1;

    [SerializeField]
    [Tooltip("The index of the group")]
    private int groupId;


    [SerializeField]
    [Tooltip("Skip some trials, e.g. when restarting an experiment")]
    private int trialsToSkip = 0;

    [SerializeField]
    [Tooltip("Ouput directory where data will be written")]
    private string outputDataDirectory = "";

    [SerializeField]
    private bool startExperimentWithTrainingPhase = true;

    

    [Header("General Experiment Data")]



    public float maxTrialDurationMins = 8f;
    public float playTrialEndWarningSoundAfterMins   = 7f;

    private float maxTrialDurationSecs = 0;
    private float playTrialEndWarningAfterSecs = 0;




    public enum Condition
    {
        FIRST_CONDITION,
        SECOND_CONDITION,
        THIRD_CONDITION,
        FOURTH_CONDITION
    }

    // for counterbalancing, set condition order depending on group index
    public List<Condition> conditionOrder = new List<Condition> { Condition.FIRST_CONDITION, Condition.SECOND_CONDITION, Condition.THIRD_CONDITION, Condition.FOURTH_CONDITION };

    public struct TrialData
    {
        public int trialNumber;
        public float trialDuration;
    }

    [Header("Scene References")]


    public APMR_SceneController sceneController;



    private int numberOfTrials;
    private string studyDataFilePathBase = "";
    private bool studyDataCanBeWritten = false;


    private bool shouldAdvanceExperiment = false;
    private bool experimentStarted = false;

    private float trialStartTime = 0.0f;
    private float trialDuration = 0.0f;


    //public ExperimentSettings experimentSettings;

    //public int numberOfInterlocutorsExpected = 4;

    public APMR_ExperimentInstructions experimentInstructions;

    private string advanceHint = "(Press Space to advance)";

    void Start()
    {
        Initialize();

        maxTrialDurationSecs = 60 * maxTrialDurationMins;
        playTrialEndWarningAfterSecs = 60 * playTrialEndWarningSoundAfterMins;
    }

    void Initialize() 
    {

        //InitializeStudyVariablesFromExperimentSettings();

        // TODO get interlocutor ID from user prefs
        //GetInterlocutorIdFromProjectionWallSettings();

        var dt = System.DateTime.Now;
        string dateString = dt.ToString("yyyyMMdd_HH_mm");
        studyDataFilePathBase = outputDataDirectory + "/aplausemr_group" + groupId + "_participant" + interlocutorId + "_date" + dateString;

        TestStudyDataPath();

        numberOfTrials = System.Enum.GetValues(typeof(Condition)).Length;

        sceneController.ClearScene();
        sceneController.DisplayInstructions("Experiment ready.\nPress Enter to start.");

    }

    // Reads experiment variables that are stored in JSON experiment settings config file
    void InitializeStudyVariablesFromExperimentSettings()
    {
        // TODO read these from UserPrefs

        //if (!experimentSettings.hasRead)
        //    experimentSettings.Read();

        //groupId = experimentSettings.config.groupId;
        //responseDirectory = experimentSettings.config.responseDirectory;
        //trialsToSkip = experimentSettings.config.trialsToSkip;

        //Debug.Log("Load group: " + groupId);
        //Debug.Log("Response directory: " + responseDirectory);
        //Debug.Log("Num trials to skip: " + trialsToSkip);

    }


    private void TestStudyDataPath()
    {
        string testFile = outputDataDirectory + "/test.txt";
        try
        {
            using (StreamWriter writer = new StreamWriter(testFile, true))
            {
                writer.WriteLine("test");
            }
            studyDataCanBeWritten = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error with data output filepath: " + e);
        }
    }


    private IEnumerator RunUntilAdvanceNotification()
    {
        while (!shouldAdvanceExperiment)
        {
            CheckForAdvanceKeypress();
            yield return null;
        }
        shouldAdvanceExperiment = false;
    }

    private IEnumerator ShowSeriesOfInstructionsUntilAdvance(List<string> instructionList)
    {
        foreach (string instruction in instructionList)
        {
            yield return ShowInstructionsUntilAdvance(instruction);
        }
    }


    private IEnumerator ShowInstructionsUntilAdvance(string instructions)
    {
        sceneController.DisplayInstructions(instructions + "\n\n" + advanceHint);
        yield return RunUntilAdvanceNotification();
        sceneController.HideInstructions();
    }
    private IEnumerator ShowInstructionsForSeconds(string instructions, float seconds)
    {
        sceneController.DisplayInstructions(instructions);
        yield return new WaitForSeconds(seconds);
        sceneController.HideInstructions();
    }   

    private IEnumerator ExperimentCoroutine()
    {

        experimentInstructions.StartExperiment(groupId);

        sceneController.StartExperiment();

        yield return ShowInstructionsUntilAdvance("Welcome to the experiment.");

        if (startExperimentWithTrainingPhase)
        {
            yield return TrainingCoroutine();
        }

        for (int i = trialsToSkip; i < numberOfTrials; i++)
        {
            yield return TrialCoroutine(i);
        }

        //WriteExperimentData();

        yield return ShowInstructionsUntilAdvance("The experiment is complete.\n\nThank you for participating.");
        
    }
    

    private IEnumerator TrainingCoroutine()
    {
        yield return ShowSeriesOfInstructionsUntilAdvance(experimentInstructions.GetPreTrainingText());

        // set condition for training round
        sceneController.PrepareScene(-1, Condition.FIRST_CONDITION);

        bool playedWarningSound = false;
        float startTime = Time.time;

        sceneController.DisplayInstructions(experimentInstructions.GetTrainingText());


        // run until advanced or timeout
        while (!shouldAdvanceExperiment)
        {
            CheckForAdvanceKeypress();

            if ((Time.time - startTime) > playTrialEndWarningAfterSecs && !playedWarningSound)
            {
                sceneController.PlayWarningSound();
                playedWarningSound = true;
            }

            if ((Time.time - startTime) > maxTrialDurationSecs)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                   NotifyServerToAdvanceExperimentServerRpc();
                }
            }
            yield return null;
        }
        shouldAdvanceExperiment = false;

        sceneController.ClearScene();

        yield return ShowInstructionsUntilAdvance("The training phase is now complete.\n\n");
    }

    private IEnumerator PreTrial(int trialNum)
    {
        yield return ShowSeriesOfInstructionsUntilAdvance(experimentInstructions.GetPreTrialText(trialNum));

        sceneController.PrepareScene(trialNum, conditionOrder[trialNum]);

        trialStartTime = Time.time;
    }
    private IEnumerator PostTrial(int trialNum)
    {

        trialDuration = Time.time - trialStartTime;

        sceneController.ClearScene();
        
        WriteTrialData(trialNum);

        yield return ShowSeriesOfInstructionsUntilAdvance(experimentInstructions.GetPostTrialText(trialNum));    
    }

    private IEnumerator TrialCoroutine(int trialNum)
    {
        yield return PreTrial(trialNum);

        bool playedWarningSound = false;
        float startTime = Time.time;

        sceneController.DisplayInstructions(experimentInstructions.GetTrialText(trialNum));

        while (!shouldAdvanceExperiment)
        {
            CheckForAdvanceKeypress();

            if ((Time.time - startTime) > playTrialEndWarningAfterSecs && !playedWarningSound)
            {
                sceneController.PlayWarningSound();
                playedWarningSound = true;
            }

            if ((Time.time - startTime) > maxTrialDurationSecs)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    NotifyServerToAdvanceExperimentServerRpc();
                }
            }
            yield return null;
        }
        shouldAdvanceExperiment = false;

        yield return PostTrial(trialNum);
    }

    void WriteExperimentData()
    {
        if (!studyDataCanBeWritten)
        {
            Debug.LogWarning("Tried to write data but data could not be written upon initial test");
            return;
        }
    }

    void WriteTrialData(int trialNum)
    {
        if (!studyDataCanBeWritten)
        {
            Debug.LogWarning("Tried to write data but data could not be written upon initial test");
            return;
        }

        TrialData trialData = new TrialData
        {
            trialNumber = trialNum,
            trialDuration = trialDuration
        };
        string writePath = studyDataFilePathBase + "_trial_data.csv";
        Sinbad.CsvUtil.SaveObjects(new List<TrialData> { trialData }, writePath);
        Debug.Log("Wrote trial data to " + writePath);

    }




    [ServerRpc(RequireOwnership = false)]
    private void StartExperimentServerRpc(ServerRpcParams serverRpcParams = default)
    {
        StartExperimentClientRpc();
    }



    [ClientRpc]
    private void StartExperimentClientRpc()
    {
        StartCoroutine(ExperimentCoroutine());
    }



    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerToAdvanceExperimentServerRpc(ServerRpcParams serverRpcParams = default)
    {
        AdvanceExperimentClientRpc();
    }

    [ClientRpc]
    private void AdvanceExperimentClientRpc()
    {
        Debug.Log("Advancing experiment...");
        shouldAdvanceExperiment = true;
    }

    public void CheckForAdvanceKeypress()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        //if (Input.GetKeyDown(KeyCode.Space))
        {
            if (runInNetworkedMode)
            {
                NotifyServerToAdvanceExperimentServerRpc();
            }
            else {
                shouldAdvanceExperiment = true;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!experimentStarted && Keyboard.current.enterKey.wasPressedThisFrame)
        //if (!experimentStarted && Input.GetKeyDown(KeyCode.Return))
        {
            if (runInNetworkedMode)
            {
                StartExperimentServerRpc();
            }
            else
            {
                StartCoroutine(ExperimentCoroutine());
            }
            experimentStarted = true;
        }

    }

}
