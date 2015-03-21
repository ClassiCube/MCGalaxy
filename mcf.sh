#!/bin/bash
#
#Change these setting according to your setup.
################################################################################################################################################################
default="normal"					##  "normal" = normal console mode, "silent" = silent background mode
mcfdir="./"							##  Folder containing MCGalaxy.exe
monobin="/opt/mono-2.10/bin/mono"	##  Where is mono? Do not change if unsure. Typically "/usr/bin/mono"
gameopt="--gc=sgen"					##  Mono garbage collector options, either "--gc=boehm" (older mono versions) or "--gc=sgen" (mono 2.8 or newer)
gamename="RedCraft - MCGalaxy"		##  Arbitrary name of server, will not affect actual server name
gamepid="${mcfdir}/mcf.pid"			##  If you do not know what this is, do not worry about it, for "silent" mode only.
gamelog="${mcfdir}/mcf.log"			##  This logs everything sent to console, if started in "silent" mode
autorestart=true					##  set to false if you'd rather not auto-restart
################################################################################################################################################################
#
# NO CHANGES BELOW. ELSE ITS ON YOUR OWN RISK
#
author=RedNoodle
if [ -f "${monobin}" ]; then
	if [ ! -x "${monobin}" ]; then
		echo -e "${monobin} file is not executable"
		echo -e "Please fix this and try again"
		exit 2
	fi
else
	echo "cannot find ${monobin}!"
	echo "If this is not correct edit the start script"
	exit 2
fi

if [ ! -f "${mcfdir}/MCGalaxy.exe" ]; then
	echo -e "cannot find ${mcfdir}/MCGalaxy.exe!"
	echo -e "If this is not correct edit the start script"
	exit 2
fi

case "$1" in
	silent)
		echo "Usage: $0 {stop|restart|status|check}"
		echo -n "Starting $gamename server in silent background mode: "
		if ps -ef |grep "${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe"|awk -F" " -v game=${monobin} 'BEGIN {status=1} ; $8 == game {status=0} ; END {exit status}' ; then
			echo -e "already active"
			exit 0
		else
			if [ -f "${gamelog}" ]; then
				cp ${gamelog} ${gamelog}.crash
			fi
			if ${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe 1>> ${gamelog} 2>&1 & sleep 3 ; then
				pid=`ps -ef |grep "${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe"|awk -F" " -v game=${monobin} 'BEGIN {status=1} ; $8 == game { print $2 } ; END {exit status}'`
				echo ${pid} > ${gamepid}
				if [ -f "${gamepid}" ] && ps h `cat "${gamepid}"` >/dev/null; then
					echo -e "....Started!"
					exit 0
		      		else
					echo -e "....Failed to start. Check logfile or run in normal mode!"
					exit 1
			     	fi
			else
				echo -e "Failed"
			fi
		fi
;;
	stop)
		echo "Usage: $0 {stop|restart|status|check}"
		echo -n "Stopping $gamename server: "
		if ! ps -ef |grep "${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe"|awk -F" " -v game=${monobin} 'BEGIN {status=1} ; $8 == game {status=0} ; END {exit status}' ; then
			echo -e "server not running or crashed."
		else
			pid=`ps -ef |grep "${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe"|awk -F" " -v game=${monobin} 'BEGIN {status=1} ; $8 == game { print $2 } ; END {exit status}'`
			echo ${pid} > ${gamepid}
			kill -9 `cat ${gamepid}`
			if ! ps -ef |grep "${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe"|awk -F" " -v game=${monobin} 'BEGIN {status=1} ; $8 == game {status=0} ; END {exit status}' ; then
				echo -e "stopped"
				exit 0
			else
				echo -e "unable to stop server or server crashed"
			fi
		fi
;;
	status)
		echo "Usage: $0 {stop|restart|status|check}"
		echo -n "`date +"%Y-%m-%d %H:%M:%S"` Checking $gamename server status: "
		if ! ps -ef |grep "${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe"|awk -F" " -v game=${monobin} 'BEGIN {status=1} ; $8 == game {status=0} ; END {exit status}' ; then
			echo -e "server not running or crashed... Restarting"
			$0
		else
			echo -e "Server still running."
		fi
;;
	check)
	        echo -n "Checking $gamename server status: "
		if ! ps -ef |grep "${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe"|awk -F" " -v game=${monobin} 'BEGIN {status=1} ; $8 == game {status=0} ; END {exit status}' ; then
        	        echo -e "offline"
	        else
	                echo -e "online"
	        fi
;;
	restart)
		echo "Usage: $0 {stop|restart|status|check}"
		echo -e "Restarting $gamename server... "
		$0 stop && $0
;;
	normal)
		echo -n "Starting $gamename server with '${gameopt}' "
		if ps -ef |grep "${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe"|awk -F" " -v game=${monobin} 'BEGIN {status=1} ; $8 == game {status=0} ; END {exit status}' ; then
   			echo -e "already active"
			exit 3
		else
			echo -e "--Hit CTRL+C multiple times to kill the script! Use '/save all' first, if you want to save"
			echo -e
			${monobin} ${gameopt} ${mcfdir}/MCGalaxy.exe
			if $autorestart ; then
				$0
			else
				exit 0
			fi
		fi
;;
	*)
		if [ -f "${mcfdir}/MCGalaxy_.update" ]; then
			if [ -f "${mcfdir}/MCGalaxy.update" ]; then
				echo
				echo Update Found!
				echo -n Applying update:
				rm ${mcfdir}/MCGalaxy.exe
				rm ${mcfdir}/MCGalaxy_.dll
				mv ${mcfdir}/MCGalaxy.update ${mcfdir}/MCGalaxy.exe
				mv ${mcfdir}/MCGalaxy_.update ${mcfdir}/MCGalaxy_.dll
				if [ -f "${mcfdir}/MCGalaxy_.update" ]; then
					if [ -f "${mcfdir}/MCGalaxy.update" ]; then
						echo -e FAILED!
						if [ -f "${mcfdir}/MCGalaxy_.dll" ]; then
							if [ -f "${mcfdir}/MCGalaxy.exe" ]; then
								$0 ${default}
							fi
						else
							echo -e Update totally derped, files missing. Please Re-download!
							exit 1
						fi
					fi
				else
					if [ -f "${mcfdir}/MCGalaxy_.dll" ]; then
						if [ -f "${mcfdir}/MCGalaxy.exe" ]; then
							echo -e SUCCESS!
							$0 ${default}
						fi
					fi
				fi
			fi
		else
			echo
			echo "No Update found or automatic update not enabled"
		fi
		$0 ${default}
		exit 1
esac
