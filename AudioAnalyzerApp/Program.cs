using NAudio.Wave;
using ScottPlot;

string fileDirectory = Path.Combine("Resources", "deneme2.wav");
string outputFolder = @"E:\Goreveler\AudioAnalyzerApp\AudioAnalyzerApp\Silences";
string outputFolderSpeak = @"E:\Goreveler\AudioAnalyzerApp\AudioAnalyzerApp\Speak";
Directory.CreateDirectory(outputFolder);


if (!File.Exists(fileDirectory))
{
    Console.WriteLine("Ses dosyası bulunamadı.");
    return;
}


using var reader = new AudioFileReader(fileDirectory);
var sampleProvider = reader.ToSampleProvider();

int sampleRate = reader.WaveFormat.SampleRate;
int channels = reader.WaveFormat.Channels;


//  Her blok 1 saniyelik olsun
int blockDurationMs = 1500;
int blockSize = (sampleRate * channels * blockDurationMs) / 1000;
float[] buffer = new float[blockSize];

double currentTime = 0;
int blockIndex = 0;
float silenceThresholdDb = -35;

var dBList = new List<double>();
var timeList = new List<double>();

while (sampleProvider.Read(buffer, 0, blockSize) > 0)
{
    // Ortalama genliği hesapla
    float avgAmplitude = buffer.Select(Math.Abs).Average();
    float dB = 20 * (float)Math.Log10(avgAmplitude + float.Epsilon);

    timeList.Add(currentTime);
    dBList.Add(dB);

    if (dB < silenceThresholdDb)
    {
        string fileName = Path.Combine(outputFolder,
     $"silence_{blockDurationMs / 1000}s_start_{currentTime:F2}s.wav");
        SaveBufferToWav(buffer, sampleRate, channels, fileName);

    
        Console.WriteLine($"✓ Sessizlik kaydedildi: {fileName}");
    }

    if (dB > silenceThresholdDb)
    {
        string fileName = Path.Combine(outputFolderSpeak,
     $"speak_{blockDurationMs / 1000}s_start_{currentTime:F2}s.wav");
        SaveBufferToWav(buffer, sampleRate, channels, fileName);


        Console.WriteLine($"✓ Konusma kaydedildi: {fileName}");
    }

    currentTime += blockDurationMs / 1000.0;
    blockIndex++;
}
//  dB grafiği çiz ve proje köküne kaydet
if (timeList.Count > 0)
{
    string outputImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\", "Resources", "output.png");
    outputImagePath = Path.GetFullPath(outputImagePath);
    Directory.CreateDirectory(Path.GetDirectoryName(outputImagePath)!);
    var plt = new ScottPlot.Plot(800, 400);
    plt.AddScatter(timeList.ToArray(), dBList.ToArray());
    plt.Title("Ses Şiddeti (dB) Zaman Grafiği");
    plt.XLabel("Zaman (saniye)");
    plt.YLabel("Desibel (dB)");
    plt.SaveFig(outputImagePath);
    Console.WriteLine("✅ Grafik kaydedildi: Resources/output.png");
}
else
{
    Console.WriteLine("⚠️ Grafik çizilemedi: timeList boş.");
}
//float[] buffer’ı .wav dosyasına dönüştüren yardımcı metod
static void SaveBufferToWav(float[] buffer, int sampleRate, int channels, string path)
{
    var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
    using var writer = new WaveFileWriter(path, waveFormat);
    writer.WriteSamples(buffer, 0, buffer.Length);
}
