Regardless of where WTB is running, it requires you to create the bot user on [Discord Developer Portal](https://discord.com/developers/applications). Make sure to copy its token as you'll need it.

## Prod
Because WingTech Bot uses Docker, this should be extremely easy. On your server, navigate to the `Bash Scripts` folder, and run `BuildAndRunBot.sh`, optionally providing a branch name. This will start up Docker and the containers for WTB.

## Dev
Put the `.env` and `config.json` files in the WingTechBot project root. You will probably need to provide the environment variable values in your IDE's run configuration. In JetBrains Rider you do it like so:

![[Pasted image 20250204161528.png|700px]]

After that, make sure PostgreSQL is running and it all should just work™️.

