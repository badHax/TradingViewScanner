namespace TVScanner.Jobs
{
    public interface ITaskDelayer
    {
        Task Delay(int milliseconds, CancellationToken token = default);
        Task Delay(TimeSpan timespan, CancellationToken token = default);
    }

    public class TaskDelayer : ITaskDelayer
    {
        public async Task Delay(int milliseconds, CancellationToken token = default)
        {
            await Task.Delay(milliseconds, token);
        }

        public async Task Delay(TimeSpan timespan, CancellationToken token = default)
        {
            await Task.Delay(timespan, token);
        }
    }
}
