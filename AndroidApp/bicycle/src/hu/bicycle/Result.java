package hu.bicycle;

import com.google.gson.annotations.SerializedName;

//Konkret kolcsonzo adatainak tarolasahoz
public class Result {
	
	
	@SerializedName("latitude")
	public float _latitude;
	
	@SerializedName("longitude")
	public float _longitude;
	
	@SerializedName("address")
	public String _address;
	
	@SerializedName("description")
	public String _desc;
	
	public String _name;
	public String source;
	
}
