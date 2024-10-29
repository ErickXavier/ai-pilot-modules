using System;
using System.Collections.Generic;
using VRageMath;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The CargoTransport class handles the transportation of cargo.
    /// All blocks used will reference the same tag defined by BLOCK_TAG.
    /// </summary>
    public class CargoTransport
    {
        private IMyRemoteControl _remoteControl; // Remote control block
        private List<Vector3D> _cargoWaypoints; // List of waypoints for cargo transport
        private int _currentWaypointIndex; // Index of the current waypoint
        private float _cargoCapacity; // Variable for cargo capacity

        /// <summary>
        /// Initializes a new instance of the CargoTransport class.
        /// </summary>
        /// <param name="remoteControl">The remote control block to be used.</param>
        public CargoTransport(IMyRemoteControl remoteControl)
        {
            _remoteControl = remoteControl;
            _cargoWaypoints = new List<Vector3D>();
            _currentWaypointIndex = 0;
            _cargoCapacity = 1000; // Default capacity
        }

        /// <summary>
        /// Toggles the cargo capacity setting.
        /// </summary>
        public void SetCargoCapacity(float capacity)
        {
            _cargoCapacity = capacity;
            Logger.Log($"Cargo Capacity is now set to {_cargoCapacity}.");
        }

        // Existing methods (AddCargoWaypoint, StartTransport, MoveToNextCargoWaypoint, StopTransport) remain unchanged
    }
}
