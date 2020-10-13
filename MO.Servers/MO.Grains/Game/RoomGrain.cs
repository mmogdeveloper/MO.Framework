using Google.Protobuf;
using Microsoft.Extensions.Logging;
using MO.Algorithm.OnlineDemo;
using MO.GrainInterfaces;
using MO.GrainInterfaces.Game;
using MO.GrainInterfaces.User;
using MO.Protocol;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Threading.Tasks;

namespace MO.Grains.Game
{
    public class RoomInfo
    {
        public Int32 GameId { get; set; }
    }

    public class RoomGrain : Grain, IRoom
    {
        private readonly IPersistentState<RoomInfo> _roomInfo;
        private readonly Dictionary<long, PlayerData> _players;
        private readonly ILogger _logger;

        private IAsyncStream<MOMsg> _stream;
        private IDisposable _reminder;
        private Queue<CommandInfo> _commands;
        private Int32 _frameCount;

        public RoomGrain(
            [PersistentState("RoomInfo", StorageProviders.DefaultProviderName)] IPersistentState<RoomInfo> roomInfo,
            ILogger<RoomGrain> logger)
        {
            _roomInfo = roomInfo;
            _logger = logger;
            _players = new Dictionary<long, PlayerData>();
            _commands = new Queue<CommandInfo>();
            _frameCount = 0;
        }

        public override async Task OnActivateAsync()
        {
            //自定义加载数据
            await _roomInfo.ReadStateAsync();
            
            //定时器
            _reminder = RegisterTimer(
                OnTimerCallback,
                this,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(100));

            var streamProvider = this.GetStreamProvider(StreamProviders.JobsProvider);
            _stream = streamProvider.GetStream<MOMsg>(Guid.NewGuid(), StreamProviders.Namespaces.ChunkSender);
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            if (_reminder != null)
                _reminder.Dispose();

            //回写数据
            await _roomInfo.WriteStateAsync();
            await base.OnDeactivateAsync();
        }

        private async Task OnTimerCallback(object obj)
        {
            //non-Reentrant 调用
            var grain = this.AsReference<IRoom>();
            await grain.Update();
        }

        private void DoCommand(CommandInfo commad)
        {
            foreach (var player in _players)
            {
                if (player.Key == commad.UserId)
                    continue;

                if (commad.CommandId == (int)CommandType.BigSkill)
                {
                    var distance = Vector3.Distance(_players[commad.UserId].Position, player.Value.Position);
                    if (distance <= DemoValue.BigSkillAttackDistance)
                    {
                        player.Value.CurBlood -= DemoValue.BigSkillAttack;
                        player.Value.BloodChanged = true;
                    }
                }
                else
                {
                    var skillDistance = 0;
                    var skillAttackDistance = 0;
                    var skillAttack = 0;
                    if (commad.CommandId == (int)CommandType.SkillC)
                    {
                        skillDistance = DemoValue.SkillCDistance;
                        skillAttackDistance = DemoValue.SkillCAttackDistance;
                        skillAttack = DemoValue.SkillCAttack;
                    }
                    else if (commad.CommandId == (int)CommandType.SkillX)
                    {
                        skillDistance = DemoValue.SkillXDistance;
                        skillAttackDistance = DemoValue.SkillXAttackDistance;
                        skillAttack = DemoValue.SkillXAttack;
                    }
                    else if (commad.CommandId == (int)CommandType.SkillZ)
                    {
                        skillDistance = DemoValue.SkillZDistance;
                        skillAttackDistance = DemoValue.SkillZAttackDistance;
                        skillAttack = DemoValue.SkillZAttack;
                    }

                    var x = (float)(Math.Cos(Math.PI * (_players[commad.UserId].Rotate.Y / 180)));
                    var z = (float)(Math.Sin(Math.PI * (_players[commad.UserId].Rotate.Y / 180)));

                    var destination = new Vector3(x, 0, z) * skillDistance;
                    var skilldestination = Vector3.Add(_players[commad.UserId].Position, destination);
                    var distance = Vector3.Distance(skilldestination, player.Value.Position);
                    if (distance <= skillAttackDistance)
                    {
                        player.Value.CurBlood -= skillAttack;
                        player.Value.BloodChanged = true;
                    }
                }
            }
        }

        public Task Update()
        {
            _frameCount++;
            MOMsg notify = new MOMsg();
            notify.ActionId = 100010;
            S2C100010 content = new S2C100010();
            content.FrameCount = _frameCount;
            if (_commands.Count != 0)
            {
                List<CommandInfo> commands = new List<CommandInfo>();
                while (_commands.Count != 0)
                {
                    var command = _commands.Dequeue();
                    DoCommand(command);
                    commands.Add(command);
                }
                content.Commands.AddRange(commands);
            }
            BloodInfoList bloodInfos = new BloodInfoList();
            foreach (var player in _players)
            {
                if (player.Value.BloodChanged)
                {
                    bloodInfos.Bloods.Add(new BloodInfo()
                    {
                        UserId = player.Key,
                        BloodValue = player.Value.CurBlood
                    });
                    player.Value.BloodChanged = false;
                }
            }
            content.CommandResult = bloodInfos.ToByteString();
            notify.Content = content.ToByteString();
            RoomNotify(notify);
            return Task.CompletedTask;
        }

        public Task RoomNotify(MOMsg msg)
        {
            return _stream.OnNextAsync(msg);
        }

        public async Task Reconnect(IUser user)
        {
            await user.SubscribeRoom(_stream.Guid);
        }

        public async Task PlayerEnterRoom(IUser user)
        {
            if (!_players.ContainsKey(user.GetPrimaryKeyLong()))
                _players[user.GetPrimaryKeyLong()] = new PlayerData(user);

            await user.SubscribeRoom(_stream.Guid);

            {
                S2C100001 content = new S2C100001();
                content.RoomId = (int)this.GetPrimaryKeyLong();
                foreach (var item in _players)
                {
                    PlayerData player = null;
                    if (_players.TryGetValue(item.Key, out player))
                    {
                        var userPoint = new UserPoint();
                        userPoint.UserId = item.Key;
                        userPoint.UserName = await player.User.GetUserName();
                        userPoint.Vector = new MsgVector3();
                        userPoint.Vector.X = player.Position.X;
                        userPoint.Vector.Y = player.Position.Y;
                        userPoint.Vector.Z = player.Position.Z;
                        userPoint.Rotation = new MsgRotation();
                        userPoint.Rotation.X = player.Rotate.X;
                        userPoint.Rotation.Y = player.Rotate.Y;
                        userPoint.Rotation.Z = player.Rotate.Z;
                        content.UserPoints.Add(userPoint);
                    }
                }
                MOMsg msg = new MOMsg();
                msg.ActionId = 100001;
                msg.Content = content.ToByteString();
                await user.Notify(msg);
            }
            {
                S2C100002 content = new S2C100002();
                content.UserId = user.GetPrimaryKeyLong();
                content.RoomId = (int)this.GetPrimaryKeyLong();
                content.UserName = await user.GetUserName();
                MOMsg msg = new MOMsg();
                msg.ActionId = 100002;
                msg.Content = content.ToByteString();
                await RoomNotify(msg);
            }
        }

        public async Task PlayerLeaveRoom(IUser user)
        {
            S2C100006 content = new S2C100006();
            content.UserId = user.GetPrimaryKeyLong();
            content.RoomId = (int)this.GetPrimaryKeyLong();
            MOMsg msg = new MOMsg();
            msg.ActionId = 100006;
            msg.Content = content.ToByteString();
            await RoomNotify(msg);

            _players.Remove(user.GetPrimaryKeyLong());
            await user.UnsubscribeRoom();
        }

        public Task PlayerReady(IUser user)
        {
            return Task.CompletedTask;
        }

        public Task PlayerGo(IUser user, float x, float y, float z,
            float rx, float ry, float rz)
        {
            if (_players.ContainsKey(user.GetPrimaryKeyLong()))
            {
                _players[user.GetPrimaryKeyLong()].SetLocation(x, y, z, rx, ry, rz);
            }
            return Task.CompletedTask;
        }

        public async Task PlayerSendMsg(IUser user, string msg)
        {
            S2C100008 content = new S2C100008();
            content.UserId = user.GetPrimaryKeyLong();
            content.Content = msg;
            MOMsg notify = new MOMsg();
            notify.ActionId = 100008;
            notify.Content = content.ToByteString();
            await RoomNotify(notify);
        }

        public Task PlayerCommand(IUser user, List<CommandInfo> commands)
        {
            foreach (var command in commands)
            {
                if (command.CommandId == (int)CommandType.Transform)
                {
                    var commandInfo = TransformInfo.Parser.ParseFrom(command.CommandContent);
                    if (_players.ContainsKey(user.GetPrimaryKeyLong()))
                    {
                        _players[user.GetPrimaryKeyLong()].SetLocation(
                            commandInfo.X,
                            commandInfo.Y,
                            commandInfo.Z,
                            commandInfo.RX,
                            commandInfo.RY,
                            commandInfo.RZ);
                    }
                }
            }

            commands.ForEach(m =>
            {
                m.UserId = user.GetPrimaryKeyLong();
                _commands.Enqueue(m);
            });
            return Task.CompletedTask;
        }
    }
}
