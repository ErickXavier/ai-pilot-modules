using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The BlockDependencies class manages the detection and initialization of required blocks for the AI Pilot Module.
    /// </summary>
    public static class BlockDependencies
    {
        public static IMyRemoteControl RemoteControl { get; private set; }
        public static IMyTextPanel MessageBlock { get; private set; }
        public static List<IMySensorBlock> ProximitySensors { get; private set; } = new List<IMySensorBlock>();
        public static List<IMyGyro> Gyroscopes { get; private set; } = new List<IMyGyro>();
        public static List<IMyThrust> Thrusters { get; private set; } = new List<IMyThrust>();
        public static IMyBatteryBlock Battery { get; private set; }
        public static List<IMyCameraBlock> Cameras { get; private set; } = new List<IMyCameraBlock>();
        public static List<IMyShipConnector> Connectors { get; private set; } = new List<IMyShipConnector>();
        public static List<IMyCargoContainer> CargoContainers { get; private set; } = new List<IMyCargoContainer>();
        public static List<IMyShipDrill> Drills { get; private set; } = new List<IMyShipDrill>();
        public static List<IMyShipGrinder> Grinders { get; private set; } = new List<IMyShipGrinder>();
        public static List<IMyShipWelder> Welders { get; private set; } = new List<IMyShipWelder>();
        public static List<IMyCockpit> PassengerSeats { get; private set; } = new List<IMyCockpit>();
        public static IMyAIRecorder AIRecorder { get; private set; }
        public static IMyOreDetector OreDetector { get; private set; }

        /// <summary>
        /// Initializes the required blocks based on the BLOCK_TAG constant.
        /// </summary>
        public static void Initialize()
        {
            RemoteControl = FindBlockByTag<IMyRemoteControl>(Autopilot.BLOCK_TAG);
            MessageBlock = FindBlockByTag<IMyTextPanel>(Autopilot.BLOCK_TAG);
            ProximitySensors = FindBlocksByTag<IMySensorBlock>(Autopilot.BLOCK_TAG);
            Gyroscopes = FindBlocksByTag<IMyGyro>(Autopilot.BLOCK_TAG);
            Thrusters = FindBlocksByTag<IMyThrust>(Autopilot.BLOCK_TAG);
            Battery = FindBlockByTag<IMyBatteryBlock>(Autopilot.BLOCK_TAG);
            Cameras = FindBlocksByTag<IMyCameraBlock>(Autopilot.BLOCK_TAG);
            Connectors = FindBlocksByTag<IMyShipConnector>(Autopilot.BLOCK_TAG);
            CargoContainers = FindBlocksByTag<IMyCargoContainer>(Autopilot.BLOCK_TAG);
            Drills = FindBlocksByTag<IMyShipDrill>(Autopilot.BLOCK_TAG);
            Grinders = FindBlocksByTag<IMyShipGrinder>(Autopilot.BLOCK_TAG);
            Welders = FindBlocksByTag<IMyShipWelder>(Autopilot.BLOCK_TAG);
            PassengerSeats = FindBlocksByTag<IMyCockpit>(Autopilot.BLOCK_TAG);
            AIRecorder = FindBlockByTag<IMyAIRecorder>(Autopilot.BLOCK_TAG);
            OreDetector = FindBlockByTag<IMyOreDetector>(Autopilot.BLOCK_TAG);
            ValidateBlocks(); // Check if all required blocks are initialized
        }

        /// <summary>
        /// Validates that all required blocks are initialized.
        /// </summary>
        private static void ValidateBlocks()
        {
            if (RemoteControl == null)
            {
                Logger.Log("Warning: Remote Control block is not found.");
            }
            if (MessageBlock == null)
            {
                Logger.Log("Warning: Message Block is not found.");
            }
            if (ProximitySensors.Count == 0)
            {
                Logger.Log("Warning: No Proximity Sensors found.");
            }
            if (Gyroscopes.Count == 0)
            {
                Logger.Log("Warning: No Gyroscopes found.");
            }
            if (Thrusters.Count == 0)
            {
                Logger.Log("Warning: No Thrusters found.");
            }
            if (Battery == null)
            {
                Logger.Log("Warning: Battery block is not found.");
            }
            if (Cameras.Count == 0)
            {
                Logger.Log("Warning: No Cameras found.");
            }
            if (Connectors.Count == 0)
            {
                Logger.Log("Warning: No Connectors found.");
            }
            if (CargoContainers.Count == 0)
            {
                Logger.Log("Warning: No Cargo Containers found.");
            }
            if (Drills.Count == 0)
            {
                Logger.Log("Warning: No Drill blocks found.");
            }
            if (Grinders.Count == 0)
            {
                Logger.Log("Warning: No Grinder blocks found.");
            }
            if (Welders.Count == 0)
            {
                Logger.Log("Warning: No Welder blocks found.");
            }
            if (PassengerSeats.Count == 0)
            {
                Logger.Log("Warning: No Passenger Seats found.");
            }
            if (AIRecorder == null)
            {
                Logger.Log("Warning: AI Recorder block is not found.");
            }
            if (OreDetector == null)
            {
                Logger.Log("Warning: Ore Detector block is not found.");
            }
        }

        /// <summary>
        /// Checks if a specific block type is present on the grid.
        /// </summary>
        /// <typeparam name="T">The type of block to check.</typeparam>
        /// <returns>True if the block is present, otherwise false.</returns>
        public static bool IsBlockPresent<T>() where T : class
        {
            return FindBlocksByTag<T>(Autopilot.BLOCK_TAG).Count > 0;
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
    }
}
