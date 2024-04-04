#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public override async UniTask<List<MailListEntry>> MailList(string userId, bool onlyNewMails)
        {
            List<MailListEntry> result = new List<MailListEntry>();
            await ExecuteReader((reader) =>
            {
                MailListEntry tempMail;
                while (reader.Read())
                {
                    int gold = reader.GetInt32(3);
                    int cash = reader.GetInt32(4);
                    string currencies = reader.GetString(5);
                    string items = reader.GetString(6);
                    tempMail = new MailListEntry()
                    {
                        Id = reader.GetInt64(0).ToString(),
                        SenderName = reader.GetString(1),
                        Title = reader.GetString(2),
                        IsRead = reader.GetBoolean(7),
                        IsClaim = reader.GetBoolean(8),
                        SentTimestamp = ((DateTimeOffset)reader.GetDateTime(9)).ToUnixTimeSeconds(),
                    };
                    if (onlyNewMails)
                    {
                        if (!tempMail.IsClaim && (gold != 0 || cash != 0 || !string.IsNullOrEmpty(currencies) || !string.IsNullOrEmpty(items)))
                            result.Add(tempMail);
                        else if (!tempMail.IsRead)
                            result.Add(tempMail);
                    }
                    else
                    {
                        result.Add(tempMail);
                    }
                }
            }, "SELECT id, senderName, title, gold, cash, currencies, items, is_read, is_claim, sent_time FROM mail WHERE receiver_id=@receiverId AND is_delete IS FALSE ORDER BY is_read ASC, sent_time DESC",
                new NpgsqlParameter("@receiverId", userId));
            return result;
        }

        public override async UniTask<Mail> GetMail(string mailId, string userId)
        {
            Mail result = new Mail();
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    result.Id = reader.GetInt32(0).ToString();
                    result.EventId = reader.GetString(1);
                    result.SenderId = reader.GetString(2);
                    result.SenderName = reader.GetString(3);
                    result.ReceiverId = reader.GetString(4);
                    result.Title = reader.GetString(5);
                    result.Content = reader.GetString(6);
                    result.Gold = reader.GetInt32(7);
                    result.Cash = reader.GetInt32(8);
                    result.ReadCurrencies(reader.GetString(9));
                    result.ReadItems(reader.GetString(10));
                    result.IsRead = reader.GetBoolean(11);
                    if (reader[12] != DBNull.Value)
                        result.ReadTimestamp = ((DateTimeOffset)reader.GetDateTime(12)).ToUnixTimeSeconds();
                    result.IsClaim = reader.GetBoolean(13);
                    if (reader[14] != DBNull.Value)
                        result.ClaimTimestamp = ((DateTimeOffset)reader.GetDateTime(14)).ToUnixTimeSeconds();
                    result.SentTimestamp = ((DateTimeOffset)reader.GetDateTime(15)).ToUnixTimeSeconds();
                }
            }, "SELECT id, event_id, sender_id, sender_name, receiver_id, title, content, gold, cash, currencies, items, is_read, read_time, is_claim, claim_time, sent_time FROM mail WHERE id=@id AND receiver_id=@receiverId AND is_delete IS FALSE",
                new NpgsqlParameter("@id", mailId),
                new NpgsqlParameter("@receiverId", userId));
            return result;
        }

        public override async UniTask<long> UpdateReadMailState(string mailId, string userId)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM mail WHERE id=@id AND receiver_id=@receiverId",
                new NpgsqlParameter("@id", mailId),
                new NpgsqlParameter("@receiverId", userId));
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                await ExecuteNonQuery("UPDATE mail SET is_read=TRUE, read_time=NOW() WHERE id=@id AND receiver_id=@receiverId AND is_read IS FALSE",
                    new NpgsqlParameter("@id", mailId),
                    new NpgsqlParameter("@receiverId", userId));
            }
            return count;
        }

        public override async UniTask<long> UpdateClaimMailItemsState(string mailId, string userId)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM mail WHERE id=@id AND receiver_id=@receiverId",
                new NpgsqlParameter("@id", mailId),
                new NpgsqlParameter("@receiverId", userId));
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                await ExecuteNonQuery("UPDATE mail SET is_claim=TRUE, claim_time=NOW() WHERE id=@id AND receiver_id=@receiverId AND isClaim IS FALSE",
                    new NpgsqlParameter("@id", mailId),
                    new NpgsqlParameter("@receiverId", userId));
            }
            return count;
        }

        public override async UniTask<long> UpdateDeleteMailState(string mailId, string userId)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM mail WHERE id=@id AND receiver_id=@receiverId",
                new NpgsqlParameter("@id", mailId),
                new NpgsqlParameter("@receiverId", userId));
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                await ExecuteNonQuery("UPDATE mail SET is_delete=TRUE, delete_time=NOW() WHERE id=@id AND receiver_id=@receiverId AND is_delete IS FALSE",
                    new NpgsqlParameter("@id", mailId),
                    new NpgsqlParameter("@receiverId", userId));
            }
            return count;
        }

        public override async UniTask<int> CreateMail(Mail mail)
        {
            return await ExecuteNonQuery("INSERT INTO mail (event_id, sender_id, sender_name, receiver_id, title, content, gold, cash, currencies, items) " +
                "VALUES (@eventId, @senderId, @senderName, @receiverId, @title, @content, @gold, @cash, @currencies, @items)",
                    new NpgsqlParameter("@eventId", mail.EventId),
                    new NpgsqlParameter("@senderId", mail.SenderId),
                    new NpgsqlParameter("@senderName", mail.SenderName),
                    new NpgsqlParameter("@receiverId", mail.ReceiverId),
                    new NpgsqlParameter("@title", mail.Title),
                    new NpgsqlParameter("@content", mail.Content),
                    new NpgsqlParameter("@gold", mail.Gold),
                    new NpgsqlParameter("@cash", mail.Cash),
                    new NpgsqlParameter("@currencies", mail.WriteCurrencies()),
                    new NpgsqlParameter("@items", mail.WriteItems()));
        }

        public override async UniTask<int> GetMailNotification(string userId)
        {
            int count = 0;
            await ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    int gold = reader.GetInt32(0);
                    int cash = reader.GetInt32(1);
                    string currencies = reader.GetString(2);
                    string items = reader.GetString(3);
                    bool isRead = reader.GetBoolean(4);
                    bool isClaim = reader.GetBoolean(5);
                    if (!isClaim && (gold != 0 || !string.IsNullOrEmpty(currencies) || !string.IsNullOrEmpty(items)))
                        count++;
                    else if (!isRead)
                        count++;
                }
            }, "SELECT gold, cash, currencies, items, is_read, is_claim FROM mail WHERE receiver_id=@receiverId AND is_delete IS FALSE",
                new NpgsqlParameter("@receiverId", userId));
            return count;
        }
    }
}
#endif