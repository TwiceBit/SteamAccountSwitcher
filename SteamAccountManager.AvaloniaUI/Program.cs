﻿using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace SteamAccountManager.AvaloniaUI
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            Dependencies.RegisterDependencies();

            return AppBuilder.Configure<App>()
                           .UsePlatformDetect()
                           .LogToTrace()
                           .UseReactiveUI();
        }
    }
}
