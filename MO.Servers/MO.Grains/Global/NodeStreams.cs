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
    public class NodeStreams : Grain, INodeStreams
    {
        private IAsyncStream<MOMsg> _stream;
        public override Task OnActivateAsync()
        {
            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _stream = streamProvider.GetStream<MOMsg>(this.GetPrimaryKey(), StreamProviders.Namespaces.ChunkSender);
            return base.OnActivateAsync();
        }

        public async Task PublishStream(MOMsg msg)
        {
            await _stream.OnNextAsync(msg);
        }
    }
}
