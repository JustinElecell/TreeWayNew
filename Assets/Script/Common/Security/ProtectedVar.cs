using UnityEngine;
using System.Collections;

namespace GameSecurity {


	public static class Rand {
		static System.Random rand;
		static bool toInit = true;

		static void init(){
			rand = new System.Random();
		}

		public static int Range(int i, int j){
			if (toInit) init();
			if (i > j){
				int t = i;
				i=j;
				j=t;
			}
			return rand.Next(i,j);
		}

	}

	public struct safeInt {
		private int val;
		private int val2;
		
		public static implicit operator int(safeInt i){ return i.val >> i.val2; 	}
		public static implicit operator float(safeInt i){ return (float) (i.val >> i.val2); 	}
		public static implicit operator safeInt(int i){	return new safeInt(i);	}
		public override string ToString(){ return (val >> val2).ToString();}
		public string ToString(string s){ return (val >> val2).ToString(s);}
		public safeInt(int v){	val2 = Rand.Range(1,6);	val = v << val2; }
	}

	public struct safeLong {
		private long val;
		private int val2;
		
		public static implicit operator long(safeLong i){ return i.val >> i.val2; 	}
		public static implicit operator float(safeLong i){ return (float) (i.val >> i.val2); 	}
		public static implicit operator safeLong(long i){	return new safeLong(i);	}
		public override string ToString(){ return (val >> val2).ToString();}
		public string ToString(string s){ return (val >> val2).ToString(s);}
		public safeLong(long v){	val2 = Rand.Range(1,6);	val = v << val2; }
	}
	
	public struct safeFloat {
		private float val;
		private float val2;
		public static implicit operator int(safeFloat i){ return (int) (i.val * i.val2); 	}
		public static implicit operator float(safeFloat i){ return i.val * i.val2; 	}
		public static implicit operator safeFloat(float i){	return new safeFloat(i);	}
		public override string ToString(){ return (val * val2).ToString();}
		public string ToString(string s){ return (val * val2).ToString(s);}
		public safeFloat(float v){ int power = 1 << Rand.Range(1,6);  val = v * power; val2 = 1f / power; }
	}

	public struct safeDouble{
		private double val;
		private float val2;
		public static implicit operator double(safeDouble i) { return (double)(i.val * i.val2);  }
		//public static implicit operator float(safeDouble i) { return (float)i.val * i.val2; }
		public static implicit operator safeDouble(double i) { return new safeDouble(i); }
		public override string ToString() { return (val * val2).ToString(); }
		public string ToString(string s) { return (val * val2).ToString(s); }
		public safeDouble(double v) { int power = 1 << Rand.Range(1, 6); val = v * power; val2 = 1f / power; }
	}

	public struct ProtectedVar {
	}

}