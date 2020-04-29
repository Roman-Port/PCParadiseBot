## Creating Modules
Modules are where you MUST put your code. Here's how you create your own:

* Create a folder for your module. It MUST follow the following naming scheme: ``Modules>[YOUR NAME]>[YOUR MODULE NAME]``. For example, the example plugin lives in ``Modules>RomanPort>ExampleModule``.
* Create a new C# file inside of your module's folder. If you followed the previous step, it's namespace should be ``RomanPort.PCParadiseBot.Modules.[YOUR NAME].[YOUR MODULE NAME]``.
* Make this new C# file extend ``PCModule``. Paste the following code into the body:
```c#
public override async Task OnInit()
{
            
}
```
* In Program.cs, add the following line inside of ``AddModules``:
```
modules.Add(new [THE NAME OF YOUR MODULE]());
```
You'll need to also ``using`` your module's namespace at the top of the file.
* Inside of this OnInit function, you can use the Bind commands below, or do everything manually.

### Using Bind commands
To add your own commands, use the following bind commands:

* ``BindToChannel`` - This will bind a callback to all messages sent in a specific channel ID.
* ``BindToCommand`` - This will bind a command prefix, and will call your callback whenever **anybody** calls your command.
* ``BindToCommandRole`` - This will bind a command prefix, and will call your callback whenever **a specific role ID** calls your command. You should use BindToCommandAdmin and BindToCommandModerator instead when you can.
* ``BindToCommandAdmin`` - This will bind a command prefix, and will call your callback whenever **an admin** calls your command.
* ``BindToCommandModerator`` - This will bind a command prefix, and will call your callback whenever **a moderator** calls your command.

To bind, choose your function of choice and type in it's parameters. Callback functions are confusing, so here is some example code that may help you:

```c#
BindToCommandAdmin("%test", async (MessageCreateEventArgs e, string content, string[] args) =>
{
    Console.WriteLine("TEST!");
});
```

### Accessing the API
You can use Program.discord to access the Discord client manually. Read up on the DSharpPlus documentation for more information.

You can also use PCStatics.Enviornment to access channel IDs, server IDs, and role IDs. __**You must use this**__, as it will change depending on the enviornment.

### Running the bot
You'll need to include a ``config.json`` file in the output directory of this project. Reach out to ``RomanPort#0001`` to obtain this file if you're on my DevTest server.