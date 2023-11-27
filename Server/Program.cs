using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;

namespace Server {
    internal class Program {

        public static string fileName = "";

        private static void TakeScreenShot() {
            try {
                // Start the process...
                Console.WriteLine("Starting the process...");
                Console.WriteLine();

                Bitmap memoryImage = new Bitmap(1920, 1080);
                Size s = new Size(memoryImage.Width, memoryImage.Height);

                Graphics memoryGraphics = Graphics.FromImage(memoryImage);

                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);

                fileName = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                          @"\Screenshot" + "_" +
                          DateTime.Now.ToString("(dd_MMMM_hh_mm_ss_tt)") + ".png");

                memoryImage.Save(fileName, ImageFormat.Png);

            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }

        }

        static void Main(string[] args) {

            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var client = new Socket(AddressFamily.InterNetwork,
                          SocketType.Dgram,
                          ProtocolType.Udp);


            var Ip = IPAddress.Parse("192.168.1.108");
            var Port = 27001;

            var remoteEP = new IPEndPoint(Ip, Port);
            listener.Bind(remoteEP);

            var msg = "";
            var len = 0;
            var buffer = new byte[1024];

            EndPoint endEP = new IPEndPoint(IPAddress.Any, 0);

            while (true) {

                len = listener.ReceiveFrom(buffer, ref endEP);

                msg = Encoding.Default.GetString(buffer, 0, len);

                if (msg == "takescreenshot") {
                    TakeScreenShot();
                    msg = fileName;
                    Console.WriteLine(fileName);
                    buffer = Encoding.Default.GetBytes(msg);

                    client.SendTo(buffer, remoteEP);
                }
            }
        }
    }
}