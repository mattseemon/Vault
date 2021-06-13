# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.1-alpha] - 2021-05-10

### Added
	1. Solution & Base Projects
		- Vault-Core - Vault Core Library
		- Vault-UX - Vault User Experience
	2. Implemented combined versioning strategy for all projects.
	3. Included Project Assets (Icons, PNG, etc)
	4. 3rd-Party-Notices.md - Full list of all packages used and licenses information 
	5. CHANGELOG.md
	6. Core Service Contracts
		- Contracts for ActivationHandler, ApplicationInfoService, FileService, DataService, NavigationService, PageService, SystemService, ThemeSelectorService, NavigationAware pages, ShellWindow
	7. Core Services
		- FileService - Service class for file operations.
		- SystemService - Service class for OS operations.

### Updated
	1. README.md - Updated with project details.

## [0.1.2-alpha] - 2021-05-11

### Added
	1. Services
		- ApplicationHostService - Service class for core hosting and starting infrastructure for the application.
		- ApplicationInfoService - Service class for application information.
		- DataService - Service class for loading and saving application data. Uses FileService to read and write to files.
		- NavigationService - Service class handling the UI navigation between pages provided by the PageService.
		- PageService - Service class providing the pages used by the navigation service.
		- SettingService - Service class managing the application settings. Uses data services to load and save.
		- ThemeSelectorService - Service class for setting the application theme and accent colors.
	2. Views/Pages
		- WelcomePage - Landing page when the application starts.
		- AboutPage - Displays information about the application.
		- LicensePage - Displays license information about the application.
		- SettingsPage - Page to manage the application settings.
	3. ViewModels for the pages
	4. Models
		- ClipboardSettings, SystemSettings - models for application settings
		- ApplicationTheme - Model for application theme
		- ApplicationConfig - Model for application configuration file. Application configuration is uses to store fixed settings like, setting file location and application urls
	5. Helper Classes like converters, extensions and constants.

## [0.1.5-alpha] - 2021-05-15

### Summary
	1. Added additional services to manage Windows, TaskbarIcon
	2. Moved Application Urls to a separate section in the appSettings.json.
		- Added a Model class to access it.
	3. Added Models for the following settings. 
		- PGP, Git, PasswordGeneration, Security, Update, Profile
	4. Added IWindow, ICloseable, IViewModel, ITaskbarInfoService, IWindowManagerService contracts.
	5. Implemented ThemeChanger, TaskbarIcon, AutoStart, AlwaysOnTop settings logic. 
	6. Did some refactoring and code reformatting

### Added
	1. Services
		- WindowManagerService - Service class for accessing open application windows.
		- TaskbarIconService = Service class for the creation, management and destruction of Taskbar Icon.
	2. Windows/Pages
		- ProfileWindow - GUI to add or edit profile properties.
	3. ViewModels
		- ProfileViewModel & TaskbarIconViewModel.
	5. Helper classes
		- WindowBase - Base class for windows. Shared/common functionality is implemented here. 
		- ViewModelBase - Base class for view models. Shared/common functionality is implemented here.
	6. Additional Extensions, converter and validators were added. 

### Changed
	1. Minor changes were made to all files as part of refactoring and code formatting.
	2. Updated Microsoft Toolkit to 7.0.2
	3. Updated CHANGELOG.md, 3rd-Party-Notices.md, README.md.

## [0.1.16-alpha] - 2021-05-26

### Added
	1. Added the following Services.
		a. HttpService - Service class to manage HttpRequests from within the application.
		b. NotificationService - Service class to display In-App notifications.
		c. UpdateService - Service class used to download and install new application updates. Uses HttpService for all requests to GitHub API to retrieve updates and NotificationService for all user interaction.
		d. CommandLineService - Service class to process command line arguments.
	2. Controls
		a. NotificationControl - control which displays in-app notifications.
			- Included "WPF.Notifications" library code into project, rather than package as the last published package was alerting due to incompatibility to .NET 5.0 Framework.
	3. Models
		- GitHubRelease, GitHubAsset, GitHubVersion - Used by UpdateService.
		- WindowSettings - Used by WindowManagerService.
	4. Windows/Pages
		- ReleaseNotesWindow - GUI to display application update release Notes
	5. Additional Extensions and converters added
		- MarkdownToFlowDocumentConverter - Converts Release Notes markdown to a FlowDocument for display.

### Changed
	1. Updated FileService to include methods for generating file hashes & do file integrity checkes.
	2. Updated WindowManagerService to include methods to save and restore WindowPosition.
	3. Lots of minor tweaks and code clean up.
	4. Updated CHANGELOG.md, 3rd-Party-Notices.md, README.md.

## [0.2.17-alpha] - 2021-06-13

### Summary
	1. Added OpenPGP Support
		a. Implemented a local datastore to store and manage OpenPGP Key Pairs. Uses [LiteDB](https://www.litedb.org/) as the backend.
		b. Implemented a Password management service to
			- provides a consistent way to display GUI's for creating, entering, changing and verifying passwords. 
			- allow for caching and retrieving passwords. 
		c. Added OpenPGP Support with the following capabilities. Uses [Bouncy Castle library](https://www.bouncycastle.org/csharp/index.html).
			- Create a new OpenPGP Key Pair. Support only RSA Encryption at the moment. Support for EC planned.
			- Import an existing OpenPGP Key Pair.
			- Export OpenPGP Key Pairs.
			- Change OpenPGP key pair passphrase.
		d. Implemented an AES based encryption service to encrypt/decrypt data during data store operations.

### Added
	1. Added the following services.
		a. EncryptionService - Service class to provide AES encryption support.
		b. PasswordService - Service class to centrally manage display of Password UX.
		3. PasswordCacheService - Service class which implements a password caching mechanism with expiration.
		4. PGPService - Service class which allows to create and manager OpenPGP public-private key cryptography.
		5. KeyStoreService - Service class which implements the management of PGP Key Store.
	2. Models
		- StoreCredentials, KeyPairInfo, AsciiArmoredKey - Represents used by the KeyStoreService
		- CompressionAlgorithm, EncryptionAlgorithm, EncryptionKeyFlags, EncryptionKeyPair, EncryptionKeyPairUser, EncryptionKeyRing, HashAlgorithm, KeyGenParams, SymmetricKeyAlgorithm - used by the PGPService.
	3. Windows/Pages
		- NewPasswordWindow, ChangePasswordWindow, PasswordWindow - allows users to create, change and enter passwords.
		- KeyStorePage - allows users to manage OpenPGP key pairs.
		- KeyPairPage - allows users to view the properties of a specific OpenPGP key pair.
	4. Additional extensions, converters and other helper classes added.

### Changed
	1. Major restructuring of model classes for better organization 
	2. Modified services to include OpenPGP support.
	3. Updated CHANGELOG.md, 3rd-Party-Notices.md, README.md.
	4. Bumped version to 0.2