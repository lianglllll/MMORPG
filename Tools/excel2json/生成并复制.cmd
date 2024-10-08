@SET EXCEL_FOLDER=.\excel
@SET JSON_FOLDER=.\out
@SET CODE_FOLDER=.\out
@SET EXE=.\excel2json.exe

@SET DEST_FOLDER_1=..\..\MMO-Client\MMOGame\Assets\Res\Files\Data
@SET DEST_FOLDER_2=..\..\MMO-SERVER\GameServer\bin\Release\net6.0\Data
@SET DEST_FOLDER_3=..\..\MMO-SERVER\GameServer\bin\Debug\net6.0\Data

@SET DEST_FOLDER_4=..\..\MMO-Client\MMOGame\Assets\Script\Manager\DataDefine
@SET DEST_FOLDER_5=..\..\MMO-SERVER\GameServer\DataDefine

Assets/Script/Manager/DataDefine
@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for /f "delims=" %%i in ('dir /b /a-d /s %EXCEL_FOLDER%\*.xlsx') do (
    @echo   processing %%~nxi 
    @CALL %EXE% --excel %EXCEL_FOLDER%\%%~nxi --json %JSON_FOLDER%\%%~ni.json --csharp %CODE_FOLDER%\%%~ni.cs --header 3 --exclude_prefix #
)


@ECHO Copying JSON files to destination folder %DEST_FOLDER% ...
for /r %JSON_FOLDER% %%i in (*.json) do (
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_1%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_2%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_3%\%%~nxi"
)

for /r %CODE_FOLDER% %%i in (*.cs) do (
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_4%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_5%\%%~nxi"
)


echo "OK"
pause