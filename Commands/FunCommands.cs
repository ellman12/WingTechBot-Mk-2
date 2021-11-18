namespace WingTechBot
{
    class SlurCommand : Command
    {
        public override void Execute()
        {
            message.Channel.SendMessageAsync($"Bad {message.Author.Mention}");
        }

        public override string LogString => $"scolded {message.Author}";
    }

    class FisheCommand : Command
    {
        public override void Execute() => message.Channel.SendFileAsync(@"Images/fishe.jpg");

        public override string LogString => "posted fishe";
    }

    class NekoCommand : Command
    {
        public override void Execute() => message.Channel.SendFileAsync($@"Images/neko{Program.Random.Next(5)}.png");

        public override string LogString => "posted smexy cat boy";
        public override string[] Aliases => new string[] { "neko", "catboy", "catboi", "catgirl" };
    }
}