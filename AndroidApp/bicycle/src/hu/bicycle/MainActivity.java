package hu.bicycle;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import hu.bicycle.Result;
import hu.bicycle.SearchResponse;

import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.Handler;
import android.app.Activity;
import android.app.ProgressDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;
import java.lang.Math;

public class MainActivity extends Activity {
	
	String url = "http://192.168.0.102:8080/api/Lenders/";
//	String url = "http://192.168.2.143:8080/api/Lenders/";
    ArrayList<SearchResponse> res = new ArrayList<SearchResponse>();
    RentalAdapter adapter;
    List<SearchResponse> response;
    ListView mainListView;
    TextView name;
    TextView longitude;
    TextView latitude;
    TextView desc;
    TextView address;
    EditText sessionEditText;
    ImageView sessionOK;
    ImageView addCode;
    ProgressDialog dialog;
    private Handler handler = new Handler();
    private Handler handler_item = new Handler();
    int tempId;
    String tempName;
    Result responseResult;
    LocationManager mlocManager;
    LocationListener mlocListener;
    LinearLayout linS;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
        Intent intent = new Intent(MainActivity.this, hu.bicycle.NotifyService.class);
		MainActivity.this.startService(intent);
        startPlayer();
        sessionEditText = (EditText) findViewById(R.id.editText_session);
	    sessionOK = (ImageView) findViewById(R.id.button_ok);
	    addCode = (ImageView) findViewById(R.id.imageViewAdd);
	    linS = (LinearLayout) findViewById(R.id.linSearch);
	    addCode.setOnClickListener(new ImageView.OnClickListener() {

			public void onClick(View arg0) {
				// sessionID beviteli mezo megjelenitese/eltuntese
				if (linS != null) {
					int v2 = linS.getVisibility();
					v2 = v2 == View.VISIBLE ? View.GONE : View.VISIBLE;
					linS.setVisibility(v2);					
				}
				if (sessionEditText != null) {
					int v2 = sessionEditText.getVisibility();
					v2 = v2 == View.VISIBLE ? View.GONE : View.VISIBLE;
					sessionEditText.setVisibility(v2);					
				}
				if (sessionOK != null) {
					int v2 = sessionOK.getVisibility();
					v2 = v2 == View.VISIBLE ? View.GONE : View.VISIBLE;
					sessionOK.setVisibility(v2);					
				}
			}
	    	
	    });
	    sessionOK.setOnClickListener(new ImageView.OnClickListener() {
			
			public void onClick(View v) {
				String text = sessionEditText.getText().toString();
				Intent i=new Intent(MainActivity.this, SendService.class);
		        try {
		        	i.putExtra("sessionID", Integer.parseInt(text));
		        	startService(i);
					
				} catch (Exception e) {
					Toast.makeText(getApplicationContext(), "Wrong identification!", Toast.LENGTH_SHORT).show();
				}
		        linS.setVisibility(View.GONE);
		        sessionEditText.setVisibility(View.GONE);
				sessionOK.setVisibility(View.GONE);
			}
		});
	    //feliratkozas esemenyekre
	    IntentFilter ifilt = new IntentFilter("hu.bicycle.SendStatusAction"); 
	    registerReceiver(mReceiver, ifilt);
	    IntentFilter ifilt1 = new IntentFilter("hu.bicycle.SendStatusError"); 
	    registerReceiver(mReceiver, ifilt1);
	    
	    dialog = ProgressDialog.show(MainActivity.this, "Download", "Please wait...");
	    runningProgressDailog();

}
    private BroadcastReceiver mReceiver = new BroadcastReceiver() {
    	//figyeli, hogy az esemeny bekovetkezett-e, ha igen a megfelelo kodreszlet lefut
    	@Override 
    	public void onReceive(Context context, Intent intent) { 
    		String action = intent.getAction(); 
    	    if (action.equals("hu.bicycle.SendStatusAction")) { 
    			String status = intent.getStringExtra("status");
    			if(status.equals("OK_NORMAL") || status.equals("OK_DANGER") || status.equals("ERROR"))
    			{
    				linS.setVisibility(View.GONE);
    				sessionEditText.setVisibility(View.GONE);
    				sessionOK.setVisibility(View.GONE);
    			}
    			else
    				if(status.equals("END_OF_SESSION")){
    					linS.setVisibility(View.VISIBLE);
    					sessionEditText.setVisibility(View.VISIBLE);
    					sessionOK.setVisibility(View.VISIBLE);
    				}

    	    }
    	    if (action.equals("hu.bicycle.SendStatusError")){
    	    	Toast.makeText(getApplicationContext(), "Wrong identification!", Toast.LENGTH_SHORT).show();
    	    }
    	} 
    };
    
    OnItemClickListener itemClickListener = new OnItemClickListener() {

		public void onItemClick(AdapterView<?> arg0, View arg1, int pos,
				long arg3) {
			setContentView(R.layout.concrete_rental);
			
			runningProgressDailogItem(response.get(pos)._id, response.get(pos)._name);
			
		}

			
		};
		//Konkret biciklikolcsonzo adatainak lekerdezese
		public void runningProgressDailogItem(int iden, String nam){
			tempId = iden;
			tempName = nam;
	    	new Thread(new Runnable()
			{

				public void run()
				{
					try
					{
						
	        	    	InputStream source = retrieveStream(url.concat(Integer.toString(tempId)));
	        	    	
	        	    	Gson gson1 = new Gson();
	        	    	
	        	    	Reader reader = new InputStreamReader(source);
	        	    	
	        	    	responseResult = gson1.fromJson(reader, Result.class);
							dialog.dismiss();
							handler_item.post(new Runnable()
							{

								public void run()
								{
									name = (TextView) findViewById(R.id.textVieName);
									name.setText(tempName);
									address = (TextView) findViewById(R.id.textViewAddress);
									address.setText(responseResult._address);
									latitude = (TextView) findViewById(R.id.textViewLatitude);
									latitude.setText(Float.toString(responseResult._latitude));
									longitude = (TextView) findViewById(R.id.textViewLongitude);
									longitude.setText(Float.toString(responseResult._longitude));
									desc = (TextView) findViewById(R.id.textViewDescrip);
									desc.setText(responseResult._desc);
								}
							});
					}
					catch (Exception e)
					{
						e.printStackTrace();
					}
				}
			}).start();
	    }
		//Biciklikolcsonzo lista lekerdezese
    public void runningProgressDailog(){
    	new Thread(new Runnable()
		{

			public void run()
			{
				try
				{
        	    	InputStream source = retrieveStream(url);
        	    	
        	    	Gson gson = new Gson();
        	    	
        	    	Reader reader = new InputStreamReader(source);
        	    	
        	    	Type listOfBooksType = new TypeToken<List<SearchResponse>>() {}.getType();
        	    	response = gson.fromJson(reader, listOfBooksType);
        	    	
        	    	Thread.sleep(1000);
						dialog.dismiss();
						handler.post(new Runnable()
						{

							public void run()
							{
								mainListView = (ListView) findViewById(R.id.mainListView);
				        		adapter = new RentalAdapter(MainActivity.this, R.layout.rental_adapter_view, response);
				        		mainListView.setAdapter(adapter);
				        		adapter.notifyDataSetChanged();
				        		mlocManager = (LocationManager)getSystemService(Context.LOCATION_SERVICE);
				        	    mlocListener = new MyLocationListener();
				        	    mlocManager.requestLocationUpdates( LocationManager.NETWORK_PROVIDER, 0, 0, mlocListener);
				        	    mainListView.setOnItemClickListener(itemClickListener);
							}
						});
				}
				catch (Exception e)
				{
					e.printStackTrace();
				}
			}
		}).start();
    }
    
    private InputStream retrieveStream(String url) {
    	
    	DefaultHttpClient client = new DefaultHttpClient(); 
        
        HttpGet getRequest = new HttpGet(url);
          
        try {
             
            HttpResponse getResponse = client.execute(getRequest);
            final int statusCode = getResponse.getStatusLine().getStatusCode();
           
            if (statusCode != HttpStatus.SC_OK) { 
               Log.w(getClass().getSimpleName(), "Error " + statusCode + " for URL " + url); 
               return null;
            }

            HttpEntity getResponseEntity = getResponse.getEntity();
            return getResponseEntity.getContent();
           
        } 
        catch (IOException e) {
        	getRequest.abort();
            Log.w(getClass().getSimpleName(), "Error for URL " + url, e);
        }
        
        return null;
        
     }
        
    //Serice inditasa
    public void startPlayer() {
        Intent i=new Intent(this, SendService.class);
        startService(i);
    }
    
    //Service leallitasa
    public void stopPlayer() {
        stopService(new Intent(this, SendService.class));
    }
    
    //menu
    @Override
	public boolean onCreateOptionsMenu(Menu menu) {
		super.onCreateOptionsMenu(menu);
		menu.add(0, 1, 0,
				"Exit");
		return true;
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case 1:
			stopPlayer();
	        stopService(new Intent(this, NotifyService.class));
//	        mlocManager.removeUpdates(mlocListener);
	        finish();
			return true;
		}
		return false;
	}


    public class MyLocationListener implements LocationListener {
    	//Aktualis pozicio megvaltozik
    	public void onLocationChanged(Location loc) {
    		//Tavolsag szamitas es rendezes
    		int R = 6371; 
    		for (int i = 0; i < response.size()-1; i++) {
    			for (int j = i+1; j < response.size(); j++) {
	    			double cur_lat = loc.getLatitude();
	    			double cur_lon = loc.getLongitude();
	    			double lat_1 = response.get(i)._latitude;
	    			double lon_1 = response.get(i)._longitude;
	    			double lat_2 = response.get(j)._latitude;
	    			double lon_2 = response.get(j)._longitude;
	    			
	    			double dLat_1 = Math.toRadians(cur_lat - lat_1);
	    			double dLon_1 = Math.toRadians(cur_lon - lon_1);
	    			double dLat_2 = Math.toRadians(cur_lat - lat_2);
	    			double dLon_2 = Math.toRadians(cur_lon - lon_2);
	    			cur_lat = Math.toRadians(cur_lat);
	    			lat_1 = Math.toRadians(lat_1);
	    			lat_2 = Math.toRadians(lat_2);
	    			
	    			double a_1 = 
	    					Math.sin(dLat_1 / 2) * Math.sin(dLat_1 / 2) + 
	    					Math.sin(dLon_1 / 2) * Math.sin(dLon_1 / 2) * Math.cos(lat_1) * Math.cos(cur_lat);
	    			double c_1 = 2 * Math.atan2(Math.sqrt(a_1), Math.sqrt(1 - a_1));
	    			double d_1 = R * c_1;
	    			double a_2 = 
	    					Math.sin(dLat_2 / 2) * Math.sin(dLat_2 / 2) + 
	    					Math.sin(dLon_2 / 2) * Math.sin(dLon_2 / 2) * Math.cos(lat_2) * Math.cos(cur_lat);
	    			double c_2 = 2 * Math.atan2(Math.sqrt(a_2), Math.sqrt(1 - a_2));
	    			double d_2 = R * c_2;
	    			if(d_2 < d_1) {
	    				SearchResponse temp = new SearchResponse();
	    				temp = response.get(i);
	    				response.set(i, response.get(j));
	    				response.set(j, temp);
	    			}
    			}
    		}
    		for (SearchResponse responseTemp : response) {
    			double cur_lat = loc.getLatitude();
    			double cur_lon = loc.getLongitude();
    			double lat_1 = responseTemp._latitude;
    			double lon_1 = responseTemp._longitude;
    			
    			double dLat_1 = Math.toRadians(cur_lat - lat_1);
    			double dLon_1 = Math.toRadians(cur_lon - lon_1);
    			cur_lat = Math.toRadians(cur_lat);
    			lat_1 = Math.toRadians(lat_1);
	    			
    			double a_1 = 
    					Math.sin(dLat_1 / 2) * Math.sin(dLat_1 / 2) + 
    					Math.sin(dLon_1 / 2) * Math.sin(dLon_1 / 2) * Math.cos(lat_1) * Math.cos(cur_lat);
    			double c_1 = 2 * Math.atan2(Math.sqrt(a_1), Math.sqrt(1 - a_1));
    			double d_1 = R * c_1;
    			
				responseTemp.setDistance(Math.round(d_1));
			}
    		adapter.notifyDataSetChanged();
    	}
    	
    	public void onProviderDisabled(String provider) {
    	}
    	
    	public void onProviderEnabled(String provider) {
    	}
    	
    	public void onStatusChanged(String provider, int status, Bundle extras) {
    	}
    }
    
    public void onBackPressed() {
    	setContentView(R.layout.activity_main);
    	dialog = ProgressDialog.show(MainActivity.this, "Loading", "Please wait...");
	    runningProgressDailog();
    	}
}

