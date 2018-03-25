1. From the solution directory, ChaosProject will be your working directory. You may move it wherever you want
	a. Edit PATH.txt in the solution directory with the path to wherever you moved ChaosProject
	b. If you have your own dynamic host name, you can put it in the 2nd line
2. Create a fresh copy of Dark Ages in ChaosProject\ChaosDa (https://s3.amazonaws.com/kru-downloads/da/DarkAges741single.exe)
3. Now we need the linux subsystem for windows to run Redis
	a. Go to "Turn Windows features on or off", and enable "Windows Subsystem for Linux"
	b. Follow through the prompts, this may require a restart
4. Now we need redis on our linux subsystem
	a. Open a bash from cmd by typing "bash" in a Windows Command Prompt
	----pull redis from an up-to-date source via wheezy----
	b. $ echo deb http://packages.dotdeb.org wheezy all >> dotdeb.org.list
	c. $ echo deb-src http://packages.dotdeb.org wheezy all >> dotdeb.org.list
	d. $ sudo mv dotdeb.org.list /etc/apt/sources.list.d
	e. $ wget -q -O - http://www.dotdeb.org/dotdeb.gpg | sudo apt-key add -
	----install redis----
	d. $ sudo apt-get update
	e. $ sudo apt-get install redis-server
5. Now that we have redis, linux, and our working dir, we need to start redis
	a. Run UpdateRedis.bat occasionally to make sure our copy of redis is up to date
	b. Run Redis.bat to start redis, this will open redis in the background with a client windows for you to interact with
6. With redis started, use ChaosTool to add new maps if you want
	a. Certain merchants are located in specific maps, and will be required (included)
	b. CONSTANTS.STARTING_LOCATION is also a required map
7. NEVER CLOSE THE REDIS CLIENT WINDOW WITH THE X BUTTON
8. dump.rdb in your ChaosProject folder is your database data
	a. if you want to start fresh, delete this file and remove merchants from merchants.cs