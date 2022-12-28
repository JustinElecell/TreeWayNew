#pragma warning disable 0168,0219,0414 // variable declared but not used.

using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using UnityEngine;

//#if !UNITY_EDITOR
//using Debug = AltDebug.Debug;
//#endif

public enum NetClientStatus
{
    None = 0,
    //Connecting,
    ConnectionError,
    //Connected,
    //Sending,
    SendError,
    ReceiveError,
    Complete,
    UnknownError,
}


public class NetClient {

 

  const int Buffer_Length = 2000;

  int _Timeout = 30000;
  int _SubTimeout = 3000;
  bool _TryingToConnect = false;
  TcpClient _Socket;
  Thread _TimeoutThread = null;
  Thread _ConnThread = null;
  Thread _SendThread = null;
  NetworkStream _Stream = null;
  byte[] _Buffer = new byte[Buffer_Length];

  public NetClientStatus Status = NetClientStatus.None;
  StringBuilder CompleteMessage = new StringBuilder();
  public string Message { get { return Decrypt(CompleteMessage.ToString()); } }
  public string Error = "";

  ~NetClient() {
    Reset();
  }

  bool _Reset = false;
  public void Reset() {
    if (_Reset) return; _Reset = true;
   // Debug.Log("NetClient::Reset");
    if (_Stream != null)
      try { _Stream.Close(); _Stream.Dispose(); _Stream = null; } catch (Exception ex) { Debug.LogError(ex.Message.ToString()); }
    if (_Socket != null)
      try { _Socket.Close(); ((IDisposable)_Socket).Dispose(); _Socket = null; } catch (Exception ex) { Debug.LogError(ex.Message.ToString()); }
    if (_SendThread != null && _SendThread.IsAlive) {
      _SendThread.Abort(); _SendThread = null;
    }

    if (_ConnThread != null && _ConnThread.IsAlive) {
      _ConnThread.Abort(); _ConnThread = null;
    }

    _TryingToConnect = false;
    _Buffer = new byte[Buffer_Length];
    NetClientStatus Result = NetClientStatus.None;
    string Message = "";
    string Error = "";
  }

  private void TimeoutCheck() {
    int time = 0;
    while (_TryingToConnect && time < _Timeout) {
      //Debug.Log("_TryingToConnect:: " + _TryingToConnect.ToString());
      //Debug.Log("NetClient::time " + time);
      Thread.Sleep(_SubTimeout);
      time += _SubTimeout;
    }
    //Debug.Log("_TryingToConnect:: " + _TryingToConnect.ToString());
    if (_TryingToConnect && _Socket.Connected == false) {
      try {
        //Debug.Log("Connection timeout");
        Status = NetClientStatus.ConnectionError;
        _TryingToConnect = false;

        Reset();
      } catch (Exception ex) { Debug.LogError(ex.Message.ToString()); }
    }
    //GameLogger.Log("NetClient::TimeoutCheck end");
  }

  protected virtual string Encrypt(string msg) {

       // return msg.UTF8Encode();
        return msg;
  }

  protected virtual string Decrypt(string msg) {

        
        return msg;
    }

  public void Send(string ip, int port, string msg, int timeout = 30000) {
        //Reset();
        //Debug.Log(ip + ":" + port + "  -> " + msg);

        // ***
        // REMARK: Only client to server request will be Encoded....
        //
        msg = Encrypt(msg);

        //IPAddress address = IPAddress.Parse(ip);
    _Timeout = timeout;
    _Socket = new TcpClient(ip,port);
    _ConnThread = new Thread(() => {
      try {
        _TryingToConnect = true;
        _TimeoutThread = new Thread(TimeoutCheck);
        _TimeoutThread.Start();
        //_Socket.Connect(address, port);     // ---> set it in constructor~  
        _TryingToConnect = false;

        _SendThread = new Thread(() => {
          if (_Socket.Connected) {
            _Socket.SendTimeout = _Timeout;
                _Socket.ReceiveTimeout = _Timeout;

                try {
              _Stream = _Socket.GetStream();
              _Stream.WriteTimeout = _Timeout;
              _Stream.ReadTimeout = _Timeout;
              byte[] sendBytes = Encoding.ASCII.GetBytes(msg);

                    /* modify to loop */

                    // =============== LOOP SENT 
                    int offset = 0;
                    int limit_offset = sendBytes.Length - 1;
                    do
                    {
                        int writeLength = Math.Min(Buffer_Length, sendBytes.Length - offset);
                        _Stream.Write(sendBytes, offset, writeLength);
                        offset += writeLength;
                    } while (offset <= limit_offset);
                    //} while (offset < limit_offset);

                    _Stream.Flush();

                    // =============== LOOP END
                    //_Stream.Write(sendBytes, 0, sendBytes.Length);


                    _Buffer = new byte[Buffer_Length];

                       /// FIX the network fail case....
                       //  we will keep waiting data come in , until we receive the end signal '$|!$'
                       //  which fix the fail request ( tested )
                       // ( 21-03-2018 )
                        bool keep_read = true;
                        //DateTime start_time = DateTime.Now;
                        while (keep_read)
                        {
                            if (_Stream.DataAvailable)
                            {
                                int b = _Stream.Read(_Buffer, 0, _Buffer.Length);
                                string m = Encoding.ASCII.GetString(_Buffer, 0, b);
                                CompleteMessage.AppendFormat("{0}", m);

                                if (m[m.Length-1] == '\n')   // 
                                    keep_read = false;

                            
                            }
                            
                            
                        }
                        Debug.Log(CompleteMessage.Length);

                    if (Status == NetClientStatus.None) Status = NetClientStatus.Complete;

            }
            catch (Exception err) {
              Status = NetClientStatus.SendError;
              Error = err.Message;
              Debug.Log("error in send: " + err.Message);
            }
            try { _Stream.Close(); } catch { }
          }
          else {
            Error = "Not Connected";
            Status = NetClientStatus.ConnectionError;
            Debug.Log("Cannot connect");
          }
        });
        _SendThread.Start();
      }
      catch (Exception err) {
        Error = "Not Connected";
        Status = NetClientStatus.ConnectionError;
        Debug.Log("Catch in connection: " + err.Message);
      }
    });
    _ConnThread.Start();
  }

    int SearchBufferForNewline(byte[] msg, int pos, int len)
    {
        int ret = -1;

        return ret;
    }

  public IEnumerator WaitForServer() {

        DateTime start_time = DateTime.Now;

        while (Status == NetClientStatus.None)
        {
            TimeSpan ts = DateTime.Now - start_time;        // modify : 2019-05-27, add timeout checking...
            if (ts.TotalMilliseconds > 10000)
            {
                Status = NetClientStatus.ConnectionError;
                Debug.Log("Error ~~~~~ timeout (10000)");
            }

            yield return null;
        }
  }

}

