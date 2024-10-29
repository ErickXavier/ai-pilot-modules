using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The UserInterface class manages the user interface for the AI Pilot Module.
    /// It provides methods for detecting screens and configuring ship roles.
    /// </summary>
    public static class UserInterface
    {
        public static List<IMyTextPanel> Screens { get; private set; } = new List<IMyTextPanel>();
        public static List<string> AvailableRoles { get; private set; } = new List<string>();
        public static string CurrentRole { get; private set; } = null; // Track the currently configured role
        private static int currentOptionIndex = 0; // Track the current option index
        private static string shipName = ""; // Track the ship name
        private static string currentScreen = "Initial Configuration"; // Track the current screen

        private static Dictionary<string, Dictionary<string, System.Action>> screenList = new Dictionary<string, Dictionary<string, System.Action>>();
        private static Dictionary<string, bool> settingsState = new Dictionary<string, bool>(); // Track toggle states

        /// <summary>
        /// Initializes the user interface by detecting screens with the specified tag.
        /// </summary>
        public static void Initialize()
        {
            Screens = FindBlocksByTag<IMyTextPanel>(Autopilot.BLOCK_TAG);
            if (Screens.Count == 0)
            {
                Logger.Log("No screens found with the tag: " + Autopilot.BLOCK_TAG);
            }
            DetectAvailableBlocks(); // Detect available blocks for role configuration
            InitializeScreenConfigurations(); // Set up screen configurations
        }

        /// <summary>
        /// Finds all blocks of type T with the specified tag.
        /// </summary>
        /// <typeparam name="T">The type of block to find.</typeparam>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>A list of blocks of type T with the specified tag.</returns>
        private static List<T> FindBlocksByTag<T>(string tag) where T : class
        {
            List<T> blocks = new List<T>();
            GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Contains(tag));
            return blocks;
        }

        /// <summary>
        /// Detects all relevant blocks available on the grid and configures roles accordingly.
        /// </summary>
        private static void DetectAvailableBlocks()
        {
            // Clear previous roles
            AvailableRoles.Clear();

            // Check for required blocks and add roles based on their presence
            if (BlockDependencies.RemoteControl != null)
            {
                AvailableRoles.Add("Miner");
            }
            if (BlockDependencies.Drills.Count > 0)
            {
                AvailableRoles.Add("Miner");
            }
            if (BlockDependencies.Grinders.Count > 0)
            {
                AvailableRoles.Add("Grinder");
            }
            if (BlockDependencies.Welders.Count > 0)
            {
                AvailableRoles.Add("Welder");
            }
            if (BlockDependencies.CargoContainers.Count > 0)
            {
                AvailableRoles.Add("Cargo Transport");
            }
            if (BlockDependencies.PassengerSeats.Count > 0)
            {
                AvailableRoles.Add("Passenger Transport");
            }
        }

        /// <summary>
        /// Initializes the screen configurations for each role.
        /// </summary>
        private static void InitializeScreenConfigurations()
        {
            screenList["Initial Configuration"] = new Dictionary<string, System.Action>
            {
                { "Set Ship Name", () => SetShipName() },
                { "Set Ship Role", () => DisplayRoleOptions() }
            };

            screenList["Miner Actions"] = new Dictionary<string, System.Action>
            {
                { "Start Mining", () => Logger.Log("Start Mining option selected.") },
                { "Stop Mining", () => Logger.Log("Stop Mining option selected.") },
                { "Go to Next Area", () => Logger.Log("Go to Next Area option selected.") },
                { "Dump Stone", () => Logger.Log("Dump Stone option selected.") },
                { "Settings", () => DisplayScreen("Miner Settings") }
            };

            screenList["Grinder Actions"] = new Dictionary<string, System.Action>
            {
                { "Start Grinding", () => Logger.Log("Start Grinding option selected.") },
                { "Stop Grinding", () => Logger.Log("Stop Grinding option selected.") },
                { "Go to Next Area", () => Logger.Log("Go to Next Area option selected.") },
                { "Dump Scrap", () => Logger.Log("Dump Scrap option selected.") },
                { "Settings", () => DisplayScreen("Grinder Settings") }
            };

            screenList["Welder Actions"] = new Dictionary<string, System.Action>
            {
                { "Start Welding", () => Logger.Log("Start Welding option selected.") },
                { "Stop Welding", () => Logger.Log("Stop Welding option selected.") },
                { "Go to Next Area", () => Logger.Log("Go to Next Area option selected.") },
                { "Settings", () => DisplayScreen("Welder Settings") }
            };

            screenList["Cargo Transport Actions"] = new Dictionary<string, System.Action>
            {
                { "Load Cargo", () => Logger.Log("Load Cargo option selected.") },
                { "Unload Cargo", () => Logger.Log("Unload Cargo option selected.") },
                { "Go to Next Area", () => Logger.Log("Go to Next Area option selected.") },
                { "Settings", () => DisplayScreen("Cargo Transport Settings") }
            };

            screenList["Passenger Transport Actions"] = new Dictionary<string, System.Action>
            {
                { "Start Transport", () => Logger.Log("Start Transport option selected.") },
                { "Stop Transport", () => Logger.Log("Stop Transport option selected.") },
                { "Go to Next Area", () => Logger.Log("Go to Next Area option selected.") },
                { "Settings", () => DisplayScreen("Passenger Transport Settings") }
            };

            // Add settings for each role
            screenList["Miner Settings"] = new Dictionary<string, System.Action>
            {
                { "Dump Stone [true/false]", () => ToggleSetting("Dump Stone") },
                { "Dump Stone Location: [Above mining area/During travel/At home]", () => Logger.Log("Dump Stone Location option selected.") },
                { "Set Mining Depth: [auto / list of numbers from 10 to 600m]", () => Logger.Log("Set Mining Depth option selected.") },
                { "Fill Detection: [true/false]", () => ToggleSetting("Fill Detection") }
            };

            screenList["Grinder Settings"] = new Dictionary<string, System.Action>
            {
                { "Dump Scrap [true/false]", () => ToggleSetting("Dump Scrap") },
                { "Set Grinding Speed: [value]", () => Logger.Log("Set Grinding Speed option selected.") }
            };

            screenList["Welder Settings"] = new Dictionary<string, System.Action>
            {
                { "Set Welding Speed: [value]", () => Logger.Log("Set Welding Speed option selected.") }
            };

            screenList["Cargo Transport Settings"] = new Dictionary<string, System.Action>
            {
                { "Set Cargo Capacity: [value]", () => Logger.Log("Set Cargo Capacity option selected.") }
            };

            screenList["Passenger Transport Settings"] = new Dictionary<string, System.Action>
            {
                { "Set Passenger Capacity: [value]", () => Logger.Log("Set Passenger Capacity option selected.") }
            };
        }

        /// <summary>
        /// Displays the specified screen based on the screen name.
        /// </summary>
        /// <param name="screenName">The name of the screen to display.</param>
        private static void DisplayScreen(string screenName)
        {
            currentScreen = screenName;
            currentOptionIndex = 0; // Reset cursor position
            foreach (var screen in Screens)
            {
                // header
                screen.WriteText("---==EX AI Pilot==---\n", false);
                // title of page
                screen.WriteText($"Page: {screenName}\n", true);
                if (screenList.ContainsKey(screenName))
                {
                    for (int i = 0; i < screenList[screenName].Count; i++)
                    {
                        var option = screenList[screenName].ElementAt(i);
                        string prefix = (i == currentOptionIndex) ? "> " : "  ";
                        screen.WriteText($"{prefix}{option.Key}\n", true);
                    }
                }
                else
                {
                    screen.WriteText("No options available.\n", true);
                }
            }
        }

        /// <summary>
        /// Handles user input from the main menu.
        /// </summary>
        public static void HandleUserInput(string input)
        {
            switch (input)
            {
                case "UP":
                    MoveCursor(-1);
                    break;
                case "DOWN":
                    MoveCursor(1);
                    break;
                case "ENTER":
                    ExecuteAction();
                    break;
                case "RETURN":
                    ReturnToPreviousScreen();
                    break;
                default:
                    Logger.Log("Invalid input: " + input);
                    break;
            }
        }

        /// <summary>
        /// Moves the cursor up or down.
        /// </summary>
        /// <param name="direction">-1 for up, 1 for down.</param>
        private static void MoveCursor(int direction)
        {
            int newIndex = currentOptionIndex + direction;
            // Ensure the cursor does not navigate to the header or title
            if (newIndex >= 0 && newIndex < screenList[currentScreen].Count)
            {
                currentOptionIndex = newIndex;
            }
            DisplayScreen(currentScreen); // Refresh the screen to show the updated cursor position
        }

        /// <summary>
        /// Executes the action based on the current cursor position.
        /// </summary>
        private static void ExecuteAction()
        {
            if (screenList[currentScreen].Count > 0)
            {
                var action = screenList[currentScreen].Values.ElementAt(currentOptionIndex);
                action.Invoke();
            }
        }

        /// <summary>
        /// Toggles the setting for the selected option.
        /// </summary>
        private static void ToggleSetting(string settingName)
        {
            if (!settingsState.ContainsKey(settingName))
            {
                settingsState[settingName] = false; // Default to false
            }
            settingsState[settingName] = !settingsState[settingName]; // Toggle the state
            Logger.Log($"{settingName} is now set to {settingsState[settingName]}.");
            DisplayScreen(currentScreen); // Refresh the screen to show the updated state
        }

        /// <summary>
        /// Returns to the previous screen (implementation can be defined as needed).
        /// </summary>
        private static void ReturnToPreviousScreen()
        {
            // Logic to return to the previous screen can be implemented here
            Logger.Log("Returning to the previous screen...");
        }
    }
}
