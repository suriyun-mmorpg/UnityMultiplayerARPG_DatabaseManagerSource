#if NET || NETCOREAPP
using Cysharp.Threading.Tasks;
using Npgsql;

namespace MultiplayerARPG.MMO
{
    public partial class PostgreSQLDatabase
    {
        public override async UniTask<int> CreateParty(bool shareExp, bool shareItem, string leaderId)
        {
            int id = 0;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                    id = reader.GetInt32(0);
            }, "INSERT INTO party (shareExp, shareItem, leaderId) VALUES (@shareExp, @shareItem, @leaderId);" +
                "SELECT LAST_INSERT_ID();",
                new NpgsqlParameter("@shareExp", shareExp),
                new NpgsqlParameter("@shareItem", shareItem),
                new NpgsqlParameter("@leaderId", leaderId));
            if (id > 0)
            {
                await ExecuteNonQuery("UPDATE characters SET partyId=@id WHERE id=@leaderId",
                    new NpgsqlParameter("@id", id),
                    new NpgsqlParameter("@leaderId", leaderId));
            }
            return id;
        }

        public override async UniTask<PartyData> ReadParty(int id)
        {
            PartyData result = null;
            await ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    result = new PartyData(id,
                        reader.GetBoolean(0),
                        reader.GetBoolean(1),
                        reader.GetString(2));
                }
            }, "SELECT shareExp, shareItem, leaderId FROM party WHERE id=@id LIMIT 1",
                new NpgsqlParameter("@id", id));
            // Read relates data if party exists
            if (result != null)
            {
                // Party members
                await ExecuteReader((reader) =>
                {
                    SocialCharacterData partyMemberData;
                    while (reader.Read())
                    {
                        // Get some required data, other data will be set at server side
                        partyMemberData = new SocialCharacterData();
                        partyMemberData.id = reader.GetString(0);
                        partyMemberData.dataId = reader.GetInt32(1);
                        partyMemberData.characterName = reader.GetString(2);
                        partyMemberData.level = reader.GetInt32(3);
                        result.AddMember(partyMemberData);
                    }
                }, "SELECT id, dataId, characterName, level FROM characters WHERE partyId=@id",
                    new NpgsqlParameter("@id", id));
            }
            return result;
        }

        public override async UniTask UpdatePartyLeader(int id, string leaderId)
        {
            await ExecuteNonQuery("UPDATE party SET leaderId=@leaderId WHERE id=@id",
                new NpgsqlParameter("@leaderId", leaderId),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateParty(int id, bool shareExp, bool shareItem)
        {
            await ExecuteNonQuery("UPDATE party SET shareExp=@shareExp, shareItem=@shareItem WHERE id=@id",
                new NpgsqlParameter("@shareExp", shareExp),
                new NpgsqlParameter("@shareItem", shareItem),
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask DeleteParty(int id)
        {
            await ExecuteNonQuery("DELETE FROM party WHERE id=@id;" +
                "UPDATE characters SET partyId=0 WHERE partyId=@id;",
                new NpgsqlParameter("@id", id));
        }

        public override async UniTask UpdateCharacterParty(string characterId, int partyId)
        {
            await ExecuteNonQuery("UPDATE characters SET partyId=@partyId WHERE id=@characterId",
                new NpgsqlParameter("@characterId", characterId),
                new NpgsqlParameter("@partyId", partyId));
        }
    }
}
#endif