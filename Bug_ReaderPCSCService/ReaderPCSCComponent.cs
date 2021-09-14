using log4net;
using PCSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace Bug_ReaderPCSCService
{
    public class ReaderPCSCComponent
    {
		private static ILog LOG = LogManager.GetLogger(typeof(ReaderPCSCComponent));
        public string PID { get; set; }
        public string VID { get; set; }
        public string expectedPCSCFriendlyName { get; set; }

		private ReaderPCSCProxy pcscProxy = new ReaderPCSCProxy();

		public void Init()
        {
            string selectedReader;
            string[] readers;
            if (string.IsNullOrEmpty(PID)
                || string.IsNullOrEmpty(VID)
                || string.IsNullOrEmpty(expectedPCSCFriendlyName))
            {
				if (LOG.IsFatalEnabled)
				{
					LOG.Fatal($"Champs PID, VID ou PCSC friendly name non définis");
				}
				return;
            }

			if (!isReaderConnected(PID, VID))
			{
				if (LOG.IsFatalEnabled)
				{
					LOG.Fatal($"Can't found expected USB device ID PID_{PID}/VID_{VID}");
				}
				return;
			}

			/*------------- Get All PCSC connected -------------*/
			try
			{
				readers = pcscProxy.getListOfReaders();
			}
			catch (PCSC.Exceptions.PCSCException e)
			{
				if (LOG.IsFatalEnabled)
				{
					LOG.Fatal("all readers are unavailable : ", e);
				}
				readers = new string[] { };
			}

			if (LOG.IsInfoEnabled)
			{
				LOG.Info($"PCSC readers connected : {string.Join(",", readers)} -- expected reader : {expectedPCSCFriendlyName}");
			}

			if (readers.Length != 0)
			{
				Regex driverReaderRegex = new Regex(expectedPCSCFriendlyName + ".*\\s+[0-9]{1,2}");
				selectedReader = readers.Where(r => driverReaderRegex.IsMatch(r)).FirstOrDefault();
                if (LOG.IsInfoEnabled)
                    LOG.Info($"reader selected : {selectedReader}");
			}
		}

		protected bool isReaderConnected(string PID, string VID)
		{
			bool result = false;
			string PnPDeviceId;

			if (LOG.IsDebugEnabled)
			{
				LOG.Debug("Try to find the USB reader with PID - " + PID + " and VID - " + VID);
			}

            SelectQuery query = new SelectQuery("SELECT Name, PNPDeviceID FROM Win32_PnPEntity");
            ManagementScope scope = new ManagementScope(@"\\" + Environment.MachineName + @"\root\CIMV2");
            ManagementObjectSearcher sercherPnpInstance = new ManagementObjectSearcher( scope, query);

			List<ManagementObject> USBCollection = sercherPnpInstance.Get().Cast<ManagementObject>().ToList();

			if (PID.Length == 4 && VID.Length == 4)
			{
				foreach (ManagementObject usb in USBCollection)
				{
					PnPDeviceId = usb.GetPropertyValue("PNPDeviceID")?.ToString();

					if (PnPDeviceId.Contains("PID_" + PID) && PnPDeviceId.Contains("VID_" + VID))
					{
						result = true;
						if (LOG.IsInfoEnabled)
						{
							LOG.Info("PnP device ID : " + PnPDeviceId);
							LOG.Info("find reader " + usb.GetPropertyValue("Name")?.ToString() + " with PID - " + PID + " and VID - " + VID);
						}
						return result;
					}
				}
			}

			return result;
		}
    }

	public class ReaderPCSCProxy
	{
        /// <summary>Factory PCSC permettant l'analyse de l'environnement PCSC</summary>
        private IContextFactory contextFactory = ContextFactory.Instance;
        /// <summary>Interface de gestion du lecteur PCQSC</summary>
        private ISCardContext scardcontext;
        public ReaderPCSCProxy()
        {
            scardcontext = contextFactory.Establish(SCardScope.System);
        }
        // ********************************************************************************
        /// <summary>
        /// Return all readers connected
        /// </summary>
        /// <returns>list of PCSC readers connected</returns>
        /// <created>LDE,17/07/2020</created>
        /// <changed>LDE,17/07/2020</changed>
        // ********************************************************************************
        public string[] getListOfReaders()
        {
            string[] result = new string[] { };
            result = scardcontext.GetReaders();

            return result;
        }
    }
}
