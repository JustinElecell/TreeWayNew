using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using SimpleJSON;
using GameSecurity;

namespace ElecellConnection
{

    public class TcpNetwork : MonoBehaviour
    {

        public static string TokenErr = "Invalid Token";

        private static TcpNetwork _instance;
        private static bool _instantiated;


        public static TcpNetwork instance
        {
            get
            {

                if (!_instantiated)
                {

                    _instance = new GameObject().AddComponent<TcpNetwork>();
                    DontDestroyOnLoad(_instance.gameObject);
                    _instantiated = true;

                    _instance.TIMEOUT["error"] = "Time out";
                    _instance.DC["error"] = "Disconnected";
                    _instance.NULL["error"] = "Null";
                }
                return _instance;
            }
        }

        bool socketReady = false;
        TcpClient mySocket;
        NetworkStream theStream;
        StreamWriter theWriter;
        //StreamReader theReader;
        String Host = "www.elecell.com";
        Int32 Port = 1111;
        float TimeOutLimit = 12f;
        float RetryInterval = 4.8f;
        float KeepAlive = 4f;
        int idx = 1;
        float aliveTimer;

        Dictionary<int, TcpQuery> queryDict = new Dictionary<int, TcpQuery>();
        Dictionary<int, JSONClass> responseDict = new Dictionary<int, JSONClass>();
        JSONClass TIMEOUT = new JSONClass();
        JSONClass DC = new JSONClass();
        JSONClass NULL = new JSONClass();

        Dictionary<string, GenericTrigger> eventDict = new Dictionary<string, GenericTrigger>();

        JSONClass tempResponse;

        bool keyDone;
        bool keyLoading;

        public static string keyIndex;

        static int ReadBufferSize = 65536;
        static byte[] readBuffer = new byte[ReadBufferSize];
        static List<string> resBuffer = new List<string>();
        string tempRead = "";
        char[] seperator = new char[] { '\n' };

        public static void RegisterErrorEvent(string error, GenericTrigger callback)
        {
            if (instance.eventDict.ContainsKey(error))
                instance.eventDict[error] = callback;
            else
                instance.eventDict.Add(error, callback);
        }

        public static void UnregisterErrorEvent(string error)
        {
            if (instance.eventDict.ContainsKey(error))
                instance.eventDict.Remove(error);
        }

        public bool isResponeReady(int _idx)
        {
            return responseDict.ContainsKey(_idx);
        }

        public JSONClass fetchResponse(int _idx)
        {
            if (!responseDict.ContainsKey(_idx))
                return null;
            tempResponse = responseDict[_idx];
            responseDict.Remove(_idx);

            if (tempResponse["error"] != null && eventDict.ContainsKey(tempResponse["error"]))
                eventDict[tempResponse["error"]]();

            return tempResponse;
        }

        public TcpQuery Query(string url, TcpForm form, GenericCallback<JSONClass> callback = null)
        {
            if (!keyDone)
            {
                form.AddField("url", url);
                form.AddField("idx", idx);
                TcpQuery qt = new TcpQuery(idx, null, callback);
                qt.startTime = Time.unscaledTime;
                queryDict.Add(qt.idx, qt);
                idx++;
                if (idx > 999) idx = 1;

                StartCoroutine(keySetting(qt, form));

                return qt;
            }

            if (!socketReady)
                setupSocket();

            form.AddField("url", url);
            form.AddField("idx", idx);

            TcpQuery q = new TcpQuery(idx, form.FinalizeD(), callback);
            q.startTime = Time.unscaledTime;
            queryDict.Add(q.idx, q);
            writeSocket(q.query);

            idx++;
            if (idx > 999) idx = 1;

            return q;
        }

        IEnumerator keySetting(TcpQuery query, TcpForm form)
        {
            if (!keyLoading)
            {
                setkey();
            }

            int retry = 0;
            float timer;

            again:
            timer = 0f;
            while (!keyDone && timer < 3f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!keyDone && retry < 2)
            {
                retry++;
                setkey();
                goto again;
            }

            if (keyDone)
            {
                query.query = form.FinalizeD();
                writeSocket(query.query);
            }
            else
            {
                if (query.callback == null)
                    responseDict.Add(query.idx, TIMEOUT);
                else
                    query.callback(TIMEOUT);
            }
        }

        public void setkey()
        {
            keyDone = false;
            keyLoading = true;

            closeSocket();
            setupSocket();

            if (socketReady)
            {
                string[] key = NodeEncrypt.init();
                theWriter.Write('#' + RSAEncrypt.Encrypt(key[0] + "@" + key[1]) + '\n');
                theWriter.Flush();
            }
            else
            {
                keyLoading = false;
            }

        }

        void Update()
        {
            if (socketReady)
            {
                if (!mySocket.Connected) closeSocket();
            }

            if (socketReady)
            {
                if (!keyDone)
                {
                    readKey();
                    return;
                }

                if (readSocket(out tempResponse))
                {  // push response to Dict, remove from query list, report clear to server
                    if (queryDict.ContainsKey(tempResponse["idx"].AsInt) && queryDict[tempResponse["idx"].AsInt].callback != null)
                    {
                        queryDict[tempResponse["idx"].AsInt].callback(tempResponse);
                    }
                    else
                    {
                        if (!responseDict.ContainsKey(tempResponse["idx"].AsInt)) responseDict.Add(tempResponse["idx"].AsInt, tempResponse);
                    }
                    queryDict.Remove(tempResponse["idx"].AsInt);

                    TcpForm form = new TcpForm();
                    form.AddField("clear", tempResponse["idx"].AsInt);

                    writeSocket(form.FinalizeD());
                }
                else if (queryDict.Count > 0)
                { // if there is active connection and pending query, check if it is necessary to resend query
                    List<int> toDelete = new List<int>();

                    foreach (TcpQuery q in queryDict.Values)
                    {
                        float age = Time.unscaledTime - q.startTime;

                        if (age > TimeOutLimit)
                        {
                            if (q.callback != null)
                                q.callback(TIMEOUT);
                            else if (!responseDict.ContainsKey(q.idx))
                                responseDict.Add(q.idx, TIMEOUT);
                            toDelete.Add(q.idx);
                        }
                        else if (age > RetryInterval * 2f && q.retry > 0)
                        {
                            q.retry--;
                            writeSocket(q.query);
                        }
                        else if (age > RetryInterval && q.retry > 1)
                        {
                            q.retry--;
                            writeSocket(q.query);
                        }
                    }

                    foreach (int i in toDelete)
                    {
                        queryDict.Remove(i);
                    }
                }
                else
                {
                    if (Time.unscaledTime - aliveTimer > KeepAlive)
                    {
                        aliveTimer = Time.unscaledTime;
                        closeSocket();
                    }
                }
            }
            else if (queryDict.Count > 0)
            { // return error if disconnected
                List<int> toDelete = new List<int>();

                foreach (TcpQuery q in queryDict.Values)
                {
                    if (q.callback != null)
                        q.callback(DC);
                    else if (!responseDict.ContainsKey(q.idx))
                        responseDict.Add(q.idx, DC);
                    toDelete.Add(q.idx);
                }

                foreach (int i in toDelete)
                {
                    queryDict.Remove(i);
                }
            }
        }

        void readCallback(IAsyncResult result)
        {
            int byteRead;
            try
            {
                if (!socketReady) return;

                byteRead = theStream.EndRead(result);

                if (byteRead < 1)
                {
                    closeSocket();
                    return;
                }

                processRead(byteRead);
                theStream.BeginRead(readBuffer, 0, ReadBufferSize, readCallback, null);
            }
            catch
            {
            }
        }

        void processRead(int byteRead)
        {
            try
            {
                tempRead += Encoding.ASCII.GetString(readBuffer, 0, byteRead);

                if (tempRead.IndexOf('\n') < 0) return;

                //Debug.Log(tempRead);

                string[] substring = tempRead.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                resBuffer.Add(substring[substring.Length - 1]);
                tempRead = "";
                for (int i = 1; i < substring.Length; i++)
                {
                    if (i > 1) tempRead += '\n';
                    tempRead += substring[i];
                }
            }
            catch
            {
                tempRead = "";
                Debug.Log("msg parse error");
            }
        }
        void setupSocket()
        {
            try
            {
                mySocket = new TcpClient(Host, Port);
                mySocket.NoDelay = true;
                theStream = mySocket.GetStream();
                theWriter = new StreamWriter(theStream, new UTF8Encoding(false, true), 65536);
                theStream.BeginRead(readBuffer, 0, ReadBufferSize, readCallback, null);
                //theReader = new StreamReader(theStream);
                socketReady = true;

                aliveTimer = Time.unscaledTime;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log("Socket error: " + e);
#endif
            }
        }


        void writeSocket(string theLine)
        {
            if (!keyDone)
                return;

            if (socketReady)
            {
                try
                {
                    theWriter.Write(theLine);
                    theWriter.Flush();
                    aliveTimer = Time.unscaledTime;
                }
                catch
                {
                    aliveTimer = Time.unscaledTime;
                    closeSocket();
                }
            }
        }

        bool readSocket(out JSONClass response)
        {
            response = NULL;

            if (resBuffer.Count == 0) return false;

            string res = resBuffer[0];
            resBuffer.RemoveAt(0);
            //Debug.Log(res.Length.ToString() + ": "+ res);

            try
            {
#if UNITY_EDITOR
                string line = NodeEncrypt.Decrypt(res);
                Debug.Log(line);
                response = (JSONClass)JSONNode.Parse(line);
#else
				response = (JSONClass) JSONNode.Parse( NodeEncrypt.Decrypt(res));
#endif

            }
            catch
            {
                return false;
            }
            aliveTimer = Time.unscaledTime;
            return true;
        }

        void readKey()
        {
            if (resBuffer.Count == 0) return;
            keyIndex = resBuffer[0];
            resBuffer.RemoveAt(0);

            int i;
            if (int.TryParse(keyIndex, out i))
            {
                keyDone = true;
                keyLoading = false;
            }
#if UNITY_EDITOR
            //Debug.Log("Key: " + keyIndex);
#endif

            keyIndex += '@';

            /*
                if (!theStream.DataAvailable) return;

                try {
                    keyIndex = theReader.ReadLine();
                    int i;
                    if (int.TryParse(keyIndex,out i)) {
                        keyDone = true;
                        keyLoading = false;
                        //closeSocket();
                    }
                #if UNITY_EDITOR
                    Debug.Log(keyIndex);
                #endif
                    keyIndex += '@';
                }
                catch {
                }
                */
        }

        void closeSocket()
        {
            if (!socketReady)
                return;
            theWriter.Close();
            //theReader.Close();
            mySocket.Close();
            socketReady = false;
            tempRead = "";
        }

    }

    public class NetworkManager : MonoBehaviour
    {
        public static T NewType<T>()
                where T : new()
        {
            return new T();
        }

        private static NetworkManager mInstance;

        const int _SIZE_ = 100;  // set it to a large enough size, ensure we don't have issues....

        private ArrayList[] listeners = new ArrayList[_SIZE_];

        JSONClass TIMEOUT = new JSONClass();

        readonly Int32 port = 1111;

        string ip_addr = "www.elecell.com";
        //string ip_addr = "128.199.229.148";

        int timeout = 5000;

        int idx = 1;

        // Use this for initialization
        void Awake()
        {

            DontDestroyOnLoad(this);   // this will make it presist across scene !!!
        }

        public static NetworkManager instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = new GameObject("NetworkManager");

                    mInstance = go.AddComponent<NetworkManager>();

                }
                return mInstance;
            }
        }

        public int getCurrentIndex()
        {
            int ret = 0;

            for (int i = 0; i < _SIZE_; i++)
            {
                if (listeners[i] == null)
                {
                    ret = i;
                    break;
                }
            }
            return ret;
        }


        //----------- function to handle the SNotification Class......

        public int addListener(OnSNotificationDelegate newListenerDelegate)
        {
            int typeInt = getCurrentIndex();

            // Create the listener ArrayList lazily
            if (listeners[typeInt] == null)
                listeners[typeInt] = new ArrayList();

            listeners[typeInt].Add(newListenerDelegate);

            return typeInt;
        }

        public void removeListener(int slot_index)
        {
            int typeInt = slot_index;

            if (listeners[typeInt] == null)
                return;

            listeners[typeInt].Clear();

            // Clean up empty listener ArrayLists
            if (listeners[typeInt].Count == 0)
                listeners[typeInt] = null;
        }

        public void removeAllListener()
        {
            for (int i = 0; i < _SIZE_; i++)
            {

                listeners[i] = null;
            }

        }

        public void postNotification(SNotification note)
        {
            int typeInt = note.type;

            if (listeners[typeInt] == null)
                return;

            foreach (OnSNotificationDelegate delegateCall in listeners[typeInt])
            {
                delegateCall(note);
            }
        }

        public void postDeleteNotification(int slot, object param)
        {
            postNotification(new SNotification(slot, param /* userinfo, contain anything you want to pass back */  ));
            removeListener(slot);
        }

        public void postWithoutDeleteNotification(int slot, object param)
        {
            postNotification(new SNotification(slot, param /* userinfo, contain anything you want to pass back */  ));
        }

        /// <summary>
        /// 
        ///  this function will send a request to the server, and handle the reply ~ 
        ///  
        ///  ** important, we need to use different PORT for different functions ( login, pvp, shop, iap...)
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callbacks"></param>
        /// <returns></returns>
        IEnumerator processRetrySend(string url, TcpForm form, Action<NetClientStatus, string> callbacks)
        {
            int try_count = 0;

            NetClientStatus status = NetClientStatus.None;
            string result_str = string.Empty;

            while (try_count++ < 3 && status != NetClientStatus.Complete)
            {
                //Debug.Log("try " + try_count);
                int serv_port = port;

                // this NetClient mainly use to communicate with server.
                NetClient client = new NetClient_S();

                form.AddField("url", url);
                form.AddField("idx", idx);

                var A = form.FinalizeD();
                //Debug.Log("Query : " + A);

                TcpQuery q = new TcpQuery(idx, A, null);
                q.startTime = Time.unscaledTime;

                // -----
                idx++;
                if (idx > 999)
                    idx = 1;

                // TODO: divided the 'port' into different function -port ....
                client.Send(ip_addr, serv_port, A, timeout);
                yield return StartCoroutine(client.WaitForServer());

                status = client.Status;
                result_str = client.Message;

                //if (result_str[result_str.Length - 1] != '\n')
                if (result_str == null || result_str.Length < 1 || result_str[result_str.Length - 1] != '\n')
                {
                    status = NetClientStatus.ReceiveError;
                }
                client.Reset(); // FIX
            }

            callbacks(status, result_str);
            yield return null;
        }

        IEnumerator processRetrySendX2(string url, TcpForm form, Action<NetClientStatus, string> callbacks)
        {
            int try_count = 0;

            NetClientStatus status = NetClientStatus.None;
            string result_str = string.Empty;

            while (try_count++ < 3 && status != NetClientStatus.Complete)
            {
                //Debug.Log("try " + try_count);
                int serv_port = port;

                // this NetClient mainly use to communicate with server.
                NetClient client = new NetClient_S();


                var A = form.FinalizeD();
                //Debug.Log("Query : " + A);

                //TcpQuery q = new TcpQuery(idx, A, null);
                //q.startTime = Time.unscaledTime;

                // TODO: divided the 'port' into different function -port ....
                client.Send(ip_addr, serv_port, A, timeout);
                yield return new WaitForSeconds(15f);

                status = NetClientStatus.Complete;
                result_str = "";

                client.Reset(); // FIX
            }
            Debug.Log(result_str);
            callbacks(status, result_str);
            yield return null;
        }

        public void TcpClear(TcpForm form)
        {
            StartCoroutine(_TcpClear(form, null));
        }
        IEnumerator _TcpClear(TcpForm form, GenericCallback<JSONClass> callback = null)
        {
            //JSONClass response;

            //----------------------- retry loop --------------
            NetClientStatus status = NetClientStatus.None;
            string result_str = string.Empty;

            yield return processRetrySendX2("", form, (s, r) => { status = s; result_str = r; });
            //------

            if (status == NetClientStatus.Complete)
            {
                Debug.Log("Clear");
            }
        }

        public virtual void Query(string url, TcpForm form, GenericCallback<JSONClass> callback = null)
        {
            StartCoroutine(_Query(url, form, callback));
        }

        IEnumerator _Query(string url, TcpForm form, GenericCallback<JSONClass> callback = null)
        {

            JSONClass response;

            //----------------------- retry loop --------------
            NetClientStatus status = NetClientStatus.None;
            string result_str = string.Empty;

            yield return processRetrySend(url, form, (s, r) => { status = s; result_str = r; });
            //------

            if (status == NetClientStatus.Complete)
            {
                string res = result_str;


                try
                {

                    string line = NodeEncrypt.Decrypt(res);
                    Debug.Log(line);
                    response = (JSONClass)JSONNode.Parse(line);

                    //=========================
                    // need to send clear for idx...
                    //TcpForm clear_from = new TcpForm();
                    //int cval = response["idx"].AsInt;
                    //clear_from.AddField("clear", cval);
                    ////Debug.Log("Try Clear : " + cval);
                    //TcpClear(clear_from);
                    //
                    //=========================


                    if (callback != null)
                        callback(response);

                }
                catch
                {
                    //
                    //Debug.Log("Error on _Query");


                    if (callback != null)
                        callback(TIMEOUT);
                }
            }
            else
            {
                if (callback != null)
                    callback(TIMEOUT);

            }

        }
    }

    public class TcpQuery
    {
        public int idx;
        public string query;
        public int retry;
        public float startTime;

        public GenericCallback<JSONClass> callback;

        public TcpQuery(int _idx, string _query, GenericCallback<JSONClass> _callback)
        {
            idx = _idx;
            query = _query;
            startTime = 0;
            retry = 2;
            callback = _callback;
        }

        public bool isDone
        {
            get { return TcpNetwork.instance.isResponeReady(idx); }
        }

        public JSONClass result
        {
            get { return TcpNetwork.instance.fetchResponse(idx); }
        }
    }

    public class TcpForm
    {
        StringBuilder sb;
        bool isNew;

        public TcpForm()
        {
            sb = new StringBuilder().Append('{');
            isNew = true;
        }

        public void AddField(string key, string value)
        {
            if (isNew)
                isNew = false;
            else
                sb.Append(',');

            sb.Append('\"').Append(key).Append("\":\"").Append(value.Replace("\"", "\\\"")).Append('\"');
        }

        public void AddField(string key, int value)
        {
            if (isNew)
                isNew = false;
            else
                sb.Append(',');

            sb.Append('\"').Append(key).Append("\":").Append(value);
        }

        public void AddField(string key, long value)
        {
            if (isNew)
                isNew = false;
            else
                sb.Append(',');

            sb.Append('\"').Append(key).Append("\":").Append(value);
        }

        public void AddField(string key, double value)
        {
            if (isNew)
                isNew = false;
            else
                sb.Append(',');

            sb.Append('\"').Append(key).Append("\":").Append(value);
        }

        public void AddField(string key, JSONClass value)
        {
            if (isNew)
                isNew = false;
            else
                sb.Append(',');

            sb.Append('\"').Append(key).Append("\":").Append(value.ToString());
        }

        public void AddField(JSONClass value)
        {
            if (isNew)
                isNew = false;
            else
                sb.Append(',');

            string _key = value.getKeyAt(0);
            sb.Append('\"').Append(_key).Append("\":").Append(value[_key].ToString());
        }

        public string FinalizeD()
        {
            sb.Replace("\\", "\\\\");
            sb.Replace("\\\\\"", "\\\"");
            sb.Replace("\n", "\\n");
            sb.Replace("\r", "\\r");
            sb.Replace("\t", "\\t");
            sb.Replace("\b", "\\b");
            sb.Replace("\f", "\\f");

            sb.Append("}");

            string encrypt = NodeEncrypt.Encrypt(sb.ToString());
            sb = new StringBuilder().Append(encrypt);
            sb.Append("\n");
            sb.Insert(0, TcpNetwork.keyIndex);

            return sb.ToString();
        }
        /*
                public string FinalizeR(){
                    sb.Replace ("\\","\\\\");
                    sb.Replace ("\\\\\"","\\\"");
                    sb.Replace ("\n","\\n");
                    sb.Replace ("\r","\\r");
                    sb.Replace ("\t","\\t");
                    sb.Replace ("\b","\\b");
                    sb.Replace ("\f","\\f");

                    sb.Append ("}");
                    return RSAEncrypt.Encrypt (sb.ToString ());
                }
        */
    }


    public class WWWPool
    {

        private static WWWPool _instance;
        private static bool _instantiated;

        public static WWWPool instance
        {
            get
            {
                if (!_instantiated)
                {
                    _instance = new WWWPool();
                    _instantiated = true;
                }

                return _instance;
            }
        }



        private WWWItem[] pool;
        private int available;
        //		private int index;
        private string[] header = new string[] { "Content-Type=application/x-www-form-urlencoded" };
        private WWWForm mform;

        private WWWPool()
        {
            pool = new WWWItem[10];
            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = new WWWItem();
            }
            mform = new WWWForm();
            mform.AddField("d", 0);
            available = pool.Length;
            //			index = 0;
        }

        public bool Available
        {
            get
            {
                return available > 0;
            }
        }

#if UNITY_WEBPLAYER
		public WWW GetWWW(string url, WWWForm form){
			return new WWW(url,form);
		}
		
		public WWW GetWWW(string url){
			return new WWW(url);
		}
#else
        public WWW GetWWW(string url, WWWForm form)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (!pool[i].taken)
                {
                    pool[i].taken = true;
                    //						index = 0;
                    available--;
                    pool[i].www = new WWW(url, form.data);
                    // pool[i].www.InitWWW(url,form.data,header);	
                    return pool[i].www;
                }
            }
            return null;
        }

        public WWW GetWWW(string url)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (!pool[i].taken)
                {
                    pool[i].taken = true;
                    //					index = 0;
                    available--;
                    pool[i].www = new WWW(url, mform.data);
                    // pool[i].www.InitWWW(url,mform.data,header);	
                    return pool[i].www;
                }
            }
            return null;
        }
#endif
        public void Release(WWW www)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (www == pool[i].www)
                {
                    //					index = 1;
                    available++;
                    pool[i].taken = false;
                    return;
                }
            }
        }

    }

    public class WWWItem
    {
        public WWW www;
        public bool taken;

        public WWWItem()
        {
            www = new WWW(null);
            taken = false;
        }
    }

    // VERSION 1.0

    public class SNotification
    {
        public int type;
        public object data;

        public SNotification(int type)
        {
            this.type = type;
        }

        public SNotification(int type, object data)
        {
            this.type = type;
            this.data = data;
        }
    }

    public delegate void OnSNotificationDelegate(SNotification note);
}