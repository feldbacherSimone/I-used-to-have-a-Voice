namespace _IUTHAV.Core_Programming.Utility {
    public interface ISessionDependable {
        /// <summary>
        /// Interface for Scriptableobjects to gain functions, that enable them on startup
        /// </summary>
        public void Enable();
        public void Reset();

    }
}
