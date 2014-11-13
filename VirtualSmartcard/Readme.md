This folder contains two projects from [**Fabio Ottavi**](http://www.codeproject.com/script/Membership/View.aspx?mid=17858):

* [**BixVReader**](http://www.codeproject.com/Articles/134010/An-UMDF-Driver-for-a-Virtual-Smart-Card-Reader) implements a virtual smartcard reader (I modified it a bit so that it compiles with Visual Studio 2013 to a Windows 8.1 driver).
* [**VirtualSmartCard**](http://www.codeproject.com/Articles/623200/A-Virtual-ISO-SmartCard) is a sample implementation of a software virtual smartcard that can be used through the virtual driver.

Being a driver, **BixVReader** needs the WDK to be built. It can be downloaded from Microsoft: [here](http://msdn.microsoft.com/en-us/windows/hardware/gg454513.aspx). Though, you'll also find here builds for Windows 7, 8 and 8.1, x86 and x64 in release mode.

To install the driver for your platform, refer to the [original article](http://www.codeproject.com/Articles/134010/An-UMDF-Driver-for-a-Virtual-Smart-Card-Reader) that explains how to use the *DevCon* utility (part of the WDK). Not explained in the article is the fact you need to install the **BixVReader-Package.cer** certificate in your root certificates authorities store in order for the driver to install successfully.

Note that if anything goes wrong with *DevCon*, you can examine the log file it produces in **C:\Windows\Inf\setupapi.dev.log**. 