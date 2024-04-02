@SET EXCEL_FOLDER=.\excel
@SET JSON_FOLDER=.\out
@SET CODE_FOLDER=.\out
@SET EXE=.\excel2json.exe

#复制到目录
@SET DEST_FOLDER_1=..\MMOGame\Assets\Resources\Data
# ----------------------------------------------------------------------------

@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for /f "delims=" %%i in ('dir /b /a-d /s %EXCEL_FOLDER%\*.xlsx') do (
    @echo   processing %%~nxi 
    @CALL %EXE% --excel %EXCEL_FOLDER%\%%~nxi --json %JSON_FOLDER%\%%~ni.json --csharp %CODE_FOLDER%\%%~ni.cs --header 3
)


@ECHO Copying JSON files to destination folder %DEST_FOLDER% ...
for /r %JSON_FOLDER% %%i in (*.json) do (
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_1%\%%~nxi"
)

echo "OK"
pause