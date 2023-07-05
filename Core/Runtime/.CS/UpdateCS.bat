echo off
cd ../

if exist IFramework (
    cd IFramework
    if exist .src (
       echo "begin pull IFramework from github"
       md .git
       xcopy /s /y /q .src .git
       rd /s /q .src
       git pull
       md .src
       xcopy /s /y /q .git .src
       rd /s /q .git
       cd ../
    ) else (
        cd ../
        rd /s /q IFramework
        goto IFramework
    )
) else (
:IFramework
    echo "begin clone IFramework from github"
    git clone -b src https://github.com/OnClick9927/IFramework
    cd IFramework
    md .src
    xcopy /s /y /q .git .src
    rd /s /q .git
    cd ../
)
echo "End IFramework"
