namespace SmallApiAuthentication.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// /
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// /
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RoleSystem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RoleSystemAdm { get; set; }
    }
}
