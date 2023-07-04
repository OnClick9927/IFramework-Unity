echo off
cd ../
if not exist IFramework (
  echo IFramework not exist,clone from src.
  git clone -b src https://github.com/OnClick9927/IFramework
) else (
  echo IFramework exists, pull to update.
  git pull
)
pause