using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyStudyTime.Services;
using MyStudyTime.Database;
using MyStudyTime.MVVM.ViewModel;

namespace MyStudyTime
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize database
            try
            {
                using (var context = new StudyTimeDbContext())
                {
                    context.Database.CreateIfNotExists();
                    System.Diagnostics.Debug.WriteLine("✓ Database initialized successfully");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Database initialization failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"  Exception type: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"  Inner exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}
