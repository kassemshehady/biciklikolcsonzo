/**
 * 
 */
package hu.takraj.bicikli.printer;

import hu.takraj.bicikli.printer.models.InvoiceModel;

import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Insets;
import java.awt.font.FontRenderContext;
import java.awt.font.LineBreakMeasurer;
import java.awt.font.TextLayout;
import java.awt.print.PageFormat;
import java.awt.print.Printable;
import java.awt.print.PrinterException;
import java.text.AttributedCharacterIterator;
import java.text.AttributedString;

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

		if (page == 1) {
			drawCenteredString("SZÁMLA -- 1. példány",
					(int) pageFormat.getImageableWidth(), 50, g.create());
		} else {
			drawCenteredString("SZÁMLA -- 2. példány",
					(int) pageFormat.getImageableWidth(), 50, g.create());
		}

		// eladó adatai
		g.drawRect(20, 70, (int) (pageFormat.getImageableWidth() / 2) - 30, 100);
		g.drawString("Eladó adatai:", 25, 90);
		g.drawString(invoice.lender_name, 35, 105);
		drawWordWrappedText(invoice.lender_address, 35, 120,
				(pageFormat.getImageableWidth() / 2) - 50, g.create());

		// vevõ adatai
		g.drawRect((int) (pageFormat.getImageableWidth() / 2) + 10, 70,
				(int) (pageFormat.getImageableWidth() / 2) - 30, 100);
		g.drawString("Vevõ adatai:",
				(int) (pageFormat.getImageableWidth() / 2) + 15, 90);

		return Printable.PAGE_EXISTS;
	}

	private void drawWordWrappedText(String text, int x, int y, double width,
			Graphics g) {

		Graphics2D g2d = (Graphics2D) g;

		AttributedString messageAS = new AttributedString(text);
		AttributedCharacterIterator messageIterator = messageAS.getIterator();
		FontRenderContext messageFRC = g2d.getFontRenderContext();
		LineBreakMeasurer messageLBM = new LineBreakMeasurer(messageIterator,
				messageFRC);

		while (messageLBM.getPosition() < messageIterator.getEndIndex()) {
			
			//TODO: NEM OK!!!
			
			int next = messageLBM.nextOffset((float)width);
			int limit = next;
			if (limit < messageIterator.getEndIndex()) {
			   for (int i = messageLBM.getPosition(); i < next; ++i) {
			      char c = text.charAt(i);
			      if (c == '\n' || c == '\r') {
			         limit = i;
			         break;
			      }
			   }
			}
			
			TextLayout textLayout = messageLBM.nextLayout((float) width, limit, false);
			y += textLayout.getAscent();
			textLayout.draw(g2d, x, y);
			y += textLayout.getDescent() + textLayout.getLeading();
			x = x;
		}
	}

	private void drawCenteredString(String s, int w, int h, Graphics g) {
		FontMetrics fm = g.getFontMetrics();
		int x = (w - fm.stringWidth(s)) / 2;
		int y = (fm.getAscent() + (h - (fm.getAscent() + fm.getDescent())) / 2);
		g.drawString(s, x, y);
	}

}
