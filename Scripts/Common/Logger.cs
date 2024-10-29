using System;
using System.Text;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The Logger class provides a simple logging utility for tracking messages and events.
    /// It can also send messages to the chat using a Broadcast Controller if available.
    /// </summary>
    public static class Logger
    {
        private static StringBuilder _logBuilder = new StringBuilder();
        private static IMyTextPanel _messageBlock; // Message block for notifications
        private static IMyBroadcastListener _broadcastController; // Broadcast Controller for chat notifications

        /// <summary>
        /// Initializes the Logger and detects the Broadcast Controller.
        /// </summary>
        public static void Initialize()
        {
            _broadcastController = FindBlockByTag<IMyBroadcastListener>(BLOCK_TAG); // Detecting the Broadcast Controller
        }

        /// <summary>
        /// Logs a message with a timestamp.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            _logBuilder.AppendLine($"{DateTime.Now}: {message}");
            SendMessageToChat(message); // Send message to the chat if the Broadcast Controller is available
        }

        /// <summary>
        /// Sends a message to the chat using the Broadcast Controller if it is available.
        /// </summary>
        /// <param name="message">The message to send.</param>
        private static void SendMessageToChat(string message)
        {
            if (_broadcastController != null)
            {
                _broadcastController.SendBroadcast(message);
            }
        }

        /// <summary>
        /// Retrieves the current log as a string.
        /// </summary>
        /// <returns>The log content.</returns>
        public static string GetLog()
        {
            return _logBuilder.ToString();
        }

        /// <summary>
        /// Clears the log content.
        /// </summary>
        public static void ClearLog()
        {
            _logBuilder.Clear();
        }

        /// <summary>
        /// Finds the first block of type T with the specified tag.
        /// </summary>
        /// <typeparam name="T">The type of block to find.</typeparam>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>The first block of type T with the specified tag.</returns>
        private static T FindBlockByTag<T>(string tag) where T : class
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Contains(tag));
            return blocks.FirstOrDefault() as T;
        }
    }
}
