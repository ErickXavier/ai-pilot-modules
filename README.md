# AI Pilot Module for Space Engineers

## Introduction

The AI Pilot Module is the cornerstone of an ambitious project aimed at expanding automation functionalities within Space Engineers. This first module, focusing on AI-driven piloting, lays the groundwork for a series of planned modules designed to leverage AI for various tasks and utilities. The upcoming modules include:

- **AI Miner**: Automated mining operations.
- **AI Grinder**: Efficient grinding and material processing.
- **AI Welder**: Intelligent welding and construction.
- **AI Cargo Transport**: Streamlined transportation of cargo.
- **AI Passenger Transportation**: Comfortable and efficient passenger travel.
- **AI Patrol**: Security and surveillance through AI-controlled patrols.
- **...and more to come!**

Stay tuned for updates, as these exciting features will be rolling out in the near future.

## AI Pilot Module

### Description

AI Pilot Module provides an AI piloting system for Space Engineers. It enables users to record paths, save paths, and automate flight through various waypoints (the end of each path). The script is configurable to some extent to suit individual needs.

### Features

- Record paths and save them for later use
- Automated flight through various waypoints
- Configurable variables to suit individual needs
- Comprehensive interface for navigation and control
- Error and warning messages for easy troubleshooting

### Installation

1. Place this script inside a Programmable Block in Space Engineers.
2. Configure the required remote control block and LCD panel with the specific tag defined in the script.
3. Compile and run the script.

### Usage

The script is meant to be run on a programmable block and requires a remote control block and an LCD panel. Both the remote control block and the LCD panel must have the configured tag (default: `[AIP]`) in their name.

#### Hotbar Configuration

Add shortcuts to the Programmable Block on your hotbar with the following commands:

- `up`: Navigate up
- `down`: Navigate down
- `set`: Confirm selection

#### Commands

- `save_home`: Save home location
- `record`: Start recording a path
- `stop_record`: Stop recording a path
- `delete_path [PATHNAME]`: Delete a specific path
- `go_home`: Go to the home location
- `fly_path [PATHNAME]`: Fly to a specific path
- `fly_all`: Fly through all paths and return home
- `wipe_all`: Wipe all data
- `clear_warning`: Clear warning message

### Author and Contributors

- ErickXavier ([Steam Profile](https://steamcommunity.com/id/ErickXavier/))
- Contributors:
  - Luisau (Thanks for the [template](https://github.com/lpenap/luisau-space-engineers/)!)

### Version

- Created: 14 August 2023
- Version: 2023.08.16

### License

Please refer to the license information provided by the author or the game's EULA.

### Support and Contributions

For support, issues, or contributions, please contact the author or follow the guidelines provided by the game's community.

- Link to the Repository: https://github.com/ErickXavier/ai-pilot-modules
- Link to the Workshop: https://steamcommunity.com/sharedfiles/filedetails/?id=3021091164

### Disclaimer

This script is intended for use in the Space Engineers game. The author is not responsible for any unintended consequences, damages, or losses incurred by using this script.

---

Feel free to stay connected with the project and contribute ideas, suggestions, or support. The AI Pilot Module is just the beginning, and the sky is not the limit!
