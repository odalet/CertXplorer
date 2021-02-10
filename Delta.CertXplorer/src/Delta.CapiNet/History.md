Version 1.2.6 WIP
---------------------------
* Now using Windows-1252 encoding (instead of UTF-8) when decoding ASN.1 Printable String objects

Version 1.2.5 (2015/11/25)
--------------------------
* BUGFIX: Prevent ASN.1 Decoder from throwing when invalid tags are created with no data.

Version 1.2.4 (2015/08/10)
--------------------------
* Added first support for Card Verifiable documents (such as Certificates used in EAC)

Version 1.2.3 (2014/11/18)
--------------------------
* Modified the ASN.1 Decoding feature (so that it can be extended)
* Modified the internal logging sub-system (to be more alike Log4Net API)

Version 1.2.2 (2014/07/09)
--------------------------
* Added support for PGP ASCII-armored files (PEM-like) to the PEM decoder

Version 1.2.1 (2014/01/12)
--------------------------
* Added Adler32 checksum computation (in Delta.CapiNet.Cryptography.Adler32)
* **Issue #16** - BUGFIX: PEM decoder could not handle lines starting with 'Comment:'.
  Now it handles **[RFC4716](http://www.ietf.org/rfc/rfc4716.txt)** compliant files.

Version 1.2.0 (2014/01/11)
--------------------------
* Added Luhn checksum computation and validation (in Delta.CapiNet.Cryptography.Luhn)

Version 1.1.2 (2014/01/05)
--------------------------
* No code change! Thus, no version change.
* Reorganized code layout (in new **Delta.Cryptography** GitHub repository)
* Added credit to OpenSSL in credits.md

Version 1.1.2 (2013/10/05)
--------------------------
* **Issue #8** - Enhancement: Added support for Certificate Trust Lists

Version 1.1.1 (2013/09/21)
--------------------------
* Added a few properties to the Certificate class + renamed X509Certificate3 property to X509.
* Added X509Extensions.GetCertificates method.
* Refactoring of UI class (extension methods + parameter reordering + added ShowCertificateDialog 
  methods taking a Certificate object as first parameter (note that this is a breaking change in the API).

Version 1.1.0 (2013/09/01)
--------------------------
* Added a PEM Decoder.

Version 1.0.3
-------------------------
* BUGFIX: Fixed handles release code (CertContextHandle and CrlContextHandle)
* BUGFIX: Prevented CertStoreHandle created from X509Store objects to release their handle (it is owned by the X509Store object).

Version 1.0.2
-------------------------
Now the assembly (and root namespace) is named **Delta.CapiNet** and is part of the **Delta** git repo.

Version 1.0.1
-------------------------
Some cleanup part of open-sourcing **CryptExplorer** (and renaming it **CertXplorer**).

Version 1.0.0
-------------------------
At this time **CapiNet** was part of a personal project called **CryptExplorer**.

-----------------------------------------------------------------------------------------
* License: [Ms-RL][msrl]
* History page: [Here][history]
* Credits page: [Here][credits]

  [msrl]: License.md "MS-RL License"
  [history]: History.md "History"
  [credits]: Credits.md "Credits"
