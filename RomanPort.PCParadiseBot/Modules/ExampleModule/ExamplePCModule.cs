using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RomanPort.PCParadiseBot.Modules.ExampleModule
{
    public class ExamplePCModule : PCModule
    {
        public override async Task OnInit()
        {
            BindToCommandAdmin("%test", async (MessageCreateEventArgs e, string content, string[] args) =>
            {
                Console.WriteLine("TEST!");
            });
        }
    }
}
