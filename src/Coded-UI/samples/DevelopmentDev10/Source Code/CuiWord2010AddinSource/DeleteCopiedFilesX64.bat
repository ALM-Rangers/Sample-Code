echo off

del /p "%CommonProgramFiles(x86)%\Microsoft Shared\VSTT\10.0\UITestExtensionPackages\Microsoft.ALMRangers.UITest.Extension.Word.WordExtension.*"

del /p "%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\Common7\IDE\PrivateAssemblies\Microsoft.ALMRangers.UITest.Extension.Word.WordCommunication.*"

del /p "%CommonProgramFiles(x86)%\Microsoft Shared\VSTT\10.0\UITestExtensionPackages\Microsoft.VisualStudio.TestTools.UITest.Sample.ExcelExtension.*"

del /p "%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.TestTools.UITest.Sample.ExcelCommunication.*"


pause
