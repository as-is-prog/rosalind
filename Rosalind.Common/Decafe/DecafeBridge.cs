using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Common;

namespace Shiorose.Decafe
{
    /// <summary>
    /// 0x51decafe サーバーとの Socket.io 通信を管理するブリッジクラス。
    /// バックグラウンドで接続を維持し、受信メッセージを ConcurrentQueue に蓄積する。
    /// </summary>
    public class DecafeBridge : IDisposable
    {
        private SocketIO _socket;
        private readonly ConcurrentQueue<DecafeMessage> _messageQueue = new ConcurrentQueue<DecafeMessage>();
        private volatile bool _connected;
        private volatile bool _isThinking;
        private readonly string _serverUrl;
        private readonly string _inhabitantId;
        private bool _disposed;
        private string _lastError;

        /// <summary>
        /// サーバーに接続中かどうか
        /// </summary>
        public bool IsConnected => _connected;

        /// <summary>
        /// 思考中かどうか
        /// </summary>
        public bool IsThinking => _isThinking;

        /// <summary>
        /// 最後に発生したエラーメッセージ（デバッグ用）
        /// </summary>
        public string LastError => _lastError;

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
            var query = new NameValueCollection
            {
                { "page", "talk" },
                { "inhabitantId", _inhabitantId }
            };

            _socket = new SocketIO(new Uri(_serverUrl), new SocketIOOptions
            {
                Query = query,
                Transport = TransportProtocol.WebSocket,
                Reconnection = true,
                ReconnectionAttempts = 5
            });

            _socket.OnConnected += (sender, e) =>
            {
                _connected = true;
            };

            _socket.OnDisconnected += (sender, e) =>
            {
                _connected = false;
            };

            _socket.OnError += (sender, e) =>
            {
                _lastError = e;
                _messageQueue.Enqueue(new DecafeMessage
                {
                    Type = DecafeMessageType.Error,
                    Content = $"Socket.IO error: {e}"
                });
            };

            _socket.OnReconnectAttempt += (sender, e) =>
            {
                _lastError = $"Reconnecting... attempt {e}";
            };

            _socket.OnReconnectError += (sender, e) =>
            {
                _lastError = $"Reconnect error: {e}";
                _messageQueue.Enqueue(new DecafeMessage
                {
                    Type = DecafeMessageType.Error,
                    Content = $"Reconnect error: {e}"
                });
            };

            _socket.On("speak:message", async response =>
            {
                try
                {
                    var data = response.GetValue<SpeakMessagePayload>(0);
                    _messageQueue.Enqueue(new DecafeMessage
                    {
                        Type = DecafeMessageType.Speak,
                        Content = data?.content ?? "",
                        Surface = data?.surface ?? 0,
                        Timestamp = data?.timestamp ?? 0
                    });
                }
                catch { }

                await Task.CompletedTask;
            });

            _socket.On("ask:question", async response =>
            {
                try
                {
                    var data = response.GetValue<AskQuestionPayload>(0);
                    _messageQueue.Enqueue(new DecafeMessage
                    {
                        Type = DecafeMessageType.AskQuestion,
                        QuestionId = data?.id ?? "",
                        Content = data?.content ?? "",
                        Choices = data?.choices ?? new string[0]
                    });
                }
                catch { }

                await Task.CompletedTask;
            });

            _socket.On("talk:chunk", async response =>
            {
                if (!_isThinking)
                {
                    _isThinking = true;
                    _messageQueue.Enqueue(new DecafeMessage
                    {
                        Type = DecafeMessageType.ThinkStart
                    });
                }

                await Task.CompletedTask;
            });

            _socket.On("talk:done", async response =>
            {
                _isThinking = false;
                _messageQueue.Enqueue(new DecafeMessage
                {
                    Type = DecafeMessageType.ThinkEnd
                });

                await Task.CompletedTask;
            });

            _socket.On("talk:error", async response =>
            {
                var content = "";
                try
                {
                    var data = response.GetValue<TalkErrorPayload>(0);
                    content = data?.error ?? "";
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

                await Task.CompletedTask;
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
                _ = _socket.EmitAsync("talk:send", new object[] { new { text } });
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
                _ = _socket.EmitAsync("talk:touch", new object[] { new { type, collision } });
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
                _ = _socket.EmitAsync("ask:answer", new object[] { new { id, choice } });
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

    // Socket.IO ペイロード用 POCO（System.Text.Json でデシリアライズ）
    internal class SpeakMessagePayload
    {
        public string content { get; set; }
        public int surface { get; set; }
        public long timestamp { get; set; }
    }

    internal class AskQuestionPayload
    {
        public string id { get; set; }
        public string content { get; set; }
        public string[] choices { get; set; }
        public long timestamp { get; set; }
    }

    internal class TalkErrorPayload
    {
        public string error { get; set; }
    }
}
