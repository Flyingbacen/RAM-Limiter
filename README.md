# RAM Limiter

This is a Fork of a Fork of ![RAM-Limiter](https://github.com/0vm/RAM-Limiter) but a minor detailing rework!

A C# program designed to reduce the memory usage of selected processes by freeing their unused working set memory. This can help improve system performance and optimize resource utilization.

## How It Works
The program reduces the memory usage of processes by using the `EmptyWorkingSet` function from the Windows API. It allows users to monitor and optimize memory usage for specific processes.

## Requirements
- Windows operating system
- .NET SDK (recommended: version 8.0 or higher)
- Administrator privileges (the program must be run with elevated rights)

## Installation
1. Ensure the .NET SDK is installed.
2. Clone the repository:
   ```bash
   git clone https://github.com/your-username/ram-limiter.git
   ```
3. Navigate to the project directory:
   ```bash
   cd ram-limiter
   ```
4. Build the project:
   ```bash
   dotnet build
   ```

## Usage
1. Run the program with administrator privileges:
   ```bash
   dotnet run
   ```
2. Enter the names of the processes you want to optimize (e.g., `chrome,discord,obs`).
3. The program will monitor these processes and periodically reduce their memory usage.

## Example Output
```plaintext
Enter the process names separated by commas (e.g., chrome,discord,obs):
chrome,discord
Process: chrome, Instances: 17, Total Memory: 27 MB
Process: Discord, Instances: 6, Total Memory: 215 MB

Process: chrome, Instances: 16, Total Memory: 35 MB
Process: Discord, Instances: 6, Total Memory: 172 MB

Process: chrome, Instances: 16, Total Memory: 14 MB
Process: Discord, Instances: 6, Total Memory: 147 MB
Waiting 3 seconds...
```

## Features
- Automatic reduction of memory usage using `EmptyWorkingSet`
- Display of current memory usage for processes
- Periodic monitoring and optimization (default: every 3 seconds)

## Known Limitations
- Only works on Windows systems
- Some processes may have restrictions that prevent memory access, resulting in errors.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing
Contributions are welcome! For major changes, please open an issue first to discuss what you would like to change.

