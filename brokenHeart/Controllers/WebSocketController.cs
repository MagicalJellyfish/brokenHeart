using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace brokenHeart.Controllers
{
    [Route("api")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {

        private CharChangeObservable ccObservable;
        public WebSocketController(CharChangeObservable observable)
        {
            ccObservable = observable;
        }

        [Route("Characters/ws")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var buffer = new byte[1024 * 4];
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                int id = int.Parse(Encoding.UTF8.GetString(buffer, 0, receiveResult.Count));

                CharChangeObserver ccObserver = new CharChangeObserver(webSocket);

                ccObserver.Subscribe(ccObservable, id);

                while(true)
                {
                    await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private class CharChangeObserver : IObserver<bool>
        {
            private IDisposable unsubscriber;
            private WebSocket ws;

            public CharChangeObserver(WebSocket webSocket)
            {
                ws = webSocket;
            }

            public virtual void Subscribe(CharChangeObservable provider, int id)
            {
                unsubscriber = provider.Subscribe(this, id);
            }

            public virtual void Unsubscribe()
            {
                unsubscriber.Dispose();
            }

            public void OnNext(bool value)
            {
                byte[] message = BitConverter.GetBytes(value);
                ws.SendAsync(message, WebSocketMessageType.Binary, true, CancellationToken.None);
            }

            public void OnCompleted() { }
            public void OnError(Exception error) { }
        }
    }

    public class CharChangeObservable : IObservable<bool>
    {
        Dictionary<int, List<IObserver<bool>>> observersById;

        public CharChangeObservable()
        {
            observersById = new Dictionary<int, List<IObserver<bool>>>();
        }

        private class Unsubscriber : IDisposable
        {
            Dictionary<int, List<IObserver<bool>>> _observersById;
            private IObserver<bool> _observer;

            public Unsubscriber(Dictionary<int, List<IObserver<bool>>> observersById, IObserver<bool> observer)
            {
                _observersById = observersById;
                _observer = observer;
            }

            public void Dispose()
            {
                for(int i = 0; i < _observersById.Count; i++)
                {
                    if (_observersById[i].Contains(_observer))
                    {
                        _observersById[i].Remove(_observer);

                        if (_observersById[i].Count == 0)
                        {
                            _observersById.Remove(i);
                        }
                    }
                }
            }
        }

        public IDisposable Subscribe(IObserver<bool> observer, int id)
        {
            if(!observersById.ContainsKey(id))
            {
                observersById.Add(id, new List<IObserver<bool>>());
            }

            if (!observersById[id].Contains(observer))
            {
                observersById[id].Add(observer);
            }

            return new Unsubscriber(observersById, observer);
        }

        public void Trigger(int id)
        {
            if(observersById.ContainsKey(id))
            {
                foreach (var observer in observersById[id])
                {
                    observer.OnNext(true);
                }
            }
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            throw new NotImplementedException();
        }
    }
}
