using log4net;
using System;
using System.Configuration;
using HttpClient;
using System.ServiceProcess;
using System.Threading;

namespace WindowsService
{
    class WindowsService : ServiceBase
    {
        private static readonly ILog _log = LogManager.GetLogger("LOGGER");
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Thread _serviceThread;
        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsService()
        {
            this.ServiceName = "IPSender";
            this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            _log.Debug("Main");
            ServiceBase.Run(new WindowsService());
        }

        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether
        ///    or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// OnStart(): Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _log.Debug("OnStart begin");
            _serviceThread = new Thread(WorkerThreadFunc);
            _serviceThread.Name = "Service worker thread";
            _serviceThread.IsBackground = true;
            _serviceThread.Start();
            _log.Debug("OnStart end");
        }

        public void WorkerThreadFunc()
        {
            _log.Debug("WorkerThreadFunc begin");
            while (!_shutdownEvent.WaitOne(0))
            {
                string ipAddress = "empty";
                string hostName = "empty";
                try
                {
                    string timeIntervalMs = ConfigurationManager.AppSettings["TimeIntervalMs"];
                    Thread.Sleep(Int32.Parse(timeIntervalMs));
                    ipAddress = IPSender.Resolver.GetIP();
                    hostName = IPSender.Resolver.GetHostName();
                    NetworkParameters networkParameters = new NetworkParameters
                    {
                        IP = ipAddress,
                        HostName = hostName,
                    };
                    HttpClient.HttpClient.RunAsync(networkParameters).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    _log.Error(e.Message);
                }
                _log.Debug(ipAddress + " " + hostName);
            }
            _log.Debug("WorkerThreadFunc end");
        }

        /// <summary>
        /// OnStop(): Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();
            _log.Debug("OnStop begin");
            _shutdownEvent.Set();
            if (!_serviceThread.Join(10000))
            { // give the thread 10 seconds to stop
                _serviceThread.Abort();
                _log.Debug("OnStop Thread Abort");
            }
            _log.Debug("OnStop end");
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            _log.Debug("Pause");
        }

        /// <summary>
        /// OnContinue(): Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();
            _log.Debug("OnContinue");
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcast Status
        /// (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event
        ///   from a Terminal Server session.
        ///   Useful if you need to determine
        ///   when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription">The Session Change
        /// Event that occured.</param>
        protected override void OnSessionChange(
                  SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }
    }
}