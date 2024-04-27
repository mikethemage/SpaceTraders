using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Services;
public class ThrottleService : IThrottleService
{
    private readonly Queue<DateTime> _requestQueue = new Queue<DateTime>();
    private readonly SemaphoreSlim _throttleSemaphore = new SemaphoreSlim(1);

    const double _burstTimeInSeconds = 60.0;
    const int _burstMaxRequests = 30;
    const int _maxRequestsPerSecond = 2;

    public async Task Throttle()
    {
        await _throttleSemaphore.WaitAsync();

        try
        {
            while (_requestQueue.Count >= _burstMaxRequests)
            {
                DateTime oldestRequest = _requestQueue.Peek();
                TimeSpan timeSinceOldest = DateTime.UtcNow - oldestRequest;
                if (timeSinceOldest.TotalSeconds > _burstTimeInSeconds)
                {
                    _requestQueue.Dequeue();
                }
                else
                {
                    await Task.Delay((int)(_burstTimeInSeconds - timeSinceOldest.TotalSeconds) * 1000);
                }
            }

            if (_requestQueue.Count >= _maxRequestsPerSecond)
            {
                DateTime newestRequest = _requestQueue.LastOrDefault();
                if ((DateTime.UtcNow - newestRequest).TotalSeconds < 1)
                {
                    DateTime secondNewestRequest = _requestQueue.ElementAt(_requestQueue.Count - 2);
                    double millisecondsSinceSecondNewest = (DateTime.UtcNow - secondNewestRequest).TotalMilliseconds;
                    if (millisecondsSinceSecondNewest < 1000)
                    {
                        await Task.Delay(1000 - (int)millisecondsSinceSecondNewest);
                    }
                }
            }

            _requestQueue.Enqueue(DateTime.UtcNow);
        }
        finally
        {
            _throttleSemaphore.Release();
        }
    }
}
