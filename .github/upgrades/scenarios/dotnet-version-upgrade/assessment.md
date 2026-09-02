# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
  - [Binding Redirect Configuration](#binding-redirect-configuration)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [Maui\qr2webmaui.csproj](#mauiqr2webmauicsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 1 | 0 require upgrade |
| Total NuGet Packages | 7 | All compatible |
| Total Code Files | 17 |  |
| Total Code Files with Incidents | 0 |  |
| Total Lines of Code | 1971 |  |
| Total Number of Issues | 0 |  |
| Estimated LOC to modify | 0+ | at least 0.0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [Maui\qr2webmaui.csproj](#mauiqr2webmauicsproj) | net10.0-android;net10.0-windows10.0.26100.1 | ✅ None | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 7 | 100.0% |
| ⚠️ Incompatible | 0 | 0.0% |
| 🔄 Upgrade Recommended | 0 | 0.0% |
| ***Total NuGet Packages*** | ***7*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| banditoth.MAUI.PreferencesExtension | 1.0.2 |  | [qr2webmaui.csproj](#mauiqr2webmauicsproj) | ✅Compatible |
| CommunityToolkit.Maui | 12.1.0 |  | [qr2webmaui.csproj](#mauiqr2webmauicsproj) | ✅Compatible |
| Microsoft.Extensions.Logging.Debug | 9.0.7 |  | [qr2webmaui.csproj](#mauiqr2webmauicsproj) | ✅Compatible |
| Microsoft.Maui.Controls | 9.0.90 |  | [qr2webmaui.csproj](#mauiqr2webmauicsproj) | ✅Compatible |
| Microsoft.Maui.Controls.Compatibility | 9.0.90 |  | [qr2webmaui.csproj](#mauiqr2webmauicsproj) | ✅Compatible |
| ZXing.Net.Maui | 0.4.0 |  | [qr2webmaui.csproj](#mauiqr2webmauicsproj) | ✅Compatible |
| ZXing.Net.Maui.Controls | 0.4.0 |  | [qr2webmaui.csproj](#mauiqr2webmauicsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;qr2webmaui.csproj</b><br/><small>net10.0-android;net10.0-windows10.0.26100.1</small>"]
    click P1 "#mauiqr2webmauicsproj"

```

## Project Details

<a id="mauiqr2webmauicsproj"></a>
### Maui\qr2webmaui.csproj

#### Project Info

- **Current Target Framework:** net10.0-android;net10.0-windows10.0.26100.1✅
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 20
- **Lines of Code**: 1971
- **Estimated LOC to modify**: 0+ (at least 0.0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["qr2webmaui.csproj"]
        MAIN["<b>📦&nbsp;qr2webmaui.csproj</b><br/><small>net10.0-android;net10.0-windows10.0.26100.1</small>"]
        click MAIN "#mauiqr2webmauicsproj"
    end

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

