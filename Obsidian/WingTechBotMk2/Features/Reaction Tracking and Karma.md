WingTech Bot tracks every[^1] reaction from every user in every text channel (including VC text channels). This is a significant expansion of the bot's original karma system, which only tracked a handful of emotes. Every reaction is a record in the [[Database]]. There are various commands to display this data in different forms.

The bot *does* track reactions you give yourself, however these are filtered out unless told otherwise.

Reactions added to or removed from messages sent before [[Config#StartDate]] are ignored.

## Commands
### /reactions
Displays all the reactions you have received this year. Self-reactions are ignored.

### /top
Calculates the karma leaderboard for this year. Karma is calculated based on `<upvotes> - <downvotes>`. Self-karma is ignored.


## Legacy Karma
WingTech Bot Mk 2 fully supports the karma system from the original bot. The format it used was a text file where one file was a year's worth of data and each line contained a user's data:
`<userId> <upvotes> <downvotes> <silver> <gold> <platinum>`. 

When Mk 2 is calculating reactions/karma, it appends this data to the results. If there is no data for a user for that year, nothing changes.


[^1]: This is partially true. WTB tracks built-in emojis (like 🙂) and custom emotes in the WingTech server (like upvotes and awards). Because the bot is not able to send custom emotes from private servers in its messages, it does not track them. This is an unfortunate limitation with Discord.