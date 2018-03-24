1. From this, ChaosProject will be your working directory. You may move it wherever you want.
2. Create a fresh copy of Dark Ages in a new folder in your working directory. (https://s3.amazonaws.com/kru-downloads/da/DarkAges741single.exe)
3. Now we need the linux subsystem for windows to run Redis.
	a. Go to "Turn Windows features on or off", and enable "Windows Subsystem for Linux".
4. Now we need redis on our linux subsystem.
	a. Open a bash from cmd by typing "bash" in a Windows Command Prompt.
	----pull redis from an up-to-date source via wheezy----
	b. $ echo deb http://packages.dotdeb.org wheezy all >> dotdeb.org.list
	c. $ echo deb-src http://packages.dotdeb.org wheezy all >> dotdeb.org.list
	d. $ sudo mv dotdeb.org.list /etc/apt/sources.list.d
	e. $ wget -q -O - http://www.dotdeb.org/dotdeb.gpg | sudo apt-key add -
	----install redis----
	d. $ sudo apt-get update
	e. $ sudo apt-get install redis-server
	d. redis can now be launched via "$ redis-server --daemonize yes", however, I will include bat files for this----
5. Now that we have redis, linux, and our working dir, copy the bat files out of this repo into the working dir
	a. Run UpdateRedis.bat occasionally to make sure our copy of redis is up to date
	b. Redis.bat to start redis when testing server things.
6. Edit "post.bat" and update directories on line 2 & 3 to your working directory and dark ages directory
7. Copy Chaos.exe.config to working directory
8. Edit Chaos.exe.config
	a. Change BaseDir value to your working directory
	b. Change DarkAgesDir value to your fresh Dark Ages folder path
	c. Change HostName value to your public hostname
		If you can't create your own dynamicDNS, running the server and launcher in debug mode will automatically make everything loopback
	d. Change RedisConfig value to your Redis server, leave default if hosting local.
9. With redis started, use MapTool to add new maps if you want
10. If you want to change the starting map, you will find it under Game > CONSTANTS > STARTING_LOCATION