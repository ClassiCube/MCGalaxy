MCGalaxy is a fully featured and customisable **ClassiCube Server Software** based on MCForge/MCLawl.

**Setup**
-----------------
Download the latest MCGalaxy release [from here](https://github.com/ClassiCube/MCGalaxy/releases)
* Windows: You need to install .NET framework 4.0. Windows 8/10/11 already have this included.
* macOS:   You need to install the [Mono framework](https://www.mono-project.com).
* Linux:   You need to install the [Mono framework](https://www.mono-project.com). (or just `apt install mono-complete` if on Ubuntu)

Run **MCGalaxy.exe** for a graphical interface, or run **MCGalaxyCLI.exe** for command line only.

Joining your server
-----------------
Run MCGalaxy.exe or MCGalaxyCLI.exe firstly. You'll see something like:
![opt3](https://github.com/user-attachments/assets/e46128bd-6a3f-422a-8076-fbd9d86fa28e)

If you are signed in to classicube.net, you can copy this URL straight into your web browser and start playing.

#### Joining from the ClassiCube client
Click **Direct connect** at the main menu.
![opt1](https://github.com/user-attachments/assets/46ad28c2-ac42-418b-a1c8-88d161503cd5)

Type your username into *Username*, ```127.0.0.1:25565``` into *IP:Port*, and leave *Mppass* blank. Then click **Connect**.
![opt2](https://github.com/user-attachments/assets/8f57d45d-ef2f-4573-95d7-bf0eb22f21af)

#### Make yourself owner
After joining, you will want to rank yourself owner so you can use all commands.

Type ```/rank [your account] owner``` into the bottom text box, then press Enter.

![opt4](https://github.com/user-attachments/assets/7d8fc147-2183-4a96-88cb-47658051eace)


Letting others join your server
-----------------
### LAN players
You need to find out your LAN/local IP address.
*  Windows: Type ```ipconfig``` into **Command Prompt**. Look for ```IPv4 address``` in the output
*  macOS: Type ```ipconfig getifaddr en0``` or ```ipconfig getifaddr en1``` into **Terminal**
*  Linux: Type ```hostname -I``` into **Terminal**. Lan IP is usually the first address in the output

#### Joining from a web browser
Enter the server URL followed by ```?ip=[lan ip]``` into the web browser.<br>
(e.g. http://www.classicube.net/server/play/d1362e7fee1a54365514712d007c8799?ip=192.168.1.30)

#### Joining from the ClassiCube client
* Click **Direct connect** at the main menu
* Type your username into *Username* textbox
* Type ```[lan ip]:25565``` into *IP:Port* textbox (e.g. ```192.168.1.30:25565```)
* Click **Connect**

### Across the internet
You usually need to port forward in your router before other players can join.

#### Joining from a web browser
Enter the server URL into the web browser

#### Joining from the ClassiCube client
* Click **Sign in**
* Type/paste the hash (e.g. ```d1362e7fee1a54365514712d007c8799```) into the *classicube.net/server/play* text box
* Click **Connect**


### Show on classicube.net server list
Click **Settings** in the MCGalaxy window, then tick the **Public** checkbox. Then click **Save**.

This makes your server appear in the server list on classicube.net and in the ClassiCube client.

Compiling - mono and .NET framework
-----------------
**With an IDE:**
* Visual Studio : Open `MCGalaxy.sln`, click `Build` in the menubar, then click `Build Solution`. (Or press F6)
* SharpDevelop: Open `MCGalaxy.sln`, click `Build` in the menubar, then click `Build Solution`. (Or press F8)

**Command line:**
* For Windows: Run `MSBuild command prompt for VS`, then type `msbuild MCGalaxy.sln` into command prompt
* Modern mono: Type `msbuild MCGalaxy.sln` into Terminal
* Older mono: Type `xbuild MCGalaxy.sln` into Terminal

Compiling - .NET 6 / .NET 7 / .NET 8
-----------------

* Compiling for .NET 6: Navigate into `CLI` directory, and then run `dotnet build MCGalaxyCLI_dotnet6.csproj`
* Compiling for .NET 7: Navigate into `CLI` directory, and then run `dotnet build MCGalaxyCLI_dotnet7.csproj`
* Compiling for .NET 8: Navigate into `CLI` directory, and then run `dotnet build MCGalaxyCLI_dotnet8.csproj`

Copyright/License
-----------------
See LICENSE for MCGalaxy license, and license.txt for code used from other software.

Docker support
-----------------
Some **unofficial** dockerfiles for running MCGalaxy in Docker:
* [using Mono](https://github.com/ClassiCube/MCGalaxy/pull/577/files)
* [using .NET core](https://github.com/ClassiCube/MCGalaxy/pull/629/files)

Documentation
-----------------
* [General documentation](https://github.com/ClassiCube/MCGalaxy/wiki)
* [API documentation](https://github.com/ClassiCube/MCGalaxy-API-Documentation)
