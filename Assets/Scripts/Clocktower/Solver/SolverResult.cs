using System.Collections.Generic;

public sealed class SolverResult
{
    public List<List<string>> ValidWorlds { get; private set; }
    public List<string> Reasons { get; private set; }

    public int ValidWorldCount { get { return ValidWorlds.Count; } }
    public bool HasExactlyOneWorld { get { return ValidWorlds.Count == 1; } }

    public SolverResult(List<List<string>> validWorlds, List<string> reasons)
    {
        ValidWorlds = validWorlds;
        Reasons = reasons;
    }
}
