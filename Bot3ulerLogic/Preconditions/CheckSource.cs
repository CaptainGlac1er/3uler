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
            using (var db = new BotDbContext())
            {
                var check1 = from c in db.Channels where c.ChannelId == (long) context.Channel.Id select c;
                if (check1.Count() == 1)
                {
                    return (check1.First().ChannelCommands.Where((check2 => check2.CommandString == Command)).Any())
                        ? Task.FromResult(PreconditionResult.FromSuccess())
                        : Task.FromResult(PreconditionResult.FromError($"Can't use {Command} command in: {context.Guild.Name} > {context.Channel.Name}"));
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError($"Can't use commands in: {context.Guild.Name} > {context.Channel.Name}"));
                }
            }
        }
    }
}
