#!/bin/bash
#Update symlinks in dosdevices:
wine cmd /c ver >> /dev/null
truncate -s 0 serialports.txt
for fn in /dev/serial/by-id/*; do
    target=$( readlink $fn )
    a="${fn##*/}"
    b="${target##*/}"
    for com in  ~/.wine/dosdevices/com*; do
    	dostarget=$( readlink $com )
    	x="${dostarget##*/}"
    	if [ "$x" = "$b" ]; then    	
    		c="${com##*/}"
    		echo "$a\t$b\t$c" >> serialports.txt
    		break
    	fi
    done
done

wine UniversalPatcher.exe
