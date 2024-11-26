@SET EXCEL_FOLDER=.\excel\DialogExecl
@SET JSON_FOLDER=.\out\DialogJson
@SET CODE_FOLDER=.\out\DialogDefine
@SET EXE=.\excel2json.exe

:: josn文件要复制到的目的地
@SET DEST_FOLDER_1=..\..\MMO-Client\MMOGame\Assets\Res\Files\Data\Dialog

:: execl转json和生成对应的define文件
@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for %%i in ("%EXCEL_FOLDER%\*.xlsx") do (
    @echo   processing %%~nxi 
    @CALL %EXE% --excel "%%i" --json "%JSON_FOLDER%\%%~ni.json" --csharp "%CODE_FOLDER%\%%~ni.cs" --header 3 --exclude_prefix #
)

:: 复制json文件
@ECHO Copying JSON files to destination folder %DEST_FOLDER% ...
for /r %JSON_FOLDER% %%i in (*.json) do (
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_1%\%%~nxi"
)


echo "OK"
pause