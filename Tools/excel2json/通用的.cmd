@SET EXCEL_FOLDER=.\excel
@SET JSON_FOLDER=.\out
@SET CODE_FOLDER=.\out
@SET EXE=.\excel2json.exe

:: josn文件要复制到的目的地
@SET DEST_FOLDER_1=..\..\MMO-Client\MMOGame\Assets\Res\Files\Data
@SET DEST_FOLDER_2=..\..\MMO-SERVER\Common\Summer\StaticData\Data
:: define文件要复制到的目的地
@SET DEST_FOLDER_4=..\..\MMO-Client\MMOGame\Assets\Scripts\LocalData\DataDefine
@SET DEST_FOLDER_5=..\..\MMO-SERVER\Common\Summer\StaticData\DataDefine

:: execl转json和生成对应的define文件
::@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for %%i in ("%EXCEL_FOLDER%\*.xlsx") do (
    @CALL %EXE% --excel "%%i" --json "%JSON_FOLDER%\%%~ni.json" --csharp "%CODE_FOLDER%\%%~ni.cs" --header 3 --exclude_prefix #
)

:: 复制json文件
::@ECHO Copying JSON files to destination folder %JSON_FOLDER% ...
for %%i in (%JSON_FOLDER%\*.json) do (
    @COPY "%%i" "%DEST_FOLDER_1%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_2%\%%~nxi"
)

:: 复制define文件
for %%i in (%CODE_FOLDER%\*.cs) do (
    @COPY "%%i" "%DEST_FOLDER_4%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_5%\%%~nxi"
)

echo "OK"
pause