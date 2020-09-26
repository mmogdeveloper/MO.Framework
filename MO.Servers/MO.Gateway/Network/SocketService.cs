using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MO.GrainInterfaces.User;
using Orleans;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MO.GrainInterfaces.Global;
using System.Collections.Generic;
using MO.Algorithm.Redis;
using MO.GrainInterfaces.Game;
using ProtoMessage;

namespace MO.Gateway.Network
{
    public class SocketService: IHostedService
    {
        private readonly IClusterClient _client;
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        
        IEventLoopGroup _bossGroup;
        IEventLoopGroup _workerGroup;
        ServerBootstrap _bootstrap = new ServerBootstrap();
        IChannel _bootstrapChannel = null;

        public SocketService(
            IClusterClient client,
            IConfiguration config,
            ILoggerFactory loggerFactory)
        {
            _client = client;
            _config = config;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<SocketService>();
        }

        //public void Test(IRoom room)
        //{
        //    Task.Run(() =>
        //    {
        //        while (true)
        //        {
        //            room.RoomNotify(new MOMsg() { ActionId = 100 });
        //            Task.Delay(1000).Wait();
        //        }
        //    });
        //}

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //测试
            /*
            List<IGlobalWorld> worldList = new List<IGlobalWorld>();
            for (int i = 0; i < 100; i++)
            {
                worldList.Add(_client.GetGrain<IGlobalWorld>(i));
            }

            for (int i = 0; i < 10000; i++)
            {
                var user = _client.GetGrain<IUser>(i);
                int index = Math.Abs(user.GetHashCode() % worldList.Count);
                await worldList[index].AddUser(user);
            }

            Console.WriteLine("开始");

            await Task.Run(() =>
            {
                foreach (var item in worldList)
                {
                    Task.Run(() =>
                    {
                        item.GlobalNotify(new ProtoMessage.MOStreamMsg() { ActionId = 10000 });
                    });
                }
            });
            */

            //var user1 = _client.GetGrain<IUser>(1);
            //var user2 = _client.GetGrain<IUser>(2);
            //var roomFactory = _client.GetGrain<IRoomFactory>(0);
            //int roomId = await roomFactory.CreateRoom(new ProtoRoomInfo() { });
            //var room = _client.GetGrain<IRoom>(roomId);
            //await room.PlayerEnterRoom(user1);
            //await room.PlayerEnterRoom(user2);
            //Test(room);
            //await Task.Delay(10000);
            //await room.PlayerLeaveRoom(user1);
            //return;

            IPAddress ip_address = IPAddress.Parse(_config.GetSection("GatewayConfig")["socket_ip_address"]);
            int port = int.Parse(_config.GetSection("GatewayConfig")["socket_port"]);
            var dispatcher = new DispatcherEventLoopGroup();
            _bossGroup = dispatcher;
            _workerGroup = new WorkerEventLoopGroup(dispatcher);

            _bootstrap.Group(_bossGroup, _workerGroup);
            _bootstrap.Channel<TcpServerChannel>();

            _bootstrap
                .Option(ChannelOption.SoBacklog, 100)
                //.Handler(new LoggingHandler("SRV-LSTN"))
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    //pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    pipeline.AddLast(new LengthFieldPrepender(
                    ByteOrder.LittleEndian, 2, 0, false));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(
                        ByteOrder.LittleEndian, UInt16.MaxValue, 0, 2, 0, 2, true));
                    pipeline.AddLast(new SocketChannelHandler(_client, _loggerFactory, _config));
                }));

            _bootstrapChannel = await _bootstrap.BindAsync(ip_address, port);
            _logger.LogInformation($"Socket服务启动成功 {ip_address}:{port}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _bootstrapChannel.CloseAsync();
                _logger.LogInformation($"Socket服务关闭成功");
            }
            finally
            {
                await Task.WhenAll(
                    _bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }
    }
}
