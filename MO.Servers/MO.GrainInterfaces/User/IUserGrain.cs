﻿using MO.GrainInterfaces.Network;
using MO.Protocol;
using Orleans;
using System;
using System.Threading.Tasks;

namespace MO.GrainInterfaces.User
{
    public interface IUserGrain : IGrainWithIntegerKey
    {
        Task BindPacketObserver(IPacketObserver observer);
        Task UnbindPacketObserver();

        Task Notify(MOMsg packet);
        Task Kick();

        Task SubscribeGlobal(Guid streamId);
        Task UnsubscribeGlobal();

        Task SubscribeRoom(Guid streamId);
        Task UnsubscribeRoom();

        Task SetNickName(string nickName);
        Task<string> GetNickName();
    }
}
