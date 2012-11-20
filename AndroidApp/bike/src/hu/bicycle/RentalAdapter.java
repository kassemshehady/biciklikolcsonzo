package hu.bicycle;

import java.util.List;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

public class RentalAdapter extends ArrayAdapter<SearchResponse> {

	public RentalAdapter(Context context, int textViewResourceId, List<SearchResponse> objects) {
		super(context, textViewResourceId, objects);
		// TODO Auto-generated constructor stub
	}
	
	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		// Ezt kéri el a listView elemenként
		
		View view = convertView;
		
		if (view == null) {
			LayoutInflater vi = (LayoutInflater) getContext(). // Az inflator egy rendszerszolgáltatás a rendszertõl kell elkérni
			getSystemService(Context.LAYOUT_INFLATER_SERVICE); 
			view = vi.inflate(R.layout.rental_adapter_view, null); // XML - bõl View-t csinál az inflator
		}
		
		SearchResponse result = getItem(position);
		
		
		// Nézetek azonosítása a létrehozott view-n
		TextView nameRental = (TextView)view.findViewById(R.id.rentalName);
		TextView addressRental = (TextView)view.findViewById(R.id.rentalAddress);
		TextView distanceRental = (TextView)view.findViewById(R.id.textViewLong);
//		final LinearLayout layout = (LinearLayout)view.findViewById(R.id.linearLayoutLower);
		
		
		// Szövegmezõk beállítása
		if (nameRental != null) {
			nameRental.setText(result.get_name());
		}
		if (addressRental != null) {
			addressRental.setText(result.get_address());
		}
		if (distanceRental != null) {
			distanceRental.setText(Double.toString(result.getDistance()) + "km");
		}		
		return view;
	}
	
	

}
