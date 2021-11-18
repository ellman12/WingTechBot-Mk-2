using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WingTechBot.Hangman
{
    public class Hangman : Game
    {
        private string word = string.Empty, check = string.Empty;
        private readonly List<char> guesses = new();
        private readonly List<string> wordGuesses = new();

        private Dictionary<ulong, int> scores;

        private static HashSet<string> banned;

        private const string DICTIONARY_PATH = @"Hangman\words.txt";
        private const string BANNED_PATH = @"Hangman\banned.txt";

        private static readonly Random random = new();

        private static readonly string[] heads = new string[]
        {
            "(O.O)",
            "(O.-)",
            "(-.-)",
            "(O.o)",
            "(>.<)",
            "(>.>)",
            "(X.X)",
            "(@.@)",
            "(o-o)",
            "(-_-)",
            "(OwO)",
            "(=.=)",
            "(OxO)",
            "(o_o)",
            "(._.)",
            "(;_;)",
            "(^.^)",
            "(~.~)",
        };

        protected override Dictionary<string, Action<IMessage, string[]>> Commands => _commands;

        private Dictionary<string, Action<IMessage, string[]>> _commands;

        int currentHostIndex = -1, strikes = 0, score = 0, total = 0;

        private bool pvp;//, foreignAllowed;
        private int clues;

        static int? dictionaryCount = null;

        protected override bool Debug => false;

        protected override void Start()
        {
            _commands = new Dictionary<string, Action<IMessage, string[]>>
            {
                { "CORRECT", CorrectSpelling }
            };

            pvp = !PromptYN(GamemasterID, AllowedChannels, true, "Would you like to face a bot? (y/n)");
            clues = Prompt(GamemasterID, AllowedChannels, (int x) => x >= 0, true, "How many clues would you like? (recommended: 0-2)");

            //if (pvp) foreignAllowed = PromptYN(GamemasterID, AllowedChannels, true, "Are foreign characters allowed? (y/n)");
        }

        public override void RunGame()
        {
            while (PlayerIDs.Count == 0 || (pvp && PlayerIDs.Count == 1))
            {
                if (PlayerIDs.Count == 0)
                {
                    WriteLine("You can't play hangman with zero players!");
                    GetPlayers();
                }

                if (pvp)
                {
                    if (PlayerIDs.Count == 1)
                    {
                        WriteLine("You need at least two players to play multiplayer!");
                        GetPlayers();
                    }
                }
            }

            if (pvp)
            {
                scores = new Dictionary<ulong, int>();

                foreach (ulong id in PlayerIDs) scores.Add(id, 0);
            }
            else InitializeDictionary();

            while (true)
            {
                if (pvp)
                {
                    AdvanceHost();
                    WriteLine($"Prompting {GetPlayer(PlayerIDs[currentHostIndex]).Mention} for next word... Check your DMs");
                    word = Prompt(PlayerIDs[currentHostIndex], PromptMode.DM, (string s) => s.Length < 100, false, "What should the word be?", true).Trim().ToUpper();
                }
                else word = GetRandomWord().ToUpper(); // Get random word
                check = word.RemoveDiacritics();

                WriteLine($"===[NEW ROUND]===\n**{GetWord(check.Count(c => char.IsLetter(c) && c.IsAmericanized()))}** letters");

                total++;

                // get clues
                List<char> letters = new();
                int actualClues = clues;

                if (pvp)
                {
                    foreach (char c in check) if (!letters.Contains(c) && c.IsAmericanized() && (clues > 6 || "aeiouyAEIOUY".Contains(c))) letters.Add(c);
                }
                else
                {
                    foreach (char c in check) if (!letters.Contains(c) && c.IsAmericanized()) letters.Add(c);
                    actualClues = letters.Count - clues <= 2 ? letters.Count - 2 : clues;
                }

                for (int i = 0; i < actualClues; i++)
                {
                    char c = letters[random.Next(letters.Count)];
                    letters.Remove(c);
                    guesses.Add(c);

                    if (letters.Count <= 0) break;

                }

                (ulong id, string text) tuple = (0, null);

                // main loop
                while (true)
                {
                    DeleteSavedMessages();
                    string screen = GetScreen();

                    if (strikes >= 6) // strike gameover
                    {
                        screen += $"Gameover! The word was: {word}";
                        SaveWriteLine(screen);
                        break;
                    }
                    else // win gameover
                    {
                        bool done = true;
                        foreach (char c in check)
                        {
                            if (char.IsLetter(c) && c.IsAmericanized()) done &= guesses.Contains(c);
                        }

                        if (done)
                        {
                            screen += "Correct!";
                            if (scores == null) score++;
                            else scores[tuple.id]++;

                            SaveWriteLine(screen);
                            break;
                        }
                        else
                        {
                            screen += "Guess a letter!";
                            SaveWriteLine(screen);
                        }
                    }

                    do
                    {
                        tuple = PromptAny(AllowedChannels, (string s) => s.Length == 1 || s.Length == word.Length, true, saveMessage: true);
                    }
                    while (currentHostIndex != -1 && PlayerIDs[currentHostIndex] == tuple.id);

                    string guess = tuple.text.Trim().ToUpper().RemoveDiacritics();

                    if (guess == check) // word guess success
                    {
                        SaveWriteLine("Correct!");

                        if (scores == null) score++;
                        else scores[tuple.id]++;
                        break;
                    }
                    else if (guess.Length == 1 && char.IsLetter(guess[0])) // letter guess
                    {
                        if (!guesses.Contains(guess[0]))
                        {
                            guesses.Add(guess[0]);
                            if (!check.Contains(guess[0])) strikes++;
                        }
                    }
                    else if (!wordGuesses.Contains(guess)) // word guess fail
                    {
                        SaveWriteLine($"{guess} is not the word.");
                        wordGuesses.Add(guess);
                        strikes++;
                    }
                }

                strikes = 0;
                guesses.Clear();
                sentMessages.Clear();
                receivedMessages.Clear();
                wordGuesses.Clear();

                if (PromptEnd()) break;
            }

            DeleteSavedMessages();

            Shutdown();
        }

        public override void Shutdown()
        {
            if (pvp)
            {
                var orderedScores = from kvp in scores orderby kvp.Value descending select kvp;

                string gameOverText = $"Gameover! Scores:\n";

                foreach (var kvp in orderedScores)
                {
                    Discord.IUser player = GetPlayer(kvp.Key);
                    gameOverText += $"{kvp.Value} - {player.Username}#{player.Discriminator}\n";
                }

                WriteLine(gameOverText);
            }
            else WriteLine($"Gameover! Got: {score}/{total}");
        }

        private string GetScreen()
        {
            string screen = "```" +
                        $"||-------\n" +
                        $"||      |\n" +
                        $"||    {GetHead()}\n" +
                        $"||    {GetBody()}\n" +
                        $"||    {GetLegs()}\n" +
                        $"||\n" +
                        $"||\n" +
                        $"====[HANGMAN]====\n" +
                        $"{GetClue()}";

            screen += "```";

            return screen;
        }

        private string GetHead()
        {
            if (strikes == 1) return heads[0];
            else if (strikes >= 2) return heads[random.Next(heads.Length)];
            else return "     ";
        }

        private string GetBody()
        {
            if (strikes == 2) return "  8  ";
            else if (strikes == 3) return "  8 -";
            else if (strikes >= 4) return "- 8 -";
            else return "     ";
        }

        private string GetLegs()
        {
            if (strikes == 5) return " /  ";
            else if (strikes == 6) return @" / \";
            else return "   ";
        }

        private string GetClue()
        {
            string clue = string.Empty;

            for (int i = 0; i < word.Length; i++)
            {
                if (guesses.Contains(check[i]) || !check[i].IsAmericanized()) clue += $"{word[i]} ";
                else clue += $"_ ";
            }

            clue += "\n" + "wrong:";

            foreach (char c in guesses)
            {
                if (!check.Contains(c)) clue += $" {c}";
            }

            foreach (string s in wordGuesses)
            {
                clue += $"\n - {s}";
            }

            return clue;
        }

        private static void InitializeDictionary()
        {
            if (banned == null)
            {
                banned = new HashSet<string>();
                FileInfo fi = new(BANNED_PATH);

                using StreamReader file = new(fi.Open(FileMode.Open));
                while (!file.EndOfStream) banned.Add(file.ReadLine());
            }
        }

        private static string GetRandomWord()
        {
            string word;

            do
            {
                var lines = File.ReadLines(DICTIONARY_PATH);

                dictionaryCount ??= lines.Count();

                word = lines.Skip(random.Next((int)dictionaryCount - 1)).Take(1).First();
            }
            while (banned.Contains(word) || word.Length <= 2 || !word.Any((char c) => "aeiouy".Contains(c)) || !word.IsAmericanized());

            return word;
        }

        private void AdvanceHost()
        {
            currentHostIndex++;
            if (currentHostIndex >= PlayerIDs.Count) currentHostIndex = 0;
        }

        private static string GetWord(int x)
        {
            if (x >= 100) throw new ArgumentOutOfRangeException(nameof(x), "must be less than 100!");
            if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), "must be non-negative!");

            if (x == 0) return "zero";

            if (10 <= x && x <= 19)
            {
                return x switch
                {
                    10 => "ten",
                    11 => "eleven",
                    12 => "twelve",
                    13 => "thirteen",
                    14 => "fourteen",
                    15 => "fifteen",
                    16 => "sixteen",
                    17 => "seventeen",
                    18 => "eighteen",
                    19 => "nineteen",
                    _ => "impossible",
                };
            }
            else
            {
                string s = (x / 10) switch
                {
                    2 => "twenty",
                    3 => "thirty",
                    4 => "fourty",
                    5 => "fifty",
                    6 => "sixty",
                    7 => "seventy",
                    8 => "eighty",
                    9 => "ninety",
                    0 => string.Empty,
                    _ => throw new Exception($"not possible {x} / 10"),
                };

                if (x % 10 != 0)
                {
                    if (x > 10) s += "-";

                    s += (x % 10) switch
                    {
                        1 => "one",
                        2 => "two",
                        3 => "three",
                        4 => "four",
                        5 => "five",
                        6 => "six",
                        7 => "seven",
                        8 => "eight",
                        9 => "nine",
                        _ => throw new Exception($"not possible {x} % 10"),
                    };
                }

                return s;
            }
        }

        private void CorrectSpelling(IMessage message, string[] args)
        {
            if (pvp && message.Author.Id == PlayerIDs[currentHostIndex])
            {
                guesses.Clear();
                wordGuesses.Clear();
                strikes = 0;

                word = Prompt(PlayerIDs[currentHostIndex], PromptMode.DM, (string s) => s.Length < 100, false, "What should the word be?", true).Trim().ToUpper();
                check = word.RemoveDiacritics();
            }
        }
    }
}