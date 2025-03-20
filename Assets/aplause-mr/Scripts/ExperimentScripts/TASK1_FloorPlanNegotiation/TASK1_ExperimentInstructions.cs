using System.Collections.Generic;
using UnityEngine;

public class TASK1_ExperimentInstructions : APMR_ExperimentInstructions
{


    public override List<string> GetPreTrialText(int trialIndex)
    {
        List<string> instructionList = base.GetPreTrainingText();
        instructionList.Add("You will see a floor plan for an apartment on the table. \n Negotiate the allocation of the rooms with your future flat-mates. Once you have reached a concensus, the experiment will continue.");
        return instructionList;
    }

}
