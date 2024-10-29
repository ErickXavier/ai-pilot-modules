using System;
using System.Collections.Generic;
using VRageMath;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The PassengerTransport class manages passenger travel.
    /// All blocks used will reference the same tag defined by BLOCK_TAG.
    /// </summary>
    public class PassengerTransport
    {
        private IMyRemoteControl _remoteControl; // Remote control block
        private List<Vector3D> _passengerWaypoints; // List of waypoints for passenger transport
        private int _currentWaypointIndex; // Index of the current waypoint
        private int _passengerCapacity; // Variable for passenger capacity

        /// <summary>
        /// Initializes a new instance of the PassengerTransport class.
        /// </summary>
        /// <param name="remoteControl">The remote control block to be used.</param>
        public PassengerTransport(IMyRemoteControl remoteControl)
        {
            _remoteControl = remoteControl;
            _passengerWaypoints = new List<Vector3D>();
            _currentWaypointIndex = 0;
            _passengerCapacity = 10; // Default capacity
        }

        /// <summary>
        /// Toggles the passenger capacity setting.
        /// </summary>
        public void SetPassengerCapacity(int capacity)
        {
            _passengerCapacity = capacity;
            Logger.Log($"Passenger Capacity is now set to {_passengerCapacity}.");
        }

        // Existing methods (AddPassengerWaypoint, StartTransport, MoveToNextPassengerWaypoint, StopTransport) remain unchanged
    }
}
