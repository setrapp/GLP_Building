package com.ritigm.glp;

import android.content.Intent;
import android.content.BroadcastReceiver;
import android.os.Bundle;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerNativeActivity;
import edu.mit.media.funf.storage.DatabaseService;
import edu.mit.media.funf.storage.NameValueDatabaseService;


public class MainActivity extends UnityPlayerNativeActivity {
	public void UnityTest(String test)
	{
		UnityPlayer.UnitySendMessage("AndroidCommunicator", "AndroidTest", test);
	}
	
	public void sendLog(String name, String value) {
		Intent i = new Intent();
		i.setAction(DatabaseService.ACTION_RECORD); // 
		Bundle b = new Bundle();
		b.putString(DatabaseService.DATABASE_NAME_KEY, "mainPipeline");
		b.putLong(NameValueDatabaseService.TIMESTAMP_KEY, System.currentTimeMillis()/1000);
		b.putString(NameValueDatabaseService.NAME_KEY, name);
		b.putString(NameValueDatabaseService.VALUE_KEY, value);
		i.putExtras(b);
		sendBroadcast(i);

	}
}
