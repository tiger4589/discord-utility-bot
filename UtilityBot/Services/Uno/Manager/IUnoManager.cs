﻿using Discord;
using Discord.Interactions;

namespace UtilityBot.Services.Uno.Manager;

public interface IUnoManager
{
    Task EnableUnoInChannel(SocketInteractionContext context, IChannel channel, IRole role);
    Task DisableUnoInChannel(SocketInteractionContext context, IChannel channel);
}