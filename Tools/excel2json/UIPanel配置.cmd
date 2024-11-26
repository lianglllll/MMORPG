@SET EXCEL_FOLDER=.\excel\UIPanel
@SET JSON_FOLDER=.\out\UIPanelJson
@SET CODE_FOLDER=.\out\UIPanelDefine
@SET EXE=.\excel2json.exe

:: josn文件要复制到的目的地
@SET DEST_FOLDER_1=..\..\MMO-Client\MMOGame\Assets\Res\Files\Data
<<<<<<< HEAD
@SET DEST_FOLDER_2=..\..\MMO-Client\MMOGame\Assets\Scripts\Manager\DataDefine
=======
>>>>>>> 71706afc428a8a0d57234e05ead5afba5bf04b94

:: execl转json和生成对应的define文件
@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for %%i in ("%EXCEL_FOLDER%\*.xlsx") do (
    @echo   processing %%~nxi 
    @CALL %EXE% --excel "%%i" --json "%JSON_FOLDER%\%%~ni.json" --csharp "%CODE_FOLDER%\%%~ni.cs" --header 3 --exclude_prefix #
)

:: 复制json文件
@ECHO Copying JSON files to destination folder %DEST_FOLDER% ...
<<<<<<< HEAD
for %%i in (%JSON_FOLDER%\*.json) do (
=======
for /r %JSON_FOLDER% %%i in (*.json) do (
>>>>>>> 71706afc428a8a0d57234e05ead5afba5bf04b94
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_1%\%%~nxi"
)

<<<<<<< HEAD
:: 复制define文件
for %%i in (%CODE_FOLDER%\*.cs) do (
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_2%\%%~nxi"
)

=======
>>>>>>> 71706afc428a8a0d57234e05ead5afba5bf04b94

echo "OK"
pause