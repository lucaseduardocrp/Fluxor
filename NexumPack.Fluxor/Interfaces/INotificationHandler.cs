using System.Threading;
using System.Threading.Tasks;

namespace NexumPack.Fluxor.Interfaces
{
    public interface INotificationHandler<TNotification> where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}
