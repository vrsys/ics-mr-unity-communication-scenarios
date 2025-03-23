using System.Collections;
using UnityEngine;

public class TASK3_SceneController : APMR_SceneController
{

    [SerializeField]
    private SurvivalItemController survivalItemController = null;

    [SerializeField]
    private int numItemsForTrainingScenario = 3;

    [SerializeField]
    private int numItemsToShow = 12;

    public override void PrepareScene(int trialIndex, APMR_ExperimentRunner.Condition condition)
    {
        base.PrepareScene(trialIndex, condition);

        int itemsToShow = trialIndex < 0 ? numItemsForTrainingScenario : numItemsToShow;

        survivalItemController.ShowBoxesForSurvivalScenario(trialIndex + 1, itemsToShow);

        // choose whether to show stacked set of boxes or spread set
        //bool showstackedBoxes = (trialIndex % 2 == 0);

        //differenceObjectController.InitializeBoxesAndShapesForTrial(trialIndex, showstackedBoxes);
    }

    public override void ClearScene()
    {
        base.ClearScene();
        
        //differenceObjectController.HideBoxesAndShapes();
    }

}
