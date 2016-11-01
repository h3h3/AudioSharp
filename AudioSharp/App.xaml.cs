// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSharp
{
    using System;
    using System.Windows;

    public partial class App
    {
        static App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            MessageBox.Show(
                args.ExceptionObject.ToString(),
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}