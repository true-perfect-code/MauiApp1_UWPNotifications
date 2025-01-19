using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
//using Microsoft.Windows.AppLifecycle;
using Microsoft.Toolkit.Uwp.Notifications;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MauiApp1_UWPNotifications.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        private NotificationManager notificationManager;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;

            //notificationManager = new NotificationManager();
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            //// need to add this because otherwise setting background activation does nothing.
            //ToastNotificationManagerCompat.OnActivated += (notificationArgs) =>
            //{
            //    // this will run everytime ToastNotification.Activated is called,
            //    // regardless of what toast is clicked and what element is clicked on.
            //    // Works for all types of ToastActivationType so long as the Windows app manifest
            //    // has been updated to support ToastNotifications. 

            //    // you can check your args here, however I'll be doing mine below to keep it cleaner.
            //    // With so many ToastNotifications it would be messy to check all of them here.

            //    //Debug.WriteLine($"A ToastNotification was just activated! Arguments: {notificationArgs.Argument}");

            //    //// using the code below to show a popup from MainPage, saying that the toast itself was clicked.
            //    //if (notificationArgs.Argument.Contains("action=toastClicked"))
            //    //    ShowPopup?.Invoke("The Toast was clicked!");

            //    AppLaunchArguments.LaunchArguments = string.Join(" ", notificationArgs.Argument);
            //};


        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        //protected override void OnActivated(object sender, EventArgs e)
        //{
        //}

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            //notificationManager.Init();

            //var currentInstance = AppInstance.GetCurrent();
            //if (currentInstance != null)
            //{
            //    var activationArgs = currentInstance.GetActivatedEventArgs();
            //    if (activationArgs.Kind == ExtendedActivationKind.AppNotification)
            //    {
            //        var notificationActivatedEventArgs = activationArgs.Data as AppNotificationActivatedEventArgs;
            //        if (notificationActivatedEventArgs != null)
            //        {
            //            notificationManager.ProcessLaunchActivationArgs(notificationActivatedEventArgs);
            //        }
            //    }
            //}
            AppLaunchArguments.LaunchArguments = string.Join(" ", args.Arguments);
            base.OnLaunched(args);
        }

        private void OnProcessExit(object? sender, EventArgs e)
        {
            notificationManager.Unregister();
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            ToastArguments toastArgs = ToastArguments.Parse(e.Argument);
            foreach (var argument in toastArgs)
                AppLaunchArguments.LaunchArguments += argument.Value;
            

            //var nPID = System.Diagnostics.Process.GetCurrentProcess().Id;
            //foreach (var argument in toastArgs)
            //{
            //   // Debug.WriteLine($"Toast argument: {argument}", nameof(OnActivated));

            //    string sString = $"Toast argument: {argument}";
            //    sString += (" PID = " + nPID.ToString());
            //    bool isQueued = this.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
            //    {
            //        Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(sString, "Information");
            //        WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
            //        _ = await md.ShowAsync();
            //    });
            //}
        }

    }

    internal class NotificationManager
    {
        private bool m_isRegistered;
        private Dictionary<int, Action<AppNotificationActivatedEventArgs>> c_map;

        public NotificationManager()
        {
            m_isRegistered = false;

            // When adding new a scenario, be sure to add its notification handler here.
            c_map = new Dictionary<int, Action<AppNotificationActivatedEventArgs>>();
            c_map.Add(9813, NotificationReceived);
        }

        public static void NotificationReceived(AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            // Verarbeite die Benachrichtigungsargumente (String)
            string arguments = notificationActivatedEventArgs.Argument;

            // Zerlege die Argumente (z. B. "action=ToastClick&scenarioTag=9813")
            var keyValuePairs = arguments.Split('&')
                .Select(part => part.Split('='))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0], parts => parts[1]);

            if (keyValuePairs.TryGetValue("action", out string action))
            {
                if (action == "ToastClick")
                {
                    Console.WriteLine("Toast mit Avatar wurde geklickt!");
                }
                else if (action == "OpenApp")
                {
                    Console.WriteLine("Toast hat die App geöffnet!");
                }
            }
        }

        ~NotificationManager()
        {
            Unregister();
        }

        public void Init()
        {
            try
            {
                AppNotificationManager notificationManager = AppNotificationManager.Default;
                notificationManager.NotificationInvoked += OnNotificationInvoked;
                // Registriere für Notifications
                notificationManager.Register();

                Console.WriteLine("NotificationManager erfolgreich initialisiert");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei NotificationManager Init: {ex.Message}");
            }
        }

        private void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            // Speichere die Argumente für spätere Verarbeitung
            AppLaunchArguments.LaunchArguments = string.Join(" ", args.Argument);

            // Rufe die Methode zur Verarbeitung der Benachrichtigungsargumente auf
            ProcessLaunchActivationArgs(args);
        }

        public void Unregister()
        {
            if (m_isRegistered)
            {
                AppNotificationManager.Default.Unregister();
                m_isRegistered = false;
            }
        }

        public void ProcessLaunchActivationArgs(AppNotificationActivatedEventArgs args)
        {
            //DispatchNotification(args);
            AppLaunchArguments.LaunchArguments = string.Join(" ", args.Argument);
        }
    }

}
