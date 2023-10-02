_Changes for v 6.5.7_:
- Fixed some inconsistent operations with processes during the update;
- Fixed the missing version control marker for off-line deployment packages;
- App now allows you to change the deployment settings without exiting from mini-launcher;
- Mini-launcher will save the window placement on “hide window” event (previously this parameter may be lost on system reboot if app hasn’t been exited manually);
- ADP has been updated.

_Previous updates:_

- Added the check for running mini-launcher during the DPArray update to avoid deployment failures;
- Added some manual overrides to support ESHQ, ESRM and OS macros executor in mini-launcher;
- `.dpd` files for DPArray now work as settings mode shortcuts (instead of deinstallation mode that is impossible for DPArray);
- Adjusted the interface behavior for DPArray redeployment;
- Some improvements applied to the deployment operator;
- Implemented the startup shortcuts creation on the level of deployment app (DPArray);
- Now you can add / remove shortcut to / from the startup menu during the deployment / updating of the product;
- Startup property in the mini-launcher has been removed; you need to start the DPArray again after update to manage it;
- Deselection of Start menu shortcuts and desktop shortcuts will remove them even if you aren’t removing the product;
- DPArray is now locked in default deployment mode (support for the fast deployment has been disabled due to importance of manual settings for shortcuts);
- Updated the interface of the mini-launcher;
- Disabled the buttons justification during the deployment;
- Added the manual refresh button for the mini-launcher that allows you to actualize the previously obtained result;
- Mini-launcher can now finish all freezing processes with the alias you’re trying to start;
- App is now able to remove shortcuts on app deinstallation;
- Updated the behavior of the “App about” interface: manual package download will be available with all statuses of updates checking; updated the server markup for updates checking method;
- ***App mini-launcher has been implemented***. It can be added to startup. This is you can be notified about available updates (once per OS boot). Also all previously deployed apps can be called from it
