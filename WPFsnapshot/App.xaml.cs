﻿using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SnapShotHelper;
using WPFsnapshot.factories;
using WPFsnapshot.model;
using WPFsnapshot.services;
using WPFsnapshot.view;
using WPFsnapshot.viewModel;

namespace WPFsnapshot
{

    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
            var mainPage = ServiceProvider.GetRequiredService<MainWindow>();
            mainPage.Show();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            services.AddSingleton<IDBconnection>(provider =>
            {
                // Replace these strings with actual values or retrieve them dynamically
                string projectName = "MyApp";
                string projectFolder = "GuidDbConnectionString";

                return new DBconnection(projectName, projectFolder);
            });

            //UC
            services.AddTransient<TabUC>();

            //VM
            services.AddTransient<TabUCVM>();

            //Factories
            services.AddSingleton<ITabUCVMFactory, TabUCVMFactory>();

            //Services
            services.AddSingleton<SelectedProjectService>();
            services.AddSingleton<UndoRedoManager>();
            services.AddSingleton<UndoRedoService>();

            

        }
        public App()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }

}
