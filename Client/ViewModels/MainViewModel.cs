using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Input;
using Client.Commands;

namespace Client.ViewModels;

public class MainViewModel {

    // Private Fields

    private string fileName;

    // Binding Properties

    public ICommand? ScreenshotButtonCommand { get; set; }

    // Constructor

    public MainViewModel() {

        SetCommands();
        ListenClientAsync();
    }

    // Functions

    private void SetCommands() {

        ScreenshotButtonCommand = new RelayCommand(TakeScreenshot);
    }

    private void TakeScreenshot(object? param) {

        var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        var Ip = IPAddress.Parse("192.168.1.108");
        var Port = 27001;

        var remoteEP = new IPEndPoint(Ip, Port);

        var msg = "takescreenshot";
        var len = 0;
        var buffer = Array.Empty<byte>();


        buffer = Encoding.Default.GetBytes(msg);
        client.SendToAsync(buffer, SocketFlags.None, remoteEP);
    }

    private async void ListenClientAsync() {

        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        var Ip = IPAddress.Parse("192.168.1.108");
        var Port = 27002;

        var remoteEP = new IPEndPoint(Ip, Port);
        listener.Bind(remoteEP);

        var msg = "";
        var len = 0;
        var buffer = Array.Empty<byte>();

        EndPoint remoteep = new IPEndPoint(IPAddress.Any, 0);

        await Task.Run(() => {
            while (true) {

                len = listener.ReceiveFrom(buffer, ref remoteep);
                msg = Encoding.Default.GetString(buffer, 0, len);
            
            }
        });
    }
}
