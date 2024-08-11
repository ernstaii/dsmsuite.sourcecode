set TARGET=Publish\DsmSuite
set PROJECTS=DsmSuite.Analyzer.Compare DsmSuite.Analyzer.DotNet DsmSuite.Analyzer.JDeps DsmSuite.Analyzer.UML DsmSuite.DsmViewer.View

for %%p in (%PROJECTS%) do dotnet publish --no-build -p:PublishProfile=FolderProfile %%p

del /q %TARGET%
mkdir %TARGET%

for %%p in (%PROJECTS%) do copy %%p\bin\Publish\* %TARGET%

rename %TARGET%\DsmSuite.DsmViewer.View.exe DsmViewer.exe

echo Published to %TARGET%.
