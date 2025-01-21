@echo off
setlocal enabledelayedexpansion

rem 获取当前脚本目录并设置protoc路径
set PROTOC_PATH=./bin/protoc.exe

rem 定义基础路径和通用后缀
set BASE_PATH=./../../MMO-SERVER/Common/Summer/Proto/
set BASE_PATH2=./../../MMO-Client/MMOGame/Assets/Scripts/Proto/
set SOURCE_SUFFIX=/ProtoSource
set OUTPUT_SUFFIX=/ProtoClass

rem 定义相对的源、目标和复制目录（不含共同后缀）
set "REL_DIRS=Chat Combat Common ControlCenter Game Login Scene DBProxy"
rem 定义复制目录，使用“-”表示不复制
set "REL_COPY_DIRS=Chat Combat Common _ Game Login Scene DBProxy"

rem 转换字符串为数组以便于索引
set i=0
for %%D in (%REL_DIRS%) do (
    set "DIR_ARR[!i!]=%%D"
    set /a i+=1
)
set num_dirs=!i!

set i=0
for %%C in (%REL_COPY_DIRS%) do (
    set "CPY_ARR[!i!]=%%C"
    set /a i+=1
)

rem 确保数组长度相同
if not "%num_dirs%"=="%i%" (
    echo Error: The number of directories and copy directories must match.
    exit /b 1
)

set /a num_dirs=!num_dirs!-1
rem 逐一处理每对目录
for /l %%i in (0, 1, !num_dirs!) do (
    set "SOURCE_DIR=%BASE_PATH%!DIR_ARR[%%i]!!SOURCE_SUFFIX!"
    set "OUTPUT_DIR=%BASE_PATH%!DIR_ARR[%%i]!!OUTPUT_SUFFIX!"
    
    rem 判断是否进行复制
    set "COPY_DIR=!CPY_ARR[%%i]!"
    if "!COPY_DIR!"=="_" (
        set "SKIP_COPY=true"
    ) else (
        set "SKIP_COPY=false"
        
        rem 判断是否为绝对路径
        if "!COPY_DIR:~1,1!" == ":" (
            set "FULL_COPY_DIR=!COPY_DIR!/"
        ) else (
            set "FULL_COPY_DIR=%BASE_PATH2%!COPY_DIR!/"
        )
    )

    rem 编译当前目录下的所有 .proto 文件，加入额外的 proto path
    for %%f in ("!SOURCE_DIR!/*.proto") do (
        :: echo Compiling %%f
        "%PROTOC_PATH%" --proto_path="!SOURCE_DIR!" --proto_path="%BASE_PATH%" --csharp_out="!OUTPUT_DIR!" "%%f"
    )
    
    rem 复制生成的文件到目标复制目录，如果需要的话
    if "!SKIP_COPY!"=="false" (
        for %%f in ("!OUTPUT_DIR!\*.*") do (
            :: echo Copying %%~nxf to !FULL_COPY_DIR!
            copy "%%f" "!FULL_COPY_DIR!" > nul
        )
    )
)

echo Compilation and copying finished.
endlocal
pause