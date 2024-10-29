using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngineers.ErickXavier.AiPilotModule
{
    /// <summary>
    /// The Scout class handles the scouting functionalities for the AI Pilot Module.
    /// </summary>
    public class Scout
    {
        public List<string> FoundOres { get; private set; } = new List<string>();
        public List<string> FoundNeutralGrids { get; private set; } = new List<string>();
        public List<string> FoundEnemyGrids { get; private set; } = new List<string>();

        /// <summary>
        /// Finds ores based on user selection.
        /// </summary>
        /// <param name="selectedOres">The ores selected by the user.</param>
        public void FindOres(List<string> selectedOres)
        {
            foreach (var ore in selectedOres)
            {
                // Logic to find the ore
                FoundOres.Add(ore);
            }
        }

        /// <summary>
        /// Finds neutral grids based on user selection.
        /// </summary>
        /// <param name="size">The size of the neutral grids to find.</param>
        public void FindNeutralGrids(string size)
        {
            // Logic to find neutral grids of the specified size
            FoundNeutralGrids.Add($"Neutral Grid of size {size}");
        }

        /// <summary>
        /// Finds enemy grids based on user selection.
        /// </summary>
        /// <param name="size">The size of the enemy grids to find.</param>
        public void FindEnemyGrids(string size)
        {
            // Logic to find enemy grids of the specified size
            FoundEnemyGrids.Add($"Enemy Grid of size {size}");
        }

        /// <summary>
        /// Saves found items to the PB data.
        /// </summary>
        public void SaveFoundItems()
        {
            // Logic to save found items to the PB data
        }
    }
}
