using System;
using System.Collections.Generic;
using VRageMath;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The Grinder class handles material processing operations.
    /// All blocks used will reference the same tag defined by BLOCK_TAG.
    /// </summary>
    public class Grinder
    {
        private IMyRemoteControl _remoteControl; // Remote control block
        private List<Vector3D> _grindingWaypoints; // List of waypoints for grinding
        private int _currentWaypointIndex; // Index of the current waypoint
        private bool _dumpScrap; // Toggle for dumping scrap

        /// <summary>
        /// Initializes a new instance of the Grinder class.
        /// </summary>
        /// <param name="remoteControl">The remote control block to be used.</param>
        public Grinder(IMyRemoteControl remoteControl)
        {
            _remoteControl = remoteControl;
            _grindingWaypoints = new List<Vector3D>();
            _currentWaypointIndex = 0;
            _dumpScrap = false; // Default to false
        }

        /// <summary>
        /// Toggles the dump scrap setting.
        /// </summary>
        public void ToggleDumpScrap()
        {
            _dumpScrap = !_dumpScrap;
            Logger.Log($"Dump Scrap is now set to {_dumpScrap}.");
        }

        // Existing methods (AddGrindingWaypoint, StartGrinding, MoveToNextGrindingWaypoint, StopGrinding) remain unchanged
    }
}
