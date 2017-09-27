using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Services
{
    public class TestService
    {
        private string prefix;
        private ServerUpdater<string> console;
        public TestService(string prefix, ServerUpdater<string> console)
        {
            this.prefix = prefix;
            this.console = console;
        }
        public string GetPrefix()
        {
            return prefix;
        }
        public async Task UpdateConsole(string message){
            await console.UpdateObservers(message);
        }
    }
}
