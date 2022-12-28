using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using SimpleJSON;
using System.Threading;

namespace ElecellConnection {

	public class MultiplayerNetwork : MonoBehaviour {

		private static MultiplayerNetwork _instance;
		private static bool _instantiated;

		public static MultiplayerNetwork instance{
			get{
				if (!_instantiated) {
					_instance = new GameObject().AddComponent<MultiplayerNetwork>();
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
		StreamReader theReader;
		String Host = "128.199.124.130";
		//String Host = "elecell.com";
		Int32 lobbyPort = 440;
		//Int32 lobbyPort = 450;
		Int32 serverPort;

		float TimeOutLimit = 13f;
		float RetryInterval = 4.5f;
		//float KeepAlive = 2f;
		int idx = 1;
		//float aliveTimer;

		int roomID;

		Dictionary<int,TcpQuery> queryDict = new Dictionary<int,TcpQuery>();
		// Dictionary<int,JSONClass> responseDict = new Dictionary<int, JSONClass> ();
		JSONClass TIMEOUT = new JSONClass();
		JSONClass DC = new JSONClass();
		JSONClass NULL = new JSONClass();

		Dictionary<string, GenericTrigger> eventDict = new Dictionary<string, GenericTrigger> ();

		List<int> clearList = new List<int>();
		JSONClass tempResponse;

		static int ReadBufferSize = 2048;
		static byte[] readBuffer = new byte[ReadBufferSize];
		static List<JSONClass> resBuffer = new List<JSONClass> ();
		// bool keyDone;
		// bool keyLoading;
		GenericCallback<JSONClass> joinRoomCallback;
		GenericCallback<JSONClass> eventReceiver;

		public GenericTrigger DisconnectCallback;

		bool disconnected = false;

		int pid;

		public static string keyIndex;

		public static void RegisterErrorEvent(string error, GenericTrigger callback){
			if (instance.eventDict.ContainsKey (error))
				instance.eventDict [error] = callback;
			else
				instance.eventDict.Add (error, callback);
		}

		public static void UnregisterErrorEvent(string error){
			if (instance.eventDict.ContainsKey (error))
				instance.eventDict.Remove (error);
		}

		public static void RegisterEventReceiver (GenericCallback<JSONClass> _eventReceiver){
			instance.eventReceiver = _eventReceiver;
		}

		public static void UnregisterEventReceiver (){
			instance.eventReceiver = null;
		}

		public static void RegisterDisconnectCB (GenericTrigger _eventReceiver){
			instance.DisconnectCallback = _eventReceiver;
			instance.disconnected = false;
		}

		public static void UnregisterDisconnectCB (){
			instance.DisconnectCallback = null;
			instance.disconnected = false;
		}

		public void JoinCreateRoom(GenericCallback<JSONClass> callback, int roomType = 0){
			checkingUnfinishedRoom = false;
			pid = int.Parse (EleCellLogin.GameServer.playerInfo.pid.Replace (".", ""));
			joinRoomCallback = callback;

			if (!socketReady)
				setupSocket (lobbyPort);
			else if (lastPort != lobbyPort) {
				closeSocket ();
				setupSocket (lobbyPort);
			}

			roomID = 0;
			if (roomType > 0)
				writeSocket ("@" + pid.ToString () + "@" + roomType.ToString() + "\n");
			else
				writeSocket ("@" + pid.ToString () + "\n");
		}

		public void CreateRoom(GenericCallback<JSONClass> callback, int roomType, int timeLimit, int mode, bool friendOnly){
			if (serverPort == 0) {
				CheckForUnfinishedRoom (new GenericCallback<JSONClass>(delegate(JSONClass value) {
					checkingUnfinishedRoom = false;
					pid = int.Parse (EleCellLogin.GameServer.playerInfo.pid.Replace (".", ""));
					joinRoomCallback = callback;

					disconnected = false;
					DisconnectCallback = null;

					if (socketReady)
						closeSocket ();

					setupSocket (serverPort);

					roomID = 0;

					writeSocket ("@" + pid.ToString () + "@" + roomType.ToString() + "@" + timeLimit.ToString() + "@" + mode.ToString() + "@" + (friendOnly?pid.ToString() : "1") + "\n");
				}));

				return;
			}

			checkingUnfinishedRoom = false;
			pid = int.Parse (EleCellLogin.GameServer.playerInfo.pid.Replace (".", ""));
			joinRoomCallback = callback;

			disconnected = false;
			DisconnectCallback = null;

			if (socketReady)
				closeSocket ();
			
			setupSocket (serverPort);

			roomID = 0;

			writeSocket ("@" + pid.ToString () + "@" + roomType.ToString() + "@" + timeLimit.ToString() + "@" + mode.ToString() + "@" + (friendOnly?pid.ToString() : "1") + "\n");
		}

		public void JoinCustomRoom(GenericCallback<JSONClass> callback, int room, int port){
			serverPort = port;

			checkingUnfinishedRoom = false;
			pid = int.Parse (EleCellLogin.GameServer.playerInfo.pid.Replace (".", ""));
			joinRoomCallback = callback;

			disconnected = false;
			DisconnectCallback = null;

			if (socketReady)
				closeSocket ();

			setupSocket (serverPort);

			roomID = 0;
			Debug.Log ("~" + pid.ToString () + "~" + room.ToString() + "\n");

			writeSocket ("~" + pid.ToString () + "~" + room.ToString() + "\n");
		}

		public void ListRoom(GenericCallback<JSONClass> callback){
			listingRoom = true;
			listTimeOut = Time.unscaledTime + 5f;
			checkingUnfinishedRoom = false;
			pid = int.Parse (EleCellLogin.GameServer.playerInfo.pid.Replace (".", ""));
			joinRoomCallback = callback;

			if (!socketReady) {
				setupSocket (lobbyPort);
			} else if (lastPort != lobbyPort) {
				if (socketReady)
					closeSocket ();

				setupSocket (lobbyPort);
			}
				

			roomID = 0;
			writeSocket ("~" + pid.ToString () + "\n");
		}

		public void ListRoom(string roomType, string timeLimit, string gameMode, GenericCallback<JSONClass> callback){
			listingRoom = true;
			listTimeOut = Time.unscaledTime + 5f;
			checkingUnfinishedRoom = false;
			pid = int.Parse (EleCellLogin.GameServer.playerInfo.pid.Replace (".", ""));
			joinRoomCallback = callback;

			if (!socketReady) {
				setupSocket (lobbyPort);
			} else if (lastPort != lobbyPort) {
				if (socketReady)
					closeSocket ();

				setupSocket (lobbyPort);
			}


			roomID = 0;
			writeSocket ("~" + pid.ToString () + "~" + roomType + "~" + timeLimit + "~" + gameMode + "\n");
		}

		bool checkingUnfinishedRoom = false;
		bool listingRoom = false;
		float listTimeOut;

		public void CheckForUnfinishedRoom(GenericCallback<JSONClass> callback){

			checkingUnfinishedRoom = true;

			pid = int.Parse (EleCellLogin.GameServer.playerInfo.pid.Replace (".", ""));
			joinRoomCallback = callback;

			if (!socketReady)
				setupSocket (lobbyPort);

			writeSocket ("#" + pid.ToString () + "\n");


		}

		public void Disconnect(){
			_instantiated = false;
			closeSocket();
			Destroy(_instance);
			#if UNITY_EDITOR
			Debug.Log ("Multiplayer Disconnected");
			#endif
		}

		public void QuickMsg (string key, string value){
			TcpFormPlain form = new TcpFormPlain ();
			form.AddField (key, value);
			Query (form);
		}

		public void QuickMsg (string key, int value){
			TcpFormPlain form = new TcpFormPlain ();
			form.AddField (key, value);
			Query (form);
		}

		public void Query (TcpFormPlain form){
			if (!socketReady)
				setupSocket (serverPort);

			form.AddField ("idx", idx);
			form.AddField ("pid", pid);
			form.AddField ("rm", roomID);

			TcpQuery q = new TcpQuery (idx, form.FinalizeD(),null);
			q.startTime = Time.unscaledTime;
			queryDict.Add (q.idx, q);
			writeSocket (q.query);

			#if UNITY_EDITOR
			Debug.Log(q.query);
			#endif

			idx++;
			if (idx > 999)	idx = 1;
		}

		void ConnectToServer(JSONClass serverData){
			closeSocket ();
			serverPort = serverData ["port"].AsInt;
			setupSocket (serverPort);

			switch (serverData ["action"].AsInt) {
			case 1:
				writeSocket ("#" + pid.ToString () + "#" + serverData["roomType"] + "#" + serverData["roomID"] + "\n");
				break;
			case 2:
				writeSocket ("$" + pid.ToString () + "$" + serverData["roomType"] + "$" + serverData["roomID"] + "\n");
				break;
			case 3:
				writeSocket ("@" + pid.ToString () + "@" + serverData["roomType"] + "\n");
				break;
			}
		}


		void Update(){
			/*
			if (debugtext.Count > 0) {
				if (debugtext [0].Length > 100) {
					TextEffect.Notice (debugtext [0].Substring(0,20) + "..." + debugtext [0].Substring(debugtext [0].Length - 20));
				}
				else
					TextEffect.Notice (debugtext [0]);
				debugtext.RemoveAt (0);
			}
			*/

			if (listingRoom && Time.unscaledTime > listTimeOut) {
				listingRoom = false;
				joinRoomCallback ((JSONClass) JSONNode.Parse("{\"rooms\":[]}"));
				closeSocket ();
				return;
			}

			if (disconnected) {
				disconnected = false;
				if (DisconnectCallback != null) {
					DisconnectCallback ();
					DisconnectCallback = null;
				}
			}

			if (socketReady) {
				if (!mySocket.Connected)
					closeSocket ();
				else {
					if (mySocket.Client.Poll (0, SelectMode.SelectRead)) {
					}
				}
			}

			if (socketReady) {
				if (readSocket(out tempResponse)){  // remove from query list, report clear to server
					if (roomID == 0 && tempResponse ["rooms"] != null) {
						listingRoom = false;
						joinRoomCallback (tempResponse);
						closeSocket ();
						return;
					}
					if (roomID == 0 && tempResponse ["action"] != null) {
						if (checkingUnfinishedRoom) {
							checkingUnfinishedRoom = false;
							serverPort = tempResponse ["port"].AsInt;
							closeSocket ();
							joinRoomCallback (tempResponse);
						}
						else 
							ConnectToServer (tempResponse);
						return;
					}
					else if (roomID == 0 && tempResponse ["room"] != null) {
						roomID = tempResponse ["room"].AsInt;
						if (joinRoomCallback != null)
							joinRoomCallback (tempResponse);

						TcpFormPlain form = new TcpFormPlain();
						form.AddField("clear",tempResponse["idx"].AsInt);

						writeSocket(form.FinalizeD());
					}
					else if (tempResponse ["idx"] != null) {
//						Debug.Log("clear:" + tempResponse ["idx"]);
						if (!clearList.Contains (tempResponse ["idx"].AsInt)) {
							clearList.Add (tempResponse ["idx"].AsInt);
							TcpFormPlain form = new TcpFormPlain ();
							form.AddField ("clear", tempResponse ["idx"].AsInt);

							writeSocket (form.FinalizeD ());
							if (eventReceiver != null)
								eventReceiver (tempResponse);
						}
					}
					else if (queryDict.ContainsKey(tempResponse["ack"].AsInt)){
						queryDict.Remove(tempResponse["ack"].AsInt);
					}
				}
				else if (queryDict.Count > 0) { // if there is active connection and pending query, check if it is necessary to resend query
					List<int> toDelete = new List<int>();

					foreach (TcpQuery q in queryDict.Values){
						float age = Time.unscaledTime - q.startTime;

						if (age > TimeOutLimit){
							toDelete.Add(q.idx);
						}
						else if (age > RetryInterval * 2f && q.retry > 0){
							q.retry--;
							writeSocket(q.query);
						}
						else if (age > RetryInterval && q.retry > 1){
							q.retry--;
							writeSocket(q.query);
						}
					}

					foreach (int i in toDelete){
						queryDict.Remove(i);
					}
				}
				/*
				else {
					if (Time.unscaledTime - aliveTimer > KeepAlive) {
						aliveTimer = Time.unscaledTime;
						closeSocket();
					}
				}
				*/
			}
			else if (queryDict.Count > 0) { // return error if disconnected
				queryDict.Clear ();
			}
		}

		void readCallback(IAsyncResult result){
			int byteRead;
			try {
				byteRead = theStream.EndRead(result);

				if (byteRead < 1) {
					closeSocket();
					return;
				}
				//Debug.Log(byteRead);
				processRead(byteRead);
				theStream.BeginRead(readBuffer,0,ReadBufferSize,readCallback,null);
			}
			catch {
				#if UNITY_EDITOR
				Debug.Log("read error");
				#endif

				disconnected = true;
			}
		}

		byte[] tempRead = new byte[0];
		char[] seperator = new char[]{'\n','\r'};

		//List<string> debugtext = new List<string> ();

		string tempstring = "";

		void processRead(int byteRead){
			if (tempRead.Length == 0)
				tempRead = readBuffer;
			else {
				byteRead += tempRead.Length;
				tempRead = tempRead.Concat (readBuffer).ToArray();
			}

			try {
				string msg = tempstring + Encoding.UTF8.GetString (tempRead, 0, byteRead);
				#if UNITY_EDITOR
				Debug.Log (msg);
				#endif
				//debugtext.Add(msg);

				string[] substring = msg.Split(seperator,StringSplitOptions.RemoveEmptyEntries);
				bool gotJSON = false;

				for (int i = 0; i < substring.Length ; i++){
					if (substring[i].Length > 3 && substring[i].StartsWith("{") && substring[i].EndsWith("}")) {
						resBuffer.Add ((JSONClass) JSONNode.Parse(substring[i]));
						gotJSON = true;
						tempstring = "";
					}
					else if (tempstring.Length > 0 || substring[i].StartsWith("{")){
						tempstring += substring[i];
					}
				}

				//if (gotJSON) {
					tempRead = new byte[0];
				//}
				//Debug.Log ("resBuffer add: " + resBuffer.Count.ToString());
			}
			catch {
				Debug.Log ("msg parse error");
			}
		}

		void writeSocket(string theLine) {
			if (socketReady){
				try {
					theWriter.Write(theLine);
					theWriter.Flush();
				}
				catch {
					closeSocket();
				}
			}
		}

		bool readSocket(out JSONClass response){
			response = null;

			if (resBuffer.Count == 0) return false;

			response = resBuffer [0];
			resBuffer.RemoveAt (0);

			return true;
		}

		int lastPort;

		void setupSocket(Int32 port) {
			lastPort = port;

			try {
				mySocket = new TcpClient(Host, port);
				mySocket.NoDelay = true;

				theStream = mySocket.GetStream();
				theWriter = new StreamWriter(theStream,new UTF8Encoding(false,true),65536);

				theStream.BeginRead(readBuffer,0,ReadBufferSize,readCallback,null);

				socketReady = true;
			}
			catch (Exception e) {
				#if UNITY_EDITOR
				Debug.Log("Socket error: " + e);
				#endif
			}
		}

		void closeSocket() {
			roomID = 0;
			if (!socketReady)
				return;
			theWriter.Close();
			mySocket.Close();
			socketReady = false;
		}

		void OnDestroy(){
			closeSocket ();
		}
	}

	public class TcpFormPlain {
		StringBuilder sb;
		bool isNew;

		public TcpFormPlain(){
			sb = new StringBuilder().Append('{');
			isNew = true;
		}

		public void AddField(string key, string value){
			if (isNew) 
				isNew = false;
			else 
				sb.Append (',');

			sb.Append ('\"').Append (key).Append ("\":\"").Append (value.Replace ("\"","\\\"")).Append ('\"');
		}

		public void AddField(string key, int value){
			if (isNew) 
				isNew = false;
			else 
				sb.Append (',');

			sb.Append ('\"').Append (key).Append ("\":").Append (value);
		}

		public string FinalizeD(){
			sb.Replace ("\\","\\\\");
			sb.Replace ("\\\\\"","\\\"");
			sb.Replace ("\n","\\n");
			sb.Replace ("\r","\\r");
			sb.Replace ("\t","\\t");
			sb.Replace ("\b","\\b");
			sb.Replace ("\f","\\f");

			sb.Append ("}");

			sb.Append ("\n");
			sb.Insert (0, MultiplayerNetwork.keyIndex);

			return sb.ToString ();
		}

	}
}