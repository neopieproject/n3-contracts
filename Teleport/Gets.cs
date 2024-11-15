using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Contracts
{
    public partial class Teleport
    {
        [Safe]
        public static BigInteger GetLockNo()
        {
            return (BigInteger)Storage.Get(Storage.CurrentContext, Prefix_Lock_No.ToByteArray());
        }

        [Safe]
        public static BigInteger GetUnlockNo()
        {
            return (BigInteger)Storage.Get(Storage.CurrentContext, Prefix_Unlock_No.ToByteArray());
        }

        [Safe]
        public static Map<string, object> GetLock(BigInteger no)
        {
            StorageMap lockMap = GetLockMap();
            State _item = (State)StdLib.Deserialize(lockMap.Get(no.ToByteArray()));
            var _map = new Map<string, object>();
            _map["blockNo"] = _item.BlockNo;
            _map["no"] = _item.No;
            _map["neoTokenAddress"] = _item.NeoTokenAddress;
            _map["sender"] = _item.Sender;
            _map["evmTokenAddress"] = _item.EvmTokenAddress;
            _map["evmReceiver"] = _item.Receiver;
            _map["amount"] = _item.Amount;
            _map["createdAt"] = _item.CreatedAt;
            return _map;
        }

        [Safe]
        public static Map<string, object> GetLocks(int perPage, int page)
        {
            BigInteger total = GetLockNo();
            var items = new List<Map<string, object>>();
            var paginate = new Map<string, object>();

            if (total.IsZero)
            {
                paginate["totalItems"] = 0;
                paginate["totalPages"] = 0;
            }
            else
            {
                BigInteger itemsPerPage = perPage > total ? total : perPage;
                BigInteger totalPages = (itemsPerPage + total - 1) / itemsPerPage;
                BigInteger startAt = total - (itemsPerPage * (page - 1));
                BigInteger endAt = startAt > itemsPerPage ? startAt - itemsPerPage : 0;
                paginate["totalItems"] = total;
                paginate["totalPages"] = totalPages;

                for (BigInteger i = startAt; i != endAt; i--)
                {
                    items.Add(GetLock(i));
                }
            }
            paginate["items"] = items;
            return paginate;
        }

        [Safe]
        public static Map<string, object> GetUnlock(BigInteger no)
        {
            StorageMap unLockMap = GetUnlockMap();
            State _item = (State)StdLib.Deserialize(unLockMap.Get(no.ToByteArray()));
            var _map = new Map<string, object>();
            _map["blockNo"] = _item.BlockNo;
            _map["no"] = _item.No;
            _map["neoTokenAddress"] = _item.NeoTokenAddress;
            _map["evmSender"] = _item.Sender;
            _map["evmTokenAddress"] = _item.EvmTokenAddress;
            _map["receiver"] = _item.Receiver;
            _map["amount"] = _item.Amount;
            _map["createdAt"] = _item.CreatedAt;
            return _map;
        }

        [Safe]
        public static Map<string, object> GetUnlocks(int perPage, int page)
        {
            BigInteger total = GetUnlockNo();
            var items = new List<Map<string, object>>();
            var paginate = new Map<string, object>();

            if (total.IsZero)
            {
                paginate["totalItems"] = 0;
                paginate["totalPages"] = 0;
            }
            else
            {
                BigInteger itemsPerPage = perPage > total ? total : perPage;
                BigInteger totalPages = (itemsPerPage + total - 1) / itemsPerPage;
                BigInteger startAt = total - (itemsPerPage * (page - 1));
                BigInteger endAt = startAt > itemsPerPage ? startAt - itemsPerPage : 0;
                paginate["totalItems"] = total;
                paginate["totalPages"] = totalPages;

                for (BigInteger i = startAt; i != endAt; i--)
                {
                    items.Add(GetUnlock(i));
                }
            }
            paginate["items"] = items;
            return paginate;
        }

        [Safe]
        public static string GetLockJson(BigInteger no)
        {
            return StdLib.JsonSerialize(GetLock(no));
        }

        [Safe]
        public static string GetUnlockJson(BigInteger no)
        {
            return StdLib.JsonSerialize(GetUnlock(no));
        }

        [Safe]
        public static List<Map<string, object>> GetTokenList()
        {
            var arr = new List<Map<string, object>>();
            StorageMap TokenNeoToEvmMap = GetTokenNeoToEvmMap();
            var tokenList = TokenNeoToEvmMap.Find(FindOptions.RemovePrefix | FindOptions.KeysOnly);
            while (tokenList.Next())
            {
                var valMap = new Map<string, object>();
                var token = (UInt160)tokenList.Value;
                valMap["token"] = token;
                valMap["symbol"] = GetSymbol(token);
                valMap["balance"] = GetBalanceOf(token, Runtime.CallingScriptHash);
                valMap["evmTokenAddress"] = TokenNeoToEvmMap[token];
                arr.Add(valMap);
            }
            return arr;
        }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Owner.ToByteArray());
        }

        [Safe]
        public static List<UInt160> GetAllSupportedNeoTokens()
        {
            StorageMap NeoTokenMap = GetTokenNeoToEvmMap();
            Iterator arr = NeoTokenMap.Find(FindOptions.KeysOnly | FindOptions.RemovePrefix);
            List<UInt160> list = new List<UInt160>();
            while (arr.Next())
            {
                list.Add(((UInt160)arr.Value));
            }
            return list;
        }

        [Safe]
        public static UInt160 GetNEOTokenAddress(UInt160 evmTokenAddess)
        {
            StorageMap EVMTokenMap = GetTokenEvmToNeoMap();
            return (UInt160)EVMTokenMap.Get(evmTokenAddess);
        }

        [Safe]
        public static List<UInt160> GetAllSupportedEvmTokens()
        {
            StorageMap EVMTokenMap = GetTokenEvmToNeoMap();
            Iterator arr = EVMTokenMap.Find(FindOptions.KeysOnly | FindOptions.RemovePrefix);
            List<UInt160> list = new List<UInt160>();
            while (arr.Next())
            {
                list.Add(((UInt160)arr.Value));
            }
            return list;
        }

        [Safe]
        public static UInt160 GetEVMTokenAddress(UInt160 evmTokenAddess)
        {
            StorageMap EVMTokenMap = GetTokenEvmToNeoMap();
            return (UInt160)EVMTokenMap.Get(evmTokenAddess);
        }

        [Safe]
        public static UInt160 GetFeeReceiver()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, Prefix_Fee_Receiver.ToByteArray());
        }
    }
}
