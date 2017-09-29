using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    Timer newTimer = new Timer(schedule.Action, null, 0, schedule.GetMilliSecondDelay());
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
                if (Schedules.Keys.Count == 0)
                {
                    output.AppendLine($"No {command} Schedules running");
                }
                else
                {
                    output.AppendLine($"Running {command} Schedules below");
                    foreach (string key in new List<string>(Schedules.Keys))
                    {
                        if (key.StartsWith(command))
                        {
                            output.AppendLine($"\t**{Schedules[key].GetQuery()}** running every {Schedules[key].GetMinuteDelay()} minutes");
                        }
                    }
                }
                return output.ToString();
            });
        }
        public async Task<string> StopAllSchedules()
        {
            StringBuilder output = new StringBuilder();
            if (Schedules.Keys.Count == 0)
            {
                output.AppendLine($"No Schedules running to stop");
            }
            else
            {
                output.Append($"Stopped Schedules below");
                foreach (string key in new List<string>(Schedules.Keys))
                {
                    Task stopSchedule = Schedules[key]?.Stop();
                    output.AppendLine($"{Schedules[key].GetQuery()} was running every {Schedules[key].GetMinuteDelay()} minutes");
                    Schedules.Remove(key);
                    await stopSchedule;
                }
            }
            return output.ToString();
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
        public Int32 GetMilliSecondDelay()
        {
            return Delay * 1000 * 60;
        }
        public Int32 GetMinuteDelay()
        {
            return Delay;
        }
        public void Connect(Timer timer)
        {
            ConnectedTimer = timer;
        }
        public void SetNewDelay(int mins)
        {
            Delay = mins;
            ConnectedTimer.Change(Timeout.Infinite, GetMilliSecondDelay());
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
