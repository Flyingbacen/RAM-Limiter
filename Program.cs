using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace RAMLimiter
{
    class Program
    {
        // Importing the Windows API function for clearing the working set memory
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hProcess);

        // Check if the program is running with administrator privileges
        static bool IsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        // Attempt to restart the program with elevated privileges if not already running as admin
        static void ElevatePrivileges()
        {
            if (!IsAdmin())
            {
                var procInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = Assembly.GetEntryAssembly().Location,
                    Verb = "runas"
                };
                try
                {
                    Process.Start(procInfo);
                    Environment.Exit(0); // Exit the current process after launching the elevated one
                }
                catch
                {
                    Console.WriteLine("Failed to request administrator privileges.");
                }
            }
        }

        // Get a list of processes by their names
        static List<Process> GetProcessesByNames(IEnumerable<string> processNames)
        {
            return processNames.SelectMany(name => Process.GetProcessesByName(name)).ToList();
        }

        // Reduce the memory usage of a specific process by clearing its working set
        static void ReduceRamUsage(Process process)
        {
            try
            {
                if (!process.HasExited && process.Id != 0)
                {
                    EmptyWorkingSet(process.Handle);
                    // Console.WriteLine($"Memory of process {process.ProcessName} ({process.Id}) has been reduced.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reducing memory for {process.ProcessName}: {ex.Message}");
            }
        }

        // Display the current memory usage of a process
        static void DisplayMemoryUsage(Process process)
        {
            try
            {
                process.Refresh(); // Refresh process properties
                // Console.WriteLine($"Process: {process.ProcessName}, Memory: {process.WorkingSet64 / (1024 * 1024)} MB");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving memory usage: {ex.Message}");
            }
        }

        // sum WorkingSet64 for same ProcessName
        static List<(string Name, long TotalBytes, int Count)> AggregateMemoryByName(IEnumerable<Process> processes)
        {
            var result = new List<(string, long, int)>();
            var aggregates = processes
                .Where(p => 
                {
                    try { p.Refresh(); return !p.HasExited; }
                    catch { return false; }
                })
                .GroupBy(p => p.ProcessName)
                .Select(g => new { Name = g.Key, Total = g.Sum(p => p.WorkingSet64), Count = g.Count() });

            foreach (var a in aggregates)
            {
                result.Add((a.Name, a.Total, a.Count));
            }

            return result;
        }

        // Monitor specified processes and reduce their memory usage periodically
        static async Task MonitorAndReduceRamAsync(IEnumerable<string> processNames)
        {
            while (true)
            {
                var processes = GetProcessesByNames(processNames);
                var aggregates = AggregateMemoryByName(processes);
                foreach (var (Name, TotalBytes, Count) in aggregates)
                {
                    Console.WriteLine($"Process: {Name}, Instances: {Count}, Total Memory: {TotalBytes / (1024 * 1024)} MB");
                }

                foreach (var process in processes)
                {
                    // DisplayMemoryUsage(process);
                    ReduceRamUsage(process);
                }
                for (int i = 3; i > 0; i--) // Wait for 3 seconds before the next cycle
                {
                    Console.Write($"\rWaiting {i} seconds...");
                    await Task.Delay(1000);
                }
                Console.Write("\r                    \n"); // clear the line
            }
        }

        static async Task Main(string[] args)
        {
            ElevatePrivileges(); // Ensure the program has elevated privileges

            Console.WriteLine("Enter the process names separated by commas (e.g., chrome,discord,obs):");
            var input = Console.ReadLine();
            var processNames = input.Split(',').Select(p => p.Trim()).ToList();

            Console.WriteLine("Starting memory optimization...");
            await MonitorAndReduceRamAsync(processNames); // Begin monitoring and reducing memory usage
        }
    }
}
