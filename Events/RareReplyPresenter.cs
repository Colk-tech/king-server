﻿using Approvers.King.Common;
using Discord;

namespace Approvers.King.Events;

public class RareReplyPresenter : DiscordMessagePresenterBase
{
    protected override async Task MainAsync()
    {
        if (SilentManager.IsSilent(Message.Author.Id) ||
            RandomUtility.IsHit(MasterManager.ReplyRate) == false) return;

        await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(MasterManager.ReplyMaxDelay)));
        using (Message.Channel.EnterTypingState())
        {
            await Task.Delay(TimeSpan.FromSeconds(RandomUtility.GetRandomFloat(MasterManager.TypingMaxDelay)));
            await Message.ReplyAsync(MessageUtility.PickRandomMessage());
        }
    }
}
