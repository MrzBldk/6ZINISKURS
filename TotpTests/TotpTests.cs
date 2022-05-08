using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TotpLibrary;
using System.Text;
using System;

namespace TotpTests
{
    public class TotpTests
    {

        private static readonly byte[] Key20Bits = Encoding.UTF8.GetBytes("12345678901234567890");
        private static readonly byte[] Key32Bits = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        private static readonly byte[] Key64Bits = Encoding.UTF8.GetBytes("1234567890123456789012345678901234567890123456789012345678901234");

        [Test(Description = "Checks if 8-digits code generated with HMAC-SHA1 and 30-second timespan is correct")]
        [TestCase("1970-01-01T00:00:59Z", "94287082")]
        [TestCase("2005-03-18T01:58:29Z", "07081804")]
        [TestCase("2005-03-18T01:58:31Z", "14050471")]
        [TestCase("2009-02-13T23:31:30Z", "89005924")]
        [TestCase("2033-05-18T03:33:20Z", "69279037")]
        [TestCase("2603-10-11T11:33:20Z", "65353130")]
        public void GenerateWithSha1 (string dateStr, string expectedCode)
        {
            DateTimeOffset date = DateTimeOffset.Parse(dateStr);
            TOTP totp = new(()=>date, TimeSpan.FromSeconds(30), "HMACSHA1", 8);
            string actualCode = totp.Generate(Key20Bits);
            Assert.That(actualCode, Is.EqualTo(expectedCode));
        }

        [Test(Description = "Checks if 8-digits code generated with HMAC-SHA256 and 30-second timespan is correct")]
        [TestCase("1970-01-01T00:00:59Z", "46119246")]
        [TestCase("2005-03-18T01:58:29Z", "68084774")]
        [TestCase("2005-03-18T01:58:31Z", "67062674")]
        [TestCase("2009-02-13T23:31:30Z", "91819424")]
        [TestCase("2033-05-18T03:33:20Z", "90698825")]
        [TestCase("2603-10-11T11:33:20Z", "77737706")]
        public void GenerateWithSha256(string dateStr, string expectedCode)
        {
            DateTimeOffset date = DateTimeOffset.Parse(dateStr);
            TOTP totp = new(() => date, TimeSpan.FromSeconds(30), "HMACSHA256", 8);
            string actualCode = totp.Generate(Key32Bits);
            Assert.That(actualCode, Is.EqualTo(expectedCode));
        }

        [Test(Description = "Checks if 8-digits code generated with HMAC-SHA512 and 30-second timespan is correct")]
        [TestCase("1970-01-01T00:00:59Z", "90693936")]
        [TestCase("2005-03-18T01:58:29Z", "25091201")]
        [TestCase("2005-03-18T01:58:31Z", "99943326")]
        [TestCase("2009-02-13T23:31:30Z", "93441116")]
        [TestCase("2033-05-18T03:33:20Z", "38618901")]
        [TestCase("2603-10-11T11:33:20Z", "47863826")]
        public void GenerateWithSha512(string dateStr, string expectedCode)
        {
            var date = DateTimeOffset.Parse(dateStr);
            TOTP totp = new(() => date, TimeSpan.FromSeconds(30), "HMACSHA512", 8);
            var actualCode = totp.Generate(Key64Bits);
            Assert.That(actualCode, Is.EqualTo(expectedCode));
        }
    }
}