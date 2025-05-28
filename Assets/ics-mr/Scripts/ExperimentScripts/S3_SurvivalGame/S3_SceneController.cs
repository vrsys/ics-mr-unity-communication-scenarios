using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3_SceneController : ICSMR_SceneController
{

    [SerializeField]
    private SurvivalItemController survivalItemController = null;

    [SerializeField]
    private int numItemsForTrainingScenario = 3;

    [SerializeField]
    private int numItemsToShow = 12;

    [SerializeField]
    private ICSMR_InstructionsDisplay survivalDescriptionDisplay = null;

    [SerializeField]
    private string taskDataResourceDirectory = "S3_SurvivalGame";

    public enum SurvivalScenario
    {
        DESERT,
        SEA,
        WINTER,
        MOUNTAINS,
        MOON,
        CAVELABYRINTH,
        SWAMP
    }

    [SerializeField]
    private List<SurvivalScenario> survivalScenarioOrder = new List<SurvivalScenario> { SurvivalScenario.MOUNTAINS, SurvivalScenario.MOON, SurvivalScenario.WINTER, SurvivalScenario.DESERT, SurvivalScenario.SEA };



    public override void StartExperiment()
    {
        survivalItemController.CreateObjects();
        survivalDescriptionDisplay.HideInstructions();  
    }


    public override void PrepareScene(int trialIndex, ICSMR_ExperimentRunner.Condition condition)
    {
        base.PrepareScene(trialIndex, condition);

        int itemsToShow = trialIndex < 0 ? numItemsForTrainingScenario : numItemsToShow;

        SurvivalScenario survivalScenario = survivalScenarioOrder[trialIndex + 1];

        string survivalItemsCsvPath = taskDataResourceDirectory + "/SurvivalItemData/" + survivalScenario.ToString() + "_survival_items";
        survivalItemController.ShowBoxesForSurvivalScenario(survivalItemsCsvPath, itemsToShow);


        // load the description text for the scenario
        string scenarioDescriptionTextPath = taskDataResourceDirectory + "/Scenarios/" + survivalScenario.ToString().ToLower();
        TextAsset scenarioDescriptionText = Resources.Load<TextAsset>(scenarioDescriptionTextPath);

        if (null == scenarioDescriptionText)
        {
            Debug.LogError("Survival description not found at " + scenarioDescriptionTextPath);
        }
        survivalDescriptionDisplay.ShowInstructions(scenarioDescriptionText.text);

    }

    public override void ClearScene()
    {
        base.ClearScene();

        survivalItemController.HideItems();
        survivalDescriptionDisplay.HideInstructions();

    }

}
