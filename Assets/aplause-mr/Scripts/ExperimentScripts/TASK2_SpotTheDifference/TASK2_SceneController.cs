using System.Collections;
using UnityEngine;

public class TASK2_SceneController : APMR_SceneController
{
    public DifferenceObjectController differenceObjectController;


    protected override void Start()
    {
        base.Start();
    }



    public override void PrepareScene(int trialIndex, APMR_ExperimentRunner.Condition condition)
    {
        base.PrepareScene(trialIndex, condition);

        if (trialIndex < 0)
        {
            differenceObjectController.ShowTrainingScenario();
        }
        else
        {
            differenceObjectController.InitializeBoxesAndShapesForTrialNew(trialIndex, (trialIndex > 0));

        }
    }

    public override void ClearScene()
    {
        base.ClearScene();
        differenceObjectController.HideBoxesAndShapes();
    }

}
