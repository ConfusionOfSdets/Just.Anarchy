using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dockhand.Client;
using Dockhand.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Just.Anarchy.Test.Component.Actions.CpuAnarchy
{
    class WhenCpuAnarchyIsTriggeredDirectly : BaseComponentTest
    {
        private DockerImage _image;
        private HttpClient _client;
        private DockerContainer _container;
        private ContainerStatsObservation _before;
        private ContainerStatsObservation _during;
        private ContainerStatsObservation _after;
        private int _totalCpus;
        private decimal _useCpus = 50;

        public override async Task GivenAsync()
        {
            var client = DockerClient.ForDirectory(@"..\..\..\..");
            _image = await client.BuildImageAsync(@"Just.ContainedAnarchy\Dockerfile", "final", "justcontainedanarchy","test");
            //_image = await client.GetImageAsync("justcontainedanarchy", "test");
            _totalCpus = 1;
            _container = await _image.StartContainerAsync(o => o
                .ExposePort(80, 5001)
                .WithCpuLimit(_totalCpus));
            _client = new HttpClient();
        }

        public override async Task WhenAsync()
        {
            // Wait for the test app initialize
            await Task.Delay(10000);

            // Get baseline memory usage then tell memory anarchy to consume the rest up to a specified level
            _before = await _container.MonitorStatsFor(TimeSpan.FromSeconds(5));
            var cpuToConsume = _useCpus - _before.AverageCpu();
            await _client.PostAsync("http://localhost:5001/anarchy/cpuanarchy", new StringContent($"{{ \"CpuLoadPercentage\":\"{(int)cpuToConsume}\" }}"));

            _before = await _container.MonitorStatsFor(TimeSpan.FromSeconds(10));
            await _client.PostAsync("http://localhost:5001/anarchy/cpuanarchy/trigger?durationSecs=15", null);

            // Wait for the cpu usage to stabilise
            await Task.Delay(5000);

            // Collect stats for 10 seconds
            _during = await _container.MonitorStatsFor(TimeSpan.FromSeconds(10));

            // Wait for cpu to settle again
            await Task.Delay(10000);

            _after = await _container.MonitorStatsFor(TimeSpan.FromSeconds(10));
        }

        [Test]
        public void ItShouldIncreaseCpuLoadWhenTriggered()
        {
            _during.AverageCpu().Should().BeGreaterThan(_before.AverageMem());
        }

        [Test]
        public void ItShouldIncreaseCpuLoadByTheSpecifiedAmount()
        {
            var expectedCpuLoad = 50m;
            _during.AverageCpu().Should().BeApproximately(expectedCpuLoad, 3m);
        }

        [Test]
        public void ItShouldIncreaseCpuLoadForTheSpecifiedTime()
        {
            _after.AverageCpu().Should().BeApproximately(_before.AverageMem(), 2m);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _container.KillAsync();
            await _container.RemoveAsync();
        }
    }
}
