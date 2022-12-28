#pragma warning disable 0168,0219,0414 // variable declared but not used.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//#if !UNITY_EDITOR
//using Debug = AltDebug.Debug;
//#endif

public class NetClient_S : NetClient {

  protected override string Encrypt(string msg) {
    return base.Encrypt(msg);
  }

  protected override string Decrypt(string msg) {
    return base.Decrypt(msg);
  }

}
