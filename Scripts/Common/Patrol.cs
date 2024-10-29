using System;
using System.Collections.Generic;
using VRageMath;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The Patrol class manages security and surveillance patrols.
    /// All blocks used will reference the same tag defined by BLOCK_TAG.
    /// </summary>
    public class Patrol
    {
        private IMyRemoteControl _remoteControl; // Remote control block
        private List<Vector3D> _patrolWaypoints; // List of waypoints for patrolling
        private int _currentWaypointIndex; // Index of the current waypoint

        /// <summary>
        /// Initializes a new instance of the Patrol class.
        /// </summary>
        /// <param name="remoteControl">The remote control block to be used.</param>
        public Patrol(IMyRemoteControl remoteControl)
        {
            _remoteControl = remoteControl;
            _patrolWaypoints = new List<Vector3D>();
            _currentWaypointIndex = 0;
        }

        /// <summary>
        /// Adds a waypoint to the patrol route.
        /// </summary>
        /// <param name="waypoint">The waypoint to add.</param>
        public void AddWaypoint(Vector3D waypoint)
        {
            _patrolWaypoints.Add(waypoint);
        }

        /// <summary>
        /// Starts the patrol sequence.
        /// </summary>
        public void StartPatrol()
        {
            if (_patrolWaypoints.Count > 0)
            {
                MoveToNextWaypoint();
            }
            else
            {
                Logger.Log("No waypoints defined for patrol.");
            }
        }

        private void MoveToNextWaypoint()
        {
            if (_currentWaypointIndex < _patrolWaypoints.Count)
            {
                Vector3D nextWaypoint = _patrolWaypoints[_currentWaypointIndex];
                _remoteControl.ClearWaypoints();
                _remoteControl.AddWaypoint(nextWaypoint, "Patrol Waypoint");
                _remoteControl.SetAutoPilotEnabled(true);
                Logger.Log($"Moving to waypoint: {nextWaypoint}");
                _currentWaypointIndex++;
            }
            else
            {
                Logger.Log("Patrol completed. Returning to start.");
                _currentWaypointIndex = 0; // Reset to start
                MoveToNextWaypoint(); // Optionally return to the first waypoint
            }
        }

        /// <summary>
        /// Stops the patrol operation.
        /// </summary>
        public void StopPatrol()
        {
            _remoteControl.SetAutoPilotEnabled(false);
            Logger.Log("Patrol stopped.");
        }
    }
}
