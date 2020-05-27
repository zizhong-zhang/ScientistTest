using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GitHub;
using GitHub.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScientistTestFramework
{
    [TestClass]
    public class UnitTest1
    {
        private readonly IScientist _scientist = new Scientist(new InMemoryResultPublisher());

        [TestMethod]
        public async Task TestMethod1()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await _scientist.ExperimentAsync<int>("assert experiment", 2, experiment =>
            {
                experiment.Use(() => DelaySeconds(2));
                experiment.Try(() => DelaySeconds(3));
            });
            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;

            Assert.IsTrue((duration < 4000));
        }

        private async Task<int> DelaySeconds(int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds)).ConfigureAwait(false);
            return seconds;
        }
    }
}
