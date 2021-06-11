using CybersecHSG.Common.Models;
using CybersecHSG.Common.Utils;
using Microsoft.Research.SEAL;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CybersecHSG.ClientConsole
{
    class Program
    {
        private static Encryptor _encryptor;
        private static Decryptor _decryptor;
        private static SEALContext _context;

        static async Task Main(string[] args)
        {
            _context = SEALUtils.GetContext();

            var keys = FitnessTrackerClient.GetKeys();
            var publicKey = SEALUtils.BuildPublicKeyFromBase64String(keys.PublicKey, _context);
            _encryptor = new Encryptor(_context, publicKey);

            var secretKey = SEALUtils.BuildSecretKeyFromBase64String(keys.SecretKey, _context);
            _decryptor = new Decryptor(_context, secretKey);

            string s = keys.SecretKey;
            //Console.WriteLine(keys.PublicKey);

            while (true)
            {
                PrintMenu();
                var option = Convert.ToInt32(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        await SendNewRun();
                        break;
                    case 2:
                        await GetMetrics();
                        break;
                }
            }
        }

        static async Task SendNewRun()
        {
            Console.Write("Enter the new running distance (km): ");
            var newRunningDistance = Convert.ToInt32(Console.ReadLine());

            if (newRunningDistance < 0)
            {
                Console.WriteLine("Running distance must be greater than 0.");
                return;
            }

            var plaintext = new Plaintext($"{newRunningDistance.ToString("X")}");
            var ciphertextDistance = new Ciphertext();
            _encryptor.Encrypt(plaintext, ciphertextDistance);

            var base64Distance = SEALUtils.CiphertextToBase64String(ciphertextDistance);



            Console.Write("Enter the new running time (hours): ");
            var newRunningTime = Convert.ToInt32(Console.ReadLine());

            if (newRunningTime < 0)
            {
                Console.WriteLine("Running time must be greater than 0.");
                return;
            }

            var plaintextTime = new Plaintext($"{newRunningTime.ToString("X")}");
            var ciphertextTime = new Ciphertext();
            _encryptor.Encrypt(plaintextTime, ciphertextTime);

            var base64Time = SEALUtils.CiphertextToBase64String(ciphertextTime);

            //var metricsRequest = new RunItem
            //{
            //    Distance = base64Distance,
            //    Time = base64Time
            //};

            //await FitnessTrackerClient.AddNewRunningDistance(metricsRequest);
        }

        private static async Task GetMetrics()
        {
            var metrics = await FitnessTrackerClient.GetMetrics();

            //var ciphertextTotalRuns = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalRuns, _context);
            var plaintextTotalRuns = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalRuns, plaintextTotalRuns);

            var ciphertextTotalDistance = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalDistance, _context);
            var plaintextTotalDistance = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalDistance, plaintextTotalDistance);

            var ciphertextTotalHours = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalHours, _context);
            var plaintextTotalHours = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalHours, plaintextTotalHours);

            PrintMetrics(plaintextTotalRuns.ToString(), plaintextTotalDistance.ToString(), plaintextTotalHours.ToString());
        }
        private static void PrintMetrics(string runs, string distance, string hours)
        {
            Console.WriteLine("********* Metrics *********");
            Console.WriteLine($"Total runs: {int.Parse(runs, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine($"Total distance: {int.Parse(distance, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine($"Total hours: {int.Parse(hours, System.Globalization.NumberStyles.HexNumber)}");
            Console.WriteLine(string.Empty);
        }
        private static void PrintMenu()
        {
            Console.WriteLine("********* Menu (enter the option number and press enter) *********");
            Console.WriteLine("1. Add running distance");
            Console.WriteLine("2. Get metrics");
            Console.Write("Option: ");
        }
    }
}
