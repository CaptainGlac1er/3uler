using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Modules.Queue
{
    public class ScheduleMaker
    {
        Dictionary<string, Schedule> Schedules;
        public ScheduleMaker()
        {
            Schedules = new Dictionary<string, Schedule>();
        }
        public async Task<bool> AddSchedule(Schedule schedule)
        {
            if (!Schedules.ContainsKey(schedule.GetReference()))
            {
                await Task.Run(() =>
                {
                    Timer newTimer = new Timer(schedule.Action, null, 0, schedule.GetDelay());
                    schedule.Connect(newTimer);
                    Schedules.Add(schedule.GetReference(), schedule);
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> StopSchedule(string command, string query)
        {
            string reference = Schedule.MakeReference(command, query);
            if (!Schedules.ContainsKey(reference))
            {
                return $"Schedule does not include {query}";
            }
            else
            {
                await Schedules[reference]?.Stop();
                Schedules.Remove(reference);
                return $"{query} schedule has been stopped";
            }
        }
        public async Task<string> GetAllSchedules(string command)
        {
            return await Task.Run<string>(() =>
            {
                StringBuilder output = new StringBuilder();
                output.Append($"Running {command} Schedules below\n```");
                foreach (string key in Schedules.Keys)
                {
                    if (key.StartsWith(command))
                    {
                        output.Append($"{Schedules[key].GetQuery()}\n");
                    }
                }
                output.Append("```");
                return output.ToString();
            });
        }
    }
    public abstract class Schedule
    {
        protected Timer ConnectedTimer;
        protected string Query;
        protected ISocketMessageChannel ChannelRequested;
        protected string Reference;
        protected int Delay;
        protected Schedule(string command, string query, ISocketMessageChannel channelRequested, int delay)
        {
            Reference = MakeReference(command, query);
            Query = query;
            ChannelRequested = channelRequested;
            Delay = delay;
        }

        public string GetReference()
        {
            return Reference;
        }
        public Int32 GetDelay()
        {
            return Delay * 1000 * 30;
        }
        public void Connect(Timer timer)
        {
            ConnectedTimer = timer;
        }
        public void SetNewDelay(int mins)
        {
            Delay = mins;
            ConnectedTimer.Change(Timeout.Infinite, GetDelay());
        }
        public async Task Stop()
        {
            await Task.Run(() => ConnectedTimer.Dispose());
        }
        public string GetQuery()
        {
            return Query;
        }
        public static string MakeReference(string command, string query)
        {
            return $"{command}({query})";
        }
        public abstract void Action(Object stateInfo);
        
    }
}
