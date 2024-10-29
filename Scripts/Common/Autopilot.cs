using System;
using System.Collections.Generic;
using System.Linq;
using VRageMath;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The Autopilot class manages path recording, navigation, and home location.
    /// It includes intelligent features for obstacle detection and autonomous actions.
    /// All blocks used will reference the same tag defined by BLOCK_TAG.
    /// Required blocks for the Autopilot include:
    /// - Remote Control
    /// - Proximity Sensor
    /// - Gyroscope
    /// - Thrusters
    /// - Battery or Power Source
    /// - Camera (optional)
    /// - Connector or Docking Port (if docking is needed)
    /// - Cargo Container (if transporting cargo)
    /// </summary>
    public class Autopilot
    {
        // ------------ Configurable Constants ------------        
        public const string BLOCK_TAG = "[AIP]"; // Tag used to identify blocks used by the script
        public const string PATH_PREFIX = "Path"; // Prefix used to identify paths in CustomData
        public const string HOME_VAR = "HOME"; // Variable name used to store the home location in CustomData
        public const int MAX_SPEED = 30; // The maximum speed of the remote control
        public const float FONT_SIZE = .6f; // The font size of the LCD panel
        public const float OBSTACLE_DETECTION_RANGE = 10f; // Range for obstacle detection
        public const float LOW_BATTERY_THRESHOLD = 20f; // Low battery threshold percentage

        private Vector3D _homeLocation = Vector3D.Zero; // The location of the home point
        private Dictionary<string, Path> _paths = new Dictionary<string, Path>(); // Paths with names
        private int _pathCount = 0; // Number of paths
        private List<Vector3D> _currentPath = new List<Vector3D>(); // Path being recorded
        private bool _isRecordingPath = false; // Status of path recording

        /// <summary>
        /// Initializes a new instance of the Autopilot class.
        /// </summary>
        public Autopilot()
        {
            BlockDependencies.Initialize(); // Initialize block dependencies
        }

        /// <summary>
        /// Checks the battery state and returns true if it's below the threshold.
        /// </summary>
        private bool CheckBatteryState()
        {
            var battery = FindBlockByTag<IMyBatteryBlock>(BLOCK_TAG);
            if (battery != null)
            {
                float currentPower = battery.CurrentStoredPower;
                float maxPower = battery.MaxStoredPower;
                float batteryPercentage = (currentPower / maxPower) * 100;

                if (batteryPercentage < LOW_BATTERY_THRESHOLD)
                {
                    Logger.Log("Battery is low: " + batteryPercentage + "%");
                    return true; // Battery is low
                }
            }
            return false; // Battery is sufficient
        }

        /// <summary>
        /// Saves the current location as the home location.
        /// </summary>
        public void SaveHome()
        {
            if (BlockDependencies.RemoteControl != null)
            {
                _homeLocation = BlockDependencies.RemoteControl.GetPosition();
                Logger.Log("Home location set to: " + _homeLocation.ToString());
                // SaveCustomData(HOME_VAR, _homeLocation.ToString()); // Uncomment when SaveCustomData is implemented
            }
            else
            {
                Logger.Log("Remote Control is not initialized.");
            }
        }

        /// <summary>
        /// Starts recording the current path.
        /// </summary>
        public void RecordPath()
        {
            _isRecordingPath = true;
            Logger.Log("Started recording path.");
        }

        /// <summary>
        /// Stops recording the current path and saves it.
        /// </summary>
        public void StopRecordingPath()
        {
            _isRecordingPath = false;
            if (_currentPath.Count > 0)
            {
                string pathName = PATH_PREFIX + _pathCount;
                _paths.Add(pathName, new Path(pathName, _currentPath));
                Logger.Log("Stopped recording path: " + pathName);
                _pathCount++;
                // SavePaths(); // Save paths to custom data
                _currentPath.Clear();
            }
        }

        /// <summary>
        /// Navigates to the home location.
        /// </summary>
        public void GoHome()
        {
            if (CheckBatteryState())
            {
                Logger.Log("Low battery detected. Returning home.");
                return; // Stop further actions if battery is low
            }

            if (BlockDependencies.RemoteControl != null)
            {
                BlockDependencies.RemoteControl.SetAutoPilotEnabled(false);
                BlockDependencies.RemoteControl.ClearWaypoints();
                BlockDependencies.RemoteControl.AddWaypoint(_homeLocation, HOME_VAR);
                BlockDependencies.RemoteControl.SetAutoPilotEnabled(true);
                Logger.Log("Navigating to home location.");
            }
            else
            {
                Logger.Log("Remote Control is not initialized.");
            }
        }

        /// <summary>
        /// Flies to the specified path.
        /// </summary>
        /// <param name="pathName">The name of the path to fly to.</param>
        public void FlyPath(string pathName)
        {
            if (CheckBatteryState())
            {
                Logger.Log("Low battery detected. Cannot fly path.");
                return; // Stop further actions if battery is low
            }

            if (BlockDependencies.RemoteControl != null && _paths.ContainsKey(pathName))
            {
                BlockDependencies.RemoteControl.SetAutoPilotEnabled(false);
                BlockDependencies.RemoteControl.ClearWaypoints();
                foreach (Vector3D waypoint in _paths[pathName].Waypoints)
                {
                    BlockDependencies.RemoteControl.AddWaypoint(waypoint, pathName);
                }
                BlockDependencies.RemoteControl.SetAutoPilotEnabled(true);
                Logger.Log("Flying to path: " + pathName);
            }
            else
            {
                Logger.Log("Path not found: " + pathName);
            }
        }

        /// <summary>
        /// Flies through all paths and returns home.
        /// </summary>
        public void FlyAllPathsAndReturnHome()
        {
            if (_paths.Count == 0)
            {
                Logger.Log("No paths recorded.");
                return;
            }
            if (CheckBatteryState())
            {
                Logger.Log("Low battery detected. Returning home.");
                return; // Stop further actions if battery is low
            }

            if (BlockDependencies.RemoteControl != null)
            {
                BlockDependencies.RemoteControl.SetAutoPilotEnabled(false);
                BlockDependencies.RemoteControl.ClearWaypoints();
                foreach (Path path in _paths.Values)
                {
                    foreach (Vector3D waypoint in path.Waypoints)
                    {
                        BlockDependencies.RemoteControl.AddWaypoint(waypoint, "IP");
                    }
                }
                BlockDependencies.RemoteControl.AddWaypoint(_homeLocation, HOME_VAR);
                BlockDependencies.RemoteControl.SetAutoPilotEnabled(true);
                Logger.Log("Flying through all paths and returning home.");
            }
            else
            {
                Logger.Log("Remote Control is not initialized.");
            }
        }

        /// <summary>
        /// Deletes the specified path.
        /// </summary>
        /// <param name="pathName">The name of the path to delete.</param>
        public void DeletePath(string pathName)
        {
            if (_paths.ContainsKey(pathName))
            {
                _paths.Remove(pathName);
                Logger.Log("Deleted path: " + pathName);
                // SaveCustomData(pathName, ""); // Uncomment when SaveCustomData is implemented
                _pathCount = _paths.Count;
                // SaveCustomData(PATH_COUNT_VAR, _pathCount.ToString()); // Uncomment when SaveCustomData is implemented
            }
            else
            {
                Logger.Log("Path not found: " + pathName);
            }
        }

        /// <summary>
        /// Wipes all data, including home location and recorded paths.
        /// </summary>
        public void WipeAllData()
        {
            _homeLocation = Vector3D.Zero;
            _paths.Clear();
            _currentPath.Clear();
            Logger.Log("All data wiped.");
        }

        /// <summary>
        /// Detects obstacles ahead and avoids collisions.
        /// </summary>
        public void DetectObstacles()
        {
            var position = BlockDependencies.RemoteControl.GetPosition();
            var direction = BlockDependencies.RemoteControl.WorldMatrix.Forward; // Get the forward direction of the ship
            var rayLength = OBSTACLE_DETECTION_RANGE;

            // Perform raycast
            if (MyAPIGateway.Physics.CastRay(position, position + direction * rayLength, out var hitInfo))
            {
                if (hitInfo.HitEntity != null)
                {
                    Logger.Log("Obstacle detected: " + hitInfo.HitEntity.Name);
                    GoHome(); // Return home if an obstacle is detected
                }
            }
        }

        /// <summary>
        /// Monitors the health of the ship and returns home if damaged.
        /// </summary>
        public void MonitorHealth()
        {
            // Implement health monitoring logic here
            // If damage is detected, call GoHome() and send a warning message
            if (BlockDependencies.RemoteControl.IsUnderControl)
            {
                // Check for damage (this is a placeholder; actual implementation may vary)
                if (BlockDependencies.RemoteControl.GetBlockHealth() < 100) // Assuming 100 is full health
                {
                    Logger.Log("Damage detected! Returning home.");
                    GoHome();
                }
            }
        }

        /// <summary>
        /// Represents a path with a name and a list of waypoints.
        /// </summary>
        public struct Path
        {
            public string Name { get; set; }
            public List<Vector3D> Waypoints { get; set; }

            public Path(string name, List<Vector3D> waypoints = null)
            {
                Name = name;
                Waypoints = waypoints ?? new List<Vector3D>();
            }
        }

        /// <summary>
        /// Finds the first block of type T with the specified tag.
        /// </summary>
        /// <typeparam name="T">The type of block to find.</typeparam>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>The first block of type T with the specified tag.</returns>
        private T FindBlockByTag<T>(string tag) where T : class
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Contains(tag));
            return blocks.FirstOrDefault() as T;
        }
    }
}
