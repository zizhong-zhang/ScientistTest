using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GitHub;
using GitHub.Internals;
using Xunit;

namespace ScientistTest
{
    public class UnitTest1
    {
        private readonly IScientist _scientist = new Scientist(new FireAndForgetResultPublisher(new InMemoryResultPublisher()));
        [Fact]
        public async Task Test1()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await _scientist.ExperimentAsync<int>("assert experiment", 3, experiment =>
            {
                experiment.Use(() => DelaySeconds(2));
                experiment.Try(() => DelaySeconds(3));
                experiment.Try("candidate2", () => DelaySeconds(3));
            });
            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;

            Assert.True(duration < 4000);
        }

        [Fact]
        public void Test2()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _scientist.Experiment<int>("assert experiment", experiment =>
            {
                experiment.Use(() => DelaySeconds(2).ConfigureAwait(false).GetAwaiter().GetResult());
                experiment.Try(() => DelaySeconds(3).ConfigureAwait(false).GetAwaiter().GetResult());
            });
            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;

            Assert.True(duration < 4000);
        }

        private async Task<int> DelaySeconds(int seconds)
        {
            await Task.Run(() => Task.Run(() => Task.Delay(TimeSpan.FromSeconds(seconds))));
            return seconds;
        }
    }
}
