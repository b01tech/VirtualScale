using System.Globalization;
using System.IO.Ports;
using System.Text;
using VirtualScale.Domain.Entities;
using VirtualScale.Worker.Configuration;

namespace VirtualScale.Worker.Services;

public class SerialHandler(Scale scale)
{
    private SerialPort serialPort = default!;
    private StringBuilder buffer = new();
    private readonly object _lock = new();

    public void StartReading()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            Console.WriteLine("Serial já está aberta.");
            return;
        }
        serialPort = new SerialPort(AppConfiguration.SerialPort, AppConfiguration.BaudRate);
        serialPort.DataReceived += SerialPort_DataReceived;
        serialPort.Open();
        Console.WriteLine($"Serial port {AppConfiguration.SerialPort} opened successfully");
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string data = serialPort.ReadExisting();
            lock (_lock)
            {
                buffer.Append(data);
                ProcessBuffer();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro leitura: {ex.Message}");
        }
    }

    private void ProcessBuffer()
    {
        while (true)
        {
            var content = buffer.ToString();
            var index = content.IndexOf("\r\n", StringComparison.Ordinal);

            if (index < 0)
                break;

            var line = content.Substring(0, index).Trim();
            buffer.Remove(0, index + 2);

            if (!string.IsNullOrWhiteSpace(line))
            {
                var (id, value) = WeightParse(line);
                if (id > 0)
                    scale.UpdateLoadCell(id, value);
            }
        }
    }

    public void StopReading()
    {
        if (!serialPort.IsOpen)
            return;
        serialPort.Close();
        serialPort.Dispose();
    }

    public static (int id, decimal value) WeightParse(string data)
    {
        // Formato
        // {id}:{constante celula}{0D}{0A}
        try
        {
            var id = int.Parse(data.Split(':')[0].Trim());
            var value = decimal.Parse(data.Split(':')[1].Trim(), CultureInfo.InvariantCulture);
            return (id, value);
        }
        catch
        {
            return (-1, 0);
        }
    }
}
