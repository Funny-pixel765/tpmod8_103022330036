using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using tp8_103022330036;

namespace tp8_103022330036
{
    public class CovidConfig
    {
        public static string ConfigPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "covidConfig.json");

        [JsonPropertyName("satuan_suhu")]
        public string SatuanSuhu { get; set; } = "Celsius";

        [JsonPropertyName("batas_hari_demam")]
        public int BatasHariDemam { get; set; }

        [JsonPropertyName("pesan_ditolak")]
        public string PesanDitolak { get; set; }

        [JsonPropertyName("pesan_diterima")]
        public string PesanDiterima { get; set; }

        public CovidConfig()
        {
            setDefault();
        }

        public CovidConfig(string satuanSuhu, int batasHariDemam, string pesanDitolak, string pesanDiterima)
        {
            this.SatuanSuhu = satuanSuhu;
            this.BatasHariDemam = batasHariDemam;
            this.PesanDitolak = pesanDitolak;
            this.PesanDiterima = pesanDiterima;
        }

        public void UbahSatuan()
        {
            if (this.SatuanSuhu == "Celsius")
            {
                SatuanSuhu = "Fahrenheit";
            }
            else
            {
                SatuanSuhu = "Celsius";
            }
            SaveNewConfig();
        }

        public void loadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                try
                {
                    string json = File.ReadAllText(ConfigPath);
                    CovidConfig config = JsonSerializer.Deserialize<CovidConfig>(json);
                    this.SatuanSuhu = config.SatuanSuhu;
                    this.BatasHariDemam = config.BatasHariDemam;
                    this.PesanDitolak = config.PesanDitolak;
                    this.PesanDiterima = config.PesanDiterima;

                 
                    Console.WriteLine("--- Loaded Config Values ---");
                    Console.WriteLine($"SatuanSuhu: {this.SatuanSuhu}");
                    Console.WriteLine($"BatasHariDemam: {this.BatasHariDemam}");
                    Console.WriteLine($"PesanDitolak: {this.PesanDitolak}");
                    Console.WriteLine($"PesanDiterima: {this.PesanDiterima}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Terjadi error saat memuat config!! " + ex.Message);
                    setDefault(); 
                }
            }
            else
            {
                SaveNewConfig();
                Console.WriteLine("File config tidak ditemukan, file baru telah dibuat.");
            }
        }

        public void SaveNewConfig()
        {
            try
            {
                string json = JsonSerializer.Serialize(this);
                File.WriteAllText(ConfigPath, json);
                Console.WriteLine("File config telah disimpan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi error, gagal menyimpan konfig baru!! " + ex.Message);
            }
        }

        public void setDefault()
        {
            this.SatuanSuhu = "Celsius";
            this.BatasHariDemam = 14;
            this.PesanDiterima = "Anda diperbolehkan masuk ke dalam gedung ini.";
            this.PesanDitolak = "Anda tidak diperbolehkan masuk ke dalam gedung ini.";
        }

        public double ConvertToCelsius(double suhu)
        {
            if (SatuanSuhu == "Fahrenheit")
            {
                return (suhu - 32) * 5 / 9;
            }
            return suhu;
        }

        public bool IsSuhuAman(double suhu)
        {
            double suhuCelsius = ConvertToCelsius(suhu);
            return suhuCelsius >= 36.5 && suhuCelsius <= 37.5;
        }

        public bool IsHariDemamAman(int hariDemam)
        {
            return hariDemam >= this.BatasHariDemam;
        }

        public bool IsConditionSafe(double suhu, int hariDemam)
        {
            bool suhuAman = IsSuhuAman(suhu);
            bool hariAman = IsHariDemamAman(hariDemam);
            return suhuAman && hariAman;
        }

        public string GetMessage(string status)
        {
            if (status == "ditolak")
            {
                return this.PesanDitolak;
            }
            else
            {
                return this.PesanDiterima;
            }
        }
    }
}

class Program
{
    static void Main()
    {
        CovidConfig cfg = new CovidConfig();
        cfg.loadConfig();

        Console.WriteLine("=== Konsultasi Kesehatan ===");
        Console.WriteLine($"Satuan suhu saat ini: {cfg.SatuanSuhu}");
        Console.Write("Apakah Anda ingin mengganti satuan suhu? (y/n): ");
        string inputUbah = Console.ReadLine().ToLower();
        if (inputUbah == "y")
        {
            cfg.UbahSatuan();
            Console.WriteLine($"Satuan suhu sekarang: {cfg.SatuanSuhu}");
        }

        Console.WriteLine("\n--- Pertanyaan Kesehatan ---");
        Console.Write($"Masukkan suhu badan Anda ({cfg.SatuanSuhu}): ");
        double bodyTemp = double.Parse(Console.ReadLine());
        Console.Write("Sudah berapa hari sejak Anda terakhir demam? ");
        int feverDays = int.Parse(Console.ReadLine());

    
        Console.WriteLine("\n--- Debug Info ---");
        Console.WriteLine($"Suhu yang dimasukkan: {bodyTemp} {cfg.SatuanSuhu}");
        Console.WriteLine($"Suhu setelah konversi: {cfg.ConvertToCelsius(bodyTemp)} Celsius");
        Console.WriteLine($"Batas Hari Demam dari config: {cfg.BatasHariDemam}");
        Console.WriteLine($"Hari sejak demam: {feverDays}");

        bool suhuAman = cfg.IsSuhuAman(bodyTemp);
        bool hariAman = cfg.IsHariDemamAman(feverDays);
        Console.WriteLine($"Suhu aman: {suhuAman}");
        Console.WriteLine($"Hari demam aman: {hariAman}");

    
        bool isAllowed = cfg.IsConditionSafe(bodyTemp, feverDays);
        Console.WriteLine($"Final result (isAllowed): {isAllowed}");

    
        Console.WriteLine("\n--- Hasil Konsultasi ---");
        if (isAllowed)
        {
            Console.WriteLine($"Message being displayed (PesanDiterima): {cfg.PesanDiterima}");
            Console.WriteLine(cfg.PesanDiterima);
        }
        else
        {
            Console.WriteLine($"Message being displayed (PesanDitolak): {cfg.PesanDitolak}");
            Console.WriteLine(cfg.PesanDitolak);
        }
    }
}