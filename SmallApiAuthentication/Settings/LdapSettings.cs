namespace SmallApiAuthentication.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public class LdapSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; } = 389;

        /// <summary>
        /// 
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SearchBase { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SearchFilter { get; set; }
    }
}
