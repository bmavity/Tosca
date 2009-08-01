@echo off

SET DIR=%~d0%~p0%

SET file.settings="%DIR%..\Settings\${environment}.settings"
SET dirs.drop="%DIR%.."

"%DIR%nant\nant.exe" /f:"%DIR%\web.deploy" -D:file.settings=%file.settings% -D:dirs.drop=%dirs.drop%
