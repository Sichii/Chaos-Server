start /B cmd /k bash -c "redis-cli config set save \"120 1 60 5 30 10\"" ^& exit
start "Redis Client" cmd /k echo USE "SHUTDOWN" TO CLOSE REDIS SAFELY ^& echo. ^& bash -c redis-cli
bash -c "redis-server --daemonize yes"