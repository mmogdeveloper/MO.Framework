using MO.GrainInterfaces;
using MO.GrainInterfaces.Global;
using Orleans;
using Orleans.Streams;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MO.Grains.Global
{
    /// <summary>
    /// stream 测试
    /// </summary>
    public class NodeStream : Grain, INodeStream, IAsyncObserver<MOMsg>
    {
        private StreamSubscriptionHandle<MOMsg> _streamHandle;
        private IAsyncStream<MOMsg> _stream;
        private INodeStreams _streams;

        public override async Task OnActivateAsync()
        {
            _streams = GrainFactory.GetGrain<INodeStreams>(0);
            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _stream = streamProvider.GetStream<MOMsg>(_streams.GetPrimaryKey(), StreamProviders.Namespaces.ChunkSender);
            _streamHandle = await _stream.SubscribeAsync(OnNextAsync);
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            if (_streamHandle != null)
                await _streamHandle.UnsubscribeAsync();

            await base.OnDeactivateAsync();
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public Task OnNextAsync(MOMsg item, StreamSequenceToken token = null)
        {
            //Console.WriteLine($"Received stream message:{item.ActionId}");
            return Task.CompletedTask;
        }
    }
}
