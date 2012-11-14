/**
 * 
 */
package hu.takraj.bicikli.printer.models;

import java.util.List;

/**
 * @author TakRaj
 *
 */
public class InvoiceModel {
	public String lender_name;
    public String lender_address;
    public String bike_name;
    public String name;
    public String address;
    public List<InvoiceItemModel> items;
    public Integer total_balance;
}
