using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Authenticator.Services
{
    public class TOTP
    {
        private readonly Func<DateTimeOffset> _timeProvider;
        private readonly TimeSpan _timeStep;
        private readonly string _algorithm;
        private readonly int _length;
        private readonly int[] _powers = new[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };

        public string Generate(byte[] key)
        {
            long count = Map(_timeProvider());
            byte[] hash = HmacHash(key, ToByteArray(count));
            string code = ComputeDigits(hash);
            return code;
        }

        private long Map(DateTimeOffset counter)
        {
            return (long)(counter.ToUniversalTime().ToUnixTimeSeconds() / _timeStep.TotalSeconds);
        }

        private byte[] ToByteArray(long counter)
        {
            byte[] buffer = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            return buffer;
        }

        private byte[] HmacHash(byte[] privateKey, byte[] counter)
        {
            using HMAC hasher = HMAC.Create(_algorithm);
            hasher.Key = privateKey;
            return hasher.ComputeHash(counter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ComputeDigits(byte[] hash)
        {
            int binaryCode = TruncateHash(hash);
            int otpCode = DoBinaryCodeReduction(binaryCode, _length);
            return otpCode.ToString(CultureInfo.InvariantCulture).PadLeft(_length, '0');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int TruncateHash(byte[] hash)
        {
            // The RFC has a hard coded index 19 in this value.
            // This is the same thing but also accommodates SHA256 and SHA512
            // hash[19] => hash[hmacComputedHash.Length - 1]
            int truncationOffset = hash[^1] & 0xF;
            int binaryCode =
                ((hash[truncationOffset + 0] & 0x7F) << 24) |
                ((hash[truncationOffset + 1] & 0xFF) << 16) |
                ((hash[truncationOffset + 2] & 0xFF) << 8) |
                ((hash[truncationOffset + 3] & 0xFF) << 0);
            return binaryCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int DoBinaryCodeReduction(int binaryCode, int length)
        {
            return binaryCode % _powers[length];
        }

        public TOTP(Func<DateTimeOffset> timeProvider, TimeSpan timeStep, string algorithm, int length)
        {
            if (length < 1 || length > 9)
                throw new ArgumentOutOfRangeException(nameof(length), "Code length must be between 1 and 9");

            _timeProvider = timeProvider;
            _timeStep = timeStep;
            _algorithm = algorithm;
            _length = length;
        }
    }
}