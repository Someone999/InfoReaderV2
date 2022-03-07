namespace InfoReader.Resource;

public interface IResourceManager<out TReader, out TWriter> : IResourceManager where TReader : IResourceContainerReader where TWriter : IResourceContainerWriter
{
    new TReader ResourceContainerReader { get; }
    new TWriter? ResourceContainerWriter { get; }
}

public interface IResourceManager
{
    IResourceContainerReader ResourceContainerReader { get; }
    IResourceContainerWriter? ResourceContainerWriter { get; }
}