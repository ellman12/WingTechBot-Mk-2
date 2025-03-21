# WingTechBot
![Online](https://img.shields.io/discord/111588824525627392?label=Servicing%20Users%3A&style=for-the-badge)

WingTech Bot is a Discord bot written in C# that provides a reaction tracker, karma system, games, cute inside jokes, and more to my private Discord server.

![WTB 1](https://github.com/user-attachments/assets/4b4ee9ce-ef60-4688-8003-6c9cbec47a7b)

![WTB 2](https://github.com/user-attachments/assets/de08d885-22ee-4045-804d-6e2b94b8a4d0)

![WTB 3](https://github.com/user-attachments/assets/6fbe8a5e-bb78-434a-9db5-ebcf6080a738)

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
1. Create a new empty Discord server (or test it on your own).
2. Using a web browser, visit the [Discord Developer Portal](https://discord.com/developers/applications) and create a new Application.
3. Add a Bot to your application.
4. Save the token for your Bot somewhere, we'll use that later.
5. Under OAuth2, enable the "bot" scope.
6. Generate an invite URL for your bot and use it to add the bot to your server.
7. Fork, build, and run this repo. It should error, create a file in the project root called "config.json"
8. Open "config.json"
9. Replace the null value under "LoginToken" with the bot token you saved earlier. 
10. Replace the null value under "UserId" with the bot's Discord user ID.
11. Replace the null value under "ServerId" with your Discord server ID (right-click your server icon on the left in Discord and choose "Copy ID", it should be one really big number).
12. If you have a designated bot channel, replace the null value under "BotChannelID" with the ID of that channel.
13. Run your fork of the repo again. WingTech Bot should deploy to your server.
14. Modify your fork of the repo at your leisure.

## License
![License](https://img.shields.io/github/license/winggar/WingTechBot?style=for-the-badge)
