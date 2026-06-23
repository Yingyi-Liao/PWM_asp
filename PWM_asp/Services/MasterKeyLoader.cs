using System;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

public static class MasterKeyLoader
{
    public static byte[] LoadMasterKey()
    {
        // Prefer environment variable or secret store
        string? base64 = Environment.GetEnvironmentVariable("PWM_MASTER_KEY");

        if (string.IsNullOrWhiteSpace(base64))
        {
            // For dev only: generate a new key (do NOT use this in prod)
            byte[] devKey = RandomNumberGenerator.GetBytes(32);
            base64 = Convert.ToBase64String(devKey);
            Console.WriteLine("DEV MASTER KEY (store securely): " + base64);
        }

        byte[] key = Convert.FromBase64String(base64);
        if (key.Length != 32)
            throw new InvalidOperationException("PWM_MASTER_KEY must be 32 bytes in Base64.");

        return key;
    }
}
