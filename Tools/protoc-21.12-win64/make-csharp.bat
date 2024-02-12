
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
copy /y out\csharp\Message.cs E:\MyProject\MMORPG\MMO-SERVER\Common\Summer\Proto\Message.cs
copy /y out\csharp\Message.cs E:\MyProject\MMORPG\MMO-Client\MMOGame\Assets\Plugins\Summer\Proto\Message.cs


echo "OK"
pause