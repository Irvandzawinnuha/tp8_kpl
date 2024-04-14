using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        double suhuBadan;
        int hariDemam;

        CovidConfig defaultConf = new CovidConfig("celcius", 14, "Anda tidak diperbolehkan masuk ke gedung ini", "Anda dipersilakan untuk masuk ke gedung ini");

        Console.Write("Berapa suhu badan Anda saat ini? ");
        suhuBadan = Convert.ToDouble(Console.ReadLine());
        Console.Write("Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam? ");
        hariDemam = Convert.ToInt32(Console.ReadLine());

        bool tepatWaktu = hariDemam < defaultConf.batas_hari_demam;
        bool terimaFahrenheit = (defaultConf.satuan_suhu == "fahrenheit") &&
                                (suhuBadan >= 97.7 && suhuBadan <= 99.5);
        bool terimaCelcius = (defaultConf.satuan_suhu == "celcius") &&
                                (suhuBadan >= 36.5 && suhuBadan <= 37.5);

        if (tepatWaktu && (terimaCelcius || terimaFahrenheit))
        {
            Console.WriteLine(defaultConf.pesan_diterima);
        }
        else
        {
            Console.WriteLine(defaultConf.pesan_ditolak);
        }
    }

    public class CovidConfig
    {
        public string satuan_suhu { get; set; }
        public int batas_hari_demam { get; set; }
        public string pesan_ditolak { get; set; }
        public string pesan_diterima { get; set; }

        public CovidConfig() { }
        public CovidConfig(string satuan_suhu, int batas_hari_demam, string pesan_ditolak, string pesan_diterima)
        {
            this.satuan_suhu = satuan_suhu;
            this.batas_hari_demam = batas_hari_demam;
            this.pesan_ditolak = pesan_ditolak;
            this.pesan_diterima = pesan_diterima;
        }

        public class CovidData
        {
            public CovidConfig covidConf;
            public const string filePath = @"covidconfig.json";

            public CovidData()
            {
                try
                {
                    ReadConfig();
                }
                catch (FileNotFoundException e)
                {
                    SetDefault();
                    WriteNewConfig();
                }
                catch (JsonException e)
                {
                    Console.WriteLine("Error parsing JSON file: " + e.Message);
                    SetDefault();
                    WriteNewConfig();
                }
            }

            public void UbahSatuan(string satuanBaru)
            {
                bool satuanValid = (satuanBaru == "celcius" || satuanBaru == "fahrenheit");

                if (satuanBaru == null || !satuanValid)
                {
                    throw new ArgumentException("Satuan suhu yang dimasukkan tidak valid. Satuan yang diizinkan adalah 'celcius' atau 'fahrenheit'");
                }

                if (satuanValid)
                {
                    covidConf.satuan_suhu = satuanBaru;

                    JsonSerializerOptions opts = new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    };

                    string updateSuhu = JsonSerializer.Serialize(covidConf, opts);
                    File.WriteAllText(filePath, updateSuhu);
                }
            }

            private void ReadConfig()
            {
                string jsonString = File.ReadAllText(filePath);
                covidConf = JsonSerializer.Deserialize<CovidConfig>(jsonString);
            }

            private void SetDefault()
            {
                covidConf = new CovidConfig("celcius", 14, "Anda tidak diperbolehkan masuk ke gedung ini", "Anda dipersilakan untuk masuk ke gedung ini");
            }

            private void WriteNewConfig()
            {
                JsonSerializerOptions opts = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                };

                string configJson = JsonSerializer.Serialize(covidConf, opts);
                File.WriteAllText(filePath, configJson);
            }
        }
    }
}