Pretty much complete, probably need to fix some compatibility issues 
across the different versions of windows (XP, Vista, 7).

Folders
-------
Aries  - Builder
mstsc  - Stub


Only request is to give credit if you choose to use some of this code
- EAX

Aries builder - Coded in C#, .NET Framework 3.5
Aries stub     - Coded in C#, .NET Framework 2.0
[b]Features:[/b]
[list]
[*]Fake error message (Show always, or only in virtual environments)
[*]Anti Virtual (VirtualBox, VMWare, Virtual PC, & Parallels)
[*]Anti Network Sniffers (Wireshark, TCPView, SysAnalyzer)
[*]Anti Sandbox (Sandboxie, Norman Sandbox, Joebox)
[*]Anti Sysinternals (Regmon, Procmon, Filemon, TCPView, etc)
[*]Ability to choose what to do when one of the anti's is found (crash/display error/silent exit)
[*]Steal Firefox Passwords
[*]Steal FileZilla passwords
[*]Custom mutex
[*]File Pumper (Add bytes in KB/MB)
[*]USB Spreader
[*]Use a random nickname on IRC (From http://www.behindthename.com/random/)
[*]Add a fake file signature to fool PEiD
[*]GMail support for emailing logs
[*]Copy the target binder files assembly information (creation dates/time/access time/etc)
[*]Encrypt the binded file with AES-256 for added anonymity
[*]Fixes file size (if the output file is smaller after compressing, pump bytes back into the file to return to the original size)
[*]Icon changing (2 methods, not reshack)
[*]Set custom assembly information
[*]Change your IRC information, so you can re-compile your IRC bot whenever you want
[*]Auto compress your file, or if compression would increase the size (if it's already packed it may increase), then it will skip compression
[*]Protect bots with a login, vhost, or both
[*]Stub is auto compressed to ~30KB
[/list]


[b]IRC Commands:[/b]
[list]
[*]`Release - Completely remove the bot from their computer
[*]`Update - Update the bot to a new version
[*]`WindowsKey - Show windows key/product key
[*]`Drives - Show removable drives connected to the computer (usb drives)
[*]`Spread - Spread to removable drives (ex: usb)
[*]`Info - Show OS version, windows username, ip, bot version
[*]`Version - show bot version
[*]`Download - download a file to their computer & run it
[*]`Speedtest - run a speed test on their computer, returns the speed in KB/s
[*]`Die - Terminate the bot instance, but don't remove it from their computer
[*]`Part - Join another channel on the irc server. 
[*]`Firefox - Email you their Firefox and Filezilla passwords
[*]`SYN - SYN Flood (http://en.wikipedia.org/wiki/SYN_flood)
[*]`HTTP - HTTP Flood
[*]`UDP - UDP Flood (http://en.wikipedia.org/wiki/UDP_flood_attack)
[*]`StopFlood - stop the DOS
[/list]


