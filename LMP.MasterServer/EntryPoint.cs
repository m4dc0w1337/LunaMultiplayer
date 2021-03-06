﻿using LMP.MasterServer.Http;
using LunaCommon;
using System;
using System.Text;
using System.Threading.Tasks;
using ConsoleLogger = LunaCommon.ConsoleLogger;
using LogLevels = LunaCommon.LogLevels;

namespace LMP.MasterServer
{
    /// <summary>
    /// This program is the one who does the punchtrough between a nat client and a nat server. 
    /// You should only run if you agree in the forum to do so and your server ip is listed in:
    /// https://raw.githubusercontent.com/DaggerES/LunaMultiPlayer/master/MasterServersList
    /// </summary>
    public static class EntryPoint
    {
        
        public static void Stop()
        {
            MasterServer.RunServer = false;
        }

        public static void MainEntryPoint(string[] args)
        {
            Console.Title = $"LMP MasterServer {LmpVersioning.CurrentVersion}";
            Console.OutputEncoding = Encoding.Unicode;

            var commandLineArguments = new Arguments(args);
            if (commandLineArguments["h"] != null)
            {
                ShowCommandLineHelp();
                return;
            }

            if (!ParseMasterServerPortNumber(commandLineArguments)) return;
            if (!ParseHttpServerPort(commandLineArguments)) return;
            if (!ParseMaxRequestsPerSecond(commandLineArguments)) return;

            ConsoleLogger.Log(LogLevels.Normal, $"Starting MasterServer at port: {MasterServer.Port}");
            ConsoleLogger.Log(LogLevels.Normal, $"Listening for GET requests at port: {LunaHttpServer.Port}");

            if (CheckPort())
            {
                MasterServer.RunServer = true;
                Task.Run(() => new LunaHttpServer().Listen());
                Task.Run(() => MasterServer.Start());
            }
        }

        private static bool CheckPort()
        {
            if (Common.PortIsInUse(MasterServer.Port))
            {
                ConsoleLogger.Log(LogLevels.Error, $"Port {MasterServer.Port} is already in use!");
                return false;
            }
            return true;
        }

        private static void ShowCommandLineHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("LMP Master server");
            Console.WriteLine("This program is only used to introduce client and standard LMP servers.");
            Console.WriteLine("Check the wiki for details about running a master server.");
            Console.WriteLine("In order to run this program you need to open the port in your router.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("/h                       ... Show this help");
            Console.WriteLine("/p:<port>                ... Start with the specified port (default port is 8700)");
            Console.WriteLine("/g:<port>                ... Reply to get petitions on the specified port (default is 8701)");
            Console.WriteLine("/f:<number>              ... Max requests per milisecond per host (default is 500, min is 0)");
            Console.WriteLine("");
        }

        #region Command line arguments parsing

        private static bool ParseMaxRequestsPerSecond(Arguments commandLineArguments)
        {
            if (commandLineArguments["f"] != null)
            {
                if (int.TryParse(commandLineArguments["f"].Trim(), out var floodSeconds) && floodSeconds >= 0)
                    FloodControl.MaxRequestsPerMs = floodSeconds;
                else
                {
                    ConsoleLogger.Log(LogLevels.Error, $"Invalid max request per second specified: {commandLineArguments["f"].Trim()}");
                    return false;
                }
            }
            return true;
        }

        private static bool ParseHttpServerPort(Arguments commandLineArguments)
        {
            if (commandLineArguments["g"] != null)
            {
                if (!ParsePortNumber(commandLineArguments, "g", out var port))
                    return false;

                LunaHttpServer.Port = port;
            }
            return true;
        }

        private static bool ParseMasterServerPortNumber(Arguments commandLineArguments)
        {
            if (commandLineArguments["p"] != null)
            {
                if (!ParsePortNumber(commandLineArguments, "p", out var port))
                    return false;

                MasterServer.Port = port;
            }
            return true;
        }

        private static bool ParsePortNumber(Arguments commandLineArguments, string parameter, out ushort portNum)
        {
            if (ushort.TryParse(commandLineArguments[parameter].Trim(), out portNum))
                return true;

            ConsoleLogger.Log(LogLevels.Error, $"Invalid port specified: {commandLineArguments[parameter].Trim()}");
            return false;
        }

        #endregion
    }
}
