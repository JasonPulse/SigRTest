using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;
using Microsoft.Owin.Hosting; // For Generic Device Support

namespace SigRTest
{
    public class ControlSystem : CrestronControlSystem
    {
        /// <summary>
        /// ControlSystem Constructor. Starting point for the SIMPL#Pro program.
        /// Use the constructor to:
        /// * Initialize the maximum number of threads (max = 400)
        /// * Register devices
        /// * Register event handlers
        /// * Add Console Commands
        /// 
        /// Please be aware that the constructor needs to exit quickly; if it doesn't
        /// exit in time, the SIMPL#Pro program will exit.
        /// 
        /// You cannot send / receive data in the constructor
        /// </summary>
        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(_ControllerEthernetEventHandler);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }
        
        /// <summary>
        /// Gets or sets signal R Web app.
        /// </summary>
        public IDisposable WebAppDisposable { get; set; }

        /// <summary>
        /// InitializeSystem - this method gets called after the constructor 
        /// has finished. 
        /// 
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        /// 
        /// Please be aware that InitializeSystem needs to exit quickly also; 
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {
                StartSigR();
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0} {1}", e.Message, e.StackTrace);
            }
        }

        private void StartSigR()
        {
            // This will not work at all on any system with a router, we get an invoke system exception on the main ip address, but if we enter the subnet ip it will work.
            // there is also another error where the processor does not release the port on program shut down it does not happen very often but a program restart will resolve.
            // Enter the ip addresses here
            this.WebAppDisposable = WebApp.Start<Startup>($"http://10.11.0.51:41780/");
            CrestronConsole.Print($"Attempting to start server please visit http://10.11.0.51:41780/signalr/negotiate to check");
            
            // If working will get something like this if its not working it will give you a cant reach webpage
            /*
             *Url	"/signalr"
                ConnectionToken	"Ql6o+N9cFNlC6Wvo2rFZ4fFPNMunYfjvlXxJaWio6RqKEIesJkhbNyi2UPJ2Hq8w0DSmmFpJeuUyMt4bn84WiNFr/Fe4ixW2vBYjtRpvxmD5xFBNPPdx+tEoxfJOyjiyA0Tr4a83Q4ejz8JjHP3/KLVXXJP6Glia9Xv1pTgcfFfxRE/+ax6M6YlA9y+3c9Wig9ljOOJiVkp+Q350cMkusazWyPsSZfbk1GyYJRLzLOTokTGUd1Q/CnmuiCX1LHMpswOlnbqM82827qKQ73BIC2OpLDdhjnfQBSVLo6GvIavqZymfka5TcbBLjxtnpFL6"
                ConnectionId	"5fac0448-348c-4cf7-bb75-fa1ebee8de89"
                KeepAliveTimeout	20
                DisconnectTimeout	30
                ConnectionTimeout	110
                TryWebSockets	false
                ProtocolVersion	"1.2"
                TransportConnectTimeout	5
                LongPollDelay	0
             */
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        void _ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void _ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void _ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;
                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;
                case (eSystemEventType.Rebooting):
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }

        }
    }
}