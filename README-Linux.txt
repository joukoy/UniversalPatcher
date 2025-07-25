Required steps to run under wine, tested with Linux mint 22.1 x64:

sudo apt install wine32 winetricks
winetricks dotnet472  # dotnet40 should work too, but not tested
winetricks jet40
#For logger, allow using serial ports:
sudo usermod -a -G dialout <userName>

#Start Universalpatcher:
sh universalpatcher.sh



