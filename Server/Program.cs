using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;

namespace Server {
    internal class Program {

        public static Bitmap? imageData;

        private static void TakeScreenShot() {
            try {
                // Start the process...
                Console.WriteLine("Starting the process...");
                Console.WriteLine();

                Bitmap memoryImage = new Bitmap(1920, 1080);
                Size s = new Size(memoryImage.Width, memoryImage.Height);

                Graphics memoryGraphics = Graphics.FromImage(memoryImage);

                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);

                imageData = memoryImage;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        static byte[] ConvertBitmapToByteArray(Bitmap bitmap) {
            using (MemoryStream stream = new MemoryStream()) {

                bitmap.Save(stream, ImageFormat.Jpeg);

                return stream.ToArray();
            }
        }

        static void Main(string[] args) {

            var listener = new UdpClient(27002);
            var client = new UdpClient(27001);

            var remoteEP = new IPEndPoint(IPAddress.Any, 0);

            var msg = "";
            var len = 0;

            var connectEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27003);

            while (true) {

                var buffer = listener.Receive(ref remoteEP);
                msg = Encoding.Default.GetString(buffer);

                if (msg == "takescreenshot") {

                    TakeScreenShot();

                    byte[] image = ConvertBitmapToByteArray(imageData);
                    var chunks = image.Chunk(ushort.MaxValue - 29);

                    foreach (var item in chunks) {
                        client.Send(item, item.Length, connectEP);
                    }

                    msg = "enddata";
                    var enddata = Encoding.Default.GetBytes(msg);
                    client.Send(enddata, enddata.Length, connectEP);
                }
            }
        }
    }
}