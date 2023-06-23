using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Program
{
	static void Main()
	{
		try
		{
			while (true)
			{
				Console.Clear();
				Console.WriteLine("╔════════════════════════════╗");
				Console.WriteLine("║        Batch File Menu     ║");
				Console.WriteLine("╠════════════════════════════╣");
				Console.WriteLine("║ Select a batch file to run ║");
				Console.WriteLine("╠════════════════════════════╣");
				List<string> batchFiles = GetBatchFiles();
				int exitOption = batchFiles.Count + 1;

				for (int i = 0; i < batchFiles.Count; i++)
				{
					string menuOption = $"{i + 1}. {batchFiles[i]}";
					int paddingSpaces = 26 - menuOption.Length; // Adjust the number based on your desired spacing

					Console.WriteLine($"║  {menuOption}{new string(' ', paddingSpaces)}║");
				}

				Console.WriteLine($"║  {exitOption}. Exit{"",-19}║");
				Console.WriteLine("╚════════════════════════════╝");

				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine();
				Console.Write("Enter your choice: ");
				string choice = Console.ReadLine();

				if (int.TryParse(choice, out int selectedOption))
				{
					if (selectedOption >= 1 && selectedOption <= batchFiles.Count)
					{
						string selectedBatchFile = batchFiles[selectedOption - 1];
						ExecuteBatchFile(selectedBatchFile);
					}
					else if (selectedOption == exitOption)
					{
						return;
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Invalid choice. Please try again.");
						Console.ResetColor();
					}
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid choice. Please try again.");
					Console.ResetColor();
				}

				Console.WriteLine();
				Console.WriteLine("Press Enter to continue...");
				Console.ReadLine();
				Console.ResetColor();
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	static List<string> GetBatchFiles()
	{
		string scriptFolder = "Scripts";
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
		string scriptFolder = "Scripts";
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

}
