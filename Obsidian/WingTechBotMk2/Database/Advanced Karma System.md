The current karma system is good, but I would like to extend it with more features. I want to use a PostgreSQL database to allow us to easily see stats like how much karma a user has given to another user. 

## Tables
### Karma

| Giver | Receiver | Amount (int) | Created At | Updated At |
| ----- | -------- | ------------ | ---------- | ---------- |
|       |          |              |            |            |

## Awards

| Giver | Receiver | Emoji | Created At | Updated At |
| ----- | -------- | ----- | ---------- | ---------- |
|       |          |       |            |            |
Tracks how often a person has reacted to another person's posts with a specific emoji. E.g., `:gold:` or `:fire:`. I want this to be available for *any* emoji, not just the typical gold/silver/platinum ones.


## Operations
- Get how much karma given to a user/all users.
- Get how much karma received from a user/all users.
- List all users.
- Increment/decrement the karma/award counter, first checking if the row exists and adding it if not.
- Ability to delete records for a user.

## Watch Out For
Any karma-related events (e.g., giving an upvote) to a post before a certain date should be ignored, as this would cause a lot of issues. This is controlled by [[config.json#Karma Start Date]]. 

Make sure it works in *all* channels, including the text channels for VC channels.
