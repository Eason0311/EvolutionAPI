using Microsoft.AspNetCore.SignalR;

namespace prjEvolutionAPI.Hubs
{
    public class CourseHub : Hub
    {
        // 讓前端呼叫，取得自己的 ConnectionId
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        // 用 ConnectionId 直接發訊息
        public async Task SendCourseStepUpdateToConnection(string connectionId, string step, object data)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveCourseStepUpdate", step, data);
        }

        // 你也可以保留原本的 UserId 發訊息方式
        public async Task SendCourseStepUpdateToUser(string userId, string step, object data)
        {
            await Clients.User(userId).SendAsync("ReceiveCourseStepUpdate", step, data);
        }
    }
}
