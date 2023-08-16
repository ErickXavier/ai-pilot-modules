#region Program Header
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

// Change this namespace
namespace SpaceEngineers.Luisau.AiPilotModule
{
  public sealed class Program : MyGridProgram
  {

    #endregion
    /////////////////////////////////////////////////////////////////
    // Your code goes below this line

    /**
  * Script Name: AI Pilot Module
  * Author: ErickXavier (https://steamcommunity.com/id/ErickXavier/)
  * Created: 14 August 2023
  * Version: 2023.08.15
  * Contributors:
  * - Luisau (Thanks for the template!) (https://github.com/lpenap/luisau-space-engineers/)
  *
  * DESCRIPTION:
  *
  * This script provides an AI piloting system for Space Engineers.
  * It allows the user to record paths, save paths, and automate flight through
  * various waypoints (the end of each path). The script is configurable to some extent to suit individual needs.
  *
  * INSTRUCTIONS:
  *
  * The script is meant to be run on a programmable block. It requires a remote control block and an LCD panel.
  * The remote control block must have the configured tag bellow in its name. The LCD panel must also have the configured tag bellow in its name.
  * The script can be configured by changing the configurable variables below.
  *
  * To be able navigate on the interface, add shortcuts on your hotbar to this Programmable Block with the following commands:
  * - up
  * - down
  * - set
  *
  * Other commands are also available:
  * - save_home
  * - record
  * - stop_record
  * - delete_path [PATHNAME]
  * - go_home
  * - fly_path [PATHNAME]
  * - fly_all
  * - wipe_all
  * - clear_warning
  */

    // ------------ Configurable Variables ------------
    const string BLOCK_TAG = "[AIP]"; // Tag used to identify blocks used by the script
    const string PATH_PREFIX = "Path"; // Prefix used to identify paths in CustomData
    const string PATH_COUNT_VAR = "PathCount"; // Variable name used to store the number of paths in CustomData
    const string HOME_VAR = "HOME"; // Variable name used to store the home location in CustomData

    // ------------ Non-Configurable Variables ------------
    int _selectedOption = 0; // The currently selected option
    bool _selectingPath = false; // Whether or not the user is selecting an path
    string _selectedPath = ""; // The currently selected path
    int numberOfOptions = 0; // The number of options available

    struct Path
    {
      public string Name { get; set; }
      public List<Vector3D> Waypoints { get; set; }

      public Path(string name, List<Vector3D> waypoints = null)
      {
        Name = name;
        Waypoints = waypoints ?? new List<Vector3D>();
      }
    }

    Vector3D _homeLocation = Vector3D.Zero; // The location of the home point
    Dictionary<string, Path> _paths = new Dictionary<string, Path>(); // Paths with names
    int _pathCount = 0; // Number of paths
    List<Vector3D> _currentPath = new List<Vector3D>(); // Path being recorded
    bool _isRecordingPath = false; // Status of path recording
    string warningMessage = ""; // Warning message to display on LCD

    IMyRemoteControl _remoteControl; // Remote control block
    IMyTextPanel _lcd; // LCD panel for displaying information

    /**
      * // The constructor, called only once every session and
      * // always before any other method is called. Use it to
      * // initialize your script.
      * //
      * // The constructor is optional and can be removed if not
      * // needed.
      * //
      * // It's recommended to set RuntimeInfo.UpdateFrequency
      * // here, which will allow your script to run itself without a
      * // timer block.
      *
      * Initialization function for setting up necessary blocks and variables.
      */
    public Program()
    {
      Runtime.UpdateFrequency = UpdateFrequency.Update10;
      try
      {
        _remoteControl = FindBlockByTag<IMyRemoteControl>(BLOCK_TAG);
        _lcd = FindBlockByTag<IMyTextPanel>(BLOCK_TAG);
        _lcd.ContentType = ContentType.TEXT_AND_IMAGE;

        GetSavedHomeLocation();
        GetSavedPaths();

        UpdateInterface();
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /*
      * Get Saved Home Location from CustomData
      */
    public void GetSavedHomeLocation()
    {
      string[] lines = GetCustomDataLines();
      foreach (string line in lines)
      {
        string[] parts = line.Split('=');
        if (parts.Length < 2)
        {
          continue;
        }
        string varName = parts[0];
        string data = parts[1];
        if (varName == HOME_VAR)
        {
          _homeLocation = ParseVector3D(data);
        }
      }
    }

    /*
      * Get Saved Paths from CustomData
      */
    public void GetSavedPaths()
    {
      // check if there are paths saved in the custom data
      string[] lines = GetCustomDataLines();
      // get the number of paths saved
      foreach (string line in lines)
      {
        string[] parts = line.Split('=');
        if (parts.Length < 2)
        {
          continue;
        }
        string varName = parts[0];
        string data = parts[1];
        if (varName == PATH_COUNT_VAR)
        {
          _pathCount = int.Parse(data);
        }
      }
      // get the paths ignoring the HOME and PathCount variables
      for (int i = 0; i < _pathCount; i++)
      {
        string pathName = PATH_PREFIX + i;
        List<Vector3D> waypoints = new List<Vector3D>();
        foreach (string line in lines)
        {
          string[] parts = line.Split('=');
          if (parts.Length < 2)
          {
            continue;
          }
          string varName = parts[0];
          string data = parts[1];
          if (varName == pathName)
          {
            waypoints.Add(ParseVector3D(data));
          }
        }
        _paths.Add(pathName, new Path(pathName, waypoints));
      }
    }

    /*
      * Gets CustomData lines split by \n
    */
    string[] GetCustomDataLines()
    {
      return Me.CustomData.Split('\n');
    }

    /**
      * Finds the first block of type T with the specified tag.
      * @param tag The tag to search for.
      * @return The first block of type T with the specified tag.
      */
    T FindBlockByTag<T>(string tag) where T : class
    {
      try
      {
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Contains(tag));
        return blocks.FirstOrDefault() as T;
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
        return null;
      }
    }

    /**
      * Main update function.
      * Records path if recording is enabled and updates the interface.
      */
    public void Update()
    {
      try
      {
        if (_isRecordingPath && (_currentPath.Count == 0 || _currentPath.Last() != _remoteControl.GetPosition()))
        {
          _currentPath.Add(_remoteControl.GetPosition());
        }
        UpdateInterface();
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Save info to PB Custom Data
      */
    public void SaveCustomData(string varName, string data)
    {
      // save it as a variable called HOME inside the programmable block custom data
      string[] lines = GetCustomDataLines();
      bool found = false;
      for (int i = 0; i < lines.Length; i++)
      {
        string line = lines[i];
        if (line.StartsWith(varName + "="))
        {
          if (data == "")
          {
            lines = lines.Where(x => x != line).ToArray();
          }
          else
          {
            lines[i] = varName + "=" + data;
          }
          found = true;
          break;
        }
      }
      if (!found)
      {
        Array.Resize(ref lines, lines.Length + 1);
        lines[lines.Length - 1] = varName + "=" + data;
      }
      Me.CustomData = string.Join("\n", lines);
    }

    /**
      * Saves Home location
      */
    public void SaveHome()
    {
      try
      {
        _homeLocation = _remoteControl.GetPosition();
        SaveCustomData(HOME_VAR, _homeLocation.ToString());

      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Records the current path and saves it.
      */
    public void RecordPath()
    {
      try
      {
        _isRecordingPath = true;
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Stops recording the current path and saves it.
      */
    public void StopRecordingPath()
    {
      try
      {
        _isRecordingPath = false;
        if (_currentPath.Count > 0)
        {
          string pathName = PATH_PREFIX + _paths.Count;
          _paths.Add(pathName, new Path(pathName, _currentPath));
          _currentPath.Clear();
          SaveCustomData(pathName, _paths[pathName].Waypoints.ToString());
          _pathCount = _paths.Count;
        }
        else
        {
          _pathCount = 0;
        }
        SaveCustomData(PATH_COUNT_VAR, _pathCount.ToString());
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Goes to the home location.
      */
    public void GoHome()
    {
      try
      {
        _remoteControl.SetAutoPilotEnabled(false);
        _remoteControl.ClearWaypoints();
        // set the current position of the remote control to a variable
        Vector3D currentPosition = _remoteControl.GetPosition();
        _remoteControl.AddWaypoint(currentPosition, "Current Position");

        // get the selected IP, if any, reverse the path and add it to the waypoints
        if (_selectedPath != "")
        {
          Path selectedPath = _paths[_selectedPath];
          // get the closest path waypoint to the remoteControl, start a new list with that waypoint, add the rest of the waypoints to the list, reverse the list and add it to the remoteControl
          Vector3D closestWaypoint = selectedPath.Waypoints.OrderBy(x => Vector3D.Distance(x, currentPosition)).First();
          int closestWaypointIndex = selectedPath.Waypoints.IndexOf(closestWaypoint);
          List<Vector3D> waypoints = selectedPath.Waypoints.GetRange(closestWaypointIndex, selectedPath.Waypoints.Count - closestWaypointIndex);
          waypoints.Reverse();
          foreach (Vector3D waypoint in waypoints)
          {
            _remoteControl.AddWaypoint(waypoint, _selectedPath);
          }

          // add the home location to the remoteControl
          _remoteControl.AddWaypoint(_homeLocation, HOME_VAR);

        }
        else
        {
          _remoteControl.AddWaypoint(_homeLocation, HOME_VAR);
        }
        _remoteControl.SetAutoPilotEnabled(true);
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Goes to the specified path.
      * @param pathName The name of the path to go to.
      */
    public void FlyPath(string pathName)
    {
      try
      {
        _selectedPath = pathName;
        // get current position of the remote control
        _remoteControl.SetAutoPilotEnabled(false);
        _remoteControl.ClearWaypoints();
        // check if remoteControl is close to the Home location
        if (_remoteControl.GetPosition().Equals(_homeLocation))
        {
          // get the selected IP, if any, reverse the path and add it to the waypoints
          if (_selectedPath != "")
          {
            Path selectedPath = _paths[_selectedPath];
            selectedPath.Waypoints.Reverse();
            foreach (Vector3D waypoint in selectedPath.Waypoints)
            {
              _remoteControl.AddWaypoint(waypoint, _selectedPath);
            }
          }
          else
          {
            _remoteControl.AddWaypoint(_homeLocation, HOME_VAR);
          }
        }
        else
        {
          // get the current position of the remote control
          _remoteControl.AddWaypoint(_remoteControl.GetPosition(), "Current Position");
          // get the selected IP, if any, reverse the path and add it to the waypoints
          if (_selectedPath != "")
          {
            Path selectedPath = _paths[_selectedPath];
            selectedPath.Waypoints.Reverse();
            foreach (Vector3D waypoint in selectedPath.Waypoints)
            {
              _remoteControl.AddWaypoint(waypoint, _selectedPath);
            }
          }
          else
          {
            _remoteControl.AddWaypoint(_homeLocation, HOME_VAR);
          }
        }

      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Flies through all paths and returns home.
      */
    public void FlyAllPathsAndReturnHome()
    {
      try
      {
        if (_paths.Count == 0)
        {
          ShowWarning("No paths recorded.");
          return;
        }
        _remoteControl.SetAutoPilotEnabled(false);
        _remoteControl.ClearWaypoints();
        foreach (Path ip in _paths.Values)
        {
          foreach (Vector3D waypoint in ip.Waypoints)
          {
            _remoteControl.AddWaypoint(waypoint, "IP");
          }
        }
        _remoteControl.AddWaypoint(_homeLocation, HOME_VAR);
        _remoteControl.SetAutoPilotEnabled(true);
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Deletes the specified path.
      * @param pathName The name of the path to delete.
      */
    public void DeletePath(string pathName)
    {
      try
      {
        if (_paths.ContainsKey(pathName))
        {
          _paths.Remove(pathName);
          SaveCustomData(pathName, "");
          _pathCount = _paths.Count;
          SaveCustomData(PATH_COUNT_VAR, _pathCount.ToString());
        }
        else
        {
          ShowWarning("Path not found: " + pathName);
        }
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Wipes all data.
      */
    public void WipeAllData()
    {
      try
      {
        _homeLocation = Vector3D.Zero;
        _paths.Clear();
        _currentPath.Clear();
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    public void AddSeparator(StringBuilder sb)
    {
      sb.AppendLine("---------------------------");
    }

    /**
      * Updates the interface.
      */
    public void UpdateInterface()
    {
      try
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("-- AIPilot --");

        if (!string.IsNullOrEmpty(warningMessage))
        {
          AddSeparator(sb);
          sb.AppendLine("Warning: ");
          sb.AppendLine(warningMessage);
        }

        if (_isRecordingPath)
        {
          AddSeparator(sb);
          sb.AppendLine("Recording Path...");
        }

        if (_selectedOption == 0)
        {
          _selectedOption = 0;
        }

        AddSeparator(sb);
        if (_selectingPath)
        {
          sb.AppendLine("Select Path:");
          int i = 0;
          numberOfOptions = _paths.Count;
          foreach (var pathName in _paths.Keys)
          {
            sb.AppendLine(i == _selectedOption ? $"> [{pathName}]" : $"[{pathName}]");
            i++;
          }
        }
        else
        {
          string pathString = _isRecordingPath ? "Stop Recording" : "Record Path";
          string[] options = new string[]
          {
"Save Home", // 0
pathString, // 1
"Go Home", // 2
"Fly Path", // 3
"Fly All Paths and Return Home", // 4
"Delete Path", // 5
"Wipe all Data" // 6
          };
          numberOfOptions = options.Length;
          for (int i = 0; i < numberOfOptions; i++)
          {
            sb.AppendLine(i == _selectedOption ? $"> [{options[i]}]" : $"[{options[i]}]");
          }
        }
        AddSeparator(sb);

        _lcd.WriteText(sb.ToString());
        // write the text to the pb console
        Echo(sb.ToString());
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Updates the LCD with the specified string.
      * @param str The string to display on the LCD.
      * @param isWarning Whether or not the message is a warning.
      */
    public void ShowWarning(string str)
    {
      try
      {
        warningMessage = str;
        UpdateInterface();
      }
      catch (Exception e)
      {
        Echo(e.ToString());
      }
    }

    /**
      * Removes the warning message.
      */
    public void RemoveWarning()
    {
      try
      {
        warningMessage = "";
        UpdateInterface();
      }
      catch (Exception e)
      {
        Echo(e.ToString());
      }
    }

    public void Save()
    {
      // Called when the program needs to save its state. Use
      // this method to save your state to the Storage field
      // or some other means.
      //
      // This method is optional and can be removed if not
      // needed.
    }

    /**
      * // The main entry point of the script, invoked every time
      * // one of the programmable block's Run actions are invoked,
      * // or the script updates itself. The updateSource argument
      * // describes where the update came from.
      * // The method itself is required, but the arguments above
      * // can be removed if not needed.
      *
      * Handles input from the user.
      * @param input The input from the user.
      */
    public void Main(string argument, UpdateType updateSource)
    {
      try
      {
        if (argument == "")
        {
          return;
        }
        if (warningMessage != "")
        {
          RemoveWarning();
        }
        switch (argument)
        {
          case "save_home": SaveHome(); break;
          case "record": RecordPath(); break;
          case "stop_record": StopRecordingPath(); break;
          case "delete_path": HandleDeletePath(argument); break;
          case "go_home": GoHome(); break;
          case "fly_path": HandleFlyPath(argument); break;
          case "fly_all": FlyAllPathsAndReturnHome(); break;
          case "wipe_all": WipeAllData(); break;
          case "up": HandleUp(); break;
          case "down": HandleDown(); break;
          case "set": HandleSet(argument); break;
          case "clear_warning": RemoveWarning(); break;
          default: ShowWarning("Unknown command: " + argument); break;
        }

        // Update the interface after handling the input
        UpdateInterface();
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Handles the go to path command.
      * @param input The input from the user.
      */
    public void HandleFlyPath(string input)
    {
      try
      {
        string[] parts = input.Split(' ');
        if (parts.Length < 2)
        {
          ShowWarning("Please specify a path name.");
          return;
        }
        string pathName = parts[1];
        FlyPath(pathName);
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Handles the delete path command.
      * @param input The input from the user.
      */
    public void HandleDeletePath(string input)
    {
      try
      {
        string[] parts = input.Split(' ');
        if (parts.Length < 2)
        {
          ShowWarning("Please specify a path name.");
          return;
        }
        string pathName = parts[1];
        DeletePath(pathName);
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Checks if the current selection is valid.
      * @return Whether or not the current selection is valid.
      */
    bool IsValidSelection(int selectedOption)
    {
      string[] screenLines = _lcd.GetText().Split('\n');
      string selectedLine = screenLines[selectedOption];
      bool isSelectingOption = selectedLine.StartsWith("[") && selectedLine.EndsWith("]");

      return isSelectingOption;
    }


    /**
      * Handles the up command.
      */
    public void HandleUp()
    {
      try
      {
        _selectedOption = (_selectedOption - 1 + numberOfOptions) % numberOfOptions;
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Handles the down command.
      */
    public void HandleDown()
    {
      try
      {
        _selectedOption = (_selectedOption + 1) % numberOfOptions;
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /**
      * Handles the set command.
      * @param input The input from the user.
      */
    public void HandleSet(string input)
    {
      try
      {
        if (_selectingPath)
        {
          // Handle selection within paths
          string selectedpathName = _paths.Keys.ElementAt(_selectedOption);
          // Perform the desired action with the selected path, such as GoToPath(selectedpathName);
          _selectingPath = false; // Return to the main menu
        }
        else
        {
          // Handle selection within main menu options
          switch (_selectedOption)
          {
            case 0: SaveHome(); break;
            case 1:
              if (_isRecordingPath)
              {
                StopRecordingPath();
              }
              else
              {
                RecordPath();
              }
              break;
            case 2: GoHome(); break;
            case 3: _selectingPath = true; break; // Enable path selection
            case 4: FlyAllPathsAndReturnHome(); break;
            case 5: _selectingPath = true; break; // Enable path selection for deletion
            case 6: WipeAllData(); break;
          }
        }
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
      }
    }

    /*
      * Parses a string into a Vector3D.
      * @param input The string to parse.
      * @return The parsed Vector3D.
      */
    public Vector3D ParseVector3D(string input)
    {
      try
      {
        var parts = input.Trim().Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        double x = double.Parse(parts[0]);
        double y = double.Parse(parts[1]);
        double z = double.Parse(parts[2]);
        return new Vector3D(x, y, z);
      }
      catch (Exception e)
      {
        ShowWarning(e.ToString());
        return Vector3D.Zero;
      }
    }

    // Your code ends above this line
    /////////////////////////////////////////////////////////////////
    #region Program Footer
  }
}
#endregion