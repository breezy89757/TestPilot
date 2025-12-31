# Changelog

All notable changes to the **TestPilot** project will be documented in this file.

## [v0.1.0] - 2026-01-01

### 🚀 Features
- **Core Engine**: Initial release of the TestPilot engine based on Playwright for .NET.
- **AI Vision**: Integrated **GPT-5** (via Azure OpenAI) to analyze test screenshots.
- **UI**: Implemented a dark-themed, responsive dashboard using the modern design system.
- **Playwright**: Support for launching Chromium, Firefox, and WebKit browsers.
- **UX**: Added smart loading states and "async-aware" button behaviors.
- **Resilience**: Added a custom, auto-reconnect modal for better session recovery.

### 🛡️ Security
- configured `appsettings.json` to be git-ignored.
- Added `appsettings.template.json` for secure distribution.

### 🐛 Fixes
- Fixed "DeploymentNotFound" 404 error by correcting the model name in configuration.
- Resolved UI state refresh issues by implementing explicit `StateHasChanged` calls.
