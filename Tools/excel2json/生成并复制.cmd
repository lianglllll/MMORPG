@SET EXCEL_FOLDER=.\excel
@SET JSON_FOLDER=.\out
@SET CODE_FOLDER=.\out
@SET EXE=.\excel2json.exe

@SET DEST_FOLDER_1=E:\MyProject\MMORPG\MMO-Client\MMOGame\Assets\Resources\Data
@SET DEST_FOLDER_2=E:\MyProject\MMORPG\MMO-SERVER\GameServer\bin\Release\net5.0\Data
@SET DEST_FOLDER_3=E:\MyProject\MMORPG\MMO-SERVER\GameServer\bin\Debug\net5.0\Data


@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for /f "delims=" %%i in ('dir /b /a-d /s %EXCEL_FOLDER%\*.xlsx') do (
    @echo   processing %%~nxi 
    @CALL %EXE% --excel %EXCEL_FOLDER%\%%~nxi --json %JSON_FOLDER%\%%~ni.json --csharp %CODE_FOLDER%\%%~ni.cs --header 3
)


@ECHO Copying JSON files to destination folder %DEST_FOLDER% ...
for /r %JSON_FOLDER% %%i in (*.json) do (
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_1%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_2%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_3%\%%~nxi"
)

echo "OK"
pause