using Google.Protobuf;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MO.WpfTest.Game
{
    public class MOClient : IDisposable
    {
        private readonly string deviceId = Guid.NewGuid().ToString("N");
        public long UserId { get; private set; }
        private string _token;

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private HttpClient _httpClient;
        private Action<MOMsg> _callback;
        private Timer _timer;

        public MOClient(Action<MOMsg> callback)
        {
            _tcpClient = new TcpClient();
            _httpClient = new HttpClient();
            _callback = callback;
            _timer = new Timer(TimerCallback, null, 1000, 1000);
        }

        public void Dispose()
        {
            _timer.Dispose();
            _tcpClient.Dispose();
            _httpClient.Dispose();
        }

        private void TimerCallback(object sender)
        {
            C2S1 content = new C2S1();
            SendPacket(content);
        }

        public void ConnectGate()
        {
            _tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 9001);
            _networkStream = _tcpClient.GetStream();
            ReceivePacket();


            C2S100000 content = new C2S100000();
            SendPacket(content);
        }

        private async void ReceivePacket()
        {
            while (true)
            {
                try
                {
                    var recBuffer = new byte[2048];
                    var recLen = await _networkStream.ReadAsync(recBuffer);
                    var buffer = new byte[recLen - 2];
                    Array.Copy(recBuffer, 2, buffer, 0, buffer.Length);
                    var msg = MOMsg.Parser.ParseFrom(buffer);
                    if (msg != null)
                    {
                        _callback(msg);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private async void SendPacket(IMessage msgContent)
        {
            if (_tcpClient.Connected)
            {
                var actionId = Int32.Parse(msgContent.GetType().Name.Substring(3));
                MOMsg msg = new MOMsg();
                msg.Token = _token;
                msg.UserId = UserId;
                msg.ActionId = actionId;
                msg.Content = msgContent.ToByteString();
                var content = msg.ToByteArray();
                var buffer = new byte[content.Length + 2];
                var len = BitConverter.GetBytes((short)content.Length);
                Array.Copy(len, buffer, 2);
                Array.Copy(content, 0, buffer, 2, content.Length);
                await _networkStream.WriteAsync(buffer);
            }
        }

        public void Login()
        {
            C2S_1003 content = new C2S_1003();
            content.DeviceId = deviceId;
            content.MobileType = 1;
            var url = $"http://localhost:8001/api/c2s1003?data={content.ToByteString().ToBase64()}";
            var result = _httpClient.GetAsync(url).Result;
            var resultContent = result.Content.ReadAsStringAsync().Result;
            var moResult = MOMsgResult.Parser.ParseFrom(ByteString.FromBase64(resultContent));
            var rep1003 = S2C_1003.Parser.ParseFrom(moResult.Content);
            UserId = rep1003.UserId;
            _token = rep1003.Token;
        }

        public void JoinRoom(int roomId)
        {
            C2S100001 content = new C2S100001();
            content.RoomId = roomId;
            SendPacket(content);
        }

        public void UploadPoint(MOPoint point)
        {
            C2S100003 content = new C2S100003();
            content.Point = point;
            SendPacket(content);
        }

        public void Exit()
        {
            C2S100005 content = new C2S100005();
            SendPacket(content);
        }
    }
}
