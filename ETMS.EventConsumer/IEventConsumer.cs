using System;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    /// <summary>
    /// 规范消费者消费动作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventConsumer<in T> : IConsumer where T : ETMS.Event.DataContract.Event
    {
        /// <summary>
        /// 消费时执行的方法
        /// </summary>
        /// <param name="eEvent"></param>
        /// <returns></returns>
        Task Consume(T eEvent);
    }
}
