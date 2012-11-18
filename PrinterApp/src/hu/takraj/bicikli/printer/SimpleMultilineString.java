/**
 * 
 */
package hu.takraj.bicikli.printer;

import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.font.LineMetrics;
import java.util.Vector;

/**
 * @author TakRaj
 * 
 */
public class SimpleMultilineString {

	private String str = null;

	public SimpleMultilineString(String s) {
		this.str = new String(s).replace("\r\n", "\n").replace("\n\r", "\n");
	}

	public int renderToGraphics(Graphics g, int posX, int posY, int width) {

		Graphics2D g2d = (Graphics2D) g;
		g2d.translate(posX, posY);
		FontMetrics fm = g.getFontMetrics();
		LineMetrics lm = fm.getLineMetrics(str, g2d);

		Vector<Vector<String>> strMatrix = new Vector<Vector<String>>(); // 1.
																			// index:
																			// rows,
																			// 2.
																			// index:
																			// words

		// Fill up matrix
		{
			String[] rows = str.split("\n");
			
			for (int i = 0; i < rows.length; i++) {
				strMatrix.add(new Vector<String>());
				
				String[] words = rows[i].split(" ");
				
				for (int j = 0; j < words.length; j++) {
					int nSubwords = (int) Math.ceil(fm.stringWidth(words[j]) / ((float)width));
					
					if (nSubwords > 1) { // önkényesen feldaraboljuk
						int subLength = words[j].length() / nSubwords;
						for (int k = 1; k < nSubwords; k++) {
							strMatrix.lastElement().add(words[j].substring(0, subLength));
							words[j] = words[j].substring(subLength);
						}
						strMatrix.lastElement().add(words[j]); // maradék
					} else {
						strMatrix.lastElement().add(words[j]); // nem kelll darabolni
					}
				}
			}
		}
		
		// Render
		int yOffset = 0;
		for (Vector<String> row : strMatrix) {
			
			int xOffset = 0;
			
			for (String word : row) {
				
				if ((width - xOffset) < fm.stringWidth(word + " ")) {
					xOffset = 0;
					yOffset += lm.getHeight();
				}
				
				g2d.drawString(word + " ", xOffset, yOffset);
				xOffset += fm.stringWidth(word + " ");
			}
			
			yOffset += lm.getHeight();
		}

		return yOffset;
	}
}
