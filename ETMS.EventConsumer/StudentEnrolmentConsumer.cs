using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.IOC;
using ETMS.LOG;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("StudentEnrolmentQueue")]
    public class StudentEnrolmentConsumer : ConsumerBase<StudentEnrolmentEvent>
    {
        private IDistributedLockDAL _distributedLockDAL;

        private IStudentContractsBLL _studentContractsBll;

        private StudentEnrolmentToken _lockKey;

        private IEventPublisher _eventPublisher;

        protected override async Task Receive(StudentEnrolmentEvent eEvent)
        {
            _eventPublisher = CustomServiceLocator.GetInstance<IEventPublisher>();
            _distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            _lockKey = new StudentEnrolmentToken(eEvent.TenantId);
            _studentContractsBll = CustomServiceLocator.GetInstance<IStudentContractsBLL>();
            _studentContractsBll.InitTenantId(eEvent.TenantId);
            await Process(eEvent);
        }

        private async Task Process(StudentEnrolmentEvent eEvent)
        {
            if (_distributedLockDAL.LockTake(_lockKey))
            {
                try
                {
                    await _studentContractsBll.StudentEnrolmentEvent(eEvent);
                }
                finally
                {
                    _distributedLockDAL.LockRelease(_lockKey);
                }
            }
            else
            {
                eEvent.TryCount++;
                Log.Warn($"【StudentEnrolmentConsumer】第{eEvent.TryCount}失败尝试，仍未获得执行锁,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
                if (eEvent.TryCount > 100)
                {
                    Log.Error($"【StudentEnrolmentConsumer】学员报名消费者，尝试了100次仍未获得执行锁,参数:{JsonConvert.SerializeObject(eEvent)}", this.GetType());
                    _distributedLockDAL.LockRelease(_lockKey);
                }
                System.Threading.Thread.Sleep(3000);  //等待三秒再执行
                _eventPublisher.Publish(eEvent);
            }
        }
    }
}
