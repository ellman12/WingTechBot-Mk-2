A significant expansion of the current karma and award system. This will track every reaction including karma and awards.

## To Do
- [ ] Make sure reactions added to your own messages count AND that they don't count towards karma and the reactions totals

## Tables

Date values all use UTC.

### ReactionEmotes
Emotes used in reactions and karma. Automatically populated as needed.
Both these values are from Discord. When combined into the form Discord uses they resemble: "<name:id>".

- Id
- Name
- CreatedAt (Auto Generated)


### Reactions
Stores every reaction by every user.

- Id (Auto Increment)
- Giver (user id)
- Receiver (user id)
- MessageId (from Discord)
- EmoteId (refs ReactionEmotes.Id)
- CreatedAt (Auto Generated)


## Operations
- Add reaction
- Reaction removed from Discord → remove row
- User deleted → remove their reactions
- Message deleted → remove reactions targeting it

I would like to try and make this as modular and generic as popular. Almost like a command system.

### Reactions
- Get all reactions received from a user for the year (or any year)
- Get all reactions received from a user for all time
- Get all reactions received from all users for the year (or any year)
- Get all reactions received from all users for all time

- Get all reactions sent to a user for the year (or any year)
- Get all reactions sent to a user for all time
- Get all reactions sent to all users for the year (or any year)
- Get all reactions sent to all users for all time

- Get all reactions sent by all users for the year (or any year)
- Get all reactions sent by all users for all time

### Karma
- Get all karma from a user for the year (or any year)
- Get all karma from a user for all time
- Get all karma from all users for the year (or any year)
- Get all karma from all users for all time

- Get all karma sent to a user for the year (or any year)
- Get all karma sent to a user for all time
- Get all karma sent to all users for the year (or any year)
- Get all karma sent to all users for all time

- Get total karma for all users for the year (or any year)
- Get total karma for all users for all time

