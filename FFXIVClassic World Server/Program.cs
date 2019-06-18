﻿/*
===========================================================================
Copyright (C) 2015-2019 FFXIV Classic Server Dev Team

This file is part of FFXIV Classic Server.

FFXIV Classic Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FFXIV Classic Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with FFXIV Classic Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using FFXIVClassic_World_Server.DataObjects;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Diagnostics;

namespace FFXIVClassic_World_Server
{
    class Program
    {
        public static Logger Log;

        static void Main(string[] args)
        {
            // set up logging
            Log = LogManager.GetCurrentClassLogger();

            bool startServer = true;

            Log.Info("==================================");
            Log.Info("FFXIV Classic World Server");
            Log.Info("Version: 0.1");            
            Log.Info("==================================");

#if DEBUG
            TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(myWriter);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Threading.Thread.Sleep(5000);
            }

#endif

            //Load Config
            ConfigConstants.Load();
            ConfigConstants.ApplyLaunchArgs(args);

            //Test DB Connection
            Log.Info("Testing DB connection... ");
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    conn.Close();

                    Log.Info("Connection ok.");
                }
                catch (MySqlException e)
                {
                    Log.Error(e.ToString());
                    startServer = false;
                }
            }

            //Check World ID
            DBWorld thisWorld = Database.GetServer(ConfigConstants.DATABASE_WORLDID);
            if (thisWorld != null)
            {
                Program.Log.Info("Successfully pulled world info from DB. Server name is {0}.", thisWorld.name);
                ConfigConstants.PREF_SERVERNAME = thisWorld.name;
            }
            else
            {
                Program.Log.Info("World info could not be retrieved from the DB. Welcome and MOTD will not be displayed.");
                ConfigConstants.PREF_SERVERNAME = "Unknown";
            }
          
            //Start server if A-OK
            if (startServer)
            {
                Server server = new Server();                
                server.StartServer();

                while (startServer)
                {
                    String input = Console.ReadLine();
                    Log.Info("[Console Input] " + input);
                    //cp.DoCommand(input, null);
                }
            }

            Program.Log.Info("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
