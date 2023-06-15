// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Viscon.Communication.Ads.Common
{
    /// <summary>
    /// Specifies known ADS errors.
    /// </summary>
    public enum AdsError : uint
    {
        /// <summary>
        /// No error
        /// </summary>
        None = 0,

        /// <summary>
        /// Internal error
        /// </summary>
        InternalError = 1,

        /// <summary>
        /// No Rtime
        /// </summary>
        NoRTime = 2,

        /// <summary>
        /// Allocation locked memory error
        /// </summary>
        AllocationLockedMemory = 3,

        /// <summary>
        /// Insert mailbox error
        /// </summary>
        InsertMailBoxError = 4,

        /// <summary>
        /// Wrong receive HMSG
        /// </summary>
        WrongReceiveHMSG = 5,

        /// <summary>
        /// target port not found
        /// </summary>
        TargetPortNotFound = 6,

        /// <summary>
        /// target machine not found
        /// </summary>
        TargetMachineNotFound = 7,

        /// <summary>
        /// "Unknown command ID
        /// </summary>
        UnknownCommandId = 8,

        /// <summary>
        /// Bad task ID
        /// </summary>
        BadTaskId = 9,

        /// <summary>
        /// No IO
        /// </summary>
        NoIO = 10,

        /// <summary>
        /// Unknown AMS command
        /// </summary>
        UnknownAmsCommand = 11,

        /// <summary>
        /// Win 32 error
        /// </summary>
        Win32Error = 12,

        /// <summary>
        /// Port not connected
        /// </summary>
        PortNotConnected = 13,

        /// <summary>
        /// Invalid AMS length
        /// </summary>
        InvalidAmsLength = 14,

        /// <summary>
        /// Invalid AMS Net ID
        /// </summary>
        InvalidAmsNetId = 15,

        /// <summary>
        /// Low Installation level
        /// </summary>
        LowInstallationLevel = 16,

        /// <summary>
        /// No debug available
        /// </summary>
        NoDebugAvailable = 17,

        /// <summary>
        /// Port disabled
        /// </summary>
        PortDisabled = 18,

        /// <summary>
        /// Port already connected
        /// </summary>
        PortAlreadyConnected = 19,

        /// <summary>
        /// AMS Sync Win32 error
        /// </summary>
        AmsSyncWin32Error = 20,

        /// <summary>
        /// AMS Sync Timeout
        /// </summary>
        AmsSyncTimeout = 21,

        /// <summary>
        /// AMS Sync AMS error
        /// </summary>
        AmsSyncAmsError = 22,

        /// <summary>
        /// AMS Sync no index map
        /// </summary>
        AmsSyncNoIndexMap = 23,

        /// <summary>
        /// Invalid AMS port
        /// </summary>
        InvalidAmsPort = 24,

        /// <summary>
        /// No memory
        /// </summary>
        NoMemory = 25,

        /// <summary>
        /// TCP send error
        /// </summary>
        TcpSendError = 26,

        /// <summary>
        /// Host unreachable
        /// </summary>
        HostUnreachable = 27,

        /// <summary>
        /// error class &lt;device error&gt;
        /// </summary>
        DeviceError = 1792,

        /// <summary>
        /// Service is not supported by server
        /// </summary>
        ServiceNotSupported = 1793,

        /// <summary>
        /// invalid index group
        /// </summary>
        InvalidIndexGroup = 1794,

        /// <summary>
        /// invalid index offset
        /// </summary>
        InvalidIndexOffset = 1795,

        /// <summary>
        /// reading/writing not permitted
        /// </summary>
        ReadWriteNotPermitted = 1796,

        /// <summary>
        /// parameter size not correct
        /// </summary>
        ParameterSizeIncorrect = 1797,

        /// <summary>
        /// invalid parameter value(s)
        /// </summary>
        InvalidParameterValue = 1798,

        /// <summary>
        /// device is not in a ready state
        /// </summary>
        DeviceNotReady = 1799,

        /// <summary>
        /// device is busy
        /// </summary>
        DeviceBusy = 1800,

        /// <summary>
        /// invalid context (must be in Windows)
        /// </summary>
        InvalidContext = 1801,

        /// <summary>
        /// out of memory
        /// </summary>
        OutOfMemory = 1802,

        /// <summary>
        /// invalid parameter value(s)
        /// </summary>
        InvalidParameterValue2 = 1803,

        /// <summary>
        /// not found (files, ...)
        /// </summary>
        NotFound = 1804,

        /// <summary>
        /// syntax error in command or file
        /// </summary>
        SyntaxError = 1805,

        /// <summary>
        /// objects do not match
        /// </summary>
        ObjectsDoNotMatch = 1806,

        /// <summary>
        /// object already exists
        /// </summary>
        ObjectAlreadyExists = 1807,

        /// <summary>
        /// symbol not found
        /// </summary>
        SymbolNotFound = 1808,

        /// <summary>
        /// symbol version invalid
        /// </summary>
        SymbolVersionInvalid = 1809,

        /// <summary>
        /// server is in invalid state
        /// </summary>
        ServerStateInvalid = 1810,

        /// <summary>
        /// AdsTransMode not supported
        /// </summary>
        AdsTransModeNotSupported = 1811,

        /// <summary>
        /// Notification handle is invalid
        /// </summary>
        InvalidNotificationHandle = 1812,

        /// <summary>
        /// Notification client not registered
        /// </summary>
        NotificationClientNotRegistered = 1813,

        /// <summary>
        /// no more notification handles
        /// </summary>
        NoMoreNotificationHandles = 1814,

        /// <summary>
        /// size for watch too big
        /// </summary>
        SizeForWatchTooBig = 1815,

        /// <summary>
        /// device not initialized
        /// </summary>
        DeviceNotInitialized = 1816,

        /// <summary>
        /// device has a timeout
        /// </summary>
        DeviceTimeout = 1817,

        /// <summary>
        /// query interface failed
        /// </summary>
        QueryInterfaceFailed = 1818,

        /// <summary>
        /// wrong interface required
        /// </summary>
        WrongInterfaceRequired = 1819,

        /// <summary>
        /// class ID is invalid
        /// </summary>
        InvalidClassId = 1820,

        /// <summary>
        /// object ID is invalid
        /// </summary>
        InvalidObjectId = 1821,

        /// <summary>
        /// request is pending
        /// </summary>
        RequestIsPending = 1822,

        /// <summary>
        /// request is aborted
        /// </summary>
        RequestIsAborted = 1823,

        /// <summary>
        /// signal warning
        /// </summary>
        SignalWarning = 1824,

        /// <summary>
        /// invalid array index
        /// </summary>
        InvalidArrayIndex = 1825,

        /// <summary>
        /// symbol not active -> release handle and try again
        /// </summary>
        SymbolNotActive = 1826,

        /// <summary>
        /// access denied
        /// </summary>
        AccessDenied = 1827,

        /// <summary>
        /// Error class &lt;client error&gt;
        /// </summary>
        ClientError = 1856,

        /// <summary>
        /// invalid parameter at service
        /// </summary>
        InvalidServiceParameter = 1857,

        /// <summary>
        /// polling list is empty
        /// </summary>
        PollingListEmpty = 1858,

        /// <summary>
        /// var connection already in use
        /// </summary>
        VarConnectionInUse = 1859,

        /// <summary>
        /// invoke ID in use
        /// </summary>
        InvokeIdInUse = 1860,

        /// <summary>
        /// timeout elapsed
        /// </summary>
        TimeoutElapsed = 1861,

        /// <summary>
        /// error in win32 subsystem
        /// </summary>
        Win32SubsystemError = 1862,

        /// <summary>
        /// Invalid client timeout value
        /// </summary>
        InvalidClientTimeout = 1863,

        /// <summary>
        /// ads-port not opened
        /// </summary>
        AdsPortNotOpened = 1864,

        /// <summary>
        /// internal error in ads sync
        /// </summary>
        InternalErrorInAdsSync = 1872,

        /// <summary>
        /// hash table overflow
        /// </summary>
        HashTableOverflow = 1873,

        /// <summary>
        /// key not found in hash
        /// </summary>
        KeyNotFoundInHash = 1874,

        /// <summary>
        /// no more symbols in cache
        /// </summary>
        NoMoreSymbolsInCache = 1875,

        /// <summary>
        /// invalid response received
        /// </summary>
        InvalidResponseReceived = 1876,

        /// <summary>
        /// sync port is locked
        /// </summary>
        SyncPortIsLocked = 1877,
    }
}