package hu.bicycle;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.HashMap;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.protocol.HTTP;
import org.json.JSONException;
import org.json.JSONObject;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.IBinder;
import android.util.Log;
import android.widget.Toast;

public class SendService extends Service {
	
	private boolean isStarting=false;
	private static final String TAG = "SendService";
//	private static final String URLPOST = "http://192.168.2.143:8080/api/Report";
	private static final String URLPOST = "http://192.168.0.102:8080/api/Report";

	public double latitude = 0;
	public double longitude = 0;
	int s_id = -1;
	static InputStream is = null;
	static String json = "";
	static JSONObject resJson;
	JSONObject jsonResponse;
	String status = "";
	String normal_time = "";
	String danger_time = "";
	String total_balance = "";
	static int statusCode = 0;
	LocationManager mlocManager;
	LocationListener mlocListener;
	LocationManager mlocManagerGPS;
	LocationListener mlocListenerGPS;
	boolean isRunThred = false;
	boolean isGPSActive = false;
	  
	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {
		s_id = intent.getIntExtra("sessionID", -1);
		Log.w(getClass().getName(), "Service start!");
		//Network alapu pozicio service
		mlocManager = (LocationManager)getSystemService(Context.LOCATION_SERVICE);
	    mlocListener = new MyLocationListener();
	    mlocManager.requestLocationUpdates( LocationManager.NETWORK_PROVIDER, 0, 0, mlocListener);
	    //GPS alapu pozicio service
	    mlocManagerGPS = (LocationManager)getSystemService(Context.LOCATION_SERVICE);
	    mlocListenerGPS = new MyLocationListenerGPS();
	    mlocManagerGPS.requestLocationUpdates( LocationManager.GPS_PROVIDER, 0, 0, mlocListener);
	    
		start();
		
	    
		return(START_NOT_STICKY);
	}
	//HTTP POST keres/valasz
	public static JSONObject getJSONfromURL(int session_id, double latitude, double longitude, String url){
		HttpResponse response;
	    try{
		    	HttpClient client = new DefaultHttpClient();
	            HttpPost request = new HttpPost(URLPOST);
	            request.setHeader("Accept", "application/json");
	            request.setHeader(HTTP.CONTENT_TYPE, "application/json");
	            JSONObject object = new JSONObject();
	            object.put("session_id",session_id);
	            object.put("latitude",latitude);
	            object.put("longitude",longitude);
	            String message=object.toString();
	            Log.v("bicycle.net", "Kuldott cucc: " + message); 

	            request.setEntity(new StringEntity(message, "UTF8"));
	            response = client.execute(request);
	            HttpEntity httpEntity = response.getEntity();
	            is = httpEntity.getContent(); 
	            statusCode = response.getStatusLine().getStatusCode();
	            Log.v("response code!!!!", response.getStatusLine().getStatusCode() + ""); 
	    }catch(Exception e){
	            Log.e("log_tag", "Error in http connection "+e.toString());
	    }
	    try {
	    	//valasz osszerakasa
            BufferedReader reader = new BufferedReader(new InputStreamReader(
                    is, "UTF8"), 8);
            StringBuilder sb = new StringBuilder();
            String line = null;
            while ((line = reader.readLine()) != null) {
                sb.append(line + "\n");                
            }
            is.close();
            Log.e("bicycle.net", "kapott cucc: " + sb.toString());
            json = sb.toString();
        } catch (Exception e) {
            Log.e("Buffer Error", "Error converting result " + e.toString());
        }
 
        try {
            resJson = new JSONObject(json);
        } catch (JSONException e) {
            Log.e("JSON Parser", "Error parsing data " + e.toString());
        }
 
	    return resJson;
	}
	    
	    
	public class MyLocationListener implements LocationListener {
		//aktualis pozicio megvaltozasa es bejelentes (NETWORK)
		public void onLocationChanged(Location loc) {
			longitude = loc.getLongitude();
			latitude = loc.getLatitude();
			if (!isRunThred) {
				if(!isGPSActive){
				isRunThred = true;
				new Thread(new Runnable()
				{
	
					public void run()
					{
						try {
							Thread.sleep(15000);
						} catch (InterruptedException e1) {
							e1.printStackTrace();
						}
					      
					    if(s_id != -1){
				            try {
				            	
				            	jsonResponse = getJSONfromURL(s_id,latitude,longitude,URLPOST);
				            	Log.w("bicycle.net", "latitude: " + latitude);
				            	Log.w("bicycle.net", "longitude: " + longitude);
				            	//ha kapcsolodott a szervezhez a valasz feldogozasa
				            	if(statusCode == 200){
				            		status = jsonResponse.getString("status");
				            		if (status.equals("END_OF_SESSION")){
				            			s_id = -1;
				            			Toast.makeText( getApplicationContext(), "End of session!", Toast.LENGTH_SHORT).show();
				            		}
				            		Log.w("bicycle.net", "status: " + status);
				            		normal_time = jsonResponse.getString("normal_time");
				            		Log.w("bicycle.net", "normal_time: " + normal_time);
				            		danger_time = jsonResponse.getString("danger_time");
				            		Log.w("bicycle.net", "danger_time: " + danger_time);
				            		total_balance = jsonResponse.getString("total_balance");
				            		Log.w("bicycle.net", "total_balance:" + total_balance);
				            		String statusTemp = "";
				            		if(status.equals("END_OF_SESSION")){
				            			statusTemp = "End of session";
				            		}
				            		if(status.equals("OK_NORMAL")){
				            			statusTemp = "Normal area";
				            		}
				            		if(status.equals("OK_DANGER")){
				            			statusTemp = "Danger area";
				            		}
				            		if(status.equals("ERROR")){
				            			statusTemp = "Error of session";
				            		}
				            		//notification-ra kiirni az adatokat
				            		Intent intent1 = new Intent();
				            		intent1.setAction(NotifyService.ACTION);
				            		intent1.putExtra("RQS", NotifyService.RQS_SEND_NOTIFICATION_1);
				            		intent1.putExtra("TARGET", "Status:" + statusTemp + " Total:" + total_balance + " Ft");
				            		sendBroadcast(intent1);
				            		
				            		//MainActivity-t ertesiteni + status
				            		Intent intent = new Intent();
				            		intent.putExtra("status", status);
				            		intent.setAction("hu.bicycle.SendStatusAction");
				            		sendBroadcast(intent);
				            		
				            	} else {
				            		//error eseten
				            		s_id = -1;
				            		Intent intent = new Intent();
				            		intent.putExtra("status", status);
				            		intent.setAction("hu.bicycle.SendStatusError");
				            		sendBroadcast(intent);
				            	}
							} catch (JSONException e) {
								e.printStackTrace();
							}
					    }
					    isRunThred = false;
					     
					}
				}).start();
			}
			}
	    }
		
	        
	    public void onProviderDisabled(String provider) {
        }

	    public void onProviderEnabled(String provider) {
	    }

	    public void onStatusChanged(String provider, int status, Bundle extras) {
        }
    }
	  
	public class MyLocationListenerGPS implements LocationListener {
		//aktualis pozicio megvaltozasa es bejelentes (GPS) es Network alapu tiltasa
		public void onLocationChanged(Location loc) {
			isGPSActive = true;
			longitude = loc.getLongitude();
			latitude = loc.getLatitude();
			if (!isRunThred) {
				isRunThred = true;
				new Thread(new Runnable()
				{
					
					public void run()
					{
						try {
							Thread.sleep(15000);
						} catch (InterruptedException e1) {
							e1.printStackTrace();
						}
						ArrayList<HashMap<String, String>> mylist = new ArrayList<HashMap<String, String>>();
						
						if(s_id != -1){
							try {
								
								jsonResponse = getJSONfromURL(s_id,latitude,longitude,URLPOST);
								Log.w("bicycle.net", "latitude: " + latitude);
								Log.w("bicycle.net", "longitude: " + longitude);
								if(statusCode == 200){
									status = jsonResponse.getString("status");
									if (status.equals("END_OF_SESSION")){
										s_id = -1;
										Toast.makeText( getApplicationContext(), "End of session!", Toast.LENGTH_SHORT).show();
									}
									Log.w("bicycle.net", "status: " + status);
									normal_time = jsonResponse.getString("normal_time");
									Log.w("bicycle.net", "normal_time: " + normal_time);
									danger_time = jsonResponse.getString("danger_time");
									Log.w("bicycle.net", "danger_time: " + danger_time);
									total_balance = jsonResponse.getString("total_balance");
									Log.w("bicycle.net", "total_balance:" + total_balance);
									String statusTemp = "";
									if(status.equals("END_OF_SESSION")){
										statusTemp = "End of session";
									}
									if(status.equals("OK_NORMAL")){
										statusTemp = "Normal area";
									}
									if(status.equals("OK_DANGER")){
										statusTemp = "Danger area";
									}
									if(status.equals("ERROR")){
										statusTemp = "Error of session";
									}
									Intent intent1 = new Intent();
									intent1.setAction(NotifyService.ACTION);
									intent1.putExtra("RQS", NotifyService.RQS_SEND_NOTIFICATION_1);
									intent1.putExtra("TARGET", "Status:" + statusTemp + " Total:" + total_balance + " Ft");
									sendBroadcast(intent1);
									
									Intent intent = new Intent();
									intent.putExtra("status", status);
									intent.setAction("hu.bicycle.SendStatusAction");
									sendBroadcast(intent);
									
								} else {
									s_id = -1;
									Intent intent = new Intent();
									intent.putExtra("status", status);
									intent.setAction("hu.bicycle.SendStatusError");
									sendBroadcast(intent);
								}
							} catch (JSONException e) {
								e.printStackTrace();
							}
						}
						isRunThred = false;
						
					}
				}).start();
			}
		}
		
		public void onProviderDisabled(String provider) {
			Toast.makeText( getApplicationContext(), "Gps Disabled", Toast.LENGTH_SHORT ).show();
		}
		
		public void onProviderEnabled(String provider) {
			Toast.makeText( getApplicationContext(), "Gps Enabled", Toast.LENGTH_SHORT).show();
		}
		
		public void onStatusChanged(String provider, int status, Bundle extras) {
		}
	}
	
	@Override
	public void onDestroy() {
	    stop();
	}
	  
	private void start( ){
	    if (!isStarting) {
	    	isStarting=true;

	  }
	}
	  
	private void stop() {
		if (isStarting) {
			Log.w(getClass().getName(), "Got to stop()!");
			isStarting=false;
//			mlocManager.removeUpdates(mlocListener);
			mlocManagerGPS.removeUpdates(mlocListener);
			stopForeground(true);
	    }
	}

	@Override
	public IBinder onBind(Intent arg0) {
		return null;
	}
}