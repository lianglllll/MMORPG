@SET EXCEL_FOLDER=.\excel
@SET JSON_FOLDER=.\out
@SET CODE_FOLDER=.\out
@SET EXE=.\excel2json.exe

:: josn文件要复制到的目的地
@SET DEST_FOLDER_1=..\..\MMO-Client\MMOGame\Assets\Res\Files\Data
@SET DEST_FOLDER_2=..\..\MMO-SERVER\GameServer\bin\Release\net6.0\Data
@SET DEST_FOLDER_3=..\..\MMO-SERVER\GameServer\bin\Debug\net6.0\Data
:: define文件要复制到的目的地
<<<<<<< HEAD
@SET DEST_FOLDER_4=..\..\MMO-Client\MMOGame\Assets\Scripts\Manager\DataDefine
=======
@SET DEST_FOLDER_4=..\..\MMO-Client\MMOGame\Assets\Script\Manager\DataDefine
>>>>>>> 71706afc428a8a0d57234e05ead5afba5bf04b94
@SET DEST_FOLDER_5=..\..\MMO-SERVER\GameServer\DataDefine

:: execl转json和生成对应的define文件
@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for %%i in ("%EXCEL_FOLDER%\*.xlsx") do (
    @echo   processing %%~nxi 
    @CALL %EXE% --excel "%%i" --json "%JSON_FOLDER%\%%~ni.json" --csharp "%CODE_FOLDER%\%%~ni.cs" --header 3 --exclude_prefix #
)

<<<<<<< HEAD

=======
>>>>>>> 71706afc428a8a0d57234e05ead5afba5bf04b94
:: 复制json文件
@ECHO Copying JSON files to destination folder %DEST_FOLDER% ...
for %%i in (%JSON_FOLDER%\*.json) do (
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_1%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_2%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_3%\%%~nxi"
)

:: 复制define文件
<<<<<<< HEAD
for %%i in (%CODE_FOLDER%\*.cs) do (
=======
for /r %CODE_FOLDER% %%i in (*.cs) do (
>>>>>>> 71706afc428a8a0d57234e05ead5afba5bf04b94
    @echo   copying %%~nxi 
    @COPY "%%i" "%DEST_FOLDER_4%\%%~nxi"
    @COPY "%%i" "%DEST_FOLDER_5%\%%~nxi"
)


echo "OK"
pause