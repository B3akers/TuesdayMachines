﻿using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TuesdayMachines.Dto;
using TuesdayMachines.Interfaces;
using TuesdayMachines.Services;

namespace TuesdayMachines.WebSockets
{
    public class WsRoulettePlayer
    {
        public AccountDTO Account;
        public WebSocket Socket;
    };

    public class WsConnectionInfo
    {
        public AccountDTO Account;
        public WebSocket Sokcet;
        public string WalletId;

        public async Task Send(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            await Sokcet.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    };

    public class RouletteGameState
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "game_state";
        [JsonPropertyName("twitchId")]
        public string TwitchId { get; set; }

        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }

        [JsonPropertyName("lastResults")]
        public int[] LastResults { get; set; }

        [JsonPropertyName("clientsCount")]
        public int ClientsCount { get; set; }

        [JsonPropertyName("roomClientsCount")]
        public int RoomClientsCount { get; set; }

        [JsonPropertyName("nextSpinTime")]
        public long NextSpinTime { get; set; }

        [JsonPropertyName("nonce")]
        public long Nonce { get; set; }
        [JsonPropertyName("balance")]
        public long Balance { get; set; }

        [JsonPropertyName("betClosed")]
        public bool BetClosed { get; set; }
    };

    public struct RouletteGamePlayerWin
    {
        [JsonPropertyName("twitchId")]
        public string TwitchId { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("win")]
        public long Win { get; set; }
    };

    public class RouletteGameBetClosed
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "bet_closed";

        [JsonPropertyName("betClosed")]
        public bool BetClosed { get; set; }
    };

    public class RouletteGameResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "game_result";

        [JsonPropertyName("number")]
        public int Number { get; set; }
        [JsonPropertyName("clientsCount")]
        public int ClientsCount { get; set; }

        [JsonPropertyName("nextSpinTime")]
        public long NextSpinTime { get; set; }

        [JsonPropertyName("nonce")]
        public long Nonce { get; set; }
    };

    public class RouletteGameWinnersResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "game_result_winners";

        [JsonPropertyName("roomClientsCount")]
        public int RoomClientsCount { get; set; }

        [JsonPropertyName("winners")]
        public List<RouletteGamePlayerWin> Winners { get; set; }
    };

    public class RouletteGameError
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "game_error";

        [JsonPropertyName("error")]
        public string Error { get; set; }
    };

    public class RouletteGameUpdateBalance
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "game_update_balance";

        [JsonPropertyName("balance")]
        public long Balance { get; set; }
    };

    public class RouletteGamePlaceBet
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "game_bet";

        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }
    };


    [JsonDerivedType(typeof(RouletteBasePacket), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(RoulettPlaceBetPacket), typeDiscriminator: "placeBet")]
    public class RouletteBasePacket
    {
    };

    public class RoulettPlaceBetPacket : RouletteBasePacket
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }
    };

    public class WebSocketRouletteHandler
    {
        private ConcurrentDictionary<string, WsConnectionInfo> _activeConnections = new ConcurrentDictionary<string, WsConnectionInfo>();
        private ConcurrentDictionary<string, ConcurrentDictionary<string, WsRoulettePlayer>> _roomsConnections = new ConcurrentDictionary<string, ConcurrentDictionary<string, WsRoulettePlayer>>();


        private readonly IPointsRepository _pointsRepository;
        private readonly LiveRouletteService _liveRouletteService;

        public WebSocketRouletteHandler(LiveRouletteService liveRouletteService, IPointsRepository pointsRepository)
        {
            _liveRouletteService = liveRouletteService;
            _pointsRepository = pointsRepository;
        }

        public int GetClientsCount()
        {
            return _activeConnections.Count;
        }

        public int GetRoomClientsCount(string roomId)
        {
            if (_roomsConnections.TryGetValue(roomId, out var players))
            {
                return players.Count;
            }

            return 0;
        }

        public async Task SendToAllInRoom(string data, string roomId)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            if (_roomsConnections.TryGetValue(roomId, out var players))
            {
                foreach (var connection in players)
                {
                    try
                    {
                        await connection.Value.Socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch { }
                }
            }
        }

        public async Task SendToAll(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            foreach (var connection in _activeConnections.Values)
            {
                try
                {
                    await connection.Sokcet.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch { }
            }
        }

        public async Task Connection(AccountDTO account, WebSocket socket)
        {
            WebSocketReceiveResult receiveResult = null;

            if (_activeConnections.TryRemove(account.Id, out var oldConnection))
            {
                if (!string.IsNullOrEmpty(oldConnection.WalletId))
                {
                    if (_roomsConnections.TryGetValue(oldConnection.WalletId, out var roomPlayers))
                    {
                        roomPlayers.TryRemove(account.Id, out _);
                    }
                }

                if (oldConnection.Sokcet.State == WebSocketState.Open
                    || oldConnection.Sokcet.State == WebSocketState.CloseReceived)
                {
                    await oldConnection.Sokcet.CloseAsync((WebSocketCloseStatus)3001, "Another connection", CancellationToken.None);
                }
            }

            var connectionInfo = new WsConnectionInfo() { Account = account, Sokcet = socket, WalletId = string.Empty };

            if (!_activeConnections.TryAdd(account.Id, connectionInfo))
            {
                await socket.CloseAsync((WebSocketCloseStatus)3001, "Another connection", CancellationToken.None);
                return;
            }

            bool authorized = false;
            string walletId = string.Empty;

            try
            {
                var buffer = new byte[1024 * 8];
                receiveResult = await socket.ReceiveAsync(
                   new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    if (!receiveResult.EndOfMessage)
                    {
                        await socket.CloseOutputAsync(WebSocketCloseStatus.MessageTooBig, string.Empty, CancellationToken.None);
                        break;
                    }

                    var data = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    if (authorized)
                    {
                        if (data == "#1")
                        {
                            await socket.SendAsync(
                                    new ArraySegment<byte>(Encoding.UTF8.GetBytes("#2")),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                        }
                        else
                        {
                            try
                            {
                                RoulettPlaceBetPacket packet = JsonSerializer.Deserialize<RoulettPlaceBetPacket>(data);
                                var resultHandler = await _liveRouletteService.HandlePacket(connectionInfo, packet);
                                if (resultHandler != null)
                                {
                                    if (string.IsNullOrEmpty(resultHandler.Item2))
                                    {
                                        await SendToAll(resultHandler.Item1);
                                    }
                                    else
                                    {
                                        await SendToAllInRoom(resultHandler.Item1, resultHandler.Item2);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        PointOperationResult accountBalance = default(PointOperationResult);
                        //User is not authorized (wallet not set)
                        //First message should be a raw wallet id (broadcaster id)
                        if (!string.IsNullOrEmpty(data) && data.Length == 24)
                        {
                            accountBalance = await _pointsRepository.GetBalance(account.TwitchId, data);
                            if (accountBalance.Success)
                                walletId = data;
                        }

                        if (string.IsNullOrEmpty(walletId))
                        {
                            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Invalid wallet", CancellationToken.None);
                            break;
                        }

                        connectionInfo.WalletId = walletId;

                        var roomPlayers = _roomsConnections.GetOrAdd(walletId, new ConcurrentDictionary<string, WsRoulettePlayer>());
                        if (!roomPlayers.TryAdd(account.Id, new WsRoulettePlayer() { Account = account, Socket = socket }))
                        {
                            //Ups another connection?
                            //
                            break;
                        }

                        var state = new RouletteGameState();
                        state.AccountId = account.Id;
                        state.TwitchId = account.TwitchId;
                        state.LastResults = _liveRouletteService.LastsResults.ToArray();
                        state.ClientsCount = GetClientsCount();
                        state.RoomClientsCount = GetRoomClientsCount(walletId);
                        state.NextSpinTime = _liveRouletteService.NextCloseBetsTime;
                        state.Nonce = _liveRouletteService.LastNonce;
                        state.BetClosed = _liveRouletteService.IsBetClosed;
                        state.Balance = accountBalance.Balance;

                        await socket.SendAsync(
                                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(state))),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);

                        authorized = true;
                    }

                    receiveResult = await socket.ReceiveAsync(
                                        new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }
            catch { }

            if (_activeConnections.TryRemove(account.Id, out var old))
            {
                if (!string.IsNullOrEmpty(old.WalletId))
                {
                    if (_roomsConnections.TryGetValue(old.WalletId, out var roomPlayers))
                    {
                        roomPlayers.TryRemove(account.Id, out _);
                    }
                }
            }

            if (socket.State == WebSocketState.Open)
            {
                await socket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, string.Empty, CancellationToken.None);
                return;
            }

            if (socket.State != WebSocketState.CloseReceived)
                return;

            if (receiveResult == null || !receiveResult.CloseStatus.HasValue)
                return;

            await socket.CloseOutputAsync(
                 receiveResult.CloseStatus.Value,
                 receiveResult.CloseStatusDescription,
                 CancellationToken.None);
        }
    }
}