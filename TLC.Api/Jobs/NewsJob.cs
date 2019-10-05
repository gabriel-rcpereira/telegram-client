using Quartz;
using System.Threading.Tasks;
using TLC.Api.Services.Contracts;

namespace TLC.Api.Jobs
{
    public class NewsJob : IJob
    {
        private readonly INewService _newService;

        public NewsJob(INewService newService)
        {
            this._newService = newService;
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            return _newService.Execute();
        }
    }
}
