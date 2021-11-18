using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WingTechBot.Handlers;

namespace WingTechBot
{
    class KarmaCommand : Command
    {
        public override void Execute()
        {
            if (Program.KarmaHandler.KarmaDictionary.Keys.Contains(requested.Id))
            {
                int[] counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

                message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]})");
            }
            else
            {
                message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any karma on record.");
            }
        }

        public override string LogString => $"reported {requested}'s karma";
        public override bool GetRequested => true;
        public override string[] Aliases => new string[] { "karma", "k" };
    }

    class AwardCommand : Command
    {
        public override void Execute()
        {
            if (Program.KarmaHandler.KarmaDictionary.Keys.Contains(requested.Id))
            {
                int[] counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

                message.Channel.SendMessageAsync($"{requested.Mention} has {counts[2]} <:silver:672249246442979338> {counts[3]} <:gold:672249212322316308> {counts[4]} <:platinum:672249164846858261>");
            }
            else
            {
                message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any awards on record.");
            }
        }

        public override string LogString => $"reported {requested}'s awards";
        public override bool GetRequested => true;
        public override string[] Aliases => new string[] { "award", "awards", "a" };
    }

    class RecordCommand : Command
    {
        public override void Execute()
        {
            if (Program.KarmaHandler.KarmaDictionary.Keys.Contains(requested.Id))
            {
                int[] counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

                message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]}) {requested} has: {counts[2]} <:silver:672249246442979338> {counts[3]} <:gold:672249212322316308> {counts[4]} <:platinum:672249164846858261>");
            }
            else
            {
                message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any awards or karma on record.");
            }
        }

        public override string LogString => $"reported {requested}'s record";
        public override bool GetRequested => true;
        public override string[] Aliases => new string[] { "record", "records", "r" };
    }

    class SaveCommand : Command
    {
        public override void Execute()
        {
            Program.KarmaHandler.Save();
            message.Channel.SendMessageAsync($"Saving karma values.");
        }

        public override string LogString => "manually saved";
        public override bool Audit => true;
        public override bool OwnerOnly => true;
    }

    class LogCommand : Command
    {
        public override void Execute()
        {
            message.Channel.SendMessageAsync("Logging Karma Dictionary to Console.");
            foreach (var entry in Program.KarmaHandler.KarmaDictionary)
            {
                Console.Write(entry.Key);
                for (int i = 0; i < entry.Value.Length; i++)
                {
                    Console.Write($" {entry.Value[i]}");
                }
                Console.WriteLine();
            }
        }

        public override string LogString => $"logging karma dictionary.";
        public override bool OwnerOnly => true;
    }

    class ReverseCommand : Command
    {
        int caseNumber;

        public override void Execute()
        {
            try
            {
                caseNumber = int.Parse(arguments[1]);
            }
            catch { throw new ArgumentException("Case number must be specified."); }

            string line;

            try
            {
                line = File.ReadLines(KarmaHandler.CASE_PATH).Skip(caseNumber).Take(1).First();
            }
            catch
            {
                throw new Exception($"{caseNumber} not found.");
            }

            string[] values = line.Split(' ');

            ulong id = ulong.Parse(values[1]);

            int emoteID = int.Parse(values[2]);
            Program.KarmaHandler.KarmaDictionary[id][emoteID] += int.Parse(values[3]);

            message.Channel.SendMessageAsync($"Karma and awards from case {caseNumber} has been returned to {Program.GetUser(id).Mention}...");

            string[] arrLine = File.ReadAllLines(KarmaHandler.CASE_PATH);
            arrLine[caseNumber] += " REVERSED";
            File.WriteAllLines(KarmaHandler.CASE_PATH, arrLine);

        }

        public override string LogString => $"reversed case #{caseNumber}.";
        public override bool OwnerOnly => true;
        public override bool Audit => true;
    }

    class ConfirmCommand : Command
    {
        int caseNumber;

        public override void Execute()
        {
            try
            {
                caseNumber = int.Parse(arguments[1]);
            }
            catch { throw new ArgumentException("Case number must be specified."); }

            string[] arrLine = File.ReadAllLines(KarmaHandler.CASE_PATH);
            try
            {
                arrLine[caseNumber] += " CONFIRMED";
            }
            catch
            {
                message.Channel.SendMessageAsync($"{caseNumber} not found.");
            }

            File.WriteAllLines(KarmaHandler.CASE_PATH, arrLine);
            message.Channel.SendMessageAsync($"{caseNumber} confirmed.");
        }

        public override string LogString => $"confirmed case #{caseNumber}.";
        public override bool OwnerOnly => true;
        public override bool Audit => true;
    }

    class RunningCommand : Command
    {
        public override void Execute()
        {
            if (Program.KarmaHandler.RunningKarma.Keys.Contains(requested.Id))
            {
                int[] counts = Program.KarmaHandler.RunningKarma[requested.Id];

                message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} running karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]})");
            }
            else
            {
                message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any running karma right now.");
            }
        }

        public override string LogString => $"reported {requested}'s running karma";
        public override bool GetRequested => true;
        public override ulong[] RequiredRoles => new ulong[] { Secrets.MOD_ROLE_ID };
    }

    class SpamCommand : Command
    {
        public override void Execute()
        {
            message.Channel.SendMessageAsync($"Updating running karma.");
            Program.KarmaHandler.ClearRunningKarma();
        }

        public override string LogString => $"manually updating running karma.";
        public override bool OwnerOnly => true;
        public override bool Audit => true;
    }

    class TopCommand : Command
    {
        public override void Execute()
        {
            List<ulong> sorted = (from kvp in Program.KarmaHandler.KarmaDictionary orderby kvp.Value[1] - kvp.Value[0] select kvp.Key).ToList();

            int numToReport = 5;

            if (arguments.Length >= 2)
            {
                bool success = int.TryParse(arguments[1], out numToReport);
                if (success)
                {
                    if (numToReport <= 0) numToReport = 5;
                    if (numToReport > Program.KarmaHandler.KarmaDictionary.Count) numToReport = Program.KarmaHandler.KarmaDictionary.Count;
                }
                else
                {
                    if (arguments[1].ToLower() == "all") numToReport = Program.KarmaHandler.KarmaDictionary.Count;
                    else numToReport = 5;
                }
            }

            string text = $"```Karma Leaderboard, Top {numToReport}\n";

            for (int i = 0; i < numToReport; i++)
            {
                try
                {
                    var user = ((IGuild)Program.BotChannel.Guild).GetUserAsync(sorted[i]).Result;
                    text += $"[{i + 1}] = {Program.KarmaHandler.KarmaDictionary[sorted[i]][0] - Program.KarmaHandler.KarmaDictionary[sorted[i]][1]} karma - {user.Username}#{user.Discriminator}\n";
                }
                catch
                {
                    text += $"[{i + 1}] = {Program.KarmaHandler.KarmaDictionary[sorted[i]][0] - Program.KarmaHandler.KarmaDictionary[sorted[i]][1]} karma - UNKNOWN USER\n";
                }
            }

            text += "```";

            message.Channel.SendMessageAsync(text);
        }

        public override string LogString => "reported leaderboard";
    }
}