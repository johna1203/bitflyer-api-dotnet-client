using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;
using Utf8Json;
using WebSocket4Net;

namespace BitFlyer.Apis
{
  public delegate void TickerEventHandler(object sender, Ticker e);
  public class RealTimeApi2
  {
    private WebSocket _webSocketClient;
    private static readonly string Uri = "wss://ws.lightstream.bitflyer.com/json-rpc";
    private bool _connected;
    private bool IsConnected => _connected;

    public event TickerEventHandler OnTicker;

    public RealTimeApi2()
    {
      _webSocketClient = CreateWebSocket();
    }

    public async Task OpenConnection()
    {
      _webSocketClient.Open();

      while (_webSocketClient.State != WebSocketState.Open)
      {
        await Task.Delay(25);
      }
    }

    private WebSocket CreateWebSocket()
    {
      var webSocket = new WebSocket(Uri);

      webSocket.Security.EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls | SslProtocols.Tls12;
      webSocket.EnableAutoSendPing = false;
      webSocket.Opened += WebsocketOnOpen;
      webSocket.Error += WebSocketOnError;
      webSocket.Closed += WebsocketOnClosed;
      webSocket.MessageReceived += WebsocketOnMessageReceive;
      webSocket.DataReceived += OnDataReceived;
      return webSocket;
    }

    public void Subscribe(string channel)
    {
      byte[] json = JsonSerializer.Serialize(new {method = "subscribe", @params = new {channel = channel}, id = 123,});
      _webSocketClient.Send(System.Text.Encoding.Default.GetString(json));
    }

    private void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
      Console.WriteLine($"OnDataReceived {e.Data}");
    }

    private void WebsocketOnMessageReceive(object sender, MessageReceivedEventArgs e)
    {

      if (e.Message.Contains("lightning_ticker"))
      {
        var ticker = JsonSerializer.Deserialize<RealtimeJsonRpc<Ticker>>(e.Message);
        OnTicker?.Invoke(this, ticker.Params.Message);
      }


      //if (ticker[""])

      // // Console.WriteLine($"WebsocketOnMessageReceive {e.Message}");
      // JObject json = JObject.Parse(e.Message);
      // if (json.ContainsKey("params"))
      // {
      //   string channel = json["params"]?["channel"]?.ToString();
      //   if (channel != null && channel.StartsWith("lightning_ticker"))
      //   {
      //     Console.WriteLine(json["params"]["message"]);
      //
      //     Console.WriteLine($"{ticker.BestBid}");
      //   }
      // }
    }

    private void WebsocketOnClosed(object sender, EventArgs e)
    {
      _connected = false;
      Console.WriteLine($"WebsocketOnClosed");
    }

    private void WebSocketOnError(object sender, ErrorEventArgs e)
    {
      Console.WriteLine($"WebSocketOnError {e.Exception.Message}");
    }

    private void WebsocketOnOpen(object sender, EventArgs e)
    {
      _connected = true;
      Console.WriteLine($"WebsocketOnOpen");
    }
  }
}