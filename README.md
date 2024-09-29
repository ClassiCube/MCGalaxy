
# MCGalaxy: **ClassiCube Server Software** 🖥️

MCGalaxy is a fully featured and customizable **ClassiCube Server Software**, originally based on MCForge/MCLawl.

---

## 🚀 Setup

Download the latest release of MCGalaxy [here](https://www.classicube.net/mcg/download/).

- **Windows**: Ensure that .NET Framework 4.0 is installed (Windows 8/10/11 already include this).
- **macOS**: Install the [Mono framework](https://www.mono-project.com).
- **Linux**: Install the [Mono framework](https://www.mono-project.com) (or use `apt install mono-complete` on Ubuntu).

To start:
- Run **MCGalaxy.exe** for a graphical interface.
- Run **MCGalaxyCLI.exe** for a command-line interface.

---

## 🌐 Joining Your Server

After running `MCGalaxy.exe` or `MCGalaxyCLI.exe`, you will see the following output:

![Server Output](https://github.com/user-attachments/assets/e46128bd-6a3f-422a-8076-fbd9d86fa28e)

If you're logged into classicube.net, you can directly copy the server URL into your browser to join and start playing.

### 🖱️ Joining from ClassiCube Client

1. Click **Direct Connect** from the main menu:
   ![Direct Connect](https://github.com/user-attachments/assets/46ad28c2-ac42-418b-a1c8-88d161503cd5)

2. Enter your username into the *Username* field and `127.0.0.1:25565` into the *IP:Port* field. Leave *Mppass* blank.
   ![Connect](https://github.com/user-attachments/assets/8f57d45d-ef2f-4573-95d7-bf0eb22f21af)

3. Click **Connect**.

### 🛠️ Make Yourself Owner

After joining, you may want to promote yourself to owner to access all server commands.  
Simply type `/rank [your username] owner` into the chat box and press Enter.

![Rank Owner](https://github.com/user-attachments/assets/7d8fc147-2183-4a96-88cb-47658051eace)

---

## 🌍 Letting Others Join Your Server

### 👥 LAN Players

To allow LAN players to join, you'll need to find your local IP address:

- **Windows**: Type `ipconfig` into **Command Prompt** and locate the `IPv4 Address`.
- **macOS**: Use the commands `ipconfig getifaddr en0` or `ipconfig getifaddr en1` in **Terminal**.
- **Linux**: Type `hostname -I` in **Terminal**. The LAN IP is usually the first address in the output.

#### 🌐 Joining from a Web Browser

Instruct others to enter the following URL format into their web browser:  
`http://www.classicube.net/server/play/d1362e7fee1a54365514712d007c8799?ip=[your LAN IP]`.

#### 🎮 Joining from ClassiCube Client

1. Click **Direct Connect**.
2. Enter your username in the *Username* field.
3. Input `[your LAN IP]:25565` into the *IP:Port* field (e.g., `192.168.1.30:25565`).
4. Click **Connect**.

### 🌐 Across the Internet

To allow players from the internet to join, you'll need to set up port forwarding on your router.

#### 🌍 Joining from a Web Browser

Simply share the server URL with others, and they can enter it in their browser.

#### 🎮 Joining from ClassiCube Client

1. Click **Sign In**.
2. Enter or paste the server hash (e.g., `d1362e7fee1a54365514712d007c8799`) into the *classicube.net/server/play* field.
3. Click **Connect**.

### 🌟 Show on Classicube.net Server List

To make your server public and visible on classicube.net, click **Settings** in the MCGalaxy window, tick the **Public** checkbox, and click **Save**.

---

## 🛠️ Compiling MCGalaxy

### Mono and .NET Framework

#### **With an IDE:**

- **Visual Studio**: Open `MCGalaxy.sln`, then click **Build** > **Build Solution** (or press `F6`).
- **SharpDevelop**: Open `MCGalaxy.sln`, then click **Build** > **Build Solution** (or press `F8`).

#### **Command Line:**

- **Windows**: Open `MSBuild command prompt for VS` and type `msbuild MCGalaxy.sln`.
- **Modern Mono**: Run `msbuild MCGalaxy.sln` in Terminal.
- **Older Mono**: Run `xbuild MCGalaxy.sln` in Terminal.

### .NET 6 / 7 / 8

- **.NET 6**: Navigate to the `CLI` directory and run `dotnet build MCGalaxyCLI_dotnet6.csproj`.
- **.NET 7**: Navigate to the `CLI` directory and run `dotnet build MCGalaxyCLI_dotnet7.csproj`.
- **.NET 8**: Navigate to the `CLI` directory and run `dotnet build MCGalaxyCLI_dotnet8.csproj`.

---

## 📄 License

See the [LICENSE](./LICENSE) file for the MCGalaxy license, and `license.txt` for licenses of code used from other software.

---

## 🐳 Docker Support

There are **unofficial** Docker files for running MCGalaxy in Docker:

- [Using Mono](https://github.com/ClassiCube/MCGalaxy/pull/577/files)
- [Using .NET Core](https://github.com/ClassiCube/MCGalaxy/pull/629/files)

---

## 📚 Documentation

- [General Documentation](https://github.com/ClassiCube/MCGalaxy/wiki)
- [API Documentation](https://github.com/ClassiCube/MCGalaxy-API-Documentation)
