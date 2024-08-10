using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLocalizationProvider.Handlers
{
    /// <summary>
    /// IMessageHandler
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IMessageHandler<in TValue>
    {
        /// <summary>
        /// HandleAsync
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task HandleAsync(TValue value);
        /// <summary>
        /// HandleException
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="topic"></param>
        /// <param name="value"></param>
        void HandleException(Exception ex, string topic, TValue value = default(TValue));
        
    }
}
