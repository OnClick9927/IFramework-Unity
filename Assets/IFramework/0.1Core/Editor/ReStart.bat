@echo off
set unityPath=%~1%
set projectPath=%~2%
for /f "delims = "" tokens = 1" %%v in ( %unityPath% ) do ( set unityPath=%%v )
for /f "delims = "" tokens = 1" %%v in ( %projectPath% ) do ( set projectPath=%%v )
start %unityPath% -projectPath %projectPath%
