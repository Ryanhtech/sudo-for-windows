using System;
using RunEverything;
using System.IO;

namespace Sudo
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] Arguments)
        {
            string AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            AssemblyName = Path.GetFileNameWithoutExtension(AssemblyName);

            if (Arguments.Length == 0)
            {
                Console.WriteLine("usage: " + AssemblyName + " command <arguments>");
                return 0;
            }

            string ArgumentsToPassString = "";

            for (int i = 0; i < Arguments.Length; i++)
            {
                if (i > 0)
                {
                    // If we skipped the command
                    ArgumentsToPassString += Arguments[i];
                }
            }

            if (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToLower() == Arguments[0])
            {
                Console.WriteLine(AssemblyName + ": can't run sudo from sudo");
                return 1;
            }

            Console.Write(AssemblyName + ": running " + Arguments[0]);
            if (Arguments.Length > 1)
            {
                Console.Write(" with " + (Arguments.Length - 1) + " arguments");
            }
            Console.Write("...");
            Console.WriteLine();

            string result = RunEverythingUtil.RunCommand(Arguments[0], ArgumentsToPassString, true, false, Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location));

            if (result == "NoPrivileges")
            {
                Console.WriteLine(AssemblyName + ": Couldn't run program because access is denied or the file doesn't exists.");
                return 1;
            }

            return 0;
        }
    }
}
