using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace DaprCounter.Controllers;

[ApiController]
[Route("[controller]")]
public class CounterController : ControllerBase
{
    private readonly string storeName = "statestore";
    private readonly string key = "counter";

    private readonly string pubsubName = "pubsub";
    private readonly string topicName = "counter";

    private readonly DaprClient _daprClient;
    private readonly ILogger<CounterController> _logger;

    public CounterController(
        DaprClient daprClient,
        ILogger<CounterController> logger) => (_daprClient, _logger) = (daprClient, logger);

    [HttpGet]
    public async Task<StateEntry<int>> GetStateCounterAsync(CancellationToken cancellationToken = default)
    {
        var entry = await _daprClient.GetStateEntryAsync<int>(storeName, key, cancellationToken: cancellationToken);
        return entry;
    }

    [HttpPost]
    public async Task<(bool, StateEntry<int>)> PostStateCounterAsync(CancellationToken cancellationToken = default)
    {
        var counter = await _daprClient.GetStateEntryAsync<int>(storeName, key, cancellationToken: cancellationToken);
        counter.Value += 1;
        var saved = await counter.TrySaveAsync();
        return (saved, counter);
    }

    [HttpGet("state")]
    public StateEntry<int> GetCounter([FromState("statestore", "counter")] StateEntry<int> counter)
    {
        return counter;
    }

    [HttpPost("state")]
    public async Task<(bool, StateEntry<int>)> PostCounterAsync([FromState("statestore", "counter")] StateEntry<int> counter)
    {
        counter.Value += 1;
        var saved = await counter.TrySaveAsync();
        return (saved, counter);
    }

    [Topic("pubsub", "counter")]
    [HttpGet("sub")]
    public void SubCounter(int counter)
    {
        _logger.LogInformation("Consuming: {}", counter);
    }

    [HttpPost("pub/{counter}")]
    public void PubCounter(int counter, CancellationToken cancellationToken = default)
    {
        _daprClient.PublishEventAsync<int>(pubsubName, topicName, counter, cancellationToken: cancellationToken);
    }
}