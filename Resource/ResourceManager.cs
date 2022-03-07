using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.Tools;

namespace InfoReader.Resource
{
    public class ResourceManager<TReader,TWriter> :IResourceManager<TReader,TWriter> where TReader : IResourceContainerReader where TWriter : IResourceContainerWriter
    {
        private static readonly Dictionary<object, ResourceManager<TReader,TWriter>> ResourceManagers = new Dictionary<object, ResourceManager<TReader, TWriter>>();
        public TReader ResourceContainerReader { get; }
        public TWriter? ResourceContainerWriter { get; }
        IResourceContainerReader IResourceManager.ResourceContainerReader => ResourceContainerReader;
        IResourceContainerWriter? IResourceManager.ResourceContainerWriter => ResourceContainerWriter;
        private ResourceManager(Dictionary<Type,object?[]?> args)
        {
            object?[] readerArgs = Array.Empty<object?>(),writerArgs = Array.Empty<object>();
            if (args.ContainsKey(typeof(TReader)))
            {
                readerArgs = args[typeof(TReader)] ?? Array.Empty<object?>();
            }
            if (args.ContainsKey(typeof(TWriter)))
            {
                writerArgs = args[typeof(TWriter)] ?? Array.Empty<object?>();
            }
            var readerIns = (IResourceContainerReader?)ReflectionTools.CreateInstance(typeof(TReader), readerArgs);
            ResourceContainerReader = (TReader)(readerIns ?? throw new InvalidOperationException());
            ResourceContainerWriter = (TWriter?)(IResourceContainerWriter?)ReflectionTools.CreateInstance(typeof(TWriter),writerArgs);
        }

        public static ResourceManager<TReader,TWriter> GetInstance(object resourceContainerId, Dictionary<Type,object?[]?> args) 
        
        {
            if (!ResourceManagers.ContainsKey(resourceContainerId))
            {
                ResourceManagers.Add(resourceContainerId,new 
                    ResourceManager<TReader,TWriter>(args));
            }
            return ResourceManagers[resourceContainerId];
        }

        ~ResourceManager()
        {
            ResourceContainerReader.Close();
            ResourceContainerWriter?.Close();
        }
    }
}
