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
    //public class RoomInfo
    //{
    //    public Int32 GameId { get; set; }
    //}

    public class RoomGrain : Grain, IRoomGrain
    {
        //private readonly IPersistentState<RoomInfo> _roomInfo;
        private readonly Dictionary<long, PlayerData> _players;
        private readonly ILogger _logger;

        private IAsyncStream<MOMsg> _stream;
        private IDisposable _reminder;
        private Queue<CommandInfo> _commands;
        private Int32 _frameCount;

        public RoomGrain(
            //[PersistentState("RoomInfo", StorageProviders.DefaultProviderName)] IPersistentState<RoomInfo> roomInfo,
            ILogger<RoomGrain> logger)
        {
            //_roomInfo = roomInfo;
            _logger = logger;
            _players = new Dictionary<long, PlayerData>();
            _commands = new Queue<CommandInfo>();
            _frameCount = 0;
        }

        public override async Task OnActivateAsync()
        {
            //自定义加载数据
            //await _roomInfo.ReadStateAsync();

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
            //await _roomInfo.WriteStateAsync();
            await base.OnDeactivateAsync();
        }

        private async Task OnTimerCallback(object obj)
        {
            //non-Reentrant 调用
            var grain = this.AsReference<IRoomGrain>();
            await grain.Update();
        }

        private bool IsJumping(PlayerData playerData)
        {
            return playerData.JumpTime < DateTime.Now &&
            playerData.JumpTime.AddSeconds(DemoValue.JumpAnimationTime) > DateTime.Now;
        }

        private float LimitSize(float x)
        {
            if (x > DemoValue.MapSize)
            {
                return DemoValue.MapSize;
            }
            if (x < -DemoValue.MapSize)
            {
                return -DemoValue.MapSize;
            }
            return x;
        }

        private Vector3 LimitSquare(Vector3 position)
        {
            position.X = LimitSize(position.X);
            position.Z = LimitSize(position.Z);
            return position;
        }

        private bool DoCommand(CommandInfo command)
        {
            PlayerData commandPlayer;
            if (!_players.TryGetValue(command.UserId, out commandPlayer))
                return false;

            var skillDistance = 0f;
            var skillAttackDistance = 0f;
            var skillAttack = 0;

            if (command.CommandId == (int)CommandType.Jump)
            {
                if (commandPlayer.JumpTime.AddSeconds(DemoValue.JumpCD) > DateTime.Now)
                    return false;
                commandPlayer.JumpTime = DateTime.Now;

                var x = (float)(Math.Sin(Math.PI * (commandPlayer.Rotate.Y / 180)));
                var z = (float)(Math.Cos(Math.PI * (commandPlayer.Rotate.Y / 180)));
                var jumpVector = new Vector3(x, 0, z) * DemoValue.JumpDistance;
                var destination = Vector3.Add(commandPlayer.Position, jumpVector);
                destination = LimitSquare(destination);
                var position = new MsgVector3();
                position.X = destination.X;
                position.Y = destination.Y;
                position.Z = destination.Z;
                commandPlayer.Position = destination;
                //Console.WriteLine("Jump:{0}-{1}-{2}", position.X, position.Y, position.Z);
                return true;
            }
            else if (command.CommandId == (int)CommandType.Transform)
            {
                var commandInfo = TransformInfo.Parser.ParseFrom(command.CommandContent);
                var position = new Vector3(commandInfo.Position.X, commandInfo.Position.Y, commandInfo.Position.Z);
                var rotate = new Vector3(commandInfo.Rotation.X, commandInfo.Rotation.Y, commandInfo.Rotation.Z);
                var positionDistance = Vector3.Distance(commandPlayer.Position, position);
                var rotateDistance = Vector3.Distance(commandPlayer.Rotate, rotate);

                position = LimitSquare(position);
                //Console.WriteLine("Transform:{0}-{1}-{2}", commandInfo.Position.X, commandInfo.Position.Y, commandInfo.Position.Z);

                if (positionDistance > DemoValue.PositionSpeed / 2)
                    return false;
                if (position.Y != 0)
                    return false;
                commandPlayer.Position = position;
                commandPlayer.Rotate = rotate;
            }
            else if (command.CommandId == (int)CommandType.BigSkill)
            {
                if (commandPlayer.BigSkillTime.AddSeconds(DemoValue.BigSkillAttackCD) > DateTime.Now)
                    return false;
                commandPlayer.BigSkillTime = DateTime.Now;
                skillAttackDistance = DemoValue.BigSkillAttackDistance;
                skillAttack = DemoValue.BigSkillAttack;
            }
            else
            {
                if (command.CommandId == (int)CommandType.SkillC)
                {
                    if (commandPlayer.SkillCTime.AddSeconds(DemoValue.SkillCAttackCD) > DateTime.Now)
                        return false;
                    commandPlayer.SkillCTime = DateTime.Now;
                    skillDistance = DemoValue.SkillCDistance;
                    skillAttackDistance = DemoValue.SkillCAttackDistance;
                    skillAttack = DemoValue.SkillCAttack;
                }
                else if (command.CommandId == (int)CommandType.SkillX)
                {
                    if (commandPlayer.SkillXTime.AddSeconds(DemoValue.SkillXAttackCD) > DateTime.Now)
                        return false;
                    commandPlayer.SkillXTime = DateTime.Now;
                    skillDistance = DemoValue.SkillXDistance;
                    skillAttackDistance = DemoValue.SkillXAttackDistance;
                    skillAttack = DemoValue.SkillXAttack;
                }
                else if (command.CommandId == (int)CommandType.SkillZ)
                {
                    if (commandPlayer.SkillZTime.AddSeconds(DemoValue.SkillZAttackCD) > DateTime.Now)
                        return false;
                    commandPlayer.SkillZTime = DateTime.Now;
                    skillDistance = DemoValue.SkillZDistance;
                    skillAttackDistance = DemoValue.SkillZAttackDistance;
                    skillAttack = DemoValue.SkillZAttack;
                }
            }

            foreach (var player in _players)
            {
                if (player.Key == command.UserId)
                    continue;

                if (IsJumping(player.Value))
                    continue;

                var x = (float)(Math.Sin(Math.PI * (commandPlayer.Rotate.Y / 180)));
                var z = (float)(Math.Cos(Math.PI * (commandPlayer.Rotate.Y / 180)));

                var destination = new Vector3(x, 0, z) * skillDistance;
                var skilldestination = Vector3.Add(commandPlayer.Position, destination);
                var distance = Vector3.Distance(skilldestination, player.Value.Position);
                //Console.WriteLine("{0},{1}", distance, skillAttackDistance);
                if (distance <= skillAttackDistance)
                {
                    player.Value.CurBlood -= skillAttack;
                    player.Value.BloodChanged = true;
                }

                if (player.Value.CurBlood == 0)
                {
                    player.Value.DeadCount++;
                    commandPlayer.KillCount++;
                }
            }
            return true;
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
                    if (DoCommand(command))
                    {
                        if (command.CommandId == (int)CommandType.Transform)
                            continue;
                        commands.Add(command);
                    }
                }
                content.Commands.AddRange(commands);
            }
            StateInfoList stateList = new StateInfoList();
            foreach (var player in _players)
            {
                var transform = new TransformInfo();
                transform.Position = new MsgVector3()
                {
                    X = player.Value.Position.X,
                    Y = player.Value.Position.Y,
                    Z = player.Value.Position.Z
                };
                transform.Rotation = new MsgVector3()
                {
                    X = player.Value.Rotate.X,
                    Y = player.Value.Rotate.Y,
                    Z = player.Value.Rotate.Z
                };
                stateList.StateInfos.Add(new StateInfo()
                {
                    UserId = player.Key,
                    BloodValue = player.Value.CurBlood,
                    KillCount = player.Value.KillCount,
                    DeadCount = player.Value.DeadCount,
                    Transform = transform
                });
            }
            content.CommandResult = stateList.ToByteString();
            notify.Content = content.ToByteString();
            foreach (var player in _players)
            {
                if (player.Value.CurBlood == 0)
                {
                    player.Value.Reset();
                }
            }
            RoomNotify(notify);
            return Task.CompletedTask;
        }

        public Task RoomNotify(MOMsg msg)
        {
            if (_players.Count == 0)
                return Task.CompletedTask;
            return _stream.OnNextAsync(msg);
        }

        public async Task Reconnect(IUserGrain user)
        {
            await user.SubscribeRoom(_stream.Guid);
        }

        public async Task PlayerEnterRoom(IUserGrain user)
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
                        var userPoint = new UserTransform();
                        userPoint.UserId = item.Key;
                        userPoint.UserName = await player.User.GetNickName();
                        userPoint.Position = new MsgVector3();
                        userPoint.Position.X = player.Position.X;
                        userPoint.Position.Y = player.Position.Y;
                        userPoint.Position.Z = player.Position.Z;
                        userPoint.Rotation = new MsgVector3();
                        userPoint.Rotation.X = player.Rotate.X;
                        userPoint.Rotation.Y = player.Rotate.Y;
                        userPoint.Rotation.Z = player.Rotate.Z;
                        content.UserTransforms.Add(userPoint);
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
                content.UserName = await user.GetNickName();
                MOMsg msg = new MOMsg();
                msg.ActionId = 100002;
                msg.Content = content.ToByteString();
                await RoomNotify(msg);
            }
        }

        public async Task PlayerLeaveRoom(IUserGrain user)
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

        public Task PlayerReady(IUserGrain user)
        {
            return Task.CompletedTask;
        }

        public async Task PlayerSendMsg(IUserGrain user, string msg)
        {
            S2C100008 content = new S2C100008();
            content.UserId = user.GetPrimaryKeyLong();
            content.Content = msg;
            MOMsg notify = new MOMsg();
            notify.ActionId = 100008;
            notify.Content = content.ToByteString();
            await RoomNotify(notify);
        }

        public Task PlayerCommand(IUserGrain user, List<CommandInfo> commands)
        {
            PlayerData commandPlayer;
            if (!_players.TryGetValue(user.GetPrimaryKeyLong(), out commandPlayer))
                return Task.CompletedTask;

            if (IsJumping(commandPlayer))
                return Task.CompletedTask;

            commands.ForEach(m =>
            {
                m.UserId = user.GetPrimaryKeyLong();
                _commands.Enqueue(m);
            });
            return Task.CompletedTask;
        }
    }
}
