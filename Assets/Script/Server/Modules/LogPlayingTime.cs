using UnityEngine;
using System.Collections;

public class LogPlayingTime : MonoBehaviour {
	
	private LogTime timer;

	public void startTracking (LogTime t, float interval = 36f, float increment = 0.01f) {
		DontDestroyOnLoad(gameObject);
		timer = t;		
		StartCoroutine(timeLooper(interval,increment));
	}
	
	IEnumerator timeLooper(float interval, float increment){
		float t = interval;
		while (true){
			while (t > 0f){
				yield return null;
				t -= Time.unscaledDeltaTime;
			}
			t = interval;

			timer.addTime( increment );
		}
	}
	
	public void destroy(){
		Destroy(gameObject);
	}
}

public class LogTime{
	private float time;
	private LogPlayingTime looper;
	
	public timerEvent timeIncrement;
	
	public static implicit operator int(LogTime i){ return (int) (i.time); 	}
	public static implicit operator float(LogTime i){ return i.time; 	}
	public static implicit operator double(LogTime i){ return (double) i.time; 	}
	public static implicit operator LogTime(float i){	return new LogTime(i);	}
	public override string ToString(){ return (time).ToString();}
	public string ToString(System.IFormatProvider format){ return (time).ToString(format);}
	public string ToString(string format){ return (time).ToString(format);}
	
	public LogTime(float interval = 36f, float increment = 0.01f){ 
		time = 0f; 
		looper = new GameObject().AddComponent<LogPlayingTime>();
		looper.startTracking(this, interval, increment);
	}

	public void setTime(float v){
		time = v;
	}
	public void addTime(float v){
		time += v;
		if (timeIncrement!=null) timeIncrement(time);
	}
	
	public void renameLooper(string s){
		looper.name = s;
	}
				
	public void release(){
		if (looper != null) {
			looper.destroy();
			looper = null;
		}
	}
}

public delegate void timerEvent(float time);