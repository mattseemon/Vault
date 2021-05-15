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

## [0.1.5-alpha]

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
		- WindowManagerService - Service class for accessing open application windows
		- TaskbarIconService = Service class for the creation, management and destruction of Taskbar Icon.
	2. Windows/Pages
		- ProfileWindow - GUI to add or edit profile properties.
	3. ViewModels
		- ProfileViewModel & TaskbarIconViewModel
	5. Helper classes
		- WindowBase - Base class for windows. Shared/common functionality is implemented here. 
		- ViewModelBase - Base class for view models. Shared/common functionality is implemented here.
	6. Additional Extensions, converter and validators were added. 

### Changed
	1. Minor changes were made to all files as part of refactoring and code formatting.
	2. Updated Microsoft Toolkit to 7.0.2
	3. Updated CHANGELOG.md, 3rd-Party-Notices.md, README.md