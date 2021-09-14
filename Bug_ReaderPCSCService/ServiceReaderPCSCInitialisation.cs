using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Xml;

namespace Bug_ReaderPCSCService
{
    public partial class ServiceReaderPCSCInitialisation : ServiceBase
    {
        private static ILog LOG = LogManager.GetLogger(typeof(ServiceReaderPCSCInitialisation));

        private const string LogFolderPath = @"C:\Temp";
        public ServiceReaderPCSCInitialisation()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            initLog4net();
            ReaderPCSCComponent readerComponent = new ReaderPCSCComponent 
            {
                PID = "6112",
                VID = "1C34",
                expectedPCSCFriendlyName = "SpringCard SpringPark Contactless"
            };
            Task.Run(() => readerComponent.Init());
        }

        private void initLog4net()
        {
            if (!Directory.Exists(LogFolderPath))
                Directory.CreateDirectory(LogFolderPath);

            // Configure log4net
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlDocument xmlConfigLog4net = new XmlDocument();
            xmlConfigLog4net.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SpringCardLog.xml");
            XmlElement fileConfig = xmlConfigLog4net.DocumentElement;
            XmlConfigurator.Configure(logRepository, fileConfig);

            if (LOG.IsInfoEnabled)
            {
                LOG.Info("----------------CREATION TEST SPRINGCARD ----------------");
            }
        }
    }
}
