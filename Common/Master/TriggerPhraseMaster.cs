using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class TriggerPhraseMaster : MasterTable<string, TriggerPhrase>;

public enum TriggerType
{
    Unknown,
    GachaExecute,
    GachaGet,
    Marugame,
    GachaRanking,
    SlotExecute,
    SlotRanking,
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class TriggerPhrase : MasterRecord<string>
{
    public override string Key => Id;

    [field: MasterStringValue("id")]
    public string Id { get; }

    [field: MasterEnumValue("trigger_type", typeof(TriggerType))]
    public TriggerType TriggerType { get; }

    [field: MasterStringValue("phrase")]
    public string Phrase { get; }
}
