# WingTechBot
![Online](https://img.shields.io/discord/111588824525627392?label=Servicing%20Users%3A&style=for-the-badge)

WingTechBot is a Discord bot written in C# that provides a karma system, games, alarms, and cute inside jokes to my private Discord server.

## Contribution

### Forking

To contribute to WingTechBot, follow these steps:

1. Fork this repository.
2. Create a branch: `git checkout -b <branch_name>`.
3. Make your changes and commit them: `git commit -m '<commit_message>'`
4. Push to the original branch: `git push origin <project_name>/<location>`
5. Create the pull request.

Alternatively see the GitHub documentation on [creating a pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request).

### Using Your Fork of WingTechBot

1. Create a new empty Discord server (or test it on your own, whatever).
2. Using a web browser, visit the [Discord Developer Portal](https://discord.com/developers/applications) and create a new Application.
3. Add a Bot to your application.
4. Save the token for your Bot somewhere, we'll use that later.
5. Under OAuth2, enable the "bot" scope.
6. Generate an invite URL for your bot and use it to add the bot to your server.
7. Fork, build, and run this repo. It should error, but create a file within "bin/save" called "config.json"
8. Open "config.json"
9. Replace the null value under "LoginToken" with the bot token you saved earlier.
10. Replace the null value under "OwnerID" with your Discord user ID (right-click yourself in Discord and choose "Copy ID", it should be one really big number)
11. Replace the null value under "ServerID" with your Discord server ID (right-click your server icon on the left in Discord and choose "Copy ID", it should be one really big number)
12. If you have a designated bot channel, replace the null value under "BotChannelID" with the ID of that channel.
13. Run your fork of the repo again. WingTechBot should deploy to your server.
14. Modify your fork of the repo at your leisure.


If you're looking for how to implement a game for WingTechBot, I'd recommend looking at the file "Counting.cs". It's a pretty simple game that shows of the basics of the WingTechBot game platform.

My code uses reflection to create the game pool. Any class deriving from the abstract class **Game** will be automatically added to the available games. However, to add games to the my version of WingTechBot, you'll have to submit a pull request as explained above.

## License

![License](https://img.shields.io/github/license/winggar/WingTechBot?style=for-the-badge)
