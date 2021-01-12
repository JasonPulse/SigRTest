using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SigRTest
{
    [HubName("Testing")]
    public class TestHub : Hub
    {
        public string Test(string prams)
        {
            return prams;
        }
    }
}