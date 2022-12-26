using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace GameSecurity {
	#if UNITY_WP8 || UNITY_METRO
	public static class StringCipher
	{
		
		private static string pss = "fe2fdsi93";
		private static byte[] key;
		
		private static void setPs(float st){
			pss = ((int) (st * 1544354353)).ToString("x") + SystemInfo.deviceUniqueIdentifier;
			key = Encoding.UTF8.GetBytes(pss);
			init = true;
		}
		
		private static bool init = false;
		
		public static string Encrypt(string plainText)
		{
			if (!init){
				setPs (0.64f * 0.89f);
			}
			
			byte[] textByte = Encoding.UTF8.GetBytes(plainText);
			
			for (int i = 0; i < textByte.Length; i++){
				if (textByte[i] != key[i % key.Length]) textByte[i] = (byte) ( (uint) textByte[i] ^ (uint) key[i % key.Length]);
			}
			
			return Encoding.UTF8.GetString(textByte,0,textByte.Length);
			
		}
		
		
		
		public static string Decrypt(string cipherText)
		{
			return Encrypt(cipherText);			
		}
	}
	
	public class EncryptionHash {
		
		private static bool initiated = false;
		private static string secretKey;
		
		public static void setKey(string key = "fR*NQLoiR0Kpz*hhCRE9Z6"){
			initiated = true;
			secretKey = key[key.Length-1] + key + key[0];
		}
		
		public static string Md5Sum(string str)
		{
			if (!initiated) setKey();			
			
			return MD5CryptoServiceProvider.GetMd5String(str + secretKey);
		}
	}
	
	public class MD5CryptoServiceProvider : MD5
	{
		public MD5CryptoServiceProvider()
			: base()
		{
		}
	}
	
	public class MD5 : IDisposable
	{
		static public MD5 Create(string hashName)
		{
			if (hashName == "MD5")
				return new MD5();
			else
				throw new NotSupportedException();
		}
		
		static public string GetMd5String(String source)
		{
			MD5 md = MD5CryptoServiceProvider.Create();
			byte[] hash;
			
			//Create a new instance of ASCIIEncoding to
			//convert the string into an array of Unicode bytes.
			UTF8Encoding enc = new UTF8Encoding();
			//            ASCIIEncoding enc = new ASCIIEncoding();
			
			//Convert the string into an array of bytes.
			byte[] buffer = enc.GetBytes(source);
			
			//Create the hash value from the array of bytes.
			hash = md.ComputeHash(buffer);
			
			StringBuilder sb = new StringBuilder();
			foreach (byte b in hash)
				sb.Append(b.ToString("x2"));
			return sb.ToString();
		}
		
		static public MD5 Create()
		{
			return new MD5();
		}
		
		#region base implementation of the MD5
		#region constants
		private const byte S11 = 7;
		private const byte S12 = 12;
		private const byte S13 = 17;
		private const byte S14 = 22;
		private const byte S21 = 5;
		private const byte S22 = 9;
		private const byte S23 = 14;
		private const byte S24 = 20;
		private const byte S31 = 4;
		private const byte S32 = 11;
		private const byte S33 = 16;
		private const byte S34 = 23;
		private const byte S41 = 6;
		private const byte S42 = 10;
		private const byte S43 = 15;
		private const byte S44 = 21;
		static private byte[] PADDING = new byte[] {
			0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		};
		#endregion
		
		#region F, G, H and I are basic MD5 functions.
		static private uint F(uint x, uint y, uint z)
		{
			return (((x) & (y)) | ((~x) & (z)));
		}
		static private uint G(uint x, uint y, uint z)
		{
			return (((x) & (z)) | ((y) & (~z)));
		}
		static private uint H(uint x, uint y, uint z)
		{
			return ((x) ^ (y) ^ (z));
		}
		static private uint I(uint x, uint y, uint z)
		{
			return ((y) ^ ((x) | (~z)));
		}
		#endregion
		
		#region rotates x left n bits.
		/// <summary>
		/// rotates x left n bits.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		static private uint ROTATE_LEFT(uint x, byte n)
		{
			return (((x) << (n)) | ((x) >> (32 - (n))));
		}
		#endregion
		
		#region FF, GG, HH, and II transformations
		/// FF, GG, HH, and II transformations
		/// for rounds 1, 2, 3, and 4.
		/// Rotation is separate from addition to prevent recomputation.
		static private void FF(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
		{
			(a) += F((b), (c), (d)) + (x) + (uint)(ac);
			(a) = ROTATE_LEFT((a), (s));
			(a) += (b);
		}
		static private void GG(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
		{
			(a) += G((b), (c), (d)) + (x) + (uint)(ac);
			(a) = ROTATE_LEFT((a), (s));
			(a) += (b);
		}
		static private void HH(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
		{
			(a) += H((b), (c), (d)) + (x) + (uint)(ac);
			(a) = ROTATE_LEFT((a), (s));
			(a) += (b);
		}
		static private void II(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
		{
			(a) += I((b), (c), (d)) + (x) + (uint)(ac);
			(a) = ROTATE_LEFT((a), (s));
			(a) += (b);
		}
		#endregion
		
		#region context info
		/// <summary>
		/// state (ABCD)
		/// </summary>
		uint[] state = new uint[4];
		
		/// <summary>
		/// number of bits, modulo 2^64 (lsb first)
		/// </summary>
		uint[] count = new uint[2];
		
		/// <summary>
		/// input buffer
		/// </summary>
		byte[] buffer = new byte[64];
		#endregion
		
		internal MD5()
		{
			Initialize();
		}
		
		/// <summary>
		/// MD5 initialization. Begins an MD5 operation, writing a new context.
		/// </summary>
		/// <remarks>
		/// The RFC named it "MD5Init"
		/// </remarks>
		public virtual void Initialize()
		{
			count[0] = count[1] = 0;
			
			// Load magic initialization constants.
			state[0] = 0x67452301;
			state[1] = 0xefcdab89;
			state[2] = 0x98badcfe;
			state[3] = 0x10325476;
		}
		
		/// <summary>
		/// MD5 block update operation. Continues an MD5 message-digest
		/// operation, processing another message block, and updating the
		/// context.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <remarks>The RFC Named it MD5Update</remarks>
		protected virtual void HashCore(byte[] input, int offset, int count)
		{
			int i;
			int index;
			int partLen;
			
			// Compute number of bytes mod 64
			index = (int)((this.count[0] >> 3) & 0x3F);
			
			// Update number of bits
			if ((this.count[0] += (uint)((uint)count << 3)) < ((uint)count << 3))
				this.count[1]++;
			this.count[1] += ((uint)count >> 29);
			
			partLen = 64 - index;
			
			// Transform as many times as possible.
			if (count >= partLen)
			{
				Buffer.BlockCopy(input, offset, this.buffer, index, partLen);
				Transform(this.buffer, 0);
				
				for (i = partLen; i + 63 < count; i += 64)
					Transform(input, offset + i);
				
				index = 0;
			}
			else
				i = 0;
			
			// Buffer remaining input
			Buffer.BlockCopy(input, offset + i, this.buffer, index, count - i);
		}
		
		/// <summary>
		/// MD5 finalization. Ends an MD5 message-digest operation, writing the
		/// the message digest and zeroizing the context.
		/// </summary>
		/// <returns>message digest</returns>
		/// <remarks>The RFC named it MD5Final</remarks>
		protected virtual byte[] HashFinal()
		{
			byte[] digest = new byte[16];
			byte[] bits = new byte[8];
			int index, padLen;
			
			// Save number of bits
			Encode(bits, 0, this.count, 0, 8);
			
			// Pad out to 56 mod 64.
			index = (int)((uint)(this.count[0] >> 3) & 0x3f);
			padLen = (index < 56) ? (56 - index) : (120 - index);
			HashCore(PADDING, 0, padLen);
			
			// Append length (before padding)
			HashCore(bits, 0, 8);
			
			// Store state in digest
			Encode(digest, 0, state, 0, 16);
			
			// Zeroize sensitive information.
			count[0] = count[1] = 0;
			state[0] = 0;
			state[1] = 0;
			state[2] = 0;
			state[3] = 0;
			
			// initialize again, to be ready to use
			Initialize();
			
			return digest;
		}
		
		/// <summary>
		/// MD5 basic transformation. Transforms state based on 64 bytes block.
		/// </summary>
		/// <param name="block"></param>
		/// <param name="offset"></param>
		private void Transform(byte[] block, int offset)
		{
			uint a = state[0], b = state[1], c = state[2], d = state[3];
			uint[] x = new uint[16];
			Decode(x, 0, block, offset, 64);
			
			// Round 1
			FF(ref a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
			FF(ref d, a, b, c, x[1], S12, 0xe8c7b756); /* 2 */
			FF(ref c, d, a, b, x[2], S13, 0x242070db); /* 3 */
			FF(ref b, c, d, a, x[3], S14, 0xc1bdceee); /* 4 */
			FF(ref a, b, c, d, x[4], S11, 0xf57c0faf); /* 5 */
			FF(ref d, a, b, c, x[5], S12, 0x4787c62a); /* 6 */
			FF(ref c, d, a, b, x[6], S13, 0xa8304613); /* 7 */
			FF(ref b, c, d, a, x[7], S14, 0xfd469501); /* 8 */
			FF(ref a, b, c, d, x[8], S11, 0x698098d8); /* 9 */
			FF(ref d, a, b, c, x[9], S12, 0x8b44f7af); /* 10 */
			FF(ref c, d, a, b, x[10], S13, 0xffff5bb1); /* 11 */
			FF(ref b, c, d, a, x[11], S14, 0x895cd7be); /* 12 */
			FF(ref a, b, c, d, x[12], S11, 0x6b901122); /* 13 */
			FF(ref d, a, b, c, x[13], S12, 0xfd987193); /* 14 */
			FF(ref c, d, a, b, x[14], S13, 0xa679438e); /* 15 */
			FF(ref b, c, d, a, x[15], S14, 0x49b40821); /* 16 */
			
			// Round 2
			GG(ref a, b, c, d, x[1], S21, 0xf61e2562); /* 17 */
			GG(ref d, a, b, c, x[6], S22, 0xc040b340); /* 18 */
			GG(ref c, d, a, b, x[11], S23, 0x265e5a51); /* 19 */
			GG(ref b, c, d, a, x[0], S24, 0xe9b6c7aa); /* 20 */
			GG(ref a, b, c, d, x[5], S21, 0xd62f105d); /* 21 */
			GG(ref d, a, b, c, x[10], S22, 0x2441453); /* 22 */
			GG(ref c, d, a, b, x[15], S23, 0xd8a1e681); /* 23 */
			GG(ref b, c, d, a, x[4], S24, 0xe7d3fbc8); /* 24 */
			GG(ref a, b, c, d, x[9], S21, 0x21e1cde6); /* 25 */
			GG(ref d, a, b, c, x[14], S22, 0xc33707d6); /* 26 */
			GG(ref c, d, a, b, x[3], S23, 0xf4d50d87); /* 27 */
			GG(ref b, c, d, a, x[8], S24, 0x455a14ed); /* 28 */
			GG(ref a, b, c, d, x[13], S21, 0xa9e3e905); /* 29 */
			GG(ref d, a, b, c, x[2], S22, 0xfcefa3f8); /* 30 */
			GG(ref c, d, a, b, x[7], S23, 0x676f02d9); /* 31 */
			GG(ref b, c, d, a, x[12], S24, 0x8d2a4c8a); /* 32 */
			
			// Round 3
			HH(ref a, b, c, d, x[5], S31, 0xfffa3942); /* 33 */
			HH(ref d, a, b, c, x[8], S32, 0x8771f681); /* 34 */
			HH(ref c, d, a, b, x[11], S33, 0x6d9d6122); /* 35 */
			HH(ref b, c, d, a, x[14], S34, 0xfde5380c); /* 36 */
			HH(ref a, b, c, d, x[1], S31, 0xa4beea44); /* 37 */
			HH(ref d, a, b, c, x[4], S32, 0x4bdecfa9); /* 38 */
			HH(ref c, d, a, b, x[7], S33, 0xf6bb4b60); /* 39 */
			HH(ref b, c, d, a, x[10], S34, 0xbebfbc70); /* 40 */
			HH(ref a, b, c, d, x[13], S31, 0x289b7ec6); /* 41 */
			HH(ref d, a, b, c, x[0], S32, 0xeaa127fa); /* 42 */
			HH(ref c, d, a, b, x[3], S33, 0xd4ef3085); /* 43 */
			HH(ref b, c, d, a, x[6], S34, 0x4881d05); /* 44 */
			HH(ref a, b, c, d, x[9], S31, 0xd9d4d039); /* 45 */
			HH(ref d, a, b, c, x[12], S32, 0xe6db99e5); /* 46 */
			HH(ref c, d, a, b, x[15], S33, 0x1fa27cf8); /* 47 */
			HH(ref b, c, d, a, x[2], S34, 0xc4ac5665); /* 48 */
			
			// Round 4
			II(ref a, b, c, d, x[0], S41, 0xf4292244); /* 49 */
			II(ref d, a, b, c, x[7], S42, 0x432aff97); /* 50 */
			II(ref c, d, a, b, x[14], S43, 0xab9423a7); /* 51 */
			II(ref b, c, d, a, x[5], S44, 0xfc93a039); /* 52 */
			II(ref a, b, c, d, x[12], S41, 0x655b59c3); /* 53 */
			II(ref d, a, b, c, x[3], S42, 0x8f0ccc92); /* 54 */
			II(ref c, d, a, b, x[10], S43, 0xffeff47d); /* 55 */
			II(ref b, c, d, a, x[1], S44, 0x85845dd1); /* 56 */
			II(ref a, b, c, d, x[8], S41, 0x6fa87e4f); /* 57 */
			II(ref d, a, b, c, x[15], S42, 0xfe2ce6e0); /* 58 */
			II(ref c, d, a, b, x[6], S43, 0xa3014314); /* 59 */
			II(ref b, c, d, a, x[13], S44, 0x4e0811a1); /* 60 */
			II(ref a, b, c, d, x[4], S41, 0xf7537e82); /* 61 */
			II(ref d, a, b, c, x[11], S42, 0xbd3af235); /* 62 */
			II(ref c, d, a, b, x[2], S43, 0x2ad7d2bb); /* 63 */
			II(ref b, c, d, a, x[9], S44, 0xeb86d391); /* 64 */
			
			state[0] += a;
			state[1] += b;
			state[2] += c;
			state[3] += d;
			
			// Zeroize sensitive information.
			for (int i = 0; i < x.Length; i++)
				x[i] = 0;
		}
		
		/// <summary>
		/// Encodes input (uint) into output (byte). Assumes len is
		///  multiple of 4.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="outputOffset"></param>
		/// <param name="input"></param>
		/// <param name="inputOffset"></param>
		/// <param name="count"></param>
		private static void Encode(byte[] output, int outputOffset, uint[] input, int inputOffset, int count)
		{
			int i, j;
			int end = outputOffset + count;
			for (i = inputOffset, j = outputOffset; j < end; i++, j += 4)
			{
				output[j] = (byte)(input[i] & 0xff);
				output[j + 1] = (byte)((input[i] >> 8) & 0xff);
				output[j + 2] = (byte)((input[i] >> 16) & 0xff);
				output[j + 3] = (byte)((input[i] >> 24) & 0xff);
			}
		}
		
		/// <summary>
		/// Decodes input (byte) into output (uint). Assumes len is
		/// a multiple of 4.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="outputOffset"></param>
		/// <param name="input"></param>
		/// <param name="inputOffset"></param>
		/// <param name="count"></param>
		static private void Decode(uint[] output, int outputOffset, byte[] input, int inputOffset, int count)
		{
			int i, j;
			int end = inputOffset + count;
			for (i = outputOffset, j = inputOffset; j < end; i++, j += 4)
				output[i] = ((uint)input[j]) | (((uint)input[j + 1]) << 8) | (((uint)input[j + 2]) << 16) | (((uint)input[j + 3]) << 24);
		}
		#endregion
		
		#region expose the same interface as the regular MD5 object
		
		protected byte[] HashValue;
		protected int State;
		public virtual bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}
		
		public virtual bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}
		public virtual byte[] Hash
		{
			get
			{
				if (this.State != 0)
					throw new InvalidOperationException();
				return (byte[])HashValue.Clone();
			}
		}
		public virtual int HashSize
		{
			get
			{
				return HashSizeValue;
			}
		}
		protected int HashSizeValue = 128;
		
		public virtual int InputBlockSize
		{
			get
			{
				return 1;
			}
		}
		public virtual int OutputBlockSize
		{
			get
			{
				return 1;
			}
		}
		
		public void Clear()
		{
			Dispose(true);
		}
		
		public byte[] ComputeHash(byte[] buffer)
		{
			return ComputeHash(buffer, 0, buffer.Length);
		}
		public byte[] ComputeHash(byte[] buffer, int offset, int count)
		{
			Initialize();
			HashCore(buffer, offset, count);
			HashValue = HashFinal();
			return (byte[])HashValue.Clone();
		}
		
		public byte[] ComputeHash(Stream inputStream)
		{
			Initialize();
			int count;
			byte[] buffer = new byte[4096];
			while (0 < (count = inputStream.Read(buffer, 0, 4096)))
			{
				HashCore(buffer, 0, count);
			}
			HashValue = HashFinal();
			return (byte[])HashValue.Clone();
		}
		
		public int TransformBlock(
			byte[] inputBuffer,
			int inputOffset,
			int inputCount,
			byte[] outputBuffer,
			int outputOffset
			)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if ((inputCount < 0) || (inputCount > inputBuffer.Length))
			{
				throw new ArgumentException("inputCount");
			}
			if ((inputBuffer.Length - inputCount) < inputOffset)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if (this.State == 0)
			{
				Initialize();
				this.State = 1;
			}
			
			HashCore(inputBuffer, inputOffset, inputCount);
			if ((inputBuffer != outputBuffer) || (inputOffset != outputOffset))
			{
				Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
			}
			return inputCount;
		}
		public byte[] TransformFinalBlock(
			byte[] inputBuffer,
			int inputOffset,
			int inputCount
			)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if ((inputCount < 0) || (inputCount > inputBuffer.Length))
			{
				throw new ArgumentException("inputCount");
			}
			if ((inputBuffer.Length - inputCount) < inputOffset)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if (this.State == 0)
			{
				Initialize();
			}
			HashCore(inputBuffer, inputOffset, inputCount);
			HashValue = HashFinal();
			byte[] buffer = new byte[inputCount];
			Buffer.BlockCopy(inputBuffer, inputOffset, buffer, 0, inputCount);
			this.State = 0;
			return buffer;
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				Initialize();
		}
		public void Dispose()
		{
			Dispose(true);
		}
	}
	
	#else	
	public static class StringCipher
	{
		// This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
		// This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
		// 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
		private static readonly byte[] initVectorBytes = Encoding.ASCII.GetBytes("tu89geji340t89u2");
		
		// This constant is used to determine the keysize of the encryption algorithm.
		private const int keysize = 256;
		
		private static string pss = "fe2fdsi93";

		private static void setPs(float st){
			pss = ((int) (st * 1544354353)).ToString("x") + SystemInfo.deviceUniqueIdentifier;
			//Debug.Log(pss);
		}
		
		private static bool init = false;
		
		public static string Encrypt(string plainText)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) return plainText;

			if (!init){
				setPs (0.64f * 0.89f);
			}
		
			string passPhrase = pss;
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
			
				byte[] keyBytes = password.GetBytes(keysize / 8);
				using (RijndaelManaged symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
							{
								cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
								cryptoStream.FlushFinalBlock();
								byte[] cipherTextBytes = memoryStream.ToArray();
								return System.Convert.ToBase64String(cipherTextBytes);
							}
						}
					}
				}
			
		}
		
		public static string Decrypt(string cipherText)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) return cipherText;

			if (!init){
				setPs (0.64f * 0.89f);
			}
			
			string passPhrase= pss;
			
			byte[] cipherTextBytes = System.Convert.FromBase64String(cipherText);
			PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
			
				byte[] keyBytes = password.GetBytes(keysize / 8);
				using(RijndaelManaged symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using(ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
					{
						using(MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
						{
							using(CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
							{
								byte[] plainTextBytes = new byte[cipherTextBytes.Length];
								int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
								return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
							}
						}
					}
				}
			
		}
	}

	public static class postData{

		static RijndaelManaged rj;
		static byte[] key;
		static byte[] IV;

		static bool inited = false;

		static void init(){
			rj = new RijndaelManaged()	{
				Padding = PaddingMode.PKCS7,
				Mode = CipherMode.CBC,
				KeySize = 256,
				BlockSize = 256,
			};

			string prm_key = "4FxDAVejIR1QSzu";
			string prm_iv = "5R7ZdpVdX0R";
			prm_key += prm_iv [prm_iv.Length - 1];
			prm_iv += "BBNSGIoCB1FxXwN7WubHA";
			prm_key += "3yJYjjU4MvjOhYnySehrxCvrv";
			prm_iv += prm_key [prm_key.Length - 1];
			prm_key += "IE=";
			prm_iv += "5ewDc91N8U=";

			key = Convert.FromBase64String(prm_key);
			IV = Convert.FromBase64String(prm_iv);

			inited = true;
		}

		public static string Encrypt(string sToEncrypt){
			if (!inited)
				init ();

			var encryptor = rj.CreateEncryptor(key, IV);	
			var msEncrypt = new MemoryStream();
			var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
			
			var toEncrypt = UTF8Encoding.UTF8.GetBytes(sToEncrypt);			
			csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
			csEncrypt.FlushFinalBlock();			
			var encrypted = msEncrypt.ToArray();			
			return (Convert.ToBase64String(encrypted));
		}

		public static string Decrypt(string sToEncrypt){
			if (!inited)
				init ();
			
			var encryptor = rj.CreateDecryptor(key, IV);	
			var msEncrypt = new MemoryStream();
			var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
			
			var toEncrypt = Convert.FromBase64String(sToEncrypt);			
			csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
			csEncrypt.FlushFinalBlock();			
			var encrypted = msEncrypt.ToArray();			
			return (UTF8Encoding.UTF8.GetString(encrypted));
		}
	}

	public class NodeEncrypt {

		static byte[] key;
		static byte[] IV;
		static RijndaelManaged rj;

		static bool inited;
		
		public static string[] init(){
			string prm_key;
			string prm_iv;

			rj = new RijndaelManaged(){
				Padding = PaddingMode.PKCS7,
				Mode = CipherMode.CBC,
				KeySize = 128,
				BlockSize = 128,
			};

			rj.GenerateKey();
			rj.GenerateIV();

			key = rj.Key;
			IV = rj.IV;
			inited = true;

			return new string[]{Convert.ToBase64String(key),Convert.ToBase64String(IV)};
		}
		
		public static string Encrypt(string sToEncrypt){
			if (!inited)
				init ();
			
			ICryptoTransform encryptor = rj.CreateEncryptor(key, IV);
			byte[] toEncrypt = UTF8Encoding.UTF8.GetBytes(sToEncrypt);
			return (Convert.ToBase64String(encryptor.TransformFinalBlock (toEncrypt, 0, toEncrypt.Length)));
		}
		public static string Decrypt(string sToEncrypt){
			if (!inited)
				init ();

			ICryptoTransform encryptor = rj.CreateDecryptor(key, IV);		
			byte[] toEncrypt = Convert.FromBase64String(sToEncrypt);
			return (UTF8Encoding.UTF8.GetString(encryptor.TransformFinalBlock(toEncrypt,0,toEncrypt.Length)));
		}


		/*
		static bool inited;
		static TripleDESCryptoServiceProvider tdes;
		
		static void init(){
			string key = "om/W+gVaS";
			string iv = "Bu9CX";
			if (!inited) key += "=UMp9j";
			key += iv[iv.Length-1];
			if (!inited) iv += "O/33Wg";
			iv += key[9];
			byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);   
			byte[] ivArray =  Convert.FromBase64String(iv);
			tdes = new TripleDESCryptoServiceProvider();
			tdes.Key = keyArray; 
			tdes.IV = ivArray;
			tdes.Mode = CipherMode.CBC;  // which is default     
			tdes.Padding = PaddingMode.PKCS7;  // which is default
			//Debug.Log("iv: " + Convert.ToBase64String(tdes.IV));

			tdes.GenerateKey ();
			tdes.GenerateIV ();
			Debug.Log (Convert.ToBase64String(tdes.Key));
			Debug.Log (Convert.ToBase64String(tdes.IV));
			inited = true;
		}
		
		public static string Encrypt(string toEncrypt) {     
			if (!inited)
				init ();
			
			byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);      
			
			ICryptoTransform cTransform = tdes.CreateEncryptor();     
			byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0,
			                                                    toEncryptArray.Length);      
			return Convert.ToBase64String(resultArray, 0,resultArray.Length, Base64FormattingOptions.None); 
		}  

		public static string Decrypt(string toDecrypt) {     
			if (!inited)
				init ();
			
			byte[] toDecryptArray = Convert.FromBase64String(toDecrypt);      
			
			ICryptoTransform cTransform = tdes.CreateDecryptor();     
			byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0,
			                                                    toDecryptArray.Length);      
			return UTF8Encoding.UTF8.GetString(resultArray, 0, resultArray.Length); 
		}  

		public static void GenerateKeyIV(out string key, out string IV)	{
			TripleDES provider = TripleDES.Create ();
			provider.Mode = CipherMode.CBC;
			provider.Padding = PaddingMode.PKCS7;
			provider.GenerateKey ();
			provider.GenerateIV ();
			key = Convert.ToBase64String(provider.Key);
			IV = Convert.ToBase64String(provider.IV);
		}
		*/
	}

	public class RSAEncrypt{
		static bool inited;
		static RSACryptoServiceProvider rsa;

		static void init(){
			string k = "ownsoc72a2iaZlCL8/xUuzuPzzQLjogs712t3H9Sv+148H/xVUfh+NEoUtzOfCJlJ/T3VD7EJJQkNQD6uMh0MY7JgK9oBvPFI7Ry5E7xL7/TAFRpxha5jAMW1YNnph6TBW854hE9v5E+rEPohXno5DlQrFE0/3FS5nnQxZxm25mAwY0XxNHb/KYNiqaXzzrdg4/BRZsVWU/j31fmPARTqihdU";
			rsa = new RSACryptoServiceProvider(2048);
			k += "QxounfUw3qQm1y0g1VgZs+RGHbEQ+mmI+2sMkQlm8Cs4CTC/y2bqTGtgV/sTLacUWObbkkwF0JWocN3Sepkxo7frDF4ozwEqTy/7bINyogtuKetF88BrrWbQoCGyw==";
			byte[] PublicKey = Convert.FromBase64String (k);
			byte[] Exponent = {1,0,1};
			RSAParameters RSAKeyInfo = new RSAParameters();
			RSAKeyInfo.Modulus = PublicKey;
			RSAKeyInfo.Exponent = Exponent;
			rsa.ImportParameters(RSAKeyInfo);
			inited = true;
		}

		public static void init (string k){
			rsa = new RSACryptoServiceProvider(2048);
			byte[] PublicKey = Convert.FromBase64String (k);
			byte[] Exponent = {1,0,1};
			RSAParameters RSAKeyInfo = new RSAParameters();
			RSAKeyInfo.Modulus = PublicKey;
			RSAKeyInfo.Exponent = Exponent;
			rsa.ImportParameters(RSAKeyInfo);
			inited = true;
		}

		public static string Encrypt(string toEncrypt) {     
			if (!inited) init ();
			byte[] res = rsa.Encrypt (UTF8Encoding.UTF8.GetBytes(toEncrypt),false);
			return Convert.ToBase64String(res, 0, res.Length, Base64FormattingOptions.None); 
		}  
	}



	public class EncryptionHash {
		
		private static bool initiated = false;
		private static string secretKey;
		
		public static void setKey(string key = "fR*NQLoiR0Kpz*hhCRE9Z6"){
			initiated = true;
			secretKey = key[key.Length-1] + key + key[0];
		}
		
		public static string Md5Sum(string str)
		{
			if (!initiated) setKey();			
			
			System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
			byte[] bytes = ue.GetBytes(str + secretKey);
			
			// encrypt bytes
			System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(bytes);
			
			// Convert the encrypted bytes back to a string (base 16)
			string hashString = "";
			
			for (int i = 0; i < hashBytes.Length; i++)
			{
				hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
			}
			
			return hashString.PadLeft(32, '0');
		}
	}
	#endif
}