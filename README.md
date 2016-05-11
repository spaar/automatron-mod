Automatron for Besiege
======================
The Automatron is a block mod for Besiege, made by me, spaar.
It adds a single new block, the Automatron, which is a block you can use to automate processes in your machine.
It can activate blocks or change their settings, on a set schedule, with a single key press.

Installation
============
The mod requires spaar's Mod Loader and TGYD's BlockLoader to be installed.
An optional, but highly recommended, dependency is an installation of Java. Without this, the key simulation feature will not work.
To install the mod itself, extract the contents of the Automatron folder in the .zip archive into the Mods folder.
This means that the Automatron.dll and KeySimulator.jar files are placed into the Mods folder itself and the Blocks folder from the archive is merged with the Blocks folder in the Mods folder.

If there is an error starting the key simulator because the mod could not find your Java installation, there are two things you can do:
First, try restarting your computer. Especially if you just installed Java, this could help the mod pick up on it.
If that doesn't help, you can manually specify the path to your Java installation. Open the in-game console by pressing Ctrl+K and enter the following command:
```
setConfigValue automatron javawPath string "<path>"
```
Enter the command exactly as written above, just replacing `<path>` by the path *to the javaw or javaw.exe file* that's included in your Java installation.

Usage
=====
In-game usage of the Automatron is described in the [Documentation file](./Documentation.md).

License
=======
The mod is licensed under the MIT license.
Check the LICENSE file for the full license.
