# CertXplorer

[![Build](https://github.com/odalet/CertXplorer/workflows/Build/badge.svg)](https://github.com/odalet/CertXplorer/actions?query=workflow%3ABuild)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=odalet_CertXplorer&metric=alert_status)](https://sonarcloud.io/dashboard?id=odalet_CertXplorer)
[![Dependabot Status](https://api.dependabot.com/badges/status?host=github&repo=odalet/CertXplorer)](https://dependabot.com)

**CertXplorer** is a GUI allowing to explore the machine's certificates stores as well as decoding **ASN.1/DER** encoded files.

## History

### 3.0.0 - WIP

* Now using Windows-1252 encoding (instead of UTF-8) when decoding **ASN.1 Printable String** objects
* Being less strict with empty **ASN.1** sub-arrays (only emits a warning to the log)
* Column headers in Certificates View now support sorting
* The Certificates View supports filtering of its contents by a string

### 2.4.16 - 2021/02/11

* This is the last version that can be built with Visual Studio 2015 and that targets .NET 4.0

Versions older than 3.0 are described [here](legacy/history.md).

## License

This work is provided under the terms of the [MIT License](LICENSE).
