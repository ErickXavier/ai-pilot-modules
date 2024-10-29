using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Luisau.AiPilotModule;

namespace SpaceEngineers.Luisau.AiPilotModule
{
    public class SampleMainFile : MyGridProgram
    {
        private Autopilot _autopilot;
        private Patrol _patrol;
        private Miner _miner;
        private Grinder _grinder;
        private Welder _welder;
        private CargoTransport _cargoTransport;
        private PassengerTransport _passengerTransport;

        public SampleMainFile()
        {
            IMyRemoteControl remoteControl = FindBlockByTag<IMyRemoteControl>("[AIP]");
            if (remoteControl != null)
            {
                _autopilot = new Autopilot(remoteControl);
                _patrol = new Patrol(remoteControl);
                _miner = new Miner(remoteControl);
                _grinder = new Grinder(remoteControl);
                _welder = new Welder(remoteControl);
                _cargoTransport = new CargoTransport(remoteControl);
                _passengerTransport = new PassengerTransport(remoteControl);
                _autopilot.LoadHomeLocation();
                _autopilot.LoadSavedPaths();
            }
            else
            {
                Logger.Log("Remote Control not found.");
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (argument == "")
            {
                return;
            }

            string[] args = argument.Split(' ');
            string command = args[0];

            switch (command)
            {
                case "save_home":
                    _autopilot.SaveHome();
                    break;
                case "record":
                    _autopilot.RecordPath();
                    break;
                case "stop_record":
                    _autopilot.StopRecordingPath();
                    break;
                case "go_home":
                    _autopilot.GoHome();
                    break;
                case "fly_path":
                    if (args.Length > 1)
                    {
                        _autopilot.FlyPath(args[1]);
                    }
                    else
                    {
                        Logger.Log("Please specify a path name.");
                    }
                    break;
                case "fly_all":
                    _autopilot.FlyAllPathsAndReturnHome();
                    break;
                case "delete_path":
                    if (args.Length > 1)
                    {
                        _autopilot.DeletePath(args[1]);
                    }
                    else
                    {
                        Logger.Log("Please specify a path name.");
                    }
                    break;
                case "wipe_all":
                    _autopilot.WipeAllData();
                    break;
                case "start_patrol":
                    _patrol.StartPatrol();
                    break;
                case "stop_patrol":
                    _patrol.StopPatrol();
                    break;
                case "add_patrol_waypoint":
                    if (args.Length > 1)
                    {
                        Vector3D waypoint = Vector3D.Parse(args[1]); // Assuming the waypoint is passed as a string
                        _patrol.AddWaypoint(waypoint);
                        Logger.Log("Waypoint added: " + waypoint);
                    }
                    else
                    {
                        Logger.Log("Please specify a waypoint.");
                    }
                    break;
                case "start_mining":
                    _miner.StartMining();
                    break;
                case "stop_mining":
                    _miner.StopMining();
                    break;
                case "add_mining_waypoint":
                    if (args.Length > 1)
                    {
                        Vector3D waypoint = Vector3D.Parse(args[1]); // Assuming the waypoint is passed as a string
                        _miner.AddMiningWaypoint(waypoint);
                        Logger.Log("Mining waypoint added: " + waypoint);
                    }
                    else
                    {
                        Logger.Log("Please specify a mining waypoint.");
                    }
                    break;
                case "start_grinding":
                    _grinder.StartGrinding();
                    break;
                case "stop_grinding":
                    _grinder.StopGrinding();
                    break;
                case "add_grinding_waypoint":
                    if (args.Length > 1)
                    {
                        Vector3D waypoint = Vector3D.Parse(args[1]); // Assuming the waypoint is passed as a string
                        _grinder.AddGrindingWaypoint(waypoint);
                        Logger.Log("Grinding waypoint added: " + waypoint);
                    }
                    else
                    {
                        Logger.Log("Please specify a grinding waypoint.");
                    }
                    break;
                case "start_welding":
                    _welder.StartWelding();
                    break;
                case "stop_welding":
                    _welder.StopWelding();
                    break;
                case "add_welding_waypoint":
                    if (args.Length > 1)
                    {
                        Vector3D waypoint = Vector3D.Parse(args[1]); // Assuming the waypoint is passed as a string
                        _welder.AddWeldingWaypoint(waypoint);
                        Logger.Log("Welding waypoint added: " + waypoint);
                    }
                    else
                    {
                        Logger.Log("Please specify a welding waypoint.");
                    }
                    break;
                case "start_cargo_transport":
                    _cargoTransport.StartTransport();
                    break;
                case "stop_cargo_transport":
                    _cargoTransport.StopTransport();
                    break;
                case "add_cargo_waypoint":
                    if (args.Length > 1)
                    {
                        Vector3D waypoint = Vector3D.Parse(args[1]); // Assuming the waypoint is passed as a string
                        _cargoTransport.AddCargoWaypoint(waypoint);
                        Logger.Log("Cargo waypoint added: " + waypoint);
                    }
                    else
                    {
                        Logger.Log("Please specify a cargo waypoint.");
                    }
                    break;
                case "start_passenger_transport":
                    _passengerTransport.StartTransport();
                    break;
                case "stop_passenger_transport":
                    _passengerTransport.StopTransport();
                    break;
                case "add_passenger_waypoint":
                    if (args.Length > 1)
                    {
                        Vector3D waypoint = Vector3D.Parse(args[1]); // Assuming the waypoint is passed as a string
                        _passengerTransport.AddPassengerWaypoint(waypoint);
                        Logger.Log("Passenger waypoint added: " + waypoint);
                    }
                    else
                    {
                        Logger.Log("Please specify a passenger waypoint.");
                    }
                    break;
                default:
                    Logger.Log("Unknown command: " + command);
                    break;
            }
        }

        private T FindBlockByTag<T>(string tag) where T : class
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<T>(blocks, x => x.CustomName.Contains(tag));
            return blocks.FirstOrDefault() as T;
        }
    }
}
