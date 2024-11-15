# Teleport

Teleport is a smart contract that facilitates the locking and unlocking of tokens between NEO and EVM-compatible blockchains. It provides a secure and efficient way to transfer assets across different blockchain networks.

## Features

- **Lock Tokens**: Lock tokens on the NEO blockchain to initiate a cross-chain transfer.
- **Unlock Tokens**: Unlock tokens on the NEO blockchain that were previously locked on an EVM-compatible blockchain.
- **Token Management**: Add and manage supported tokens for cross-chain transfers.
- **Ownership Management**: Transfer contract ownership and manage contract permissions.
- **Event Handling**: Emit events for lock and unlock operations to facilitate off-chain tracking.

## Smart Contract Methods

### Lock

Locks tokens on the NEO blockchain.

```csharp
public static void Lock(UInt160 neoTokenAddress, UInt160 sender, UInt160 receiver, BigInteger amount)
```

### Unlock

Unlocks tokens on the NEO blockchain.

```csharp
public static void Unlock(BigInteger no, UInt160 evmTokenAddress, UInt160 sender, UInt160 receiver, BigInteger amount)
```

### Getters

- `GetLockNo()`: Returns the current lock number.
- `GetUnlockNo()`: Returns the current unlock number.
- `GetLock(BigInteger no)`: Returns details of a specific lock.
- `GetUnlock(BigInteger no)`: Returns details of a specific unlock.
- `GetTokenList()`: Returns a list of supported tokens.
- `GetOwner()`: Returns the contract owner.
- `GetFeeReceiver()`: Returns the fee receiver address.
- `GetAllSupportedNeoTokens()`: Returns a list of all supported NEO tokens.
- `GetAllSupportedEvmTokens()`: Returns a list of all supported EVM tokens.

### Admin Methods

- `AddToken(UInt160 neoAddress, UInt160 evmAddress)`: Adds a new token for cross-chain transfers.
- `SetFeeReceiver(UInt160 account)`: Sets the fee receiver address.
- `TransferOwnership(UInt160 account)`: Transfers contract ownership.
- `Claim()`: Claims NEO gas rewards.
- `Vote(Neo.Cryptography.ECC.ECPoint target)`: Votes for a candidate.
- `Update(ByteString nefFile, string manifest)`: Updates the contract.

## Events

- `OnLock`: Emitted when tokens are locked.
- `OnUnlock`: Emitted when tokens are unlocked.

## Usage

To build the project, run the following command:

```sh
dotnet build
```

## License

This project is licensed under the MIT License.
