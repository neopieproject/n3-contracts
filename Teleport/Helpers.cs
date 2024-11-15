using System;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Contracts
{
    public partial class Teleport
    {
        private static StorageMap GetLockMap() =>
            new(Storage.CurrentContext, Prefix_Lock.ToByteArray());

        private static StorageMap GetUnlockMap() =>
            new(Storage.CurrentContext, Prefix_Unlock.ToByteArray());

        private static StorageMap GetTokenNeoToEvmMap() =>
            new(Storage.CurrentContext, Prefix_Token_Neo.ToByteArray());

        private static StorageMap GetTokenEvmToNeoMap() =>
            new(Storage.CurrentContext, Prefix_Token_Evm.ToByteArray());

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }

        private static void SafeTransfer(UInt160 token, UInt160 from, UInt160 to, BigInteger amount)
        {
            var result = (bool)
                Contract.Call(
                    token,
                    "transfer",
                    CallFlags.All,
                    new object[] { from, to, amount, null }
                );
            Assert(result, "Transfer Fail");
        }

        private static string GetSymbol(UInt160 contractHash)
        {
            return (string)
                Contract.Call(contractHash, "symbol", CallFlags.ReadOnly, new object[] { });
        }

        private static void IsContractOwnerOrAssert()
        {
            Assert(Runtime.CheckWitness(GetOwner()), "Not authorized.");
        }

        private static BigInteger GetBalanceOf(UInt160 contractHash, UInt160 account)
        {
            return (BigInteger)
                Contract.Call(
                    contractHash,
                    "balanceOf",
                    CallFlags.ReadOnly,
                    new object[] { account }
                );
        }
    }
}
