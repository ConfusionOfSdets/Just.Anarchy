using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dockhand.Client;
using Dockhand.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Just.Anarchy.Test.Component.Actions.MemoryAnarchy
{
    class WhenMemoryAnarchyIsTriggeredDirectly : BaseComponentTest
    {
        private DockerImage _image;
        private HttpClient _client;
        private DockerContainer _container;
        private ContainerStatsObservation _before;
        private ContainerStatsObservation _during;
        private ContainerStatsObservation _after;
        private int _totalMemory;
        private decimal _useMemory;

        public override async Task GivenAsync()
        {
            var client = DockerClient.ForDirectory(@"..\..\..\..");
            _image = await client.BuildImageAsync(@"Just.ContainedAnarchy\Dockerfile", "final", "justcontainedanarchy","test");
            //_image = await client.GetImageAsync("justcontainedanarchy", "test");
            _useMemory = 128;
            _totalMemory = 256;
            _container = await _image.StartContainerAsync(o => o
                .ExposePort(80, 5001)
                .WithMemoryLimit(_totalMemory));
            _client = new HttpClient();

            // Wait for the app to initialize
            await Task.Delay(5000);
        }

        public override async Task WhenAsync()
        {
            // Get baseline memory usage then tell memory anarchy to consume the rest up to a specified level
            _before = await _container.MonitorStatsFor(TimeSpan.FromSeconds(5));
            var memoryToConsume = _useMemory - (_before.AverageMem()/100)*_totalMemory;
            await _client.PostAsync("http://localhost:5001/anarchy/memoryanarchy", new StringContent($"{{ \"AmountMb\":\"{(int)memoryToConsume}\" }}"));

            // Trigger memory consumption
            await _client.PostAsync("http://localhost:5001/anarchy/memoryanarchy/trigger?durationSecs=17", null);

            // Wait for the memory usage to build
            await Task.Delay(5000);

            // Collect stats for 10 seconds
            _during = await _container.MonitorStatsFor(TimeSpan.FromSeconds(10));

            // Wait for memory to empty again
            await Task.Delay(5000);

            _after = await _container.MonitorStatsFor(TimeSpan.FromSeconds(10));
        }

        [Test]
        public void ItShouldIncreaseMemoryWhenTriggered()
        {
            _during.AverageMem().Should().BeGreaterThan(_before.AverageMem());
        }

        [Test]
        public void ItShouldIncreaseMemoryUsageByTheSpecifiedAmount()
        {
            var expectedMemoryPercentage = (128m / (decimal) _totalMemory) * 100m;
            _during.AverageMem().Should().BeApproximately(expectedMemoryPercentage, 5m);
        }

        [Test]
        public void ItShouldIncreaseMemoryUsageForTheSpecifiedTime()
        {
            _after.AverageMem().Should().BeApproximately(_before.AverageMem(), 5m);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _container.KillAsync();
            await _container.RemoveAsync();
        }
    }
}
