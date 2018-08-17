dotnet publish

Import-Module Posh-SSH

# Scrape the project name
$Project = Get-ChildItem -Filter *.csproj | Select-Object -First 1
$PeriodPosition = $Project.Name.IndexOf(".")
$ProjectName = $Project.Name.Substring(0, $PeriodPosition)

# Set the credentials
$Password = ConvertTo-SecureString 'HoloLens123' -AsPlainText -Force
$Credential = New-Object System.Management.Automation.PSCredential ('pi', $Password)
$RemoteHost = '10.39.1.48'

# Set the file paths
$FilePath = ""
$UnixTime = [int][double]::Parse((Get-Date -UFormat %s))
$SftpPath = '/home/pi/Desktop/dotnetProjects/' + $ProjectName 
$MkdirCommand =  'rm -Rf' + $SftpPath + '&& mkdir ' + $SftpPath


# Create the SFTP session
$SFTPSession = New-SFTPSession -ComputerName $RemoteHost -Credential $Credential
$SSHSession = New-SSHSession -ComputerName $RemoteHost -Credential $Credential

# Create the directory to put the files in
$InvokeResult = Invoke-SSHCommand -Command $MkdirCommand -Session ($SSHSession).SessionId  -EnsureConnection
SFTPSession
# Move to the output directory of the script
cd .\bin\Debug\netcoreapp2.0\publish

# Get all files to be uploaded
$Files = Get-ChildItem

# Upload the files
Foreach($File in $Files)
{
    Set-SFTPFile -SessionId ($SFTPSession).SessionId -LocalFile $File -RemotePath $SftpPath -Overwrite
}

#Disconnect all SFTP Sessions
Get-SFTPSession | % { Remove-SFTPSession -SessionId ($_.SessionId) }
Get-SSHSession | % { Remove-SSHSession -SessionId ($_.SessionId) }

cd ../../../
