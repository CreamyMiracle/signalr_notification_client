using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface INotificationsAPI
    {
        [Get("/notifications/{group}")]
        Task<IEnumerable<Payload>> GetNotifications(string group);

        [Post("/notifications/{group}")]
        Task<Payload> PostNotification(string group, Payload msg);
    }
}
