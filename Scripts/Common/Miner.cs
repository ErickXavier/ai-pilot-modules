using System;
using System.Collections.Generic;
using VRageMath;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The Miner class manages mining operations.
    /// All blocks used will reference the same tag defined by BLOCK_TAG.
    /// </summary>
    public class Miner
    {
        private IMyRemoteControl _remoteControl; // Remote control block
        private List<Vector3D> _miningWaypoints; // List of waypoints for mining
        private int _currentWaypointIndex; // Index of the current waypoint
        private bool _dumpStone; // Toggle for dumping stone
        private bool _fillDetection; // Toggle for fill detection

        /// <summary>
        /// Initializes a new instance of the Miner class.
        /// </summary>
        /// <param name="remoteControl">The remote control block to be used.</param>
        public Miner(IMyRemoteControl remoteControl)
        {
            _remoteControl = remoteControl;
            _miningWaypoints = new List<Vector3D>();
            _currentWaypointIndex = 0;
            _dumpStone = false; // Default to false
            _fillDetection = false; // Default to false
        }

        /// <summary>
        /// Toggles the dump stone setting.
        /// </summary>
        public void ToggleDumpStone()
        {
            _dumpStone = !_dumpStone;
            Logger.Log($"Dump Stone is now set to {_dumpStone}.");
        }

        /// <summary>
        /// Toggles the fill detection setting.
        /// </summary>
        public void ToggleFillDetection()
        {
            _fillDetection = !_fillDetection;
            Logger.Log($"Fill Detection is now set to {_fillDetection}.");
        }

        // Existing methods (AddMiningWaypoint, StartMining, MoveToNextMiningWaypoint, StopMining) remain unchanged
    }
}
