# Fabio Ottavi's Work #

This folder contains two projects from [**Fabio Ottavi**](http://www.codeproject.com/script/Membership/View.aspx?mid=17858):

* [**BixVReader**](http://www.codeproject.com/Articles/134010/An-UMDF-Driver-for-a-Virtual-Smart-Card-Reader) implements a virtual smartcard reader (I modified it a bit so that it compiles with Visual Studio 2013 to a Windows 8.1 driver).
* [**VirtualSmartCard**](http://www.codeproject.com/Articles/623200/A-Virtual-ISO-SmartCard) is a sample implementation of a software virtual smartcard that can be used through the virtual driver.

## Driver installation ##

Being a driver, **BixVReader** needs the WDK to be built. It can be downloaded from Microsoft: [here](http://msdn.microsoft.com/en-us/windows/hardware/gg454513.aspx). Though, you'll also find here builds that should work in Windows 7, 8 and 8.1, x86 or x64.

If you only intend to install the binaries, then after having downloaded this repository,

* First install `\Delta.Cryptography\VirtualSmartcard\BixVReaderInstall\<bitness>\BixVReader-Package.cer` into your local machine's Root Certificates Authority store (otherwise, Windows security will prevent the driver from installing).
* Open a command prompt with Admin rights
* CD to `\Delta.Cryptography\VirtualSmartcard\BixVReaderInstall\<bitness>`
* Run `install.cmd`
* If anything goes wrong, you can examine the log file it produces in `C:\Windows\Inf\setupapi.dev.log`. 

## License ##

Those two projects are licensed under the [Code Project Open License (CPOL)](http://www.codeproject.com/info/cpol10.aspx).