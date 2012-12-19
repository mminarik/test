using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Assembly_Version
{
    class AssemblyUtil
    {
        private static string fileName = "c:\\AssemblyInfo.cs";
        private static string versionStr = null;
        private static string vcsNumber = null;
        private static string position = "0";

        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-number:"))
                {
                    position = args[i].Substring("-number:".Length).ToString();
                }
            }

            if (args[0].EndsWith("cs"))
            {
                fileName = args[0];
            }

            versionStr = args[Convert.ToInt32(position)];
            vcsNumber = versionStr.Remove(0, versionStr.Length + 1 - (versionStr.Length - versionStr.LastIndexOf('.')));
            versionStr = versionStr.Remove(versionStr.LastIndexOf("."), versionStr.Length - versionStr.LastIndexOf("."));

            if (fileName == "")
            {
                System.Console.WriteLine("Usage: Assembly Version <path to AssemblyInfo.cs file> [options]");
                System.Console.WriteLine("Options: ");
                System.Console.WriteLine("  -number: identifie product (Symfonie NG - 4)");
                return;
            }

            if (!File.Exists(fileName))
            {
                System.Console.WriteLine("Error: Can not find file \"" + fileName + "\"");
                return;
            }

            System.Console.Write("Processing \"" + fileName + "\"...");
            StreamReader reader = new StreamReader(fileName);
            StreamWriter writer = new StreamWriter(fileName + ".out");
            String line;

            while ((line = reader.ReadLine()) != null)
            {
                line = ProcessLine(line);
                writer.WriteLine(line);
            }

            reader.Close();
            writer.Close();

            File.Delete(fileName);
            File.Move(fileName + ".out", fileName);
            System.Console.WriteLine("Done!");
        }

        private static string ProcessLine(string line)
        {
            line = ProcessLinePart(line, "[assembly: AssemblyVersion(\"", versionStr);
            line = ProcessLinePart(line, "[assembly: AssemblyFileVersion(\"", versionStr);
            line = ProcessLinePart(line, "[assembly: AssemblyDescription(\"", vcsNumber);

            return line;
        }

        private static string ProcessLinePart(string line, string part, string version)
        {
            int spos = line.IndexOf(part);
            if (spos >= 0)
            {
                spos += part.Length;
                int epos = line.IndexOf('"', spos);
                //string oldVersion = line.Substring(spos, epos - spos);
                string newVersion = "";

                if (versionStr != null)
                {
                    newVersion = version;

                    StringBuilder str = new StringBuilder(line);
                    str.Remove(spos, epos - spos);
                    str.Insert(spos, newVersion);
                    line = str.ToString();
                }
            }
            return line;
        }
    }
}