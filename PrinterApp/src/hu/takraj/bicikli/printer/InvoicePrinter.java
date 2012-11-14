/**
 * 
 */
package hu.takraj.bicikli.printer;

import hu.takraj.bicikli.printer.models.InvoiceModel;

import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.print.PageFormat;
import java.awt.print.Printable;
import java.awt.print.PrinterException;

/**
 * @author TakRaj
 *
 */
public class InvoicePrinter implements Printable {
	
	private InvoiceModel invoice = null;
	
	public InvoicePrinter(InvoiceModel invoice) {
		super();
		this.invoice = invoice;
	}

	/* (non-Javadoc)
	 * @see java.awt.print.Printable#print(java.awt.Graphics, java.awt.print.PageFormat, int)
	 */
	@Override
	public int print(Graphics g, PageFormat pageFormat, int page)
			throws PrinterException {
		
		if (page > 0) {
	         return NO_SUCH_PAGE;
	    }
		
		Graphics2D g2d = (Graphics2D)g;
	    g2d.translate(pageFormat.getImageableX(), pageFormat.getImageableY());

	    // Now we perform our rendering
	    g.drawString("Hello world!", 100, 100);
		
		return Printable.PAGE_EXISTS;
	}

}
