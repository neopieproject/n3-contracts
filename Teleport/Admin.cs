using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Contracts
{
    public partial class Teleport
    {
        public static void Update(ByteString nefFile, string manifest)
        {
            IsContractOwnerOrAssert();
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void AddToken(UInt160 neoAddress, UInt160 evmAddress)
        {
            IsContractOwnerOrAssert();
            StorageMap TokenNeoToEvmMap = GetTokenNeoToEvmMap();
            TokenNeoToEvmMap.Put(neoAddress, evmAddress);
            StorageMap TokenEvmToNeoMap = GetTokenEvmToNeoMap();
            TokenEvmToNeoMap.Put(evmAddress, neoAddress);
        }

        public static void SetFeeReceiver(UInt160 account)
        {
            Assert(account != null && account.IsValid, "account is not valid.");
            IsContractOwnerOrAssert();
            Storage.Put(Storage.CurrentContext, Prefix_Fee_Receiver.ToByteArray(), account);
        }

        public static void TransferOwnership(UInt160 account)
        {
            Assert(account != null && account.IsValid, "account is not valid.");
            IsContractOwnerOrAssert();
            Storage.Put(Storage.CurrentContext, Prefix_Owner.ToByteArray(), account);
        }

        public static void Claim()
        {
            var currentBalance = GAS.BalanceOf(Runtime.ExecutingScriptHash);

            Assert(
                NEO.Transfer(Runtime.ExecutingScriptHash, Runtime.ExecutingScriptHash, 0),
                "Neo gas rewards claim failed."
            );

            var newBalance = GAS.BalanceOf(Runtime.ExecutingScriptHash);

            Assert(newBalance > currentBalance, "Claim amount is 0.");
            GAS.Transfer(
                Runtime.ExecutingScriptHash,
                GetFeeReceiver(),
                newBalance - currentBalance
            );
        }

        public static void Vote(Neo.Cryptography.ECC.ECPoint target)
        {
            IsContractOwnerOrAssert();
            Assert(NEO.Vote(Runtime.ExecutingScriptHash, target), "Vote failed.");
        }
    }
}
