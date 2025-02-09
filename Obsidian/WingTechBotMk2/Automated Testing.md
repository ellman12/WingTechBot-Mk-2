The most important parts of WTB (like [[Reaction Tracking and Karma]]) have fully automated testing with NUnit v3. All of this was set up to facilitate supporting TDD (😍).

Instead of having massive class files with many tests in them, the tests are broken up based on the method being tested, with the format of `<MethodName>Tests.cs`. Setting it up like this also removes the need to have very long and obnoxious names for the tests, because you can name them just on what they're testing for. E.g., `ReactionEmoteDoesNotExist()` or `FailsWhenReactionExists()`. 

## Setting Up Testing
### Model Tests
The model tests are very simple to set up and only require a `.env` file in the project's root folder, since it doesn't interact with the bot at all.

### Integration Tests
The integration tests are more involved and therefore require more setup. First, you'll need a `.env` file in the project root just like Model Tests. You will also need a `bt_config.json` and `wtb_config.json`.

Next, you'll need to set up a bot on the [Discord Developer Portal](https://discord.com/developers/applications) which will act as the bot tester. This will simulate a real human user for features like [[Reaction Tracking and Karma]]. Get the token and put it in the `bt_config.json`. 

From there, everything should just work™️. 

