$appFolder = "C:\Users\gab_r\Documents\Estudo\Desenvolvimento\dot-net\poc-schedule\ScheduleApi\bin\Debug\netcoreapp2.2\publish\"
$username = "gab_r"
$appName = "Telegram Client Application"

$acl = Get-Acl $appFolder
$aclRuleArgs = $username, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $appFolder

New-Service -Name TelegramClientApp -BinaryPathName "dotnet " + $appFolder + "ScheduleApi.dll" -Credential $username -Description "TelegramClient App" -DisplayName "TelegramClient App" -StartupType Automatic