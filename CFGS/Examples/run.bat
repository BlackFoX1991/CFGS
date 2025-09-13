REM Run CFGS and test examples
@echo off
SET scriptpath=%CD%
cd ..
SET PATH=%PATH%;%CD%\bin\Debug\net9.0\Path
start "" "cfgs.exe" -w "%scriptpath%\main.cfgs" -r -m