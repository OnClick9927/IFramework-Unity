echo off
cd ../
rd /s /q IFramework
git clone -b src https://github.com/OnClick9927/IFramework
cd IFramework
rd /s /q .git
cd ../

pause