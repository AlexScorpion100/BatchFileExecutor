using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Program
{
    public static void Main(string[] args)
    {
        try
        {
            bool hasArgs = args != null && args.Length > 0 ? true : false;
            while (true && !hasArgs)
            {
                List<string> batchFiles = GetBatchFiles();
                batchFiles.AddRange(GetAutoHotkeyFiles());
                int exitOption = -99;
                string choice = default;

                if (hasArgs)
                {
                    choice = args.First();
                    Console.WriteLine(args);
                    Console.ReadLine();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("╔════════════════════════════╗");
                    Console.WriteLine("║        Batch File Menu     ║");
                    Console.WriteLine("╠════════════════════════════╣");
                    Console.WriteLine("║ Select a batch file to run ║");
                    Console.WriteLine("╠════════════════════════════╣");

                    for (int i = 0; i < batchFiles.Count; i++)
                    {
                        string menuOption = $"{i + 1}. {batchFiles[i]}";
                        int paddingSpaces = 26 - menuOption.Length; // Adjust the number based on your desired spacing

                        Console.WriteLine($"║  {menuOption}{new string(' ', (paddingSpaces - (i + 1).ToString().Length) + (i + 1).ToString().Length)}║");
                    }

                    Console.WriteLine("║ -99. Exit                  ║");
                    Console.WriteLine("╚════════════════════════════╝");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine();
                    Console.Write("Enter your choice: ");
                    choice = Console.ReadLine();
                }

                if (int.TryParse(choice, out int selectedOption))
                {
                    if (selectedOption >= 1 && selectedOption <= batchFiles.Count)
                    {
                        string selectedBatchFile = batchFiles[selectedOption - 1];

                        if (selectedBatchFile.EndsWith(".bat"))
                        {
                            ExecuteBatchFile(selectedBatchFile);
                        }
                        else if (selectedBatchFile.EndsWith(".ahk"))
                        {
                            ExecuteAutoHotkeyFile(selectedBatchFile);
                        }
                    }
                    else if (selectedOption == exitOption)
                    {
                        return;
                    }
                    else
                    {
                        if (!hasArgs)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid choice. Please try again.");
                            Console.ResetColor();
                        }
                    }
                }
                else
                {
                    if (!hasArgs)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ResetColor();
                    }
                }
                if (!hasArgs)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    Console.ResetColor();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            Console.ReadLine();
        }
    }

    static List<string> GetBatchFiles()
    {
        string scriptFolder =AppDomain.CurrentDomain.BaseDirectory + "/Scripts";
        string[] files = Directory.GetFiles(scriptFolder, "*.bat");
        List<string> batchFiles = new List<string>();

        foreach (string file in files)
        {
            batchFiles.Add(Path.GetFileName(file));
        }

        return batchFiles;
    }

    static void ExecuteBatchFile(string fileName)
    {
        string scriptFolder = AppDomain.CurrentDomain.BaseDirectory +"/Scripts";
        string filePath = Path.Combine(scriptFolder, fileName);

        try
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/C \"{filePath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine();
            Console.WriteLine("Batch file executed successfully.");

            Console.WriteLine();
            // Set console output color to green
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Output:");
            Console.WriteLine(output);

            // Reset console output color
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error executing batch file: " + ex.Message);
            Console.ResetColor();
        }
    }

    static List<string> GetAutoHotkeyFiles()
    {
        string scriptFolder = "Scripts";
        string[] files = Directory.GetFiles(scriptFolder, "*.ahk");
        List<string> batchFiles = new List<string>();

        foreach (string file in files)
        {
            batchFiles.Add(Path.GetFileName(file));
        }

        return batchFiles;
    }

    static void ExecuteAutoHotkeyFile(string ahkFilePath)
    {
        // Locate AutoHotkey installation path from registry
        string autoHotkeyExePath = GetAutoHotkeyExePath();
        string scriptFolder = "Scripts";

        if (string.IsNullOrEmpty(autoHotkeyExePath))
        {
            Console.WriteLine("AutoHotkey is not installed or its installation path could not be found.");
            Console.WriteLine("Please provide the path to the AutoHotkey executable manually.");
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            return;
        }

        try
        {
            Process process = new Process();
            process.StartInfo.FileName = autoHotkeyExePath;
            process.StartInfo.Arguments = Path.Combine(scriptFolder, ahkFilePath);
            process.Start();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AutoHotkey script started.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error running AutoHotkey script: " + ex.Message);
            Console.ResetColor();
        }

    }

    static string GetAutoHotkeyExePath()
    {
        string autoHotkeyKey = @"Software\AutoHotkey";
        using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(autoHotkeyKey))
        {
            if (regKey != null)
            {
                object exePathValue = regKey.GetValue("ExePath");
                if (exePathValue != null)
                {
                    return exePathValue.ToString();
                }
            }
        }

        string[] possiblePaths = {
            @"C:\Program Files\AutoHotkey\AutoHotkey.exe",  // Default installation path
            @"C:\Program Files (x86)\AutoHotkey\AutoHotkey.exe"
            // Add more paths if necessary
        };

        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        return null;
    }
}
