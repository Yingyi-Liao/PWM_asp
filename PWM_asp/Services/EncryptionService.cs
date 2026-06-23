using PWM_asp.Services;
using System;
using System.Security.Cryptography;

public interface IEncryptionService
{
    (string encryptedPassword, string encryptedDataKey) Encrypt(string plaintextPassword);
    string Decrypt(string encryptedPassword, string encryptedDataKey);
}


public class EncryptionService : IEncryptionService
{
    private readonly byte[] _masterKey;

    public EncryptionService(byte[] masterKey)
    {
        _masterKey = masterKey ?? throw new ArgumentNullException(nameof(masterKey));
        if (_masterKey.Length != 32)
            throw new ArgumentException("Master key must be 32 bytes (AES-256).", nameof(masterKey));
    }

    public (string encryptedPassword, string encryptedDataKey) Encrypt(string plaintextPassword)
    {
        if (plaintextPassword == null)
            throw new ArgumentNullException(nameof(plaintextPassword));

        // 1. Generate per-password data key
        byte[] dataKey = RandomNumberGenerator.GetBytes(32);

        // 2. Encrypt password with data key
        string encryptedPassword = AesStringEncryptor.Encrypt(plaintextPassword, dataKey);

        // 3. Encrypt data key (as Base64) with master key
        string dataKeyBase64 = Convert.ToBase64String(dataKey);
        string encryptedDataKey = AesStringEncryptor.Encrypt(dataKeyBase64, _masterKey);

        return (encryptedPassword, encryptedDataKey);
    }

    public string Decrypt(string encryptedPassword, string encryptedDataKey)
    {
        if (encryptedPassword == null)
            throw new ArgumentNullException(nameof(encryptedPassword));
        if (encryptedDataKey == null)
            throw new ArgumentNullException(nameof(encryptedDataKey));

        // 1. Decrypt data key using master key
        string dataKeyBase64 = AesStringEncryptor.Decrypt(encryptedDataKey, _masterKey);
        byte[] dataKey = Convert.FromBase64String(dataKeyBase64);

        // 2. Decrypt password using data key
        return AesStringEncryptor.Decrypt(encryptedPassword, dataKey);
    }
}

