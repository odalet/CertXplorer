﻿using System.IO;
using Delta.CertXplorer.Configuration;
using Delta.CertXplorer.Diagnostics;
using Delta.CertXplorer.Logging;

namespace Delta.CertXplorer.ApplicationModel
{
    public abstract class BaseApplication
    {
        private const string defaultApplicationSettingsFileName = "app.settings.xml";
        private const string defaultApplicationSettingsDefaultContent =
            @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<settings></settings>";

        private string applicationSettingsFileName = defaultApplicationSettingsFileName;
        private string applicationCulture = string.Empty;

        protected BaseApplication()
        {
            ApplicationCulture = This.Application.Culture.ToString();
            ApplicationSettingsDefaultContent = defaultApplicationSettingsDefaultContent;
        }

        protected string ApplicationCulture
        {
            get => applicationCulture;
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (applicationCulture == value) return;
                applicationCulture = value;
                This.SetApplicationCulture(applicationCulture);
            }
        }

        protected string ApplicationSettingsFileName
        {
            get => BuildPathRootedFileName(applicationSettingsFileName, defaultApplicationSettingsFileName);
            set => applicationSettingsFileName = value;
        }

        protected string ApplicationSettingsDefaultContent { get; set; }

        protected abstract void InitializeThisApplication();

        protected virtual IExceptionHandlerService CreateExceptionHandlerService() => null;

        protected virtual ILogService CreateLogService() => null;

        protected virtual ISettingsService CreateSettingsService() => new SettingsService();

        protected virtual void LoadApplicationSettings(ISettingsService settingsService)
        {
            if (string.IsNullOrEmpty(ApplicationSettingsFileName))
                ApplicationSettingsFileName = defaultApplicationSettingsFileName;

            if (!File.Exists(ApplicationSettingsFileName))
            {
                using var writer = File.CreateText(ApplicationSettingsFileName);
                writer.Write(ApplicationSettingsDefaultContent);
                writer.Close();
            }

            settingsService.SetApplicationSettingsStore(ApplicationSettingsFileName);
        }

        protected virtual void AddOtherServices() { }

        protected virtual T AddService<T>(T service) where T : class
        {
            This.Services.AddService(service);
            This.Logger.Verbose(string.Format("Service {0} was successfully created.", typeof(T)));
            return service;
        }

        protected virtual string BuildPathRootedFileName(string fileName, string defaultFileName)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = Path.Combine(This.Application.RootDirectory, defaultFileName);

            if (!Path.IsPathRooted(fileName))
                fileName = Path.Combine(This.Application.RootDirectory, fileName);

            return fileName;
        }
    }
}
