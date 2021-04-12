using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ETMS.LOG;
using System.Transactions;
using ETMS.Entity;

namespace ETMS.EventConsumer
{
    /// <summary>
    /// 消费者基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConsumerBase<T> : IEventConsumer<T> where T : ETMS.Event.DataContract.Event
    {
        public virtual string ClassName { get; set; }

        /// <summary>
        /// 执行消息消费的方法
        /// </summary>
        /// <param name="eEvent">当前队列的消息</param>
        public async Task Consume(T eEvent)
        {
            ClassName = eEvent.GetType().Name;
            try
            {
                Console.WriteLine($"【{ClassName}】准备处理消息:{eEvent.Id}");
                Log.Debug($"【{ClassName}】准备处理消息:{eEvent.Id},参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
                await Receive(eEvent);
                Console.WriteLine($"【{ClassName}】处理完成:{eEvent.Id}");
                Log.Debug($"【{ClassName}】处理完成:{eEvent.Id}", this.GetType());
            }
            catch (EtmsFatalException eMsg)
            {
                Console.WriteLine($"【{ClassName}】消息处理失败{eEvent.Id}");
                Log.Fatal($"【{ClassName}】消息处理失败,参数:{JsonConvert.SerializeObject(eEvent)}", eMsg, this.GetType());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"【{ClassName}】消息处理失败{eEvent.Id}");
                Log.Error($"【{ClassName}】消息处理失败,参数:{JsonConvert.SerializeObject(eEvent)}", ex, this.GetType());
            }
        }

        /// <summary>
        /// 消费者接收消息
        /// </summary>
        /// <param name="eEvent">消息体</param>
        protected abstract Task Receive(T eEvent);

        protected virtual TransactionScope GetTransactionScope()
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead });
        }
    }
}
