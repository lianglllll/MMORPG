
@echo off

set foo="%cd%\out\csharp"

if not exist %foo% (
    md %foo%
)

::遍历文件
for %%i in (proto/*.proto) do ( 
    %cd%/bin/protoc -I=proto --csharp_out=out/csharp %%i
    echo %%i Done
)


::复制文件
copy /y out\csharp\Message.cs ..\..\MMO-SERVER\Common\Summer\Proto\Message.cs
copy /y out\csharp\Message.cs ..\..\MMO-Client\MMOGame\Assets\Script\Proto\Message.cs


echo "OK"
pause