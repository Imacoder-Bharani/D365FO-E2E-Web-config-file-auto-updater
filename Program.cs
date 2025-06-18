using System.Xml;
using System.Diagnostics;

class ModifyWebConfigWithoutUpdatingManually
{
    System.ConsoleColor errorColor = ConsoleColor.DarkRed;
    System.ConsoleColor inputReceiveColor = ConsoleColor.Yellow;
    System.ConsoleColor userInputColor = ConsoleColor.Cyan;
    System.ConsoleColor feedBackForUserColor = ConsoleColor.Green;
    private string originalPath = @"C:\Temp\Original\web.config";
    private string runningPath = @"C:\Temp\Running\web.config";
    private string destPath = @"K:\AosService\WebRoot\web.config";

    /// <summary>
    ///     This method asks the user to provide the login credentials to connect with sandbox and then passes 
    ///     to actual method which updates the web file. Then it aks for the user if the console can move to next 
    ///     task or to main menu. If user input is to cloe the application then the method returns false which 
    ///     means to close the current application.
    /// </summary>
    /// <returns> Boolean </returns>
    public Boolean GetInputForWebRoot()
    {
        Boolean runNextProcess = true;
        string defaultWebRootPath = runningPath;
        string? webRootPath = "";
        string? databaseName = "";
        string? dbServer = "";
        string? userName = "";
        string? sqlPassword = "";

        Console.ForegroundColor = inputReceiveColor;
        Console.Write($"\n\nDefault web root path is {defaultWebRootPath}");
        Console.Write("\nCopy and Paste the Web Root Path and hit Enter. Keep empty if default: ");
        Console.ForegroundColor = userInputColor;
        webRootPath = Console.ReadLine();
        if (webRootPath == "")
            webRootPath = defaultWebRootPath;

        Console.ForegroundColor = inputReceiveColor;
        Console.Write("\nCopy and Paste the Database Name and hit Enter: ");
        Console.ForegroundColor = userInputColor;
        databaseName = Console.ReadLine();

        Console.ForegroundColor = inputReceiveColor;
        Console.Write("\nCopy and Paste the DB Server Name and hit Enter: ");
        Console.ForegroundColor = userInputColor;
        dbServer = Console.ReadLine();

        Console.ForegroundColor = inputReceiveColor;
        Console.Write("\nCopy and Paste the Username and hit Enter: ");
        Console.ForegroundColor = userInputColor;
        userName = Console.ReadLine();

        Console.ForegroundColor = inputReceiveColor;
        Console.Write("\nCopy and Paste the Password and hit Enter: ");
        Console.ForegroundColor = userInputColor;
        sqlPassword = Console.ReadLine();

        string inputString = $"DataAccess.Database={databaseName};DataAccess.DbServer={dbServer};DataAccess.SqlPwd={sqlPassword};DataAccess.SqlUser={userName}";

        this.ProcessWebRootUpdate(webRootPath, inputString);

        Console.ForegroundColor = feedBackForUserColor;
        Console.WriteLine("\nWeb Config file is updated successfully.\nPress 3 to move file from Running to Web Root.\nPress 1 to move to main menu.\nElse press any other key to exit!");

        Console.ForegroundColor = userInputColor;
        string getInput = Console.ReadLine();

        if (int.TryParse(getInput, out int type))
        {
            if (type == 3)
            {
                runNextProcess = this.copyFileSetDefaultProcess(false);
            }
            else if (type == 1)
            {
                runNextProcess = this.mainMenu();
            }
            else
            {
                runNextProcess = false;
            }
        }
        else
        {
            runNextProcess = false;
        }
        return runNextProcess;
    }

    /// <summary>
    ///     Performs the update of web configuration file with the parameters provided by user
    /// </summary>
    /// <param name="_filePath"> Path where the file recides </param>
    /// <param name="_credentials"> The credentials by which the file will get updated </param>
    private void ProcessWebRootUpdate(string _filePath, string _credentials)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(_filePath);

        if (doc is not null)
        {
            string[] keyValues = _credentials.Split(';');

            foreach (string pair in keyValues)
            {
                string[] parts = pair.Split('=');
                if (parts.Length == 2)
                {
                    //Fetching the XML node which has the same pattern
                    XmlNode node = doc.SelectSingleNode($"//add[@key='{parts[0].Trim()}']");
                    if (node is not null)
                    {
                        node.Attributes["value"].Value = parts[1].Trim();
                    }
                    //saving the each updated value
                    doc.Save(_filePath);
                }
            }
        }
    }

    /// <summary>
    ///     Copies the file with default paths or else from the provided by user
    /// </summary>
    /// <param name="isOriginalToWebRoot"> Checks if moves to webroot foler from original </param>
    /// <param name="isPathProvided"> Checks if path is provided </param>
    /// <param name="_sourceOriginal"> To provide path of Original Folder </param>
    /// <param name="_sourceRunning"> To provide path of Running Folder</param>
    /// <param name="_destPath"> The destination path means the standard web root folder path </param>
    /// <returns> Boolean </returns>
    public Boolean CopyFile(Boolean isOriginalToWebRoot = false,
        Boolean isPathProvided = false,
        string _sourceOriginal = "",
        string _sourceRunning = "",
        string _destPath = "")
    {
        Boolean runNextProcess = true;
        string infoMsg = "moved from Original to Web Root Folder";
        string sourceOriginal;
        string sourceRunning;
        string dest = @"K:\AosService\WebRoot\web.config";
        if (!isPathProvided)
        {
            sourceOriginal = originalPath;
            sourceRunning = runningPath;
            dest = destPath;
        }
        else
        {
            sourceOriginal = _sourceOriginal;
            sourceRunning = _sourceRunning;
            dest = _destPath;
        }
        if (isOriginalToWebRoot)
        {
            File.Copy(sourceOriginal, dest, true);
            infoMsg = "moved from Original to Web Root Folder";
        }
        else
        {
            File.Copy(sourceRunning, dest, true);
            infoMsg = "moved from Running to Web Root Folder";
        }

        Console.ForegroundColor = feedBackForUserColor;
        Console.WriteLine("\nFile has been " + infoMsg + "\nPress 4 to reset IIS.\nPress 1 to navigate to main menu.\nElse press any other key to exit.");
        Console.ForegroundColor = userInputColor;
        string input = Console.ReadLine();
        if (int.TryParse(input, out int type))
        {
            if (type == 4)
            {
                runNextProcess = this.RestartIIS();
            }
            else if (type == 1)
            {
                runNextProcess = this.mainMenu();
            }
            else
            {
                runNextProcess = false;
            }
        }
        else
        {
            runNextProcess = false;
        }
        return runNextProcess;
    }

    /// <summary>
    ///     To restart IIS Express
    /// </summary>
    /// <returns> Boolean </returns>
    public Boolean RestartIIS()
    {
        Boolean exitOnceDone = true;
        ProcessStartInfo startInfo = new ProcessStartInfo("iisreset.exe")
        {
            Arguments = "/restart",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        Console.ForegroundColor = feedBackForUserColor;
        Console.WriteLine("\nRestarting IIS Express");
        string message;
        using (Process process = System.Diagnostics.Process.Start(startInfo))
        {
            //process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode == 0)
            {
                Console.WriteLine(output);
                message = "Completed\nPress 1 to navigate to mainmenu.\nElse press any other key to exit";
            }
            else
            {
                Console.WriteLine(error);
                Console.WriteLine("Exitcode: " + process.ExitCode);
                message = "Failed. Press 4 to rerun the process.\nPress 1 to navigate to mainmenu.\nElse press any other key to exit.";
            }
        }
        Console.ForegroundColor = feedBackForUserColor;
        Console.WriteLine("\nIIS Reset " + message);
        Console.ForegroundColor = userInputColor;
        string inputUser = Console.ReadLine();
        if (int.TryParse(inputUser, out int type))
        {
            if (type == 4)
            {
                exitOnceDone = this.RestartIIS();
            }
            else if (type == 1)
            {
                exitOnceDone = this.mainMenu();
            }
            else
            {
                exitOnceDone = false;
            }
        }
        else
        {
            exitOnceDone = false;
        }
        return exitOnceDone;
    }

    /// <summary>
    ///     The actual segretaion method of the tasks which calls the related process based on the Key 
    ///     provided by user
    /// </summary>
    /// <param name="_key"> The integer value from 1 to 5 </param>
    /// <param name="_runProcess"> To check if we can run next process or to stop </param>
    /// <returns> Boolean </returns>
    private Boolean sequenceCall(int _key, Boolean _runProcess)
    {
        switch (_key)
        {
            case 1:
                _runProcess = this.GetInputForWebRoot();//Dev will provide input and actual update method will be called inside. Then will ask to move to Web Root folder. And the IIS Reset.
                break;
            case 2:
                _runProcess = this.copyFileSetDefaultProcess(false);
                break;
            case 3:
                _runProcess = this.copyFileSetDefaultProcess(true);
                break;
            case 4:
                _runProcess = this.RestartIIS();
                break;
            default:
                _runProcess = false;
                break;
        }
        return _runProcess;
    }

    /// <summary>
    ///     Checks with user if to copy file from Original or from Running to Standard webroot.
    ///     Then based on user input, the actual copy method will get executed from here
    /// </summary>
    /// <param name="isOriginalTo"> Code understands if this is true then from Original else Running </param>
    /// <returns> Boolean </returns>
    public Boolean copyFileSetDefaultProcess(Boolean isOriginalTo)
    {
        Boolean runProcess = true;
        Console.ForegroundColor = inputReceiveColor;
        if (isOriginalTo)
        {
            Console.WriteLine($"\nThe default original path is {originalPath}. Press N or n if to choose different path.Keep blank if choosing default.");
        }
        else
        {
            Console.WriteLine($"\nThe default running path is {runningPath}. Press N or n if to choose different path.Keep blank if choosing default.");
        }
        Console.ForegroundColor = userInputColor;
        string inputForPath = Console.ReadLine();
        if (string.IsNullOrEmpty(inputForPath))
        {
            runProcess = this.CopyFile(isOriginalTo);
        }
        else if (inputForPath != "N" && inputForPath != "n")
        {
            Console.ForegroundColor = feedBackForUserColor;
            Console.WriteLine("\nProvided input doesn't match with either n or N.\nPress 1 to navigate to mainmenu.\nPress any other number key to exit.");
            Console.ForegroundColor = userInputColor;
            string input = Console.ReadLine();
            if (int.TryParse(input, out int type))
            {
                if (type == 1)
                {
                    runProcess = this.mainMenu();
                }
                else
                {
                    runProcess = false;
                }
            }
            else
            {
                runProcess = false;
            }
        }
        else
        {
            Console.ForegroundColor = inputReceiveColor;
            Console.WriteLine("\nInput the path for source");
            Console.ForegroundColor = userInputColor;
            string sourcePath = Console.ReadLine();
            Console.ForegroundColor = inputReceiveColor;
            Console.WriteLine("\nInput the path for destination");
            Console.ForegroundColor = userInputColor;
            string destPath = Console.ReadLine();
            if (isOriginalTo)
            {
                runProcess = this.CopyFile(isOriginalTo, true, sourcePath, "", destPath);
            }
            else
            {
                runProcess = this.CopyFile(isOriginalTo, true, "", sourcePath, destPath);
            }
        }
        return runProcess;
    }
    /// <summary>
    ///     The main menu of the process which starts when application is become active and guides user
    ///     on how to use the application
    /// </summary>
    /// <returns> Boolean </returns>
    public Boolean mainMenu()
    {
        Boolean runProcess;
        string headLine = @"__________.__                               .__/\          _____                                          .__     
\______   \  |__ _____ ____________    ____ |__)/______   /  _  \ ______ _____________  _________    ____ |  |__  
 |    |  _/  |  \\__  \\_  __ \__  \  /    \|  |/  ___/  /  /_\  \\____ \\____ \_  __ \/  _ \__  \ _/ ___\|  |  \ 
 |    |   \   Y  \/ __ \|  | \// __ \|   |  \  |\___ \  /    |    \  |_> >  |_> >  | \(  <_> ) __ \\  \___|   Y  \
 |______  /___|  (____  /__|  (____  /___|  /__/____  > \____|__  /   __/|   __/|__|   \____(____  /\___  >___|  /
        \/     \/     \/           \/     \/        \/          \/|__|   |__|                    \/     \/     \/";
        string welcomeMessage = "\n\nWelcome to the Console to handle tedious tasks in a better way.\nHere are " +
"options. Press respective key for the task you want to accomplish\n\n" +
"1) Hit 1 and enter to update your Web Config File.\n2) Press 2 and enter to copy webconfig file from Running folder to WebRoot Folder.\n3) Press 3 and enter to copy webconfig file from Original folder to WebRoot Folder.\n4) Press 4 and enter to reset IIS Express.\n5) Press 5 and enter to exit.\n\n";

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(headLine);
        Console.WriteLine(welcomeMessage);
        Console.ForegroundColor = userInputColor;
        string actionType = Console.ReadLine();
        if (int.TryParse(actionType, out int type))
        {
            if (type >= 1 && type <= 5)
            {
                runProcess = true;
                runProcess = this.sequenceCall(type, runProcess);
            }
            else
            {
                Console.ForegroundColor = errorColor;
                Console.WriteLine("\nNo option is available for the given input. Exiting the Console...");
                runProcess = false;
            }
        }
        else
        {
            Console.ForegroundColor = errorColor;
            Console.WriteLine("\nSpecified value is not correct. Exiting the Console...");
            runProcess = false;
        }

        return runProcess;
    }
}

class Program
{
    /// <summary>
    ///     The start point of the application which triggers the actual code
    /// </summary>
    /// <param name="args"> args </param>
    static void Main(string[] args)
    {
        Boolean runProcess = true;

        while (runProcess)
        {
            try
            {
                ModifyWebConfigWithoutUpdatingManually obj = new ModifyWebConfigWithoutUpdatingManually();

                runProcess = obj.mainMenu();

                if (!runProcess)
                {
                    break;
                }
            }
            catch (FormatException e)
            {
                Program.processException(true, e.Message);
                continue;
            }
            catch (DirectoryNotFoundException e)
            {
                Program.processException(false, e.Message);
                continue;
            }
            catch (FileNotFoundException e)
            {
                Program.processException(false, e.Message);
                continue;
            }
            catch (Exception e)
            {
                Program.processException(false, e.Message);
                continue;
            }
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n\n\n\n\n\nClosing....");
        Thread.Sleep(200);
        Environment.Exit(0);
        return;
    }

    /// <summary>
    ///     For exception handling
    /// </summary>
    /// <param name="_isFormatException"> If error is related to formatting </param>
    /// <param name="_errorMessage"> The actual error message </param>
    private static void processException(Boolean _isFormatException, string _errorMessage)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\n" + _errorMessage);
        if (_isFormatException)
        {
            Console.WriteLine("\nInvalid input. Going back to main menu. Press any key to continue");
        }
        else
        {
            Console.WriteLine("\nAn unexpected error occured. Going back to main menu. Press any key to continue");
        }
        Console.ReadKey();
    }
}
