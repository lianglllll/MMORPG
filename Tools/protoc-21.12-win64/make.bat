@echo off
setlocal enabledelayedexpansion

rem 获取当前脚本目录并设置protoc路径
set PROTOC_PATH=./bin/protoc.exe

rem 定义源、目标和复制目录对，以空格分隔
set "SOURCE_DIRS=./../../MMO-SERVER/Common/Summer/Proto/ProtoSource ./../../MMO-SERVER/GameServer/Net/Proto/ProtoSource ./../../MMO-SERVER/ControlCenter/Net/Proto/ProtoSource"
set "OUTPUT_DIRS=./../../MMO-SERVER/Common/Summer/Proto/ProtoClass ./../../MMO-SERVER/GameServer/Net/Proto/ProtoClass ./../../MMO-SERVER/ControlCenter/Net/Proto/ProtoClass"
set "COPY_DIRS=D:/ D:/ D:/"

rem 定义额外的 proto 搜索路径
set "COMMON_PROTO_PATH=./../../MMO-SERVER"

rem 转换字符串为数组以便于索引
set i=0
for %%S in (%SOURCE_DIRS%) do (
    set "SRC_ARR[!i!]=%%S"
    set /a i+=1
)
set num_sources=!i!

set i=0
for %%O in (%OUTPUT_DIRS%) do (
    set "OUT_ARR[!i!]=%%O"
    set /a i+=1
)

set i=0
for %%C in (%COPY_DIRS%) do (
    set "CPY_ARR[!i!]=%%C"
    set /a i+=1
)

rem 确保数组长度相同
if not "%num_sources%"=="%i%" (
    echo Error: The number of source, output, and copy directories must match.
    exit /b 1
)

set /a num_sources=!num_sources!-1
rem 逐一处理每对目录
for /l %%i in (0, 1, !num_sources!) do (
    set "SOURCE_DIR=!SRC_ARR[%%i]!"
    set "OUTPUT_DIR=!OUT_ARR[%%i]!"
    set "COPY_DIR=!CPY_ARR[%%i]!"

    rem 编译当前目录下的所有 .proto 文件，加入额外的 proto path
    for %%f in ("!SOURCE_DIR!/*.proto") do (
        echo Compiling %%f
        "%PROTOC_PATH%" --proto_path="!SOURCE_DIR!" --proto_path="%COMMON_PROTO_PATH%" --csharp_out="!OUTPUT_DIR!" "%%f"
    )
    
    rem 复制生成的文件到目标复制目录
    for %%f in ("!OUTPUT_DIR!\*.*") do (
        echo Copying %%~nxf to !COPY_DIR!
        copy "%%f" "!COPY_DIR!\" > nul
    )
)

echo Compilation and copying finished.
endlocal
pause