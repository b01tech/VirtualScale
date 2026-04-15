using System.Globalization;
using System.IO.Ports;
using System.Text;
using VirtualScale.Domain.Entities;
using VirtualScale.Worker.Configuration;

namespace VirtualScale.Worker.Services;

public class SerialHandler(Scale scale)
{
    private SerialPort? serialPort;
    private readonly StringBuilder buffer = new();
    private readonly object _lock = new();

    public string? DesiredPort { get; private set; } = AppConfiguration.SerialPort;
    public string? ConnectedPort { get; private set; }
    public bool DesiredConnected { get; private set; } = true;
    public SerialConnectionState State { get; private set; } = SerialConnectionState.Disconnected;
    public string? LastError { get; private set; }

    public IReadOnlyList<string> GetAvailablePorts()
    {
        return SerialPort.GetPortNames().OrderBy(p => p).ToArray();
    }

    public void RequestConnect(string? portName)
    {
        if (!string.IsNullOrWhiteSpace(portName))
        {
            DesiredPort = portName.Trim();
        }
        DesiredConnected = true;
    }

    public void RequestDisconnect()
    {
        DesiredConnected = false;
        Disconnect();
        State = SerialConnectionState.Disconnected;
        LastError = null;
    }

    public bool IsConnected => serialPort is not null && serialPort.IsOpen;

    public bool EnsureConnected()
    {
        if (!DesiredConnected)
        {
            return false;
        }

        if (IsConnected)
        {
            State = SerialConnectionState.Connected;
            LastError = null;
            return true;
        }

        if (string.IsNullOrWhiteSpace(DesiredPort))
        {
            State = SerialConnectionState.Error;
            LastError = "Porta serial não definida";
            return false;
        }

        try
        {
            State = SerialConnectionState.Connecting;
            LastError = null;

            Disconnect();
            serialPort = new SerialPort(DesiredPort, AppConfiguration.BaudRate)
            {
                ReadTimeout = AppConfiguration.Timeout,
                WriteTimeout = AppConfiguration.Timeout,
                NewLine = "\r\n"
            };
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();

            ConnectedPort = DesiredPort;
            State = SerialConnectionState.Connected;
            return true;
        }
        catch (Exception ex)
        {
            ConnectedPort = null;
            State = SerialConnectionState.Error;
            LastError = ex.Message;
            Disconnect();
            return false;
        }
    }

    private void SerialPort_DataReceived(object? sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            var port = serialPort;
            if (port is null || !port.IsOpen)
            {
                return;
            }

            string data = port.ReadExisting();
            lock (_lock)
            {
                buffer.Append(data);
                ProcessBuffer();
            }
        }
        catch (Exception ex)
        {
            State = SerialConnectionState.Error;
            LastError = ex.Message;
            Disconnect();
        }
    }

    private void ProcessBuffer()
    {
        while (true)
        {
            var content = buffer.ToString();
            var index = content.IndexOf("\r\n", StringComparison.Ordinal);

            if (index < 0)
            {
                break;
            }

            var line = content.Substring(0, index).Trim();
            buffer.Remove(0, index + 2);

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("----", StringComparison.Ordinal))
            {
                continue;
            }

            var (id, value) = WeightParse(line);
            if (id > 0)
            {
                scale.UpdateLoadCell(id, value);
            }
        }
    }

    private void Disconnect()
    {
        try
        {
            if (serialPort is null)
            {
                return;
            }

            serialPort.DataReceived -= SerialPort_DataReceived;
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            serialPort.Dispose();
        }
        catch
        {
        }
        finally
        {
            serialPort = null;
            ConnectedPort = null;
        }
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
