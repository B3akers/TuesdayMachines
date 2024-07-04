﻿using MongoDB.Driver;
using TuesdayMachines.Dto;

namespace TuesdayMachines.Interfaces
{
    public struct PointModifyCommand
    {
        public string TwitchUserId;
        public long value;
    };

    public struct PointOperationResult
    {
        public bool Success;
        public long Balance;
    };

    public interface IPointsRepository
    {
        void SetPoints(string twitchUserId, string broadcasterAccountId, long value);
        void AddPoints(string twitchUserId, string broadcasterAccountId, long value);
        void AddPoints(List<PointModifyCommand> users, string broadcasterAccountId);
        PointOperationResult TakePoints(string twitchUserId, string broadcasterAccountId, long value);
        Task<PointOperationResult> GetBalance(string twitchUserId, string broadcasterAccountId);
        Task<IAsyncCursor<WalletDTO>> GetUserWallets(string twitchUserId);
    }
}
