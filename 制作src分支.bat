@echo off
set b="version"
set version ="1"
REM 获取版本号
for /f "tokens=1,2* delims=:," %%a in (Assets/IFramework/package.json) do (
    echo %%a| findstr %b% >nul && (
       set version=  %%b
    ) || (
        @REM echo %%a nnn %b%
    )
)


set version=%version: =%
echo on
git subtree split --prefix=Assets/IFramework --branch src
git push origin src:src
git tag %version% src
git push origin src --tags
set cur=%~dp0


pause