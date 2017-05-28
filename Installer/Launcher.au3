Const $AppTitle = 'Anfloga'
Const $MB_ICONERROR = 16
 
If RegRead('HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client', 'Version') < "4.5.2" and RegRead('HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\NET Framework Setup\NDP\v4\Client', 'Version') < "4.5.2" And Then
    MsgBox($MB_ICONERROR, $AppTitle, 'The .NET Framework runtime v4.5.2 is required to run.')
	ShellExecute("https://www.microsoft.com/en-us/download/details.aspx?id=42643")
    Exit 1
EndIf
 
If RegRead('HKEY_LOCAL_MACHINE\Software\Microsoft\XNA\Framework\v4.0', 'Installed') <> 1 And RegRead('HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\XNA\Framework\v4.0', 'Installed') <> 1 Then
    MsgBox($MB_ICONERROR, $AppTitle, 'The Microsoft XNA Framework runtime v4.0 is required to run.')
	ShellExecute("http://www.microsoft.com/en-us/download/details.aspx?id=20914")
    Exit 1
EndIf
 
Exit RunWait('Data\Anfloga.exe')