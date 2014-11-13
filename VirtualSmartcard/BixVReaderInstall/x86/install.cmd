@echo off

set logfile=%windir%\inf\setupapi.dev.log

rem first clear the devcon log
if exist "%logfile%" (
	del "%logfile%"
)

%~dp0devcon.exe install %~dp0BixVReader-Package\BixVReader.inf root\BixVirtualReader

if %ERRORLEVEL% neq 0 (
	echo ERROR: %ERRORLEVEL%
	echo.
	if exist "%logfile%" (
		type "%logfile%"
		echo.
		echo Something wen wrong. Make sure you installed BixVReader-Package.cer into your local machine's Root Certificate Authorities Store.
		echo.
		echo Alternatively, you can also examine what went wrong here: "%logfile%"
		echo.
	)
) else (
	%~dp0devcon.exe find root\BixVirtualReader
)
