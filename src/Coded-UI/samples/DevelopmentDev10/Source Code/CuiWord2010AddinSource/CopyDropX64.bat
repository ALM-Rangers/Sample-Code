echo off

xcopy /y "%~dp0\WordExtension\bin\Debug\Microsoft.ALMRangers.UITest.Extension.Word.WordExtension.*" "%CommonProgramFiles(x86)%\Microsoft Shared\VSTT\10.0\UITestExtensionPackages\*.*"

xcopy /y "%~dp0\WordExtension\bin\Debug\Microsoft.ALMRangers.UITest.Extension.Word.WordCommunication.*" "%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\Common7\IDE\PrivateAssemblies\*.*"

pause
