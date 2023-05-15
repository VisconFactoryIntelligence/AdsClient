namespace Viscon.Communication.Ads.Common
{
    public enum AdsTransmissionMode : uint
    {
        None = 0,
        ClientCycle = 1,
        ClientOnChange = 2,

        /// <summary>
        /// Cyclic transmission.
        /// </summary>
        Cyclic = 3,

        /// <summary>
        /// Transmission on value change.
        /// </summary>
        OnChange = 4,
        CyclicInContext = 5,
        OnChangeInContext = 6,
    }
}