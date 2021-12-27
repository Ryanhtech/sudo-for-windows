/* RunEverything
 * 
 * Copyright 2021 Ryanhtech Labs.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/


using System.Collections.Generic;

namespace RunEverything
{
    class RunEverythingUtil
    {
        public static bool HasPrivileges()
        {
            // ----------------------------------------------------------------
            // Check if the app has admin privileges.
            //
            // Return value:
            //          bool
            //
            //-----------------------------------------------------------------

            using (System.Security.Principal.WindowsIdentity AppStatus = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                System.Security.Principal.WindowsPrincipal AppPrincipal = new System.Security.Principal.WindowsPrincipal(AppStatus);

                // Returns the boolean result: if RunEverything has been run as admin, return true.

                return AppPrincipal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }

        public static string RunCommand(string Command,
                                        string Arguments,
                                        bool IsElevated,
                                        bool HideCMD,
                                        string WorkingDir
        )
        {
            // Create process info + process class

            System.Diagnostics.Process CommandProcess              =        new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo CommandProcessInfo =        new System.Diagnostics.ProcessStartInfo();

            if (HideCMD)
            {
                // If the CMD window must be hidden
                CommandProcessInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            }


            if (!System.IO.Directory.Exists(WorkingDir))
            {
                // If the directory that the user typed in doesn't exist, return "DirMissing"
                return "DirMissing";
            }


            CommandProcessInfo.FileName              =       Command;            //
            CommandProcessInfo.Arguments             =       Arguments;          // Set the Process Info to the info that the user typed in
            CommandProcessInfo.WorkingDirectory      =       WorkingDir;         //
            if (IsElevated)
            {
                CommandProcessInfo.UseShellExecute   =       true;               //
                CommandProcessInfo.Verb              =       "runas";            // If the program must be run as admin
            }


            // Copy the start info to the Process class

            CommandProcess.StartInfo                 =       CommandProcessInfo;


            try
            {
                // Start the process under a try/catch
                CommandProcess.Start();
            }


            catch (System.ComponentModel.Win32Exception Exception)
            {
                // If the file doesn't exist, or if the user clicked on "No" in the UAC prompt

                if (Exception.ErrorCode == -2147467259)
                {
                    // If the user denied the elevation of the program
                    return "NoPrivileges";
                }

                // If the file doesn't exist
                return "FileMissing";
            }

            catch (System.InvalidOperationException)
            {
                // If the user typed ""
                return "NoProgramEntered";
            }

            // Return nothing: the operation completed successfully
            return "";
        }

        public static void AlertUserAdministrator()
        {
            // This function alerts the user if he ran RunEverything as administrator,
            // to avoid exposing to the GHSA-vxf2-x72m-x9m2 vulnerability.

            if (HasPrivileges())
            {
                // If RunEverything has been run as administrator, show the warning.
                System.Windows.Forms.MessageBox.Show(
                    "You ran RunEverything as administrator.\n" +
                    "All programs that you will run using RunEverything will be run as administrator too. Please be " +
                    "extremely careful at what you will be running. Run ONLY software you trust.\n\n" +
                    "If you don't want this to happen, please do not run RunEverything as administrator.\nClick OK to continue.",
                    "Warning - RunEverything",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning
                );
            }
        }
    }
}
