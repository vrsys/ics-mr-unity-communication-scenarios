using System.Collections.Generic;
using UnityEngine;

public class APMR_ExperimentInstructions : MonoBehaviour
{
    public virtual void StartExperiment(int experimentIndex)
    {

    }
    /*

    public abstract void PrepareTrainingPhase();
    public abstract void PrepareTrial(int trialIndex);
    public abstract void ClearTask();
    */

    public virtual List<string> GetPreTrialText(int trialIndex)
    {
        return new List<string>{"Trial " + (trialIndex + 1) + " about to start."};
    }

    public virtual string GetTrialText(int trialIndex)
    {
        return "Trial " + (trialIndex + 1) + " in progress." ;
    }
    public virtual List<string> GetPostTrialText(int trialIndex)
    {
        return new List<string> { "Trial " + (trialIndex + 1) + " complete."};
    }
    public virtual List<string> GetPreTrainingText()
    {
        return new List<string> { "Training phase about to start."};
    }
    public virtual string GetTrainingText()
    {
        return  "Training phase in progress." ;
    }
    public virtual List<string> GetPostTrainingText()
    {
        return new List<string> { "Training phase complete." };
    }



}
