# A CSS autoprefixer for ASP.NET Core

[![Build status](https://ci.appveyor.com/api/projects/status/1272n9680fbqcsop?svg=true)](https://ci.appveyor.com/project/madskristensen/weboptimizer-autoprefixer)
[![NuGet](https://img.shields.io/nuget/v/LigerShark.WebOptimizer.AutoPrefixer.svg)](https://nuget.org/packages/LigerShark.WebOptimizer.AutoPrefixer/)

This package compiles TypeScript, ES6 and JSX files into ES5 by hooking into the [LigerShark.WebOptimizer](https://github.com/ligershark/WebOptimizer) pipeline.

## Usage

Automatically add vendor prefixes to all CSS files. To set that up, do this:

```c#
services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyCssFiles()
            .AutoPrefixCss();
});
```

It will detect the user agent of the browser and only serve the venor prefixes that applies to that browser.