namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// Defines the contract for queue processing components in the SimpleMessageBus system.
    /// </summary>
    /// <remarks>
    /// This interface serves as a marker interface for queue processors, allowing for dependency injection
    /// registration and service discovery. Concrete implementations handle the specifics of reading messages
    /// from different queue providers (Azure, Amazon, FileSystem, etc.) and dispatching them to message handlers.
    /// 
    /// The interface is intentionally empty to allow maximum flexibility in implementation while providing
    /// a common contract for the dependency injection system.
    /// </remarks>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IQueueProcessor
#pragma warning restore CA1040 // Avoid empty interfaces
    {

    }

}