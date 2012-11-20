package hu.bicycle;

import java.util.List;

import com.google.gson.annotations.SerializedName;

public class SearchResponse {
	
	public List<Result> results;
	
	@SerializedName("id")
	public int _id;
	
	public int get_id() {
		return _id;
	}

	public void set_id(int _id) {
		this._id = _id;
	}

	public float get_latitude() {
		return _latitude;
	}

	public void set_latitude(float _latitude) {
		this._latitude = _latitude;
	}

	public float get_longitude() {
		return _longitude;
	}

	public void set_longitude(float _longitude) {
		this._longitude = _longitude;
	}

	public String get_name() {
		return _name;
	}

	public void set_name(String _name) {
		this._name = _name;
	}

	public String get_address() {
		return _address;
	}

	public void set_address(String _address) {
		this._address = _address;
	}

	@SerializedName("latitude")
	public float _latitude;
	
	@SerializedName("longitude")
	public float _longitude;
	
	@SerializedName("name")
	public String _name;
	
	@SerializedName("address")
	public String _address;
	
	public double distance;
	
	public double getDistance() {
		return distance;
	}

	public void setDistance(double distance) {
		this.distance = distance;
	}

}
