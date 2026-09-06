using CustomPlantClass.Networking;
using CustomPlantClass.Runtime.Tasks;
using UnityEngine.Networking;
using WaitUntil = CustomPlantClass.Runtime.Tasks.WaitUntil;

namespace CustomPlantClass.Runtime
{
    /*
    public class AsyncHelper
    {
        public static IDelay WaitUntilTCPCommand(string commandName)
        {
            var comListener = new CommandListener(commandName, (data) => { });
            return new WaitUntil(() => comListener.isCommandReceived);
        }
        public static IDelay WaitUntilTCPCommand(string commandName, Action<string> onCommandReceived)
        {
            var comListener = new CommandListener(commandName, onCommandReceived);
            return new WaitUntil(() => comListener.isCommandReceived);
        }
        private class CommandListener : ICommandListener
        {
            public string CommandName { get; }

            private Action<string> onCommandReceived;
            internal bool isCommandReceived = false;

            public void OnCommandReceived(string data)
            {
                isCommandReceived = true;
                onCommandReceived?.Invoke(data);
            }
            public CommandListener(string commandName, Action<string> onCommandReceived)
            {
                CommandName = commandName;
                this.onCommandReceived = onCommandReceived;
            }
        }
    }
    */
}