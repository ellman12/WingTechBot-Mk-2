`/gato` sends a random picture of a cat. Images are stored directly on disk in their own folder, and renamed to a uuid when uploaded.


## Operations

| Command      | Arguments                             | Description                                       |
| ------------ | ------------------------------------- | ------------------------------------------------- |
| /gato        | Optional number for how many to send. | Picks a random gato                               |
| /gato-add    | Message with an image(s)              | If the message contains an image(s), upload them. |
| /gato-list   |                                       | Lists all the files.                              |
| /gato-remove | filename (uuid)                       | Remove the image file with this uuid.             |

