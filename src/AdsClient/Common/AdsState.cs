using System;

namespace Ads.Client.Common
{
    public class AdsState
    {
        const byte ADSSTATE_INVALID         = 0;  //ADS Status: invalid
        const byte ADSSTATE_IDLE            = 1;  //ADS Status: idle
        const byte ADSSTATE_RESET           = 2;  //ADS Status: reset.
        const byte ADSSTATE_INIT            = 3;  //ADS Status: init
        const byte ADSSTATE_START           = 4;  //ADS Status: start
        const byte ADSSTATE_RUN             = 5;  //ADS Status: run
        const byte ADSSTATE_STOP            = 6;  //ADS Status: stop
        const byte ADSSTATE_SAVECFG         = 7;  //ADS Status: save configuration
        const byte ADSSTATE_LOADCFG         = 8;  //ADS Status: load configuration
        const byte ADSSTATE_POWERFAILURE    = 9;  //ADS Status: Power failure
        const byte ADSSTATE_POWERGOOD       = 10; //ADS Status: Power good
        const byte ADSSTATE_ERROR           = 11; //ADS Status: Error
        const byte ADSSTATE_SHUTDOWN        = 12; //ADS Status: Shutdown
        const byte ADSSTATE_SUSPEND         = 13; //ADS Status: Suspend
        const byte ADSSTATE_RESUME          = 14; //ADS Status: Resume
        const byte ADSSTATE_CONFIG          = 15; //ADS Status: Configuration
        const byte ADSSTATE_RECONFIG        = 16; //ADS Status: Reconfiguration

        public ushort State { get; set; }
        public ushort DeviceState { get; set; }

        public override string ToString()
        {
            return String.Format("Ads state: {0} ({1}) Device state: {2}", State, AdsStateDescripton, DeviceState);
        }

        public string AdsStateDescripton
        {
            get
            {
                string desc = "";
                switch (State)
                {
                    case ADSSTATE_INVALID: desc = "Invalid"; break;
                    case ADSSTATE_IDLE: desc = "Idle"; break;
                    case ADSSTATE_RESET: desc = "Reset"; break;
                    case ADSSTATE_INIT: desc = "Init"; break;
                    case ADSSTATE_START: desc = "Start"; break;
                    case ADSSTATE_RUN: desc = "Run"; break;
                    case ADSSTATE_STOP: desc = "Stop"; break;
                    case ADSSTATE_SAVECFG: desc = "Save configuration"; break;
                    case ADSSTATE_LOADCFG: desc = "Load configuration"; break;
                    case ADSSTATE_POWERFAILURE: desc = "Power failure"; break;
                    case ADSSTATE_POWERGOOD: desc = "Power good"; break;
                    case ADSSTATE_ERROR: desc = "Error"; break;
                    case ADSSTATE_SHUTDOWN: desc = "Shutdown"; break;
                    case ADSSTATE_SUSPEND: desc = "Suspend"; break;
                    case ADSSTATE_RESUME: desc = "Resume"; break;
                    case ADSSTATE_CONFIG: desc = "Configuration"; break;
                    case ADSSTATE_RECONFIG: desc = "Reconfiguration"; break;
                }
                return desc;
            }
        }

    }
}
