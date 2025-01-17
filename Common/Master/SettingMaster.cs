using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Approvers.King.Common;

public class SettingMaster : MasterTable<string, Setting>
{
    private string GetString(string key)
    {
        var record = Find(key);
        if (record == null)
        {
            LogManager.LogError("Setting not found: " + key);
            return string.Empty;
        }

        return record.Value;
    }

    private int GetInt(string key)
    {
        var value = GetString(key);
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        if (int.TryParse(value, out var result) == false)
        {
            LogManager.LogError("Failed to parse setting: " + key);
            return 0;
        }

        return result;
    }

    public int ReplyMaxDuration => GetInt(nameof(ReplyMaxDuration));
    public int TypingMaxDuration => GetInt(nameof(TypingMaxDuration));
    public int DailyResetTime => GetInt(nameof(DailyResetTime));
    public int MonthlyResetDay => GetInt(nameof(MonthlyResetDay));
    public int BirthdayMonth => GetInt(nameof(BirthdayMonth));
    public int BirthdayDay => GetInt(nameof(BirthdayDay));
    public int MaxRareReplyProbabilityPermillage => GetInt(nameof(MaxRareReplyProbabilityPermillage));
    public int RareReplyProbabilityStepPermillage => GetInt(nameof(RareReplyProbabilityStepPermillage));
    public int PricePerGachaOnce => GetInt(nameof(PricePerGachaOnce));
    public int PricePerGachaTenTimes => GetInt(nameof(PricePerGachaTenTimes));
    public int PricePerGachaOnceCertain => GetInt(nameof(PricePerGachaOnceCertain));
    public int PurchaseInfoRankingViewUserCount => GetInt(nameof(PurchaseInfoRankingViewUserCount));
    public int PricePerSlotOnce => GetInt(nameof(PricePerSlotOnce));
    public string SlotReelRollingFormat => GetString(nameof(SlotReelRollingFormat));
    public string SlotLeverFormat => GetString(nameof(SlotLeverFormat));
    public int UserSlotExecuteLimitPerDay => GetInt(nameof(UserSlotExecuteLimitPerDay));
    public int SlotMaxConditionOffsetPermillage => GetInt(nameof(SlotMaxConditionOffsetPermillage));
    public int SlotMinConditionOffsetPermillage => GetInt(nameof(SlotMinConditionOffsetPermillage));
    public int SlotRepeatPermillageUpperBound => GetInt(nameof(SlotRepeatPermillageUpperBound));
}

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
public class Setting : MasterRecord<string>
{
    [field: MasterStringValue("key")]
    public override string Key { get; }

    [field: MasterStringValue("value")]
    public string Value { get; }
}
