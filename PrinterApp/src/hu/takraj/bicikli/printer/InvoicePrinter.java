/**
 * 
 */
package hu.takraj.bicikli.printer;

import hu.takraj.bicikli.printer.models.InvoiceItemModel;
import hu.takraj.bicikli.printer.models.InvoiceModel;

import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.print.PageFormat;
import java.awt.print.Printable;
import java.awt.print.PrinterException;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Calendar;

/**
 * @author TakRaj
 * 
 */
public class InvoicePrinter implements Printable {

	private InvoiceModel invoice = null;
	private String dateString = null;

	public InvoicePrinter(InvoiceModel invoice) {
		super();
		this.invoice = invoice;

		DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		this.dateString = dateFormat.format(Calendar.getInstance().getTime());
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see java.awt.print.Printable#print(java.awt.Graphics,
	 * java.awt.print.PageFormat, int)
	 */
	@Override
	public int print(Graphics g, PageFormat pageFormat, int page)
			throws PrinterException {

		if (page > 1) {
			return NO_SUCH_PAGE;
		}

		Graphics2D g2d = (Graphics2D) g;
		g2d.translate(pageFormat.getImageableX(), pageFormat.getImageableY());

		// Now we perform our rendering
		g.drawRect(0, 0, (int) pageFormat.getImageableWidth(),
				(int) pageFormat.getImageableHeight());

		drawCenteredString("SZÁMLA -- " + (page + 1) + ". példány",
				(int) pageFormat.getImageableWidth(), 50, g.create());

		final double halfPageWidth = pageFormat.getImageableWidth() / 2;

		// eladó + vevõ fejléc
		g.translate(0, 70);

		// eladó adatai
		g.drawRect(20, 0, (int) halfPageWidth - 30, 100);
		g.drawString("Eladó adatai:", 25, 20);
		drawWordWrappedText(invoice.lender_name, 35, 35, halfPageWidth - 50,
				g.create());
		drawWordWrappedText(invoice.lender_address, 35, 55, halfPageWidth - 50,
				g.create());

		// vevõ adatai
		g.drawRect((int) halfPageWidth + 10, 0, (int) halfPageWidth - 30, 100);
		g.drawString("Vevõ adatai:", (int) halfPageWidth + 15, 20);
		drawWordWrappedText(invoice.name, (int) halfPageWidth + 25, 35,
				halfPageWidth - 50, g.create());
		drawWordWrappedText(invoice.address, (int) halfPageWidth + 25, 55,
				halfPageWidth - 50, g.create());

		// számla fejléc
		g.translate(0, 120);
		g.drawString("Tétel", 5, 10);

		drawRightAlignedString("Egységár", 210, 10, g.create());
		drawRightAlignedString("Menny.", 265, 10, g.create());
		drawRightAlignedString("Nettó", 325, 10, g.create());
		drawRightAlignedString("ÁFA", 370, 10, g.create());
		drawRightAlignedString("Bruttó", 445, 10, g.create());

		// számla tételek
		int sum_brutto = 0;
		for (InvoiceItemModel item : invoice.items) {
			g.translate(0, 20);
			g.drawString(item.title, 5, 10);

			drawRightAlignedString(item.base_unit_price + " Ft", 210, 10,
					g.create());
			drawRightAlignedString(item.amount + " perc", 265, 10, g.create());
			
			double netto = Math.round(item.amount * item.base_unit_price);
			
			drawRightAlignedString((int)netto + " Ft", 325, 10, g.create());
			drawRightAlignedString(item.vat + " %", 370, 10, g.create());
			
			double brutto = Math.round(netto * ((item.vat / 100) + 1));
			
			drawRightAlignedString((int)brutto + " Ft", 445, 10, g.create());

			sum_brutto += brutto;
		}

		// összesítés, kelt, aláírás
		g.translate(0, 40);
		drawRightAlignedString("Összesen nettó: " + (int)invoice.total_balance
				+ " Ft", 445, 10, g.create());
		g.translate(0, 20);
		drawRightAlignedString("Összesen bruttó: " + (int)sum_brutto + " Ft", 445,
				10, g.create());

		g.translate(0, 40);
		g.drawString("Kelt: " + dateString, 5, 10);

		g.translate(0, 40);
		g.drawString("Eladó aláírása: ", 5, 10);
		g.drawString("Vevõ aláírása: ", (int) halfPageWidth + 10, 10);

		return Printable.PAGE_EXISTS;
	}

	private void drawWordWrappedText(String text, int x, int y, double width,
			Graphics g) {

		new SimpleMultilineString(text).renderToGraphics(g.create(), x, y,
				(int) width);
	}

	private void drawCenteredString(String s, int w, int h, Graphics g) {
		FontMetrics fm = g.getFontMetrics();
		int x = (w - fm.stringWidth(s)) / 2;
		int y = (fm.getAscent() + (h - (fm.getAscent() + fm.getDescent())) / 2);
		g.drawString(s, x, y);
	}

	private void drawRightAlignedString(String s, int rightX, int y, Graphics g) {
		FontMetrics fm = g.getFontMetrics();
		int x = rightX - fm.stringWidth(s);
		g.drawString(s, x, y);
	}

}
