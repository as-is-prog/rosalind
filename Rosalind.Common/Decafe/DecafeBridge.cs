using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SocketIOClient;

namespace Shiorose.Decafe
{
    /// <summary>
    /// 0x51decafe サーバーとの Socket.io 通信を管理するブリッジクラス。
    /// バックグラウンドで接続を維持し、受信メッセージを ConcurrentQueue に蓄積する。
    /// </summary>
    public class DecafeBridge : IDisposable
    {
        private SocketIOClient.SocketIO _socket;
        private readonly ConcurrentQueue<DecafeMessage> _messageQueue = new ConcurrentQueue<DecafeMessage>();
        private volatile bool _connected;
        private volatile bool _isThinking;
        private readonly string _serverUrl;
        private readonly string _inhabitantId;
        private bool _disposed;

        /// <summary>
        /// サーバーに接続中かどうか
        /// </summary>
        public bool IsConnected => _connected;

        /// <summary>
        /// 思考中かどうか
        /// </summary>
        public bool IsThinking => _isThinking;

        /// <summary>
        /// DecafeBridge を作成します。
        /// </summary>
        /// <param name="serverUrl">0x51decafe サーバーの URL</param>
        /// <param name="inhabitantId">inhabitant の識別子</param>
        public DecafeBridge(string serverUrl, string inhabitantId)
        {
            _serverUrl = serverUrl;
            _inhabitantId = inhabitantId;
        }

        /// <summary>
        /// Socket.io サーバーに接続し、各イベントハンドラを登録します。
        /// </summary>
        public async Task ConnectAsync()
        {
            _socket = new SocketIOClient.SocketIO(_serverUrl, new SocketIOOptions
            {
                Query = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("page", "talk"),
                    new KeyValuePair<string, string>("inhabitantId", _inhabitantId)
                }
            });

            _socket.OnConnected += (sender, e) =>
            {
                _connected = true;
            };

            _socket.OnDisconnected += (sender, e) =>
            {
                _connected = false;
            };

            _socket.On("speak:message", response =>
            {
                try
                {
                    var data = response.GetValue<JObject>(0);
                    var content = data.Value<string>("content") ?? "";
                    var surface = data.Value<int?>("surface") ?? 0;
                    var timestamp = data.Value<long?>("timestamp") ?? 0;

                    _messageQueue.Enqueue(new DecafeMessage
                    {
                        Type = DecafeMessageType.Speak,
                        Content = content,
                        Surface = surface,
                        Timestamp = timestamp
                    });
                }
                catch { }
            });

            _socket.On("ask:question", response =>
            {
                try
                {
                    var data = response.GetValue<JObject>(0);
                    var questionId = data.Value<string>("id") ?? "";
                    var content = data.Value<string>("content") ?? "";
                    var choicesToken = data["choices"];
                    var choices = choicesToken != null ? choicesToken.ToObject<string[]>() : new string[0];

                    _messageQueue.Enqueue(new DecafeMessage
                    {
                        Type = DecafeMessageType.AskQuestion,
                        QuestionId = questionId,
                        Content = content,
                        Choices = choices
                    });
                }
                catch { }
            });

            _socket.On("talk:chunk", response =>
            {
                if (!_isThinking)
                {
                    _isThinking = true;
                    _messageQueue.Enqueue(new DecafeMessage
                    {
                        Type = DecafeMessageType.ThinkStart
                    });
                }
            });

            _socket.On("talk:done", response =>
            {
                _isThinking = false;
                _messageQueue.Enqueue(new DecafeMessage
                {
                    Type = DecafeMessageType.ThinkEnd
                });
            });

            _socket.On("talk:error", response =>
            {
                var content = "";
                try
                {
                    var data = response.GetValue<JObject>(0);
                    content = data.Value<string>("error") ?? data.Value<string>("message") ?? "";
                }
                catch
                {
                    try { content = response.GetValue<string>(0); } catch { }
                }

                _messageQueue.Enqueue(new DecafeMessage
                {
                    Type = DecafeMessageType.Error,
                    Content = content
                });
            });

            await _socket.ConnectAsync();
        }

        /// <summary>
        /// Socket.io サーバーから切断します。
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_socket != null)
            {
                await _socket.DisconnectAsync();
                _connected = false;
            }
        }

        /// <summary>
        /// キューに未処理のメッセージがあるかどうかを返します。
        /// </summary>
        public bool HasPendingMessages()
        {
            return !_messageQueue.IsEmpty;
        }

        /// <summary>
        /// キューからメッセージを1つ取り出します。
        /// </summary>
        /// <returns>取り出したメッセージ。キューが空の場合は null。</returns>
        public DecafeMessage DequeueMessage()
        {
            if (_messageQueue.TryDequeue(out var msg))
                return msg;
            return null;
        }

        /// <summary>
        /// talk:send イベントを emit します（fire-and-forget）。
        /// </summary>
        /// <param name="text">送信するテキスト</param>
        public void SendTalk(string text)
        {
            if (_socket != null && _connected)
            {
                _ = _socket.EmitAsync("talk:send", text);
            }
        }

        /// <summary>
        /// talk:touch イベントを emit します。
        /// </summary>
        /// <param name="type">タッチの種類（click / move）</param>
        /// <param name="collision">コリジョン名</param>
        public void SendTouch(string type, string collision)
        {
            if (_socket != null && _connected)
            {
                _ = _socket.EmitAsync("talk:touch", new { type, collision });
            }
        }

        /// <summary>
        /// ask:answer イベントを emit します。
        /// </summary>
        /// <param name="id">質問ID</param>
        /// <param name="choice">選択した回答</param>
        public void SendAnswer(string id, string choice)
        {
            if (_socket != null && _connected)
            {
                _ = _socket.EmitAsync("ask:answer", new { id, choice });
            }
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_socket != null)
                {
                    try
                    {
                        _socket.DisconnectAsync().Wait();
                    }
                    catch { }
                    _socket.Dispose();
                    _socket = null;
                }
            }
        }
    }
}
