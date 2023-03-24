using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using OpenTabletDriver.Daemon.Contracts.RPC.Messages;
using StreamJsonRpc;

namespace OpenTabletDriver.Daemon.Contracts.RPC
{
    public interface IRpcClient<T> where T : class
    {
        T? Instance { get; }
        bool IsConnected { get; }
        event EventHandler? Connected;
        event EventHandler? Disconnected;
        Task ConnectAsync();
        void Disconnect();
    }

    public class RpcClient<T> : IRpcClient<T> where T : class
    {
        private readonly string _pipeName;

        public RpcClient(string pipeName)
        {
            _pipeName = pipeName;
        }

        private NamedPipeClientStream? _stream;
        private JsonRpc? _rpc;

        public T? Instance { private set; get; }
        public bool IsConnected { private set; get; }

        public event EventHandler? Connected;
        public event EventHandler? Disconnected;

        public async Task ConnectAsync()
        {
            _stream = GetStream();
            await _stream.ConnectAsync(5000);

            _rpc = new JsonRpc(new MessageHandler(_stream));
            _rpc.Disconnected += (_, _) => OnDisconnected();

            Instance = _rpc.Attach<T>();
            _rpc.StartListening();

            OnConnected();
        }

        public void Disconnect()
        {
            _rpc?.Dispose();
        }

        protected virtual void OnConnected()
        {
            IsConnected = true;
            Connected?.Invoke(this, EventArgs.Empty);
        }

        private void OnDisconnected()
        {
            IsConnected = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private NamedPipeClientStream GetStream()
        {
            return new NamedPipeClientStream(
                ".",
                _pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough | PipeOptions.CurrentUserOnly
            );
        }
    }
}
