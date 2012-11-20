package hu.bicycle;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.Uri;
import android.os.IBinder;

public class NotifyService extends Service {
	
	final static String ACTION = "NotifyServiceAction";
	//final static String STOP_SERVICE = "";
	final static int RQS_STOP_SERVICE = 1;
	final static int RQS_SEND_NOTIFICATION_1 = 2;
	
	NotifyServiceReceiver notifyServiceReceiver;
	
	private static final int MY_NOTIFICATION_ID_1=1;
	private NotificationManager notificationManager;
	private Notification myNotification;
	
	Context myContext;
	String myNotificationTitle = "Data of Rental:";
	
	@Override
	public void onCreate() {
		// TODO Auto-generated method stub
		notifyServiceReceiver = new NotifyServiceReceiver();
		super.onCreate();
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {
		// TODO Auto-generated method stub
		IntentFilter intentFilter = new IntentFilter();
		intentFilter.addAction(ACTION);
		registerReceiver(notifyServiceReceiver, intentFilter);
		
		// Send Notification
		notificationManager = 
			(NotificationManager)getSystemService(Context.NOTIFICATION_SERVICE);
		myNotification = new Notification(R.drawable.bike_noti, 
				"Welcome to the bicycle app!", 
				System.currentTimeMillis());
		myContext = getApplicationContext();
//		myNotification.defaults |= Notification.DEFAULT_SOUND;
		myNotification.flags |= Notification.FLAG_AUTO_CANCEL;
		
		return super.onStartCommand(intent, flags, startId);
	}

	@Override
	public void onDestroy() {
		// TODO Auto-generated method stub
		this.unregisterReceiver(notifyServiceReceiver);
		super.onDestroy();
	}

	@Override
	public IBinder onBind(Intent arg0) {
		// TODO Auto-generated method stub
		return null;
	}

	public class NotifyServiceReceiver extends BroadcastReceiver{

		@Override
		public void onReceive(Context arg0, Intent arg1) {
			// TODO Auto-generated method stub
			int rqs = arg1.getIntExtra("RQS", 0);
			if (rqs == RQS_STOP_SERVICE){
				stopSelf();
			}else if(rqs == RQS_SEND_NOTIFICATION_1){
				String myTarget = arg1.getStringExtra("TARGET");
				SendNotification(MY_NOTIFICATION_ID_1, myTarget);
				
			}
		}
		
		private void SendNotification(int id, String target){
			String notificationText = target;
			Intent myIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(target));
			PendingIntent pendingIntent 
				= PendingIntent.getActivity(myContext, 
						0, myIntent, 
						Intent.FLAG_ACTIVITY_NEW_TASK);
			
			myNotification.setLatestEventInfo(myContext, 
					myNotificationTitle, 
					notificationText, 
					pendingIntent);
			notificationManager.notify(id, myNotification);
		}
	}
	
}
