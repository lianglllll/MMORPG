@echo off

:: 设置输出路径，不存在就创建
set outPath=%cd%\out\csharp
if not exist "%outPath%" (
    md "%outPath%"
)

:: 遍历文件
set sourceFileDirectoryPath=..\..\MMO-SERVER\GameServer\Net\Proto
for %%i in (proto\*.proto) do (
    %cd%\bin\protoc -I=proto --csharp_out=%outPath% %%i
    echo %%i Done
)

:: 复制文件
set copyToTarget1=..\..\MMO-SERVER\Common\Summer\Proto
set copyToTarget2=..\..\MMO-Client\MMOGame\Assets\Scripts\Proto
for %%i in ("%outPath%\*.cs") do (
    copy /y "%%i" "%copyToTarget1%"
    copy /y "%%i" "%copyToTarget2%"
)

echo OK
pause