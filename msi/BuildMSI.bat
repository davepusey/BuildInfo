SET BINDIR=C:\Program Files (x86)\WiX Toolset v3.10\bin

del *.msi
del *.wixobj
del *.wixpdb

"%BINDIR%\candle.exe" *.wxs
"%BINDIR%\light.exe" *.wixobj -sice:ICE38 -sice:ICE43 -sice:ICE69

pause