# CertXplorer

**CertXplorer** is a GUI allowing to explore the machine's certificates stores as well as decoding **ASN.1/DER** encoded files.

## History

### 3.0.0 - WIP

* Now using Windows-1252 encoding (instead of UTF-8) when decoding **ASN.1 Printable String** objects
* Being less strict with empty **ASN.1** sub-arrays (only emits a warning to the log)

### 2.4.16 - 2021/02/11

* This is the last version that can be built with Visual Studio 2015 and that targets .NET 4.0

Versions older than 3.0 are described [here](legacy/history.md).

## License

This work is provided under the terms of the [MIT License](LICENSE).
