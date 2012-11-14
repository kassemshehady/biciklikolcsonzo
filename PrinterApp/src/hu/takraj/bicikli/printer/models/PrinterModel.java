/**
 * 
 */
package hu.takraj.bicikli.printer.models;

/**
 * @author TakRaj
 *
 */
public class PrinterModel {
	public Integer lender_id = null;
	public String printer_ip = null;
	
	public PrinterModel(Integer lender_id, String printer_ip) {
		this.lender_id = lender_id;
		this.printer_ip = printer_ip;
	}
}
