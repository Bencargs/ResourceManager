using Concurrency;

namespace ResourceManagerTests;

[TestFixture]
public class ResourceManagerTests
{
    private ResourceManager _resourceManager = new();
    private object _resource1 = new();
    private object _resource2 = new();

    [SetUp]
    public void Setup()
    {
        _resource1 = new object();
        _resource2 = new object();
        _resourceManager = new ResourceManager();
    }

    [Test]
    public void TestSingleResourceLock()
    {
        // Test acquiring a single resource
        Assert.IsTrue(_resourceManager.TryLock(_resource1), "Failed to lock resource1.");
    }

    [Test]
    public void TestResourceLockTimeout()
    {
        // Lock resource1 in a separate thread
        Task.Run(() => _resourceManager.TryLock(_resource1));
        Thread.Sleep(100); // Ensure the lock is taken

        // Try to lock resource1 again with a timeout
        Assert.IsFalse(_resourceManager.TryLock(_resource1, 5, 500), "Unexpectedly succeeded in locking resource1.");
    }

    [Test]
    public void TestMultipleResourceLocks()
    {
        // Test acquiring multiple resources sequentially
        Assert.IsTrue(_resourceManager.TryLock(_resource1), "Failed to lock resource1.");
        Assert.IsTrue(_resourceManager.TryLock(_resource2), "Failed to lock resource2.");
    }

    [Test]
    public void TestResourceRelease()
    {
        // Test resource release
        _resourceManager.TryLock(_resource1);
        _resourceManager.Release(_resource1);

        // Try to lock again to confirm release
        Assert.IsTrue(_resourceManager.TryLock(_resource1), "Failed to re-lock resource1 after release.");
    }

    [Test]
    public void TestContentionAndBackoff()
    {
        // Lock a resource in a separate thread
        var task = Task.Run(() => _resourceManager.TryLock(_resource1, 5, 5000));

        Thread.Sleep(100); // Ensure the resource is locked by the task

        // Try to lock the same resource from the test thread, expecting a backoff and eventually giving up
        Assert.IsFalse(_resourceManager.TryLock(_resource1, 2, 1000), "Unexpectedly succeeded in locking resource1 during contention.");

        task.Wait(); // Ensure any background work completes
    }
}