The Automatron
==============
The Automatron is a block you can use to automate processes in your machine. It can activate blocks or change their settings, on a set schedule, with a single key press.

Configuring an Automatron
=========================
The basic principle is that each Automatron has a number of _actions_ which it activates when it is activated itself.
You can configure an Automatron using the normal block mapper menu. There, you can set a key which activates it and open the configuration window.
In addition, there is an option that allows you change how the Automatron is activated: By default, it activates when the configured key is first pressed down,
if you choose the option "Trigger: Release" instead, it will activate when the configured key is released.
A click on the "Configure" button will open the configuration window. It consists of a list of actions, an "Add Action" button and a "Close" button.
Note that opening the configuration window will also close the block mapper and temporarily deactivate the block mapper tool.
When you close the configuration window using the "Close" button, the block mapper will be reopened and the block mapper tool reactivated.

Adding Actions
--------------
After pressing "Add Action", the Add Action menu will open. It lists the available action types as buttons.
A click on one of them will add an action of the corresponding type and open its configuration screen.

The available types and their options are as follows:

- Change Toggle Value  
  Description: Changes the value of a toggle button of a block when activated.  
  Options:
    - Block: The block whose toggle you want to change. Use the "Select Block" button to select one, as described in the section "Selecting Blocks".
	- Toggle: Which toggle of the block you want to change. There will be one button for each available toggle, the order is the same as in the block mapper.
	- Mode: In Toggle mode, the value of the toggle is always set to the opposite of what it is currently.
	        In Set mode, the the toggle is set to a specified value, regardless of what value it currently has.
	- Change to: Only available in Set mode. What value the toggle should be set to.
	
- Change Slider Value  
  Description: Changes the value of a slider of a block when activated.  
  Options:
    - Block: The block whose slider you want to change. Use the "Select Block" button to select one, as described in the section "Selecting Blocks".
	- Slider: Which slider of the block you want to change. There will be one button for each available slider, the order is the same as in the block mapper.
	- Change to: What value the slider should be set to. The slider itself has the same limits that apply in the normal block mapper but the text field can be used to enter any value.

- Delay  
  Description: Delays activation of the next action by the specified amount.  
  Options:
    - Delay: How many seconds or frames the next activation should be delayed by.
	- Mode: Whether the delay is specified in seconds or in frames.
	
- Press Keys  
  Description: Simulates a key press. Requires the KeySimulator to be installed.  
  Options:
    - Keys: A comma-separated list of keys to press/release. Available special keys (enter without quotes):
	        "alt", "ctrl", "shift", "left arrow", "right arrow", "up arrow", "down arrow", "numpad X" where X is a number from 0 to 9.
	- Mode: In Press mode, a normal key press will be simulated for each of the specified keys, just as if you press and then release a key on your keyboard.
	        In Hold mode, the specified keys will be held down, until they are either pressed on the real keyboard or released again.
			In Release mode, the specified keys will be released again, assuming they were held down.
			
Selecting Blocks
----------------
When you enter the Select Block mode, the configuration screen will be hidden. You can select a block by clicking on it with the right mouse button.
You can exit the Select Block mode without selecting a block by pressing the Escape key.

Modifying Actions
-----------------
You can modify the order in which actions are activated using the Up and Down buttons in the configuration screen.
You can remove actions using the X button in the same screen.
You can reopen the configuration screen of an action by clicking on it.