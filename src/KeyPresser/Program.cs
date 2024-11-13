using Microsoft.Extensions.Configuration;

namespace KeyPresser
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();
            var appConfiguration = configuration.GetSection("AppConfiguration").Get<AppConfiguration>();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1(appConfiguration));
        }
    }
}