using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Contracts
{
    public struct State
    {
        public BigInteger BlockNo;
        public BigInteger No;
        public UInt160 NeoTokenAddress;
        public UInt160 EvmTokenAddress;
        public UInt160 Sender;
        public UInt160 Receiver;
        public BigInteger Amount;
        public BigInteger CreatedAt;
    }

    [ContractPermission("*", "*")]
    public partial class Teleport : SmartContract
    {
        protected const byte Prefix_Owner = 0x01;
        protected const byte Prefix_Lock = 0x02;
        protected const byte Prefix_Lock_No = 0x03;
        protected const byte Prefix_Token_Neo = 0x04;
        protected const byte Prefix_Token_Evm = 0x05;
        protected const byte Prefix_Unlock = 0x06;
        protected const byte Prefix_Unlock_No = 0x07;
        protected const byte Prefix_Fee_Receiver = 0x8;

        public static void Lock(
            UInt160 neoTokenAddress,
            UInt160 sender,
            UInt160 receiver,
            BigInteger amount
        )
        {
            UInt160 contractItselft = Runtime.ExecutingScriptHash;
            StorageMap TokenNeoToEvmMap = GetTokenNeoToEvmMap();
            UInt160 evmTokenAdderss = (UInt160)TokenNeoToEvmMap.Get(neoTokenAddress);
            Assert(evmTokenAdderss != null, "Can't find destination token hash.");
            Assert(amount > 0, "amount neeeds to be more than 0.");
            Assert(sender.IsValid, "sender is not valid.");
            Assert(receiver != contractItselft, "Receiver is invalid.");
            Assert(receiver.IsValid, "receiver is not valid.");
            Assert(Runtime.CheckWitness(sender), "CheckWitness failed.");

            SafeTransfer(neoTokenAddress, sender, Runtime.ExecutingScriptHash, amount);

            var lockNo = GetLockNo() + 1;
            StorageMap lockMap = GetLockMap();

            lockMap.Put(
                lockNo.ToByteArray(),
                StdLib.Serialize(
                    new State
                    {
                        BlockNo = Ledger.CurrentIndex,
                        No = lockNo,
                        NeoTokenAddress = neoTokenAddress,
                        EvmTokenAddress = evmTokenAdderss,
                        Sender = sender,
                        Receiver = receiver,
                        Amount = amount,
                        CreatedAt = Runtime.Time
                    }
                )
            );

            Storage.Put(Storage.CurrentContext, Prefix_Lock_No.ToByteArray(), lockNo);

            OnLock(lockNo, neoTokenAddress, evmTokenAdderss, sender, receiver, amount);
        }

        public static void Unlock(
            BigInteger no,
            UInt160 evmTokenAddress,
            UInt160 sender,
            UInt160 receiver,
            BigInteger amount
        )
        {
            IsContractOwnerOrAssert();

            UInt160 contractItselft = Runtime.ExecutingScriptHash;
            StorageMap TokenEvmToNeoMap = GetTokenEvmToNeoMap();
            UInt160 neoTokenAddress = (UInt160)TokenEvmToNeoMap.Get(evmTokenAddress);
            Assert(neoTokenAddress != null, "Can't find neo token hash.");
            Assert(amount > 0, "amount neeeds to be more than 0.");

            var unLockNo = GetUnlockNo() + 1;
            StorageMap unLockMap = GetUnlockMap();
            Assert(unLockMap.Get(no.ToByteArray()) == null, "Already Locked.");
            Assert(no == unLockNo, "Unlock number doesnt' match.");
            Assert(receiver != contractItselft, "Receiver is invalid.");
            Assert(receiver != neoTokenAddress, "Receiver is invalid.");

            unLockMap.Put(
                unLockNo.ToByteArray(),
                StdLib.Serialize(
                    new State
                    {
                        BlockNo = Ledger.CurrentIndex,
                        No = unLockNo,
                        NeoTokenAddress = neoTokenAddress,
                        EvmTokenAddress = evmTokenAddress,
                        Sender = sender,
                        Receiver = receiver,
                        Amount = amount,
                        CreatedAt = Runtime.Time
                    }
                )
            );

            Storage.Put(Storage.CurrentContext, Prefix_Unlock_No.ToByteArray(), unLockNo);

            SafeTransfer(neoTokenAddress, contractItselft, receiver, amount);
            OnUnlock(unLockNo, neoTokenAddress, evmTokenAddress, sender, receiver, amount);
        }

        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object[] data) { }

        public static void _deploy(object data, bool update)
        {
            if (update)
                return;
            var tx = (Transaction)Runtime.ScriptContainer;
            Storage.Put(Storage.CurrentContext, Prefix_Owner.ToByteArray(), tx.Sender);
        }
    }
}
