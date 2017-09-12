pushd "%~dp0node\"

echo Installing packages...
call npm install autoprefixer@7.1.4 --no-save


echo Compressing artifacts and cleans up...
"%~dp07z.exe" a -r "%~dp0..\src\node_files.zip" ./node * > nul
rmdir /S /Q "%~dp0node\node_modules" > nul


:done
echo Done
popd