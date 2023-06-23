using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices.ComTypes;

namespace CA_Server
{
    public partial class Client : System.Windows.Forms.Form
    {
        private bool isRunning;
        private bool connected = false;
        private Thread client = null;
        private struct MyClient
        {
            public string username;
            public string key;
            public TcpClient client;
            public NetworkStream stream;
            public byte[] buffer;
            public StringBuilder data;
            public EventWaitHandle handle;
        };

        private MyClient obj;
        private Task send = null;
        private bool exit = false;
        private string selectedImagePath;

        public Client()
        {
            InitializeComponent();
        }

        private void Log(string msg = "") // clear the log if message is not supplied or is empty
        {
            
            if (!exit)
            {
                logBox.Invoke((MethodInvoker)delegate
                {
                    if (msg.Length > 0)
                    {
                        logBox.AppendText(string.Format("[ {0} ] {1}{2}", DateTime.Now.ToString("HH:mm"), msg, Environment.NewLine));
                    }
                    else
                    {
                        logBox.Clear();
                    }
                });
            }
            
        }

        private string ErrorMsg(string msg)
        {
            return string.Format("ERROR: {0}", msg);
        }

        private string SystemMsg(string msg)
        {
            return string.Format("SYSTEM: {0}", msg);
        }

        private void Connected(bool status)
        {
            if (!exit)
            {
                connectButton.Invoke((MethodInvoker)delegate
                {
                    connected = status;
                    if (status)
                    {
                        addBox.Enabled = false;
                        portBox.Enabled = false;
                        unBox.Enabled = false;
                        keyBox.Enabled = false;
                        connectButton.Text = "Disconnect";
                        connectButton.BackColor = Color.Red;
                        Log(SystemMsg("You are now connected"));
                    }
                    else
                    {
                        addBox.Enabled = true;
                        portBox.Enabled = true;
                        unBox.Enabled = true;
                        keyBox.Enabled = true;
                        connectButton.Text = "Connect";
                        connectButton.BackColor = Color.FromArgb(144, 238, 144);
                        Log(SystemMsg("You are now disconnected"));
                        
                    }
                });
            }
        }

        private void Read(IAsyncResult result)
        {
            int bytes = 0;
            if (obj.client.Connected)
            {
                try
                {
                    bytes = obj.stream.EndRead(result);
                }
                catch (Exception ex)
                {
                    Log(ErrorMsg(ex.Message));
                }
            }
            if (bytes > 0)
            {
                obj.data.AppendFormat("{0}", Encoding.UTF8.GetString(obj.buffer, 0, bytes));
                try
                {
                    if (obj.stream.DataAvailable)
                    {
                        obj.stream.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(Read), null);
                    }
                    else
                    {
                        Log(obj.data.ToString());
                        obj.data.Clear();
                        obj.handle.Set();
                    }
                }
                catch (Exception ex)
                {
                    obj.data.Clear();
                    Log(ErrorMsg(ex.Message));
                    obj.handle.Set();
                }
            }
            else
            {
                obj.client.Close();
                obj.handle.Set();
            }
        }

        private void ReadAuth(IAsyncResult result) //Purpose: Authentication
        {
            int bytes = 0;
            if (obj.client.Connected)
            {
                try
                {
                    bytes = obj.stream.EndRead(result);
                }
                catch (Exception ex)
                {
                    Log(ErrorMsg(ex.Message));
                }
            }
            if (bytes > 0)
            {
                obj.data.AppendFormat("{0}", Encoding.UTF8.GetString(obj.buffer, 0, bytes));
                try
                {
                    if (obj.stream.DataAvailable)
                    {
                        obj.stream.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(ReadAuth), null);
                    }
                    else
                    {
                        JavaScriptSerializer json = new JavaScriptSerializer(); // feel free to use JSON serializer
                        Dictionary<string, string> data = json.Deserialize<Dictionary<string, string>>(obj.data.ToString());
                        if (data.ContainsKey("status") && data["status"].Equals("authorized"))
                        {
                            Connected(true);
                        }
                        obj.data.Clear();
                        obj.handle.Set();
                    }
                }
                catch (Exception ex)
                {
                    obj.data.Clear();
                    Log(ErrorMsg(ex.Message));
                    obj.handle.Set();
                }
            }
            else
            {
                obj.client.Close();
                obj.handle.Set();
            }
        }

        private bool Authorize()
        {
            bool success = false;
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("username", obj.username);
            data.Add("key", obj.key);
            JavaScriptSerializer json = new JavaScriptSerializer(); // feel free to use JSON serializer
            Send(json.Serialize(data));
            while (obj.client.Connected)
            {
                try
                {
                    obj.stream.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(ReadAuth), null);
                    obj.handle.WaitOne();
                    if (connected)
                    {
                        success = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log(ErrorMsg(ex.Message));
                }
            }
            if (!connected)
            {
                Log(SystemMsg("Unauthorized"));
            }
            return success;
        }

        private void Connection(IPAddress ip, int port, string username, string key)
        {
            try
            {
                obj = new MyClient();
                obj.username = username;
                obj.key = key;
                obj.client = new TcpClient();
                obj.client.Connect(ip, port);
                obj.stream = obj.client.GetStream();
                obj.buffer = new byte[obj.client.ReceiveBufferSize];
                obj.data = new StringBuilder();
                obj.handle = new EventWaitHandle(false, EventResetMode.AutoReset);
                if (Authorize())
                {
                    while (obj.client.Connected)
                    {
                        try
                        {
                            obj.stream.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(Read), null);
                            obj.handle.WaitOne();
                        }
                        catch (Exception ex)
                        {
                            Log(ErrorMsg(ex.Message));
                        }
                    }
                    obj.client.Close();
                    Connected(false);
                }
            }
            catch (Exception ex)
            {
                Log(ErrorMsg(ex.Message));
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                obj.client.Close();
            }
            else if (client == null || !client.IsAlive)
            {
                string address = addBox.Text.Trim();
                string number = portBox.Text.Trim();
                string username = unBox.Text.Trim();
                bool error = false;
                IPAddress ip = null;
                if (address.Length < 1)
                {
                    error = true;
                    Log(SystemMsg("Address is required"));
                }
                else
                {
                    try
                    {
                        ip = Dns.Resolve(address).AddressList[0];
                    }
                    catch
                    {
                        error = true;
                        Log(SystemMsg("Address is not valid"));
                    }
                }
                int port = -1;
                if (number.Length < 1)
                {
                    error = true;
                    Log(SystemMsg("Port number is required"));
                }
                else if (!int.TryParse(number, out port))
                {
                    error = true;
                    Log(SystemMsg("Port number is not valid"));
                }
                else if (port < 0 || port > 65535)
                {
                    error = true;
                    Log(SystemMsg("Port number is out of range"));
                }
                if (username.Length < 1)
                {
                    error = true;
                    Log(SystemMsg("Username is required"));
                }
                if (!error)
                {
                    // encryption key is optional
                    client = new Thread(() => Connection(ip, port, username, keyBox.Text))
                    {
                        IsBackground = true
                    };
                    client.Start();
                }
            }
        }

        private void Write(IAsyncResult result) //Purpose: KIV
        {
            if (obj.client.Connected)
            {
                try
                {
                    obj.stream.EndWrite(result);
                }
                catch (Exception ex)
                {
                    Log(ErrorMsg(ex.Message));
                }
            }
        }

        private void BeginWrite(string msg) // Send the message to a specific client
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            if (obj.client.Connected)
            {
                try
                {
                    obj.stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(Write), null);
                }
                catch (Exception ex)
                {
                    Log(ErrorMsg(ex.Message));
                }
            }
        }

        private void Send(string msg)
        {
            if (send == null || send.IsCompleted)
            {
                send = Task.Factory.StartNew(() => BeginWrite(msg));
            }
            else
            {
                send.ContinueWith(antecendent => BeginWrite(msg));
            }
        }


        private void SendBox_KeyDown(object sender, KeyEventArgs e) //Press Enter to send
        {
            
             if (e.KeyCode == Keys.Enter)
             {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (sendBox.Text.Length > 0)
                {
                    string msg = sendBox.Text;
                    sendBox.Clear();
                    Log(string.Format("{0} (You): {1}", obj.username, msg));
                    if (connected)
                    {
                        Send(msg);
                    }
                }
             }

        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            exit = true;
            if (connected)
            {
                obj.client.Close();
            }
        }

        //Separation 2: Disconnect Button click

        
        private void clearButton_Click(object sender, EventArgs e)
        {
            Log();
        }

        private void hideCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (keyBox.PasswordChar == '*')
            {
                keyBox.PasswordChar = '\0';
            }
            else
            {
                keyBox.PasswordChar = '*';
            }
        }


        private void exportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog.Title = "Export Chat";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    File.WriteAllText(filePath, logBox.Text);
                    MessageBox.Show("Chat exported successfully!", "Export Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while exporting the chat: {ex.Message}", "Export Chat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                string message = sendBox.Text;

                // Check if a text message or an image is being sent
                if (!string.IsNullOrEmpty(message))
                {
                    // Send the text message to the server
                    SendMessageToServer(message);
                    Log("You: " + message);
                }
                else if (selectedImagePath != null)
                {
                    try
                    {
                        // Send the selected image to the server
                        SendImageToServer(selectedImagePath);
                        Log("You: Image sent");

                        selectedImagePath = null; // Clear the selected image path
                    }
                    catch (Exception ex)
                    {
                        Log("Error sending image: " + ex.Message);
                    }
                }
                else
                {
                    Log("Nothing to send.");
                }

                // Clear the message textbox
                sendBox.Clear();
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
                    if (obj.stream.DataAvailable)
                    {
                        int bytesRead = obj.stream.Read(buffer, 0, buffer.Length);
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(buffer, receivedData, bytesRead);
                        string receivedMessage = Encoding.ASCII.GetString(receivedData);

                        sb.Append(receivedMessage);

                        if (receivedMessage.EndsWith("\n"))
                        {
                            var message = sb.ToString();

                            // check if message if image
                            if (message.StartsWith("gambar:"))
                            {
                                var base64ImageData = message.Substring("gambar:".Length);
                                DisplayDataFromBase64(base64ImageData);

                            }
                            else
                            {

                                Log("Server: " + message);
                            }

                            sb.Clear();

                        }


                    }
                }
                catch (IOException)
                {
                    // Connection closed by the server
                    Log("Disconnected from server.");
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
            logBox.Invoke((MethodInvoker)delegate {

                logBox.Paste();
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
                logBox.Paste();
            }
            catch (Exception ex)
            {
                Log("Error displaying received image: " + ex.Message);
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
            obj.stream.Write(messageBytes, 0, messageBytes.Length);
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
            obj.stream.Write(messageBytes, 0, messageBytes.Length);
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                return memoryStream.ToArray();
            }
        }

        private void imageButton_Click(object sender, EventArgs e)
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
                    obj.stream.Write(bytesToSend, 0, bytesToSend.Length);

                    Log("You: Image sent");

                    // Display the image in the RichTextBox control
                    Image selectedImage = Image.FromFile(imagePath);
                    Clipboard.SetImage(selectedImage);
                    logBox.Paste();
                }
            }
        }
    }

}

