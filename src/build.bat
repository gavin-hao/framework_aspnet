
cd /d %~dp0
set p=%VS110COMNTOOLS%
path  %p:~0,-6%IDE\;

devenv /build Release .\Dongbo.Framework.sln

pause