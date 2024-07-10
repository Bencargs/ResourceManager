namespace Concurrency;

public class ResourceManager
{
    private readonly Dictionary<object, object> _resourcesToAcquire = new();
    private readonly List<object> _acquiredResources = new();
    private readonly object _lockObject = new();
    private const int DefaultMaxAttempts = 100;
    private const int DefaultTimeoutMs = 5000;
    
    public bool TryLock(
        object resource, 
        int maxAttempts = DefaultMaxAttempts, 
        int timeout = DefaultTimeoutMs)
    {
        var attempts = 0;
        var backoffExponent = 1;
        var random = new Random();
        timeout += random.Next(100);
        var requiredResources = new Queue<object>(new []{ resource });
        
        while (attempts < maxAttempts)
        {
            if (!requiredResources.TryDequeue(out var resourceRequest))
            {
                // we've already acquired all the resources we need
                return true;
            }

            if (!TryAcquireResource(resourceRequest, timeout))
            {
                // we're unable to acquire a required resource due to deadlock
                // release everything and re-attempt at a random time in future
                ReleaseAllResources();
                requiredResources = new Queue<object>(_resourcesToAcquire.Keys);

                // Calculate backoff and jitter
                int backoffMs = (int)(Math.Pow(2, backoffExponent) * 10);
                int jitterMs = random.Next(10);
                Thread.Sleep(backoffMs + jitterMs);

                backoffExponent++;
                attempts++;
            }
        }

        // Unable to resolve deadlock
        return false;
    }

    public void Release(object resource)
    {
        lock (_lockObject)
        {
            if (_resourcesToAcquire.TryGetValue(resource, out var resourceLock))
            {
                if (Monitor.IsEntered(resourceLock))
                {
                    Monitor.Exit(resourceLock);
                }
                _acquiredResources.Remove(resource);
            }
        }
    }
    
    private bool TryAcquireResource(object resource, int timeout)
    {
        // add this additional request to our total set of required resources
        if (!_resourcesToAcquire.TryGetValue(resource, out var resourceLock))
            _resourcesToAcquire[resource] = resourceLock = new object();
        
        if (Monitor.TryEnter(resourceLock, timeout))
        {
            lock (_lockObject)
            {
                _acquiredResources.Add(resource);
            }
            return true;
        }

        return false;
    }
    
    private void ReleaseAllResources()
    {
        List<object> resourcesToRelease;
        lock (_lockObject)
        {
            resourcesToRelease = new List<object>(_acquiredResources);
        }
        resourcesToRelease.ForEach(Release);
    }
}