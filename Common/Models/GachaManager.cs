﻿using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Common;

public class GachaManager : Singleton<GachaManager>
{
    private readonly List<GachaProbability> _replyMessageTable = new();

    /// <summary>
    ///     現在のメッセージに反応する確率
    /// </summary>
    public float RareReplyRate { get; private set; }

    /// <summary>
    ///     各メッセージの確率
    /// </summary>
    public IReadOnlyList<GachaProbability> ReplyMessageTable => _replyMessageTable;

    public bool IsTableEmpty => ReplyMessageTable.Count == 0;

    public async Task LoadAsync()
    {
        await using var app = AppService.CreateSession();

        var probabilities = await app.GachaProbabilities.ToListAsync();
        _replyMessageTable.Clear();
        _replyMessageTable.AddRange(probabilities.Where(x => x.RandomMessage != null));

        var rareReplyRatePermillage = await app.AppStates.GetIntAsync(AppStateType.RareReplyProbabilityPermillage);
        RareReplyRate = NumberUtility.GetPercentFromPermillage(rareReplyRatePermillage ?? 0);
    }

    public async Task SaveAsync()
    {
        await using var app = AppService.CreateSession();

        app.GachaProbabilities.RemoveRange(app.GachaProbabilities);
        app.GachaProbabilities.AddRange(_replyMessageTable);

        var rareReplyRatePermillage = NumberUtility.GetPermillageFromPercent(RareReplyRate);
        await app.AppStates.SetIntAsync(AppStateType.RareReplyProbabilityPermillage, rareReplyRatePermillage);

        await app.SaveChangesAsync();
    }

    public void RefreshMessageTable()
    {
        var messages = MasterManager.RandomMessageMaster
            .GetAll(x => x.Type == RandomMessageType.GeneralReply)
            .Select(x => new GachaProbability()
            {
                RandomMessageId = x.Id,
                Probability = _replyMessageTable.FirstOrDefault(m => m.RandomMessageId == x.Id)?.Probability ?? 0
            })
            .ToList();

        _replyMessageTable.Clear();
        _replyMessageTable.AddRange(messages);
    }

    public GachaProbability? Roll()
    {
        if (RandomManager.IsHit(RareReplyRate) == false) return null;
        return GetRandomResult();
    }

    public GachaProbability RollWithoutNone()
    {
        return GetRandomResult();
    }

    private GachaProbability GetRandomResult()
    {
        var totalRate = _replyMessageTable.Sum(x => x.Probability);
        if (totalRate <= 0)
        {
            return _replyMessageTable[0];
        }

        var value = RandomManager.GetRandomFloat(totalRate);

        foreach (var element in _replyMessageTable)
        {
            if (value < element.Probability) return element;
            value -= element.Probability;
        }

        return _replyMessageTable[^1];
    }

    public void ShuffleRareReplyRate()
    {
        var step = MasterManager.SettingMaster.RareReplyProbabilityStepPermillage;
        var max = MasterManager.SettingMaster.MaxRareReplyProbabilityPermillage;
        var rates = Enumerable.Range(0, max / step)
            .Select(i => NumberUtility.GetPercentFromPermillage((i + 1) * step));
        RareReplyRate = RandomManager.PickRandom(rates);
    }

    public void ShuffleMessageRates()
    {
        var borders = Enumerable.Range(0, _replyMessageTable.Count - 1)
            .Select(x => (float)Math.Pow(RandomManager.GetRandomFloat(1f), 2))
            .Select(x => (int)Math.Floor(x * 100f))
            .OrderBy(x => x)
            .ToList();
        borders.Add(100);
        var randomIndices = RandomManager.Shuffle(Enumerable.Range(0, _replyMessageTable.Count)).ToList();

        _replyMessageTable[randomIndices[0]].Probability = borders[0] * 0.01f;
        for (var i = 1; i < _replyMessageTable.Count; i++)
        {
            _replyMessageTable[randomIndices[i]].Probability = (borders[i] - borders[i - 1]) * 0.01f;
        }
    }
}
