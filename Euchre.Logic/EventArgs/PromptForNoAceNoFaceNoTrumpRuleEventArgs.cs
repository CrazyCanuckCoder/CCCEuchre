namespace Euchre.Logic.EventArgs;

public class PromptForNoAceNoFaceNoTrumpRuleEventArgs
{
    public PromptForNoAceNoFaceNoTrumpRuleEventArgs()
    {
    }

    /// <summary>
    /// True to indicate the user wants to invoke the rule.
    /// </summary>
    public bool InvokeRule { get; set; }

}
