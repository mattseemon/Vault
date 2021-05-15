<h1 align="center">VAULT</h1>

<p align="center"><img width="200px" height="200px" src="./assets/Vault-512.png" alt="Vault" /></p>

<p align="center">A file system based secrets repository, powered by OpenPGP.<br/></p>

<p align="center"><img src="https://img.shields.io/github/license/mattseemon/vault?style=for-the-badge" alt="License"/></p>

<p align="center"><br/>[ <a href="#getting_started">Getting Started</a> | <a href="#installation">Installation</a> | <a href="#development">Development</a> | <a href="#contributing">Contributing</a> | <a href="#credits">Credits</a> | <a href="#license">License</a> | <a href="#author">Author</a> ]<br/><br/></p>

<a name="about"></a>
For the past two decades, I have used various techniques to manage my confidential, personal information from flat files, encrypted volumes like [TrueCrypt](https://keepass.info/), [BitLocker](https://support.microsoft.com/en-us/windows/turn-on-device-encryption-0c453637-bc88-5f74-5105-741561aae838), to 3rd party tools like, [KeePass](https://keepass.info/), [Boxcryptor](https://www.boxcryptor.com/en/) and [SafeInCloud](https://safe-in-cloud.com/en/). Each have their pros and cons. 

Everybody has different ways they manage this and different needs.

So I decided, I will take a look at what I my requirements were and have listed them below 

 * It should be file system based
 * Files should be encrypted at a file level
 * Files should be plain text, yet structured if needed to be
 * Files should have a version history
 * Use Open Source Tools

With these requirements in mind, I set out to searching for a the right tool and I came across [pass](https://www.passwordstore.org/). **Pass** fit my requirements to the T, with just one issue, it is a linux based tool. 

I have nothing against linux, just that most of my primary systems are Windows & I don't want to install WSL or anything else to get this to work.
Looking at the home page of **pass**, I noticed there are a few windows based tools for the same listed there.

Looking at how **pass** was designed and the simplicity of it, started me down a path to create something of my own.

## Getting Started <a name="getting_started"></a>

To run this application locally, you can use the following guide.

1. Clone this repository 
   ```bash
   $ git clone https://github.com/mattseemon/vault
   ```
2. Open solution `src/Vault.sln` in Visual Studio 2019
3. Press `F5` to run application in debug mode or `Ctrl+F5` to run application normally.

## Installation <a name="installation"></a>

Channel | Release
------- | -------
Stable | [![Stable Release](https://img.shields.io/github/v/release/mattseemon/vault?label=%20&logo=windows&style=for-the-badge)](https://github.com/mattseemon/Vault/releases/latest)
Pre Release | [![Pre Release](https://img.shields.io/github/v/release/mattseemon/vault?include_prereleases&label=%20&logo=windows&style=for-the-badge)](https://github.com/mattseemon/Vault/releases)

 * Future application updates are installed directly from within the App.

## Development <a name="development"></a>

VAULT was developed using the below tools and technologies.
 * [Visual Studio 2019 Community Edition](https://visualstudio.microsoft.com/)
 * [Windows Presentation Foundation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/introduction-to-wpf?view=netframeworkdesktop-4.8)
 * [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
 * [.NET Framework](https://docs.microsoft.com/en-gb/dotnet/)

### Libraries used

 Library | Version 
 ------- | -------
[Newtonsoft.Json](http://james.newtonking.com/projects/json-net.aspx)|13.0.1
[MahApps.Metro)](https://github.com/MahApps/MahApps.Metro)|2.4.5
[MahApps.Metro.IconPacks.Material](https://github.com/MahApps/MahApps.Metro.IconPacks)|4.8.0
[Microsoft.Extensions.Hosting](https://github.com/dotnet/runtime)|5.0.0
[Microsoft.Toolkit.Mvvm](https://github.com/windows-toolkit/WindowsCommunityToolkit)|7.0.2
[Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon)|1.1.0

## Contributing <a name="contributing"></a>

If you want to contribute to the project, check out the wiki article [here](https://github.com/mattseemon/Vault/wiki/Contributing-to-Vault-Project). 

## Credits <a name="credits"></a>

 * Application Icon from the [Google Suits](https://www.iconfinder.com/iconsets/google-suits-1) icon set by [Chamestudio Pvt Ltd](https://www.iconfinder.com/chamedesign).
 * Full list of [3rd Party Notices](3rd-Party-Notices.md)

## License <a name="license"></a>
The source code in this repository is covered under the MIT License listed [here](LICENSE]). Feel free to use this in your own projects with attribution!

> Copyright (c) 2021 Matt Seemon
>  
> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
> 
> The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

## Author <a name="author"></a>

[Matt Seemon](@mattseemon)

<p align="center">Built with <img src="./assets/heart.png" alt="Matt Seemon" /> in Goa, India.</p>
<p align="center"><img src="https://forthebadge.com/images/badges/open-source.svg" alt="Open Source" />&nbsp;
  <img src="https://forthebadge.com/images/badges/you-didnt-ask-for-this.svg" alt="You didn't ask for this" />&nbsp;
  <img src="https://forthebadge.com/images/badges/powered-by-responsibility.svg" alt="Powered By Responsibility"/></p>
