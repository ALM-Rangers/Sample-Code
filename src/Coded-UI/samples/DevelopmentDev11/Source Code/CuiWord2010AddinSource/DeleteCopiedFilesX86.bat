echo off

del /p "%CommonProgramFiles%\Microsoft Shared\VSTT\10.0\UITestExtensionPackages\Microsoft.ALMRangers.UITest.Extension.Word.WordExtension.*"

del /p "%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\PrivateAssemblies\Microsoft.ALMRangers.UITest.Extension.Word.WordCommunication.*"

del /p "%CommonProgramFiles%\Microsoft Shared\VSTT\10.0\UITestExtensionPackages\Microsoft.VisualStudio.TestTools.UITest.Sample.ExcelExtension.*"

del /p "%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.TestTools.UITest.Sample.ExcelCommunication.*"


pause
