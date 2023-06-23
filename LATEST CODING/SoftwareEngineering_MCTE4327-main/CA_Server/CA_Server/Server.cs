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
using System.Runtime.InteropServices.ComTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CA_Server
{
    public partial class Server : System.Windows.Forms.Form
    {

        private bool active = false;
        private Thread listener = null;
        private long id = 0;
        private string selectedImagePath;


        private struct MyClient
        {
            public long id;
            public StringBuilder username;
            public TcpClient client;
            public NetworkStream stream;
            public byte[] buffer;
            public StringBuilder data;
            public EventWaitHandle handle;
        };

        private MyClient obj;
        private ConcurrentDictionary<long, MyClient> clients = new ConcurrentDictionary<long, MyClient>();
        private Task send = null;
        private Thread disconnect = null;
        private bool exit = false;


        public Server()
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

        private void Active(bool status)
        {
            if (!exit)
            {
                startButton.Invoke((MethodInvoker)delegate
                {
                    active = status;
                    if (status)
                    {
                        addBox.Enabled = false;
                        portBox.Enabled = false;
                        unBox.Enabled = false;
                        keyBox.Enabled = false;
                        startButton.Text = "Stop";
                        startButton.BackColor = Color.Red;
                        Log(SystemMsg("Server has started"));
                    }
                    else
                    {
                        addBox.Enabled = true;
                        portBox.Enabled = true;
                        unBox.Enabled = true;
                        keyBox.Enabled = true;
                        startButton.Text = "Start";
                        startButton.BackColor = Color.FromArgb(144, 238, 144);
                        Log(SystemMsg("Server has stopped"));
                        Disconnect();
                       
                    }
                });
            }
        }

      

        private void Read(IAsyncResult result) // Purpose: KIV
        {
            MyClient obj = (MyClient)result.AsyncState;
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
                        obj.stream.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(Read), obj);

                    }
                    else
                    {
                        string msg = string.Format("{0}: {1}", obj.username, obj.data);
                        Log(msg);
                        Send(msg, obj.id);
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
            MyClient obj = (MyClient)result.AsyncState;
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
                        obj.stream.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(ReadAuth), obj);
                    }
                    else
                    {
                        JavaScriptSerializer json = new JavaScriptSerializer(); 
                        Dictionary<string, string> data = json.Deserialize<Dictionary<string, string>>(obj.data.ToString());
                        if (!data.ContainsKey("username") || data["username"].Length < 1 || !data.ContainsKey("key") || !data["key"].Equals(keyBox.Text))
                        {
                            obj.client.Close();
                        }
                        else
                        {
                            obj.username.Append(data["username"].Length > 200 ? data["username"].Substring(0, 200) : data["username"]);
                            Send("{\"status\": \"authorized\"}", obj);
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

        private bool Authorize(MyClient obj) //Purpose: Authentication
        {
            bool success = false;
            while (obj.client.Connected)
            {
                try
                {
                    obj.stream.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(ReadAuth), obj);
                    obj.handle.WaitOne();
                    if (obj.username.Length > 0)
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
            return success;
        }

        
        private void Connection(MyClient obj) //Purpose: Check connection
        {
            if (Authorize(obj))
            {
                //clients.TryAdd(obj.id, obj);
  
                string msg = string.Format("{0} has connected", obj.username);
                Log(SystemMsg(msg));
                Send(SystemMsg(msg), obj.id);
                while (obj.client.Connected)
                {
                    try
                    {
                        obj.stream.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(Read), obj);
                        obj.handle.WaitOne();
                    }
                    catch (Exception ex)
                    {
                        Log(ErrorMsg(ex.Message));
                    }
                }
                obj.client.Close();
                clients.TryRemove(obj.id, out MyClient tmp);
                msg = string.Format("{0} has disconnected", tmp.username);
                Log(SystemMsg(msg));
                Send(msg, tmp.id);
            }
        }
        

        private void Listener(IPAddress ip, int port) //Purpose: KIV
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(ip, port);
                listener.Start();
                Active(true);
                while (active)
                {
                    if (listener.Pending())
                    {
                        try
                        {
                            MyClient obj = new MyClient();
                            obj.id = id;
                            obj.username = new StringBuilder();
                            obj.client = listener.AcceptTcpClient();
                            obj.stream = obj.client.GetStream();
                            obj.buffer = new byte[obj.client.ReceiveBufferSize];
                            obj.data = new StringBuilder();
                            obj.handle = new EventWaitHandle(false, EventResetMode.AutoReset);
                            Thread th = new Thread(() => Connection(obj))
                            {
                                IsBackground = true
                            };
                            th.Start();
                            id++;
                        }
                        catch (Exception ex)
                        {
                            Log(ErrorMsg(ex.Message));
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
                Active(false);
            }
            catch (Exception ex)
            {
                Log(ErrorMsg(ex.Message));
            }
            finally
            {
                if (listener != null)
                {
                    listener.Server.Close();
                }
            }
        }

        //Separation 1: Start Button 

        private void Write(IAsyncResult result) //Purpose: KIV
        {
            MyClient obj = (MyClient)result.AsyncState;
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

        private void BeginWrite(string msg, MyClient obj) // Send the message to a specific client
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            if (obj.client.Connected)
            {
                try
                {
                    obj.stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(Write), obj);
                }
                catch (Exception ex)
                {
                    Log(ErrorMsg(ex.Message));
                }
            }
        }

        private void BeginWrite(string msg, long id = -1) // send the message to everyone except the sender or set ID to lesser than zero to send to everyone
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            foreach (KeyValuePair<long, MyClient> obj in clients)
            {
                if (id != obj.Value.id && obj.Value.client.Connected)
                {
                    try
                    {
                        obj.Value.stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(Write), obj.Value);
                    }
                    catch (Exception ex)
                    {
                        Log(ErrorMsg(ex.Message));
                    }
                }
            }
        }

        private void Send(string msg, MyClient obj) //Duplication
        {
            if (send == null || send.IsCompleted)
            {
                send = Task.Factory.StartNew(() => BeginWrite(msg, obj));
            }
            else
            {
                send.ContinueWith(antecendent => BeginWrite(msg, obj));
            }
        }

        private void Send(string msg, long id = -1) //Duplication
        {
            if (send == null || send.IsCompleted)
            {
                send = Task.Factory.StartNew(() => BeginWrite(msg, id));
            }
            else
            {
                send.ContinueWith(antecendent => BeginWrite(msg, id));
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
                    Log(string.Format("{0} (You): {1}", unBox.Text.Trim(), msg));
                    Send(string.Format("{0}: {1}", unBox.Text.Trim(), msg));
                }
            }
        }

        private void Disconnect(long id = -1) // isconnect everyone if ID is not supplied or is lesser than zero
        {
            if (disconnect == null || !disconnect.IsAlive)
            {
                disconnect = new Thread(() =>
                {
                    if (id >= 0)
                    {
                        clients.TryGetValue(id, out MyClient obj);
                        obj.client.Close();
                       
                    }
                    else
                    {
                        foreach (KeyValuePair<long, MyClient> obj in clients)
                        {
                            obj.Value.client.Close();
                        
                        }
                    }
                })
                {
                    IsBackground = true
                };
                disconnect.Start();
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (active)
            {
                active = false;
            }
            else if (listener == null || !listener.IsAlive)
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
                    listener = new Thread(() => Listener(ip, port))
                    {
                        IsBackground = true
                    };
                    listener.Start();
                }
            }
        }

        private void disButton_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            exit = true;
            active = false;
            Disconnect();  
        }

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
            if (obj.client != null && obj.stream != null)
            {
                string message = sendBox.Text;

                // Check if a text message or an image is being sent
                if (!string.IsNullOrEmpty(message))
                {
                    // Send the text message to the client
                    SendMessageToClient(message);
                    Log("You (Server): " + message);
                }
                else if (selectedImagePath != null)
                {
                    try
                    {
                        // Send the selected image to the client
                        SendImageToClient(selectedImagePath);
                        Log("You (Server): Image sent");

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

        private void SendMessageToClient(string message)
        {
            var payload = message + Environment.NewLine;
            byte[] messageBytes = Encoding.ASCII.GetBytes(payload);
            obj.stream.Write(messageBytes, 0, messageBytes.Length);
        }

        private void SendImageToClient(string imagePath)
        {
            // Load the image from the specified file
            Image image = Image.FromFile(imagePath);


            // Convert the image to a byte array
            byte[] imageBytes = ImageToByteArray(image);

            string imageEncoded = Convert.ToBase64String(imageBytes);

            var payloadToSend = "gambar:" + imageEncoded + Environment.NewLine;

            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(payloadToSend);

            //---send the text---
            Console.WriteLine("Sending : " + 1);
            obj.stream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                return memoryStream.ToArray();
            }
        }



        private bool IsImageData(byte[] data)
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
