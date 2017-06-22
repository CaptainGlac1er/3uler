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
            console.UpdateObservers("service created");
        }
        public string GetPrefix()
        {
            return prefix;
        }
        public void UpdateConsole(string message){
            console.UpdateObservers(message);
        }
    }
}
