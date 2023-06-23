using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TCP_Client_2
{
    public partial class s : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isRunning;
        private string selectedImagePath;
        private StringBuilder chatHistory;
        private SaveFileDialog saveFileDialog1;
        public s()
        {
            InitializeComponent();
            chatHistory = new StringBuilder();
            saveFileDialog1 = new SaveFileDialog();
        }

        private void s_Load(object sender, EventArgs e)
        {

        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                int port = 3000;
                client = new TcpClient();
                client.Connect(IPAddress.Loopback, port);
                stream = client.GetStream();

                AppendMessage("Connected to server.");

                // Start a new thread to receive messages from the server
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.SetApartmentState(ApartmentState.STA);
                receiveThread.Start();
                label1.Text = "Connected";
            }
            catch (SocketException)
            {
                // Handle exception
            }
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                isRunning = false;
                stream.Close();
                client.Close();
                label1.Text = "Disconnected";

                AppendMessage("Disconnected from server.");
        }
    }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                string message = textBox1.Text;

                // Check if a text message or an image is being sent
                if (!string.IsNullOrEmpty(message))
                {
                    // Send the text message to the server
                    SendMessageToServer(message);
                    AppendMessage("You: " + message);
                }
                else if (selectedImagePath != null)
                {
                    try
                    {
                        // Send the selected image to the server
                        SendImageToServer(selectedImagePath);
                        AppendMessage("You: Image sent");

                        selectedImagePath = null; // Clear the selected image path
                    }
                    catch (Exception ex)
                    {
                        AppendMessage("Error sending image: " + ex.Message);
                    }
                }
                else
                {
                    AppendMessage("Nothing to send.");
                }

                // Clear the message textbox
                textBox1.Clear();
            }
        }
        private void ReceiveMessages()
        {
            isRunning = true;
            byte[] buffer = new byte[1024];
            StringBuilder sb = new StringBuilder();
            while (isRunning)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(buffer, receivedData, bytesRead);
                        string receivedMessage = Encoding.ASCII.GetString(receivedData);

                        sb.Append(receivedMessage);

                        if (receivedMessage.EndsWith("\n")) {
                            var message = sb.ToString();

                            // check if message if image
                            if (message.StartsWith("gambar:"))
                            {
                                var base64ImageData = message.Substring("gambar:".Length);
                                DisplayDataFromBase64(base64ImageData);

                            } else
                            {

                                AppendMessage("Server: " + message);
                            }

                            sb.Clear();

                        }

                        
                    }
                }
                catch (IOException)
                {
                    // Connection closed by the server
                    AppendMessage("Disconnected from server.");
                    break;
                }
                catch (SocketException)
                {
                    // Handle exception
                }

                //Thread.Sleep(100);  // Add a small delay to prevent high CPU usage
            }
        }
        private bool IsImageData(string data)
        {
            // Check if the received data represents an image
            // You can implement your own logic here based on the image file format
            // For simplicity, this implementation checks if the first bytes match the JPEG file format

            byte[] jpegSignature = new byte[] { 0xFF, 0xD8, 0xFF };
            for (int i = 0; i < jpegSignature.Length; i++)
            {
                if (data[i] != jpegSignature[i])
                    return false;
            }

            return true;
        }
        private void DisplayDataFromBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            // Display the image in the RichTextBox control
            Clipboard.SetImage(image);
            richTextBox1.Invoke((MethodInvoker)delegate {

            richTextBox1.Paste();
            });
      
        }
        private void DisplayReceivedImage(byte[] imageData)
        {
            // Create a temporary file to store the image
            string tempImagePath = Path.Combine(Path.GetTempPath(), "ReceivedImage.jpg");

            try
            {
                // Save the received image to the temporary file
                File.WriteAllBytes(tempImagePath, imageData);

                // Load the image from the temporary file
                Image receivedImage = Image.FromFile(tempImagePath);

                // Display the image in the RichTextBox control
                Clipboard.SetImage(receivedImage);
                richTextBox1.Paste();
            }
            catch (Exception ex)
            {
                AppendMessage("Error displaying received image: " + ex.Message);
            }
            finally
            {
                // Delete the temporary file
                File.Delete(tempImagePath);
            }
        }
        private void SendMessageToServer(string message)
        {
            var paylaod = message + Environment.NewLine;
            byte[] messageBytes = Encoding.ASCII.GetBytes(paylaod);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
        private void SendImageToServer(string imagePath)
        {
            // Load the image from the specified file
            Image image = Image.FromFile(imagePath);

            // Convert the image to a byte array
            byte[] imageBytes = ImageToByteArray(image);

            string base64ImageRepresentation = Convert.ToBase64String(imageBytes);
            
            // Send the image to the server
            byte[] messageBytes = Encoding.ASCII.GetBytes(base64ImageRepresentation);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                return memoryStream.ToArray();
            }
        }

        private void AppendMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendMessage), new object[] { message });
                return;
            }
            chatHistory.AppendLine(message);
            richTextBox1.SelectionBackColor = Color.Green;
            richTextBox1.AppendText(message + Environment.NewLine);
        }
        private void SaveChatHistory(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, chatHistory.ToString());
                MessageBox.Show("Chat history saved successfully.", "Chat Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving chat history: " + ex.Message, "Chat Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Select an Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string imagePath = openFileDialog.FileName;

                    // Read the image file
                    byte[] imageBytes = File.ReadAllBytes(imagePath);

                    string imageEncoded = Convert.ToBase64String(imageBytes);

                    var payloadToSend = "gambar:" + imageEncoded + Environment.NewLine;

                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(payloadToSend);

                    // Send the image to the server
                    stream.Write(bytesToSend, 0, bytesToSend.Length);

                    AppendMessage("You: Image sent");

                    // Display the image in the RichTextBox control
                    Image selectedImage = Image.FromFile(imagePath);
                    Clipboard.SetImage(selectedImage);
                    richTextBox1.Paste();
                }
            }
        }

        private void ExportButtom_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                SaveChatHistory(filePath);
            }
        }
    }
}
