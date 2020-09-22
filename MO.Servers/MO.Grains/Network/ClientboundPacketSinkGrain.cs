using Google.Protobuf;
using MO.GrainInterfaces.Network;
using Orleans;
using ProtoMessage;
using System;
using System.Threading.Tasks;

namespace MO.Grains.Network
{
    internal class ClientboundPacketSinkGrain : Grain, IClientboundPacketSink
    {
        private GrainObserverManager<IClientboundPacketObserver> _subsManager;

        public override Task OnActivateAsync()
        {
            _subsManager = new GrainObserverManager<IClientboundPacketObserver>();
            _subsManager.ExpirationDuration = new TimeSpan(0, 0, 20);
            return base.OnActivateAsync();
        }

        // Clients call this to subscribe.
        public Task Subscribe(IClientboundPacketObserver observer)
        {
            _subsManager.Subscribe(observer);
            return Task.CompletedTask;
        }

        // Also clients use this to unsubscribe themselves to no longer receive the messages.
        public Task UnSubscribe(IClientboundPacketObserver observer)
        {
            _subsManager.Unsubscribe(observer);
            return Task.CompletedTask;
        }

        public Task SendPacket(MOMsg packet)
        {
            if (_subsManager.Count == 0)
                DeactivateOnIdle();
            else
                _subsManager.Notify(n => n.ReceivePacket(packet));

            return Task.CompletedTask;
        }

        public Task Close()
        {
            _subsManager.Notify(n => n.OnClosed());
            _subsManager.Clear();
            DeactivateOnIdle();
            return Task.CompletedTask;
        }
    }
}