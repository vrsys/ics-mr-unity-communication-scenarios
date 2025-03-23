using System.Collections;
using UnityEngine;

public class TASK3_SceneController : APMR_SceneController
{

    [SerializeField]
    private SurvivalItemController survivalItemController = null;

    public override void PrepareScene(int trialIndex, APMR_ExperimentRunner.Condition condition)
    {
        base.PrepareScene(trialIndex, condition);

        survivalItemController.ShowBoxesForSurvivalScenario(trialIndex + 1);

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
