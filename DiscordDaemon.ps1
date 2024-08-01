$loop = 1;
while ($loop)
{
    $process = Start-Process "C:\Khu.2\Khu.2\bin\Debug\net7.0\Khu.2.exe" -Wait -NoNewWindow -PassThru
    switch ($process.ExitCode)
    {
        0 {"Exiting."; $loop = 0;}
        1 {"Restarting..."; Start-Sleep -s 3}
        default {"Unhandled Exit Code, Exiting."; $loop = 0}
    }
}

# In your bot, you can use Environment.Exit(0) to kill, or Environment.Exit(1) to restart.
