// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
namespace Viscon.Communication.Ads.Common;

/// <summary>
/// Specifies the reserved ports for ADS routing.
/// </summary>
/// <remarks>
/// This list is imported from ads-client (https://github.com/jisotalo/ads-client), which sourced
/// most values from the TwinCAT.Ads.dll by Beckhoff.
/// </remarks>
public enum AdsReservedPort : ushort
{
    None = 0,
    /// <summary>
    /// AMS Router
    /// </summary>
    Router = 1,
    /// <summary>
    /// AMS Debugger
    /// </summary>
    Debugger = 2,
    /// <summary>
    /// The TCom Server. Dpc or passive level.
    /// </summary>
    R0_TComServer = 10,
    /// <summary>
    /// TCom Server Task. RT context.
    /// </summary>
    R0_TComServerTask = 11,
    /// <summary>
    /// TCom Serve Task. Passive level.
    /// </summary>
    R0_TComServer_PL = 12,
    /// <summary>
    /// TwinCAT Debugger
    /// </summary>
    R0_TcDebugger = 20,
    /// <summary>
    /// TwinCAT Debugger Task
    /// </summary>
    R0_TcDebuggerTask = 21,
    /// <summary>
    /// The License Server
    /// </summary>
    R0_LicenseServer = 30,
    /// <summary>
    /// Logger
    /// </summary>
    Logger = 100,
    /// <summary>
    /// Event Logger
    /// </summary>
    EventLog = 110,
    /// <summary>
    /// application for coupler (EK), gateway (EL), etc.
    /// </summary>
    DeviceApplication = 120,
    /// <summary>
    /// Event Logger UM
    /// </summary>
    EventLog_UM = 130,
    /// <summary>
    /// Event Logger RT
    /// </summary>
    EventLog_RT = 131,
    /// <summary>
    /// Event Logger Publisher
    /// </summary>
    EventLogPublisher = 132,
    /// <summary>
    /// R0 Realtime
    /// </summary>
    R0_Realtime = 200,
    /// <summary>
    /// R0 Trace
    /// </summary>
    R0_Trace = 290,
    /// <summary>
    /// R0 IO
    /// </summary>
    R0_IO = 300,
    /// <summary>
    /// NC (R0)
    /// </summary>
    R0_NC = 500,
    /// <summary>
    /// R0 Satzausführung
    /// </summary>
    R0_NCSAF = 501,
    /// <summary>
    /// R0 Satzvorbereitung
    /// </summary>
    R0_NCSVB = 511,
    /// <summary>
    /// Preconfigured Nc2-Nc3-Instance
    /// </summary>
    R0_NCINSTANCE = 520,
    /// <summary>
    /// R0 ISG
    /// </summary>
    R0_ISG = 550,
    /// <summary>
    /// R0 CNC
    /// </summary>
    R0_CNC = 600,
    /// <summary>
    /// R0 Line
    /// </summary>
    R0_LINE = 700,
    /// <summary>
    /// R0 PLC
    /// </summary>
    R0_PLC = 800,
    /// <summary>
    /// Tc2 PLC RuntimeSystem 1
    /// </summary>
    Tc2_Plc1 = 801,
    /// <summary>
    /// Tc2 PLC RuntimeSystem 2
    /// </summary>
    Tc2_Plc2 = 811,
    /// <summary>
    /// Tc2 PLC RuntimeSystem 3
    /// </summary>
    Tc2_Plc3 = 821,
    /// <summary>
    /// Tc2 PLC RuntimeSystem 4
    /// </summary>
    Tc2_Plc4 = 831,
    /// <summary>
    /// R0 RTS
    /// </summary>
    R0_RTS = 850,
    /// <summary>
    /// Tc3 PLC RuntimeSystem 1
    /// </summary>
    Tc3_Plc1 = 851,
    /// <summary>
    /// Tc3 PLC RuntimeSystem 2
    /// </summary>
    Tc3_Plc2 = 852,
    /// <summary>
    /// Tc3 PLC RuntimeSystem 3
    /// </summary>
    Tc3_Plc3 = 853,
    /// <summary>
    /// Tc3 PLC RuntimeSystem 4
    /// </summary>
    Tc3_Plc4 = 854,
    /// <summary>
    /// Tc3 PLC RuntimeSystem 5
    /// </summary>
    Tc3_Plc5 = 855,
    /// <summary>
    /// Camshaft Controller (R0)
    /// </summary>
    CamshaftController = 900,
    /// <summary>
    /// R0 CAM Tool
    /// </summary>
    R0_CAMTOOL = 950,
    /// <summary>
    /// R0 User
    /// </summary>
    R0_USER = 2000,
    R3_CTRLPROG = 10000,
    /// <summary>
    /// System Service (AMSPORT_R3_SYSSERV)
    /// </summary>
    SystemService = 10000,
    R3_SYSCTRL = 10001,
    R3_SYSSAMPLER = 10100,
    R3_TCPRAWCONN = 10200,
    R3_TCPIPSERVER = 10201,
    R3_SYSMANAGER = 10300,
    R3_SMSSERVER = 10400,
    R3_MODBUSSERVER = 10500,
    R3_AMSLOGGER = 10502,
    R3_XMLDATASERVER = 10600,
    R3_AUTOCONFIG = 10700,
    R3_PLCCONTROL = 10800,
    R3_FTPCLIENT = 10900,
    R3_NCCTRL = 11000,
    R3_NCINTERPRETER = 11500,
    R3_GSTINTERPRETER = 11600,
    R3_STRECKECTRL = 12000,
    R3_CAMCTRL = 13000,
    R3_SCOPE = 14000,
    R3_CONDITIONMON = 14100,
    R3_SINECH1 = 15000,
    R3_CONTROLNET = 16000,
    R3_OPCSERVER = 17000,
    R3_OPCCLIENT = 17500,
    R3_MAILSERVER = 18000,
    R3_EL60XX = 19000,
    R3_MANAGEMENT = 19100,
    R3_MIELEHOME = 19200,
    R3_CPLINK3 = 19300,
    R3_VNSERVICE = 19500,
    /// <summary>
    /// Multiuser
    /// </summary>
    R3_MULTIUSER = 19600,
    /// <summary>
    /// Default (AMS router assigns)
    /// </summary>
    USEDEFAULT = 65535,
}