using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace DominatorHouseCore.Diagnostics.Exceptions
{
    class GlobusExceptionHandler
    {
        [DllImport("kernel32.dll")]
        static extern ErrorModes SetErrorMode(ErrorModes uMode);

        [Flags]
        public enum ErrorModes : uint
        {
            //SYSTEM_DEFAULT = 0x0,
            //SEM_FAILCRITICALERRORS = 0x0001,
            //SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            //SEM_NOOPENFILEERRORBOX = 0x8000
        }

        // Call to disable error report dialogs over App
        public static void DisableErrorDialog()
        {
            try
            {
                var dwMode = SetErrorMode(ErrorModes.SEM_NOGPFAULTERRORBOX);
                SetErrorMode(dwMode | ErrorModes.SEM_NOGPFAULTERRORBOX);
            }
            catch { }
        }

        // Call to disable error report dialogs over System
        public static void DisableErrorDialogForSystem()
        {
            //[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting]
            //"ForceQueue"=dword:00000001
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Windows Error Reporting", true);
                key.SetValue("ForceQueue", 1);

                key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\Windows Error Reporting", true);
                key.SetValue("ForceQueue", 1);
            }
            catch { }

            //[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\Consent]
            //"DefaultConsent"=dword:00000001
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Windows Error Reporting\Consent", true);
                if (key != null)
                    key.SetValue("DefaultConsent", 1);

                key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\Windows Error Reporting\Consent", true);
                if (key != null)
                    key.SetValue("DefaultConsent", 1);
            }
            catch
            {
            }

            // [HKLM|HKCU]\Software\Microsoft\Windows\Windows Error Reporting\DontShowUI
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Windows Error Reporting", true);
                key.SetValue("DontShowUI", 1);
            }
            catch { }

            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\Windows Error Reporting", true);
                key.SetValue("DontShowUI", 1);
            }
            catch { }
        }

        public static void SetupGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                try
                {

                    HandleGlobalException(e.ExceptionObject as Exception, o.ToString());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            Application.Current.DispatcherUnhandledException += (o, e) =>
             {
                 e.Exception.DebugLog();
                 e.Handled = true;
             };
            Application.Current.Dispatcher.UnhandledExceptionFilter += (o, e) =>
            {
                e.Exception.DebugLog();
            };
            TaskScheduler.UnobservedTaskException += (o, e) =>
            {
                try
                {
                    e.SetObserved();
                    //HandleGlobalException(e.Exception, o.ToString());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();

                }
            };

            // Exception within jobs
            FluentScheduler.JobManager.JobException += job =>
            {
                try
                {
                    HandleGlobalException(job.Exception, job.Name);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

        }


        /// <summary>
        /// Application will be exit after notifying user on Unhandled exception occurred
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="senderString"></param>
        internal static void HandleGlobalException(Exception exception, string senderString)
        {
            try
            {
                if (exception != null)
                {
                    UIDiagnostic.Fatal(exception, "Unhandled exception has been thrown from {0}", senderString);
                }
                else
                    UIDiagnostic.Fatal("Unhandled exception has been thrown from {0}", senderString);
            }
            catch (Exception ex)
            {
                UIDiagnostic.Fatal(ex, "Unhandled exception has been thrown in HandleGlobalException()");
            }
        }

    }
}
