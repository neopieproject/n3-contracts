using Neo;
using System.ComponentModel;
using System.Numerics;

namespace Contracts
{
    public partial class Teleport
    {
        [DisplayName("Lock")]
        public static event LockEventHandler OnLock;
        public delegate void LockEventHandler(
            BigInteger lockId,
            UInt160 neoTokenAddress,
            UInt160 evmTokenAddress,
            UInt160 sender,
            UInt160 receiver,
            BigInteger amount
        );

        [DisplayName("Unlock")]
        public static event UnlockEventHandler OnUnlock;
        public delegate void UnlockEventHandler(
            BigInteger unlockId,
            UInt160 neoTokenAddress,
            UInt160 evmTokenAddress,
            UInt160 sender,
            UInt160 receiver,
            BigInteger amount
        );
    }
}
