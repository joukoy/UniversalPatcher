powershell  -Executionpolicy Unrestricted -File Create-Zip.ps1
echo k:| del BETA\UniversalPatcher-Beta.Zip
move UniversalPatcher-Full.Zip BETA\UniversalPatcher-Beta.Zip
pause