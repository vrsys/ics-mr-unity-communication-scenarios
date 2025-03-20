using System.Collections.Generic;

public class TASK4_ExperimentInstructions : APMR_ExperimentInstructions
{
    public override List<string> GetPreTrainingText()
    {
        List<string> instructionList = base.GetPreTrainingText();
        instructionList[0] = instructionList[0] + "\n\nDiscuss and choose 6 items from your lists.";
        return instructionList;
    }
    public override string GetTrainingText()
    {
        return null;
    }

    public override List<string> GetPreTrialText(int trialIndex)
    {
        List<string> instructionList = base.GetPreTrialText(trialIndex);
        instructionList[0] = instructionList[0] + "\n\nDiscuss and choose 6 items from your lists.";
        return instructionList;
    }

    public override string GetTrialText(int trialIndex)
    {
        return null;
    }
}
