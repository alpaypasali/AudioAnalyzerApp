# 🎧 AudioAnalyzerApp

Bu C# konsol uygulaması, .WAV ses dosyalarını analiz eder:

- Sessizlik anlarını tespit eder
- Sessiz ve sesli blokları ayrı ayrı `.wav` dosyası olarak kaydeder
- Desibel-zaman grafiği oluşturur (`output.png`)

## Kullanılan Kütüphaneler
- [NAudio](https://github.com/naudio/NAudio) — ses işleme
- [ScottPlot](https://scottplot.net) — grafik çizimi

## Başlatma

```bash
dotnet run
