@echo off

set logfile=%windir%\inf\setupapi.dev.log

rem first clear the devcon log
if exist "%logfile%" (
	del "%logfile%"
)

%~dp0devcon.exe remove root\BixVirtualReader

if %ERRORLEVEL% neq 0 (
	echo ERROR: %ERRORLEVEL%
	echo.
	if exist "%logfile%" (
		type "%logfile%"
		echo.
		echo There was an error. You can examine what went wrong here: "%logfile%"
		echo.
	)
)