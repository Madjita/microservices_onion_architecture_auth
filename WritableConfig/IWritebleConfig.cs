using Microsoft.Extensions.Options;

namespace WritableConfig
{
    public interface IWritableConfig<T> : IOptions<T> where T : class, new()
    {
        /// <summary>
        /// Get object from configuration
        /// </summary>
        /// <returns></returns>
        T GetConfigObject();
        
        /// <summary>
        /// Set configuration state as object
        /// </summary>
        /// <param name="config"></param>
        void SetConfig(T config);

        /// <summary>
        /// Method to notify all Handlers about changes in configuration
        /// </summary>
        void NotifyAllHandlers();

        /// <summary>
        /// Method to add EventHandler to notify after changes
        /// </summary>
        /// <param name="eventHandler"> EventHandler to notify </param>
        void AddHandlerToNotification(EventHandler eventHandler);

        string GetSection();
    }
}
