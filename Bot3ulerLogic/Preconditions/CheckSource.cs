using Bot3ulerLogic.Config.Objects;
using Discord.Commands;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Bot3ulerLogic.Preconditions
{
    class CheckSource : PreconditionAttribute
    {
        string Command { get; set; }
        public CheckSource(string command)
        {
            Command = command;
        }
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            GuildConfig config = services.GetService<GuildConfig>();
            return (config.GetCommandsOfGuildChannel(context.Guild.Id, context.Channel.Id).Contains(Command)) ? Task.FromResult(PreconditionResult.FromSuccess()) : Task.FromResult(PreconditionResult.FromError("Can't use command here"));            
        }
    }
}
