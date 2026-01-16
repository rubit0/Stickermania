namespace Scanning
{
    /// <summary>
    /// Represents different states of a scanned sticker.
    /// </summary>
    public enum ScanState
    {
        /// <summary>
        /// The system is scanning.
        /// </summary>
        Active,
        /// <summary>
        /// The system stoped scanning.
        /// </summary>
        Disabled,
        /// <summary>
        /// A new entity has been detected.
        /// </summary>
        Detected,
        /// <summary>
        /// The detected entity is beeing verified (user needs to hold).
        /// </summary>
        Verifying,
        /// <summary>
        /// The detected entity has been confirmed.
        /// </summary>
        Confirmed,
        /// <summary>
        /// Detected entity lost.
        /// </summary>
        Lost
    }
}