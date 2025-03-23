using System.Collections;
using UnityEngine;

public class TASK2_SceneController : APMR_SceneController
{
    public DifferenceObjectController differenceObjectController;

    public override void PrepareScene(int trialIndex, APMR_ExperimentRunner.Condition condition)
    {
        base.PrepareScene(trialIndex, condition);

        // choose whether to show stacked set of boxes or spread set
        bool showstackedBoxes = (trialIndex % 2 == 0);

        differenceObjectController.InitializeBoxesAndShapesForTrial(trialIndex, showstackedBoxes);
    }

    public override void ClearScene()
    {
        base.ClearScene();
        differenceObjectController.HideBoxesAndShapes();
    }

}
